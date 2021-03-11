using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using Unity.MARS.Companion.Core;
using Unity.MARS.Settings;
using Unity.XRTools.ModuleLoader;
using Unity.XRTools.Utils;
using UnityEditor;
using UnityEngine;

namespace Unity.MARS.Companion.CloudStorage
{
    [ScriptableSettingsPath(MARSCore.UserSettingsFolder)]
    class GenesisCloudStorageModule : ScriptableSettings<GenesisCloudStorageModule>,
        IModuleBehaviorCallbacks, IProvidesCloudStorage
    {
        internal const int MaxKeyLengthBytes = 1024;
        internal const string CancelRequestWarning = "The user canceled the request; Cancel exceptions directly following this message can be ignored.";

        const string k_GenesisEndpoint = "https://api.unity.com/v1/mars_companion";
        const string k_CacheControlHeaderOptions = "public, no-cache, no-store, max-age=0";
        const string k_InvalidKeyRegex = @"[\[\]\#\?\*\n\r]";
        const string k_InvalidKeySubstringRegex = @"[_\[\]\#\?\*\n\r]";
        const int k_RequestHandshakeTimeout = 50;
        const string k_ApplicationJsonContentType = "application/json";
        const string k_CancelAllRequestsWarning = "Canceling all requests; Cancel exceptions directly following this message can be ignored.";

        static readonly Uri k_GetObjectsUri = new Uri(k_GenesisEndpoint + "/get_objects");
        static readonly byte[] k_RequestCanceledMessage = Encoding.UTF8.GetBytes("Request was canceled");

#pragma warning disable 649, 414
        [SerializeField]
        bool m_OfflineMode;

        [SerializeField]
        float m_TestingDelay;
#pragma warning restore 649, 414

        //readonly HttpClient m_Client = new HttpClient(new HttpClientHandler { Proxy = WebRequest.GetSystemWebProxy() }) { Timeout = Timeout.InfiniteTimeSpan };
        readonly HttpClient m_Client = new HttpClient { Timeout = Timeout.InfiniteTimeSpan };
        string m_ProjectIdentifier;
        readonly Dictionary<RequestHandle, Request> m_Requests = new Dictionary<RequestHandle, Request>();

        internal IMarsIdentity IdentityProvider { get; set; }
        internal Dictionary<RequestHandle, Request> Requests => m_Requests;

        // Local method use only -- created here to reduce garbage collection. Collections must be cleared before use
        static readonly Dictionary<RequestHandle, Request> k_RequestsCopy = new Dictionary<RequestHandle, Request>();

        static class HttpContentTypes
        {
            public const string ApplicationOctetStream = "application/octet-stream";
        }

        class MemoryStreamContent : HttpContent
        {
            const int k_ChunkSize = 4096;
            readonly byte[] m_Bytes;

            public event Action OnStarted;
            public event Action<int> OnProgress;

            public MemoryStreamContent(byte[] bytes)
            {
                m_Bytes = bytes;
            }

            protected override Task SerializeToStreamAsync(Stream stream, TransportContext context)
            {
                var length = m_Bytes.Length;
                return Task.Run(() =>
                {
                    OnStarted?.Invoke();
                    for (var i = 0; i < length; i += k_ChunkSize)
                    {
                        var count = Math.Min(k_ChunkSize, length - i);
                        stream.Write(m_Bytes, i, count);
                        OnProgress?.Invoke(i + count);
                    }
                });
            }

            protected override bool TryComputeLength(out long length)
            {
                length = m_Bytes.Length;
                return true;
            }
        }

        internal enum RequestStatus
        {
            // ReSharper disable once UnusedMember.Global
            Pending,
            Uploading,
            Downloading,
            Succeeded,
            Failed,
            Canceled
        }

        internal abstract class Request : IDisposable
        {
            const int k_BufferSize = 4096;

            public float StartTime { get; private set; }
            protected RequestStatus Status { get; private set; }

            protected HttpStatusCode m_StatusCode;

            protected readonly ProgressCallback m_Progress;
            readonly CancellationTokenSource m_TokenSource = new CancellationTokenSource();
            Thread m_SendThread;

            long m_TotalBytesToUpload;
            long m_TotalBytesToDownload;
            long m_BytesUploaded;
            int m_BytesDownloaded;
            protected byte[] m_ResponseBytes;

            protected Request(ProgressCallback progress)
            {
                m_Progress = progress;
            }

            public void Send(HttpClient client, HttpRequestMessage genesisRequest, string key,
                HttpMethod httpMethod, int timeout, byte[] payload = null)
            {
                if (timeout > 0)
                    m_TokenSource.CancelAfter(new TimeSpan(0, 0, timeout));

                StartTime = Time.realtimeSinceStartup;
                m_SendThread = new Thread(() =>
                {
                    try
                    {
                        var response = client.SendAsync(genesisRequest, m_TokenSource.Token).Result;
                        if (!response.IsSuccessStatusCode || response.Content == null)
                        {
                            Status = RequestStatus.Failed;
                            return;
                        }

                        var responseFromGenesis = response.Content.ReadAsStringAsync().Result;
                        if (string.IsNullOrEmpty(responseFromGenesis))
                        {
                            Status = RequestStatus.Failed;
                            return;
                        }

                        if (responseFromGenesis.ToLower().Contains("error"))
                        {
                            Debug.Log($"Genesis request error loading key: {key}\n{responseFromGenesis}");
                            Status = RequestStatus.Failed;
                            return;
                        }

                        var securedObject = ParseJsonToGenesisSecuredObject(responseFromGenesis);

                        // ReSharper disable once StringLiteralTypo
                        if (!securedObject.url.Contains("Goog-Algorithm") && securedObject.object_rename == key) // URL has Google Authentication and key matches
                        {
                            Status = RequestStatus.Failed;
                            return;
                        }

                        var request = new HttpRequestMessage(httpMethod, securedObject.url);
                        var headers = request.Headers;
                        headers.Add("Cache-Control", k_CacheControlHeaderOptions);

                        SendRequest(client, request, payload);
                    }
                    catch (Exception exception)
                    {
                        Debug.LogWarning($"Exception processing request {key} - {httpMethod.Method}:");
                        Debug.LogException(exception);
                        Status = RequestStatus.Canceled;
                    }
                });

                m_SendThread.Start();
            }

            public void Send(HttpClient client, HttpRequestMessage request, int timeout, byte[] payload = null)
            {
                if (timeout > 0)
                    m_TokenSource.CancelAfter(new TimeSpan(0, 0, timeout));

                StartTime = Time.realtimeSinceStartup;
                m_SendThread = new Thread(() =>
                {
                    SendRequest(client, request, payload);
                });

                m_SendThread.Start();
            }

            void SendRequest(HttpClient client, HttpRequestMessage request, byte[] payload)
            {
                if (payload != null && payload.Length > 0)
                {
                    m_TotalBytesToUpload = payload.Length;
                    var requestContent = new MemoryStreamContent(payload);
                    requestContent.OnStarted += () => Status = RequestStatus.Uploading;
                    requestContent.OnProgress += bytesUploaded => m_BytesUploaded = bytesUploaded;
                    request.Content = requestContent;
                    requestContent.Headers.ContentType = MediaTypeHeaderValue.Parse(HttpContentTypes.ApplicationOctetStream);
                }

                try
                {
                    var response = client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, m_TokenSource.Token).Result;

                    m_StatusCode = response.StatusCode;
                    var content = response.Content;
                    if (!response.IsSuccessStatusCode)
                    {
                        Status = RequestStatus.Failed;
                        if (content != null)
                            m_ResponseBytes = content.ReadAsByteArrayAsync().Result;

                        return;
                    }

                    if (content == null)
                    {
                        Status = RequestStatus.Succeeded;
                        return;
                    }

                    var contentLength = content.Headers.ContentLength;

                    // TODO: read stream content with unspecified length
                    if (contentLength == null)
                    {
                        Debug.LogError("Response did not specify content length");
                        Status = RequestStatus.Failed;
                        return;
                    }

                    Status = RequestStatus.Downloading;
                    m_TotalBytesToDownload = contentLength.Value;
                    var stream = content.ReadAsStreamAsync().Result;
                    m_ResponseBytes = new byte[m_TotalBytesToDownload];
                    int bytesRead;
                    var count = Math.Min(k_BufferSize, (int)m_TotalBytesToDownload);
                    while ((bytesRead = stream.Read(m_ResponseBytes, m_BytesDownloaded, count)) > 0)
                    {
                        m_BytesDownloaded += bytesRead;
                        count = Math.Min(k_BufferSize, (int)(m_TotalBytesToDownload - m_BytesDownloaded));
                    }

                    Status = RequestStatus.Succeeded;
                }
                catch (Exception exception)
                {
                    Debug.LogWarning($"Exception processing request {request.RequestUri}:");
                    Debug.LogException(exception);
                    Status = RequestStatus.Canceled;
                }
            }

            public void ReadFile(string path)
            {
                StartTime = Time.realtimeSinceStartup;
                m_SendThread = new Thread(() =>
                {
                    FileStream file;
                    try
                    {
                        file = new FileStream(path, FileMode.Open);
                        var length = (int)file.Length;
                        m_ResponseBytes = new byte[length];
                        Status = RequestStatus.Downloading;
                        file.ReadAsync(m_ResponseBytes, 0, length).Wait();
                    }
                    catch (Exception e)
                    {
                        Debug.LogException(e);
                        Status = RequestStatus.Failed;
                        return;
                    }

                    try
                    {
                        file.Close();
                    }
                    catch (Exception e)
                    {
                        Debug.LogException(e);
                        Status = RequestStatus.Failed;
                    }

                    Status = RequestStatus.Succeeded;
                });

                m_SendThread.Start();
            }

            public virtual void Update()
            {
                var uploadProgress = 0f;
                if (m_TotalBytesToUpload > 0)
                    uploadProgress = (float)m_BytesUploaded / m_TotalBytesToUpload;

                var downloadProgress = 0f;
                if (m_TotalBytesToDownload > 0)
                    downloadProgress = (float)m_BytesDownloaded / m_TotalBytesToDownload;

                m_Progress?.Invoke(uploadProgress, downloadProgress);
            }

            public void Cancel()
            {
                m_TokenSource.Cancel();
                m_ResponseBytes = k_RequestCanceledMessage;
                Status = RequestStatus.Canceled;
            }

            public void Dispose()
            {
                m_TokenSource.Dispose();
                m_SendThread?.Abort();
            }

            public bool IsDone()
            {
                return Status == RequestStatus.Succeeded || Status == RequestStatus.Canceled || Status == RequestStatus.Failed;
            }
        }

