using System;
using Unity.MARS.Data;
using Unity.MARS.Settings;
using Unity.XRTools.Utils;
using UnityEditor.XRTools.Utils;
using UnityEngine;
using UnityEngine.Scripting.APIUpdating;

namespace UnityEditor.MARS.Data
{
    [ScriptableSettingsPath(MARSCore.UserSettingsFolder)]
    [MovedFrom("Unity.MARS")]
    public sealed class MarsUserGeoLocations : EditorScriptableSettings<MarsUserGeoLocations>
    {
        static readonly UserGeoLocation[] k_DefaultLocations =
        {
            new UserGeoLocation(new GeographicCoordinate(37.7749,-122.4194),"San Francisco, United States"),
            new UserGeoLocation(new GeographicCoordinate(47.6062,-122.3321),"Seattle, United States"),
            new UserGeoLocation(new GeographicCoordinate(30.2672,-97.7431),"Austin, United States"),
            new UserGeoLocation(new GeographicCoordinate(40.4406,-79.9959),"Pittsburgh, United States"),
            new UserGeoLocation(new GeographicCoordinate(4.8087,-75.6906),"Pereira, Colombia"),
            new UserGeoLocation(new GeographicCoordinate(45.5017,-73.5673),"Montreal, Canada"),
            new UserGeoLocation(new GeographicCoordinate(51.4545,-2.5879),"Bristol, United Kingdom"),
            new UserGeoLocation(new GeographicCoordinate(52.1917,-1.7083),"Stratford-upon-Avon, United Kingdom"),
            new UserGeoLocation(new GeographicCoordinate(50.8225,-0.1372),"Brighton, United Kingdom"),
            new UserGeoLocation(new GeographicCoordinate(51.5074,-0.1278),"London, United Kingdom"),
            new UserGeoLocation(new GeographicCoordinate(48.8566,2.3522),"Paris, France"),
            new UserGeoLocation(new GeographicCoordinate(45.1885,5.7245),"Grenoble, France"),
            new UserGeoLocation(new GeographicCoordinate(55.6761,12.5683),"Copenhagen, Denmark"),
            new UserGeoLocation(new GeographicCoordinate(52.5200,13.4050),"Berlin, Germany"),
            new UserGeoLocation(new GeographicCoordinate(59.3293,18.0686),"Stockholm, Sweden"),
            new UserGeoLocation(new GeographicCoordinate(54.8985,23.9036),"Kaunas, Lithuania"),
            new UserGeoLocation(new GeographicCoordinate(60.1699,24.9384),"Helsinki, Finland"),
            new UserGeoLocation(new GeographicCoordinate(54.6872,25.2797),"Vilnius, Lithuania"),
            new UserGeoLocation(new GeographicCoordinate(1.3521,103.8198),"Singapore, Singapore"),
            new UserGeoLocation(new GeographicCoordinate(37.5665,126.9780),"Seoul, South Korea"),
            new UserGeoLocation(new GeographicCoordinate(31.2304,121.4737),"Shanghai, China"),
            new UserGeoLocation(new GeographicCoordinate(35.6804,139.7690),"Tokyo, Japan")
        };

        [SerializeField]
        UserGeoLocation[] m_UserGeoLocations = k_DefaultLocations;

        public UserGeoLocation[] UserGeoLocations => m_UserGeoLocations;

        void OnValidate()
        {
            if (m_UserGeoLocations.Length == 0)
                m_UserGeoLocations = k_DefaultLocations;
        }
    }

    [Serializable]
    [MovedFrom("Unity.MARS")]
    public sealed class UserGeoLocation
    {
        public static readonly UserGeoLocation NullIsland = new UserGeoLocation(
            new GeographicCoordinate(k_DefaultLatitude, k_DefaultLongitude), k_NullIslandName);

        const string k_NullIslandName = "Null Island, Middle of Nowhere";
        // default simulated coordinates are @ Null Island
        const float k_DefaultLatitude = 0f;
        const float k_DefaultLongitude = 0f;

        [SerializeField]
        GeographicCoordinate m_Location;

        [SerializeField]
        string m_Name;

        public GeographicCoordinate Location => m_Location;

        public string Name => m_Name;

        public UserGeoLocation()
        {
            m_Location = NullIsland.Location;
            m_Name = string.Empty;
        }

        public UserGeoLocation(GeographicCoordinate location, string name)
        {
            m_Location = location;
            m_Name = name;
        }

        public override string ToString()
        {
            return string.Format("{0} : {1}", Name, Location.ToString());
        }

        public override int GetHashCode()
        {
            return (Name.GetHashCode() * 397) ^ Location.GetHashCode();
        }
    }
}
