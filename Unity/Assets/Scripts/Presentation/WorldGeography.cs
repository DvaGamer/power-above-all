using System;
using UnityEngine;

namespace PowerAboveAll
{
    // Fiziksel dünya ile siyasi/oynanış sahipliği ayrı dosya ve kimlikler taşır.
    [Serializable] public sealed class GeoPath { public float[] points; }
    [Serializable] public sealed class GeoMesh
    {
        public string id, layer, regionId;
        public int lod;
        public float[] points, bounds;
        public int[] triangles;
        public GeoPath[] paths;
    }
    [Serializable] public sealed class GeoRiver
    {
        public string id, name;
        public int rank;
        public float[] points, bounds;
    }
    [Serializable] public sealed class PhysicalGeography
    {
        public int schema;
        public string crs, sources;
        public GeoMesh[] chunks;
        public GeoRiver[] rivers;
    }
    [Serializable] public sealed class PoliticalEntity
    {
        public string id, name, source, confidence;
        public GeoMesh[] areas;
    }
    [Serializable] public sealed class AtlasRegion
    {
        public string id, politicalEntityId, seatId, source, confidence;
        public GeoMesh[] areas;
    }
    [Serializable] public sealed class AtlasSettlement
    {
        public string id, name, regionId, politicalEntityId, source;
        public float longitude, latitude;
        public int rank;
    }
    [Serializable] public sealed class AtlasRoute
    {
        public string id, from, to, source, confidence;
        public float[] points;
    }
    [Serializable] public sealed class AtlasTerrain
    {
        public string id, kind, source, confidence;
        public float longitude, latitude, radius;
    }
    [Serializable] public sealed class AtlasWorld
    {
        public int schema, year;
        public PoliticalEntity[] entities;
        public AtlasRegion[] regions;
        public AtlasSettlement[] settlements;
        public AtlasRoute[] roads;
        public AtlasTerrain[] terrain;
    }
    [Serializable] public sealed class AtlasRelief { public int schema; public ReliefArea[] features; }
    [Serializable] public sealed class ReliefArea { public string id,name;public int rank;public float[] points,bounds; }
    public static class AtlasProjection
    {
        public const float LongitudeScale = 6.9465837f, LatitudeScale = 10;
        public static Vector3 Project(float longitude, float latitude, float height = 0) =>
            new Vector3(longitude * LongitudeScale, height, latitude * LatitudeScale);
        public static Vector2 Geographic(Vector3 point) => new Vector2(point.x / LongitudeScale, point.z / LatitudeScale);
    }
}