        class BytesRequest : Request
        {
            readonly Action<bool, long, byte[]> m_Callback;

            public BytesRequest(Action<bool, long, byte[]> callback, ProgressCallback progress = null) : base(progress)
            {
                m_Callback = callback;
            }

            public override void Update()
            {
                base.Update();

                try
                {
                    switch (Status)
                    {
                        case RequestStatus.Canceled:
                        case RequestStatus.Failed:
                            m_Progress?.Invoke(1, 1);
                            m_Callback(false, (int)m_StatusCode, m_ResponseBytes);
                            break;

                        case RequestStatus.Succeeded:
                            m_Progress?.Invoke(1, 1);
                            m_Callback(true, (int)m_StatusCode, m_ResponseBytes);
                            break;
                    }
                }
                catch (Exception exception)
                {
                    Debug.LogException(exception);
                }
            }

            public override string ToString()
            {
                return $"BytesRequest - {StartTime} - {Status}";
            }
        }

        class TextureRequest : Request
        {
            LoadTextureCallback m_Callback;

            public TextureRequest(LoadTextureCallback callback, ProgressCallback progress = null) : base(progress)
            {
                m_Callback = callback;
            }

            public override void Update()
            {
                base.Update();

                try
                {
                    switch (Status)
                    {
                        case RequestStatus.Canceled:
                        case RequestStatus.Failed:
                            m_Progress?.Invoke(1, 1);
                            m_Callback(false, (int)m_StatusCode, null, null);
                            break;

                        case RequestStatus.Succeeded:
                            var texture = new Texture2D(2, 2);
                            texture.LoadImage(m_ResponseBytes);
                            m_Progress?.Invoke(1, 1);
                            m_Callback(true, (int)m_StatusCode, texture, m_ResponseBytes);
                            break;
                    }
                }
                catch (Exception exception)
                {
                    Debug.LogException(exception);
                }
            }

