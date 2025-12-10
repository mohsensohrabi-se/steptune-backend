using StepTune.Domain.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace StepTune.Domain.Workouts.ValueObjects
{
    public sealed class GeoPoint : IEquatable<GeoPoint>
    {
        public double Latitude { get; }
        public double Longitude { get; }

        private const double MinLat = -90.0;
        private const double MaxLat = 90.0;
        private const double MinLng = -180.0;
        private const double MaxLng = 180;

        private GeoPoint(double latitude,double longitude) 
        {
            Latitude = latitude;
            Longitude = longitude;
        }

        public static Result<GeoPoint> Create(double lat, double lng)
        {
            if (lat is <MinLat or > MaxLat)
                return Result.Failure<GeoPoint>("Latitude must be between -90 and +90");
            if (lng is <MinLng or >MaxLng)
                return Result.Failure<GeoPoint>("Longitude must be between -180 and +180");
            return Result.Success<GeoPoint>(new GeoPoint(lat,lng));
        }
        public bool Equals(GeoPoint? other)
        {
            if(other == null) return false;
            return Math.Abs(Latitude - other.Latitude)<0.000001 && 
                Math.Abs(Longitude -other.Longitude) < 0.000001;
        }
        public override bool Equals(object? obj)
        {
            return obj is GeoPoint other && Equals(other);
        }
        public override int GetHashCode()
        {
            return HashCode.Combine(Math.Round(Latitude,6), Math.Round(Longitude,6));
        }

        public override string ToString()
        {
            return $"{Latitude},{Longitude}";
        }
    }
}
