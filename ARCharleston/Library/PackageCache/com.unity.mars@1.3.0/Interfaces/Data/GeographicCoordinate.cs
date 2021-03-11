using System;
using UnityEngine;

namespace Unity.MARS.Data
{
    [Serializable]
    public struct GeographicCoordinate
    {
        public static readonly GeographicCoordinate Invalid = new GeographicCoordinate();

        public double latitude;
        public double longitude;

        public GeographicCoordinate(double latitude, double longitude)
        {
            this.latitude = latitude;
            this.longitude = longitude;
        }

        public override string ToString()
        {
            return String.Format("latitude: {0} , longitude: {1}", latitude, longitude);
        }

        public static implicit operator Vector2(GeographicCoordinate coordinate)
        {
            return new Vector2((float)coordinate.latitude, (float)coordinate.longitude);
        }

        public static implicit operator GeographicCoordinate(Vector2 vec)
        {
            return new GeographicCoordinate(vec.x, vec.y);
        }

        public static implicit operator GeographicCoordinate(LocationInfo location)
        {
            return new GeographicCoordinate(location.latitude, location.longitude);
        }
    }
}