            public override string ToString()
            {
                return $"TextureRequest - {StartTime}";
            }
        }

        // ReSharper disable InconsistentNaming
        [Serializable]
        class GenesisSecuredObject
        {
            public string object_rename;
            public string url;

            public GenesisSecuredObject(string objectRename, string url)
            {
                this.url = url;
                object_rename = objectRename;
            }
        }

        [Serializable]
        class GenesisSecuredURLObjectListing
        {
            public List<GenesisSecuredObject> objects;

            public GenesisSecuredURLObjectListing(List<GenesisSecuredObject> objects) { this.objects = objects; }
        }
        // ReSharper restore InconsistentNaming

        /// <summary>
        /// Set the current project identifier
        /// </summary>
        public void SetProjectIdentifier(string id) { m_ProjectIdentifier = id; }

        /// <summary>
        /// Get the current project identifier
        /// </summary>
        public string GetProjectIdentifier() { return m_ProjectIdentifier; }

        /// <summary>
        /// Save to the cloud asynchronously the data of an object of a certain type with a specified key
        /// </summary>
        /// <param name="key"> string that uniquely identifies this instance of the type. </param>
        /// <param name="serializedObject"> string serialization of the object being saved. </param>
        /// <param name="callback"> a callback when the asynchronous call is done to show whether it was successful,
        /// with the response code and string. </param>
        /// <param name="progress">Called every frame while the request is in progress with two 0-1 values indicating
        /// upload and download progress, respectively</param>
        /// <param name="timeout">The timeout duration (in seconds) for this request</param>
        /// <returns>A handle to this request which can be used to cancel it</returns>
        public RequestHandle CloudSaveAsync(string key, string serializedObject, Action<bool, long, string> callback = null,
            ProgressCallback progress = null, int timeout = CloudStorageDefaults.DefaultTimeout)
        {
            return CloudSaveAsyncBytes(key, Encoding.UTF8.GetBytes(serializedObject), timeout, callback, progress);
        }

