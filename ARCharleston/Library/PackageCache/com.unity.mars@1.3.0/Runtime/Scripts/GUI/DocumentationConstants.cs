namespace Unity.MARS
{
    internal class DocumentationConstants
    {
        const string k_ManualLatest =
            "https://docs.unity3d.com/Packages/com.unity.mars@latest/index.html?subfolder=/manual/";
        const string k_PageAndAnchor = ".html%23";

        const string k_MarsConcepts = "MARSConcepts";

        const string k_ConceptsBase = k_ManualLatest + k_MarsConcepts + k_PageAndAnchor;

        internal const string ProxyDocs = k_ConceptsBase + "proxy";
        internal const string ProxyGroupDocs = k_ConceptsBase + "proxy-groups";
        internal const string ReplicatorsDocs = k_ConceptsBase + "replicators";
        internal const string SessionDocs = k_ConceptsBase + "the-mars-session-and-the-camera";
    }
}