        /// <summary>
        /// Save to the cloud asynchronously data in a byte array with a specified key
        /// </summary>
        /// <param name="key"> string that uniquely identifies this instance of the type. </param>
        /// <param name="bytes">Bytes array of the object being saved</param>
        /// <param name="callback"> a callback when the asynchronous call is done to show whether it was successful,
        /// with the response code and string. </param>
        /// <param name="progress">Called every frame while the request is in progress with two 0-1 values indicating
        /// upload and download progress, respectively</param>
        /// <param name="timeout">The timeout duration (in seconds) for this request</param>
        /// <returns>A handle to this request which can be used to cancel it</returns>
        public RequestHandle CloudSaveAsync(string key, byte[] bytes, Action<bool, long, string> callback = null,
            ProgressCallback progress = null, int timeout = CloudStorageDefaults.DefaultTimeout)
        {
            return CloudSaveAsyncBytes(key, bytes, timeout, callback, progress);
        }

        RequestHandle CloudSaveAsyncBytes(string key, byte[] bytes, int timeout, Action<bool, long, string> callback,
            ProgressCallback progress)
        {
            if (!IsValidKey(key))
            {
                Debug.LogWarning($"Invalid cloud storage key: {key}");
                return default;
            }

            if (m_OfflineMode)
            {
                callback?.Invoke(false, 0, null);
                return default;
            }

            var httpMethod = HttpMethod.Put;
            if (bytes == null || bytes.Length == 0)
                httpMethod = HttpMethod.Delete;

            var genesisRequest = GenesisRequestBuilder(key, httpMethod);
            if (genesisRequest == null)
            {
                callback?.Invoke(false, 0, null);
                return default;
            }

            var requestHandle = RequestHandle.Create($"{key} - {httpMethod.Method}");
            void ConvertResponseToString(bool success, long responseCode, byte[] responseBytes)
            {
                var response = responseBytes == null ? null : Encoding.UTF8.GetString(responseBytes);
                callback?.Invoke(success, responseCode, response);
            }

            var request = new BytesRequest(ConvertResponseToString, progress);
            m_Requests[requestHandle] = request;
            request.Send(m_Client, genesisRequest, key, httpMethod, timeout, bytes);
            return requestHandle;
        }

        static GenesisSecuredObject ParseJsonToGenesisSecuredObject(string responseFromGenesis)
        {
            var jsonString = responseFromGenesis.Replace("\"object\"", "\"object_rename\""); // Replace C# reserved words
            var securedObjectListing = JsonUtility.FromJson<GenesisSecuredURLObjectListing>(jsonString);
            var securedObject = securedObjectListing.objects[0];
            return securedObject;
        }

        HttpRequestMessage GenesisRequestBuilder(string key, HttpMethod httpMethod)
        {
            var projectId = m_ProjectIdentifier;
            if (!ProjectIdentifierIsValid(projectId))
                return null;

            if (IdentityProvider == null)
                return null;

            var request = new HttpRequestMessage(HttpMethod.Post, k_GetObjectsUri);
            var headers = request.Headers;
            headers.Add("Authorization", $"Bearer {IdentityProvider.Token}");
            headers.Add("Cache-Control", "max-age=0, no-cache, no-store");
            headers.Add("Pragma", "no-cache");
            var secureLinkRequestBody = $@"{{""project_id"":""{projectId}"", ""object_list"":[""{key}""], ""http_method"":""{httpMethod.Method}""}}";
            request.Content = new StringContent(secureLinkRequestBody, Encoding.UTF8, k_ApplicationJsonContentType);
            return request;
        }

        /// <summary>
        /// Load from the cloud asynchronously the data of an object which was saved with a known key
        /// </summary>
        /// <param name="key"> string that uniquely identifies this instance of the type. </param>
        /// <param name="callback">a callback which returns whether the operation was successful, as well as the
        /// serialized string of the object if it was. </param>
        /// <param name="progress">Called every frame while the request is in progress with two 0-1 values indicating
        /// upload and download progress, respectively</param>
        /// <param name="timeout">The timeout duration (in seconds) for this request</param>
        public RequestHandle CloudLoadAsync(string key, Action<bool, long, string> callback, ProgressCallback progress = null,
            int timeout = CloudStorageDefaults.DefaultTimeout)
        {
            void ConvertResponseToString(bool success, long responseCode, byte[] bytes)
            {
                string response = null;
                if (bytes != null)
                    response = Encoding.UTF8.GetString(bytes);

                callback(success, responseCode, response);
            }

            return CloudLoadAsyncBytes(key, ConvertResponseToString, timeout, progress);
        }

        /// <summary>
        /// Load from the cloud asynchronously the byte array which was saved with a known key
        /// </summary>
        /// <param name="key"> string that uniquely identifies this instance of the type. </param>
        /// <param name="callback">a callback which returns whether the operation was successful, as well as the
        /// response code and byte array if it was. If the operation failed, the byte array will contain the error string</param>
        /// <param name="progress">Called every frame while the request is in progress with two 0-1 values indicating
        /// upload and download progress, respectively</param>
        /// <param name="timeout">The timeout duration (in seconds) for this request</param>
        public RequestHandle CloudLoadAsync(string key, Action<bool, long, byte[]> callback, ProgressCallback progress = null,
            int timeout = CloudStorageDefaults.DefaultTimeout)
        {
            return CloudLoadAsyncBytes(key, callback, timeout, progress);
        }

        RequestHandle CloudLoadAsyncBytes(string key, Action<bool, long, byte[]> callback, int timeout, ProgressCallback progress)
        {
            if (callback == null)
            {
                Debug.LogWarning($"{nameof(CloudLoadAsync)} callback is null");
                return default;
            }

            if (!IsValidKey(key))
            {
                Debug.LogWarning($"Invalid cloud storage key: {key}");
                return default;
            }

            if (m_OfflineMode)
            {
                progress?.Invoke(1, 1);
                callback(false, 0, null);
                return default;
            }

            var httpMethod = HttpMethod.Get;
            var requestToGenesis = GenesisRequestBuilder(key, httpMethod);
            if (requestToGenesis == null)
            {
                progress?.Invoke(1, 1);
                callback(false, 0, null);
                return default;
            }

            var requestHandle = RequestHandle.Create($"{key} - {httpMethod}");
            var request = new BytesRequest(callback, progress);
            m_Requests[requestHandle] = request;
            request.Send(m_Client, requestToGenesis, key, httpMethod, timeout);
            return requestHandle;
        }

        /// <summary>
        /// Load a texture asynchronously from cloud storage
        /// </summary>
        /// <param name="key">String that uniquely identifies this instance of the type. </param>
        /// <param name="callback">A callback which returns whether the operation was successful, as well as the
        /// response payload if it was. If the operation failed, the byte array will contain the error string</param>
        /// <param name="progress">Called every frame while the request is in progress with two 0-1 values indicating
        /// upload and download progress, respectively</param>
        /// <param name="timeout">The timeout duration (in seconds) for this request</param>
        public RequestHandle CloudLoadTextureAsync(string key, LoadTextureCallback callback, ProgressCallback progress = null,
            int timeout = CloudStorageDefaults.DefaultTimeout)
        {
            return CloudLoadAsyncTexture(key, callback, timeout, progress);
        }

        /// <summary>
        /// Load a texture asynchronously from local storage
        /// </summary>
        /// <param name="path">Path to the texture. </param>
        /// <param name="callback">A callback which returns whether the operation was successful, as well as the
        /// response payload if it was. If the operation failed, the byte array will contain the error string</param>
        /// <param name="progress">Called every frame while the request is in progress with two 0-1 values indicating
        /// upload and download progress, respectively</param>
        /// <param name="timeout">The timeout duration (in seconds) for this request</param>
        public RequestHandle LoadLocalTextureAsync(string path, LoadTextureCallback callback, ProgressCallback progress = null,
            int timeout = CloudStorageDefaults.DefaultTimeout)
        {
            if (callback == null)
            {
                Debug.LogWarning($"{nameof(LoadLocalTextureAsync)} callback is null");
                return default;
            }

            if (m_OfflineMode)
            {
                progress?.Invoke(1, 1);
                callback(false, 0, null, null);
                return default;
            }

            var requestHandle = RequestHandle.Create($"{path} - file");
            var request = new TextureRequest(callback, progress);
            m_Requests[requestHandle] = request;
            request.ReadFile(path);
            return requestHandle;
        }

        /// <summary>
        /// Cancel a request with the given request handle
        /// </summary>
        /// <param name="handle">The handle to the request, which was returned by the method which initiated it</param>
        public void CancelRequest(RequestHandle handle)
        {
            if (m_Requests.TryGetValue(handle, out var request))
            {
                // Remove the request before canceling it in case callbacks try to modify or cancel requests
                m_Requests.Remove(handle);
                request.Cancel();
            }
        }

        /// <summary>
        /// Cancel all currently active requests
        /// </summary>
        public void CancelAllRequests()
        {
            if (m_Requests.Count > 0)
                Debug.LogWarning(k_CancelAllRequestsWarning);

            CancelAllRequests(m_Requests);
        }

        static void CancelAllRequests(Dictionary<RequestHandle, Request> requests)
        {
            k_RequestsCopy.Clear();
            foreach (var kvp in requests)
            {
                k_RequestsCopy[kvp.Key] = kvp.Value;
            }

            // Clear requests before canceling them in case callbacks try to modify or cancel requests
            requests.Clear();
            foreach (var kvp in k_RequestsCopy)
            {
                kvp.Value.Cancel();
            }

            k_RequestsCopy.Clear();
        }

        RequestHandle CloudLoadAsyncTexture(string key, LoadTextureCallback callback, int timeout,  ProgressCallback progress)
        {
            if (callback == null)
            {
                Debug.LogWarning($"{nameof(CloudLoadTextureAsync)} callback is null");
                return default;
            }

            if (m_OfflineMode)
            {
                progress?.Invoke(1, 1);
                callback(false, 0, null, null);
                return default;
            }

            var httpMethod = HttpMethod.Get;
            var requestToGenesis = GenesisRequestBuilder(key, httpMethod);
            if (requestToGenesis == null)
            {
                callback(false, 0, null, null);
                return default;
            }

            var requestHandle = RequestHandle.Create($"{key} - {httpMethod.Method}");
            var request = new TextureRequest(callback, progress);
            m_Requests[requestHandle] = request;
            request.Send(m_Client, requestToGenesis, key, httpMethod, timeout);
            return requestHandle;
        }

        /// <summary>
        /// Checks if the provided Project Identifier is valid
        /// </summary>
        /// <param name="projectID"></param>
        /// <returns>True if the project identifier is valid, false otherwise.</returns>
        static bool ProjectIdentifierIsValid(string projectID)
        {
            // Basic check, to be made more comprehensive with Genesis
            return projectID != null && projectID.Length > 32;
        }

        void UpdateRequests()
        {
            UpdateRequests(m_Requests);
        }

        void UpdateRequests(Dictionary<RequestHandle, Request> requests)
        {
            if (requests.Count == 0)
                return;

            k_RequestsCopy.Clear();
            foreach (var kvp in requests)
            {
                k_RequestsCopy[kvp.Key] = kvp.Value;
            }

            var time = Time.realtimeSinceStartup;
            foreach (var kvp in k_RequestsCopy)
            {
                var request = kvp.Value;
                var elapsed = time - request.StartTime;
                if (elapsed < m_TestingDelay)
                    continue;

                request.Update();

                if (request.IsDone())
                {
                    request.Dispose();
                    m_Requests.Remove(kvp.Key);
                    continue;
                }

                if (elapsed > k_RequestHandshakeTimeout)
                {
                    // Remove the request before canceling it in case callbacks try to modify or cancel requests
                    requests.Remove(kvp.Key);
                    request.Cancel();
                }
            }

            k_RequestsCopy.Clear();
        }

        void IModule.LoadModule()
        {
#if UNITY_EDITOR
            EditorApplication.update += UpdateRequests;

            // Play mode should act the same as player builds
            if (EditorApplication.isPlayingOrWillChangePlaymode)
                return;

            CloudProjectSettings.RefreshAccessToken(null);
            IdentityProvider = new OAuthTokenGenesisModule();
            CoroutineUtils.StartCoroutine(IdentityProvider.SignIn(onlyCached: true));
#endif
        }

        void IModule.UnloadModule()
        {
#if UNITY_EDITOR
            // ReSharper disable once DelegateSubtraction
            EditorApplication.update -= UpdateRequests;
#endif
        }

        void IModuleBehaviorCallbacks.OnBehaviorAwake() { }

        void IModuleBehaviorCallbacks.OnBehaviorEnable() { }

        void IModuleBehaviorCallbacks.OnBehaviorStart() { }

        void IModuleBehaviorCallbacks.OnBehaviorUpdate()
        {
#if !UNITY_EDITOR
            UpdateRequests(m_Requests);
#endif
        }

        void IModuleBehaviorCallbacks.OnBehaviorDisable() { }

        void IModuleBehaviorCallbacks.OnBehaviorDestroy() { }

        void IFunctionalityProvider.LoadProvider() { }

        void IFunctionalityProvider.ConnectSubscriber(object obj) { this.TryConnectSubscriber<IProvidesCloudStorage>(obj); }

        void IFunctionalityProvider.UnloadProvider() { }

        /// <summary>
        /// Perform a get request to the given uri, adding the given token to the request header
        /// </summary>
        /// <param name="uri">The web address of the object to be gotten</param>
        /// <param name="authToken">The authentication token to be added to the request header</param>
        /// <param name="callback">An action to be called when the request is completed, whose parameters are:
        ///     (bool) success    - Whether or not the request was successful
        ///     (string) response - A string value containing the server's response</param>
        /// <param name="timeout">The timeout duration (in seconds) for this request</param>
        public RequestHandle AuthenticatedGetRequest(string uri, string authToken, Action<bool, string> callback,
            int timeout = CloudStorageDefaults.DefaultTimeout)
        {
            var cancellationTimeout = Timeout.InfiniteTimeSpan;
            if (timeout > 0)
                cancellationTimeout = new TimeSpan(0, 0, timeout);

            var cancellationToken = new CancellationTokenSource();
            cancellationToken.CancelAfter(cancellationTimeout);
            var httpRequest = new HttpRequestMessage(HttpMethod.Get, uri);
            var headers = httpRequest.Headers;
            headers.Add("Cache-Control", "no-cache");
            headers.Add("Authorization", $"Bearer {authToken}");
            var requestHandle = RequestHandle.Create($"{uri} - GET");

            void ConvertResponseToString(bool success, long responseCode, byte[] response)
            {
                callback(success, response != null ? Encoding.UTF8.GetString(response) : null);
            }

            var request = new BytesRequest(ConvertResponseToString);
            request.Send(m_Client, httpRequest, timeout);
            m_Requests[requestHandle] = request;
            return requestHandle;
        }

        /// <summary>
        /// Perform a get request to the given uri, adding the given token to the request header
        /// </summary>
        /// <param name="uri">The web address of the object to be gotten</param>
        /// <param name="callback">An action to be called when the request is completed, whose parameters are:
        ///     (bool) success    - Whether or not the request was successful
        ///     (string) response - A string value containing the server's response</param>
        /// <param name="timeout">The timeout duration (in seconds) for this request</param>
        public RequestHandle GetRequest(string uri, Action<bool, string> callback, int timeout = CloudStorageDefaults.DefaultTimeout)
        {
            var cancellationTimeout = Timeout.InfiniteTimeSpan;
            if (timeout > 0)
                cancellationTimeout = new TimeSpan(0, 0, timeout);

            var cancellationToken = new CancellationTokenSource();
            cancellationToken.CancelAfter(cancellationTimeout);
            var httpRequest = new HttpRequestMessage(HttpMethod.Get, uri);
            var requestHandle = RequestHandle.Create($"{uri} - GET");

            void ConvertResponseToString(bool success, long responseCode, byte[] response)
            {
                callback(success, response != null ? Encoding.UTF8.GetString(response) : null);
            }

            var request = new BytesRequest(ConvertResponseToString);
            request.Send(m_Client, httpRequest, timeout);
            m_Requests[requestHandle] = request;
            return requestHandle;
        }

        internal static bool IsValidKeySubstring(string keySubstring)
        {
            if (Regex.IsMatch(keySubstring, k_InvalidKeySubstringRegex))
                return false;

            try
            {
                XmlConvert.VerifyXmlChars(keySubstring);
            }
            catch
            {
                return false;
            }

            return true;
        }

        static bool IsValidKey(string key)
        {
            if (key == "." || key == "..")
                return false;

            if (Encoding.UTF8.GetByteCount(key) > MaxKeyLengthBytes)
                return false;

            if (Regex.IsMatch(key, k_InvalidKeyRegex))
                return false;

            try
            {
                XmlConvert.VerifyXmlChars(key);
            }
            catch
            {
                return false;
            }

            return true;
        }
    }
}
