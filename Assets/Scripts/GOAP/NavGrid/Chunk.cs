using System;
using System.Collections.Generic;
using UnityEngine;

public class Chunk
{
    public const int SXZ = 10;

    public readonly CIndex cIndex;
    public readonly Array3d<GridPoint> Points;
    public readonly List<short> Indices = new ();
    
    private const int DISTANCE_NVP = 15 * 15;
    private const float DISTANCE_TO_FLOOR = 0.7F;
    private static int _chunkMaxHeight = 1;
    private readonly HashSet<CIndex> _calcPoints = new ();
    private bool _pointsIsGenerated = false;
    private readonly int sy;

    public Chunk(CIndex cIndex, int sy)
    {
        this.cIndex = cIndex;
        this.sy = sy;

        Points = new Array3d<GridPoint>(SXZ, sy, SXZ);
        _chunkMaxHeight = Math.Max(_chunkMaxHeight, sy);
    }

    public bool GeneratePoints(IGenerateCallback generateCallback)
    {
        if (_pointsIsGenerated) return false;
        Indices.Clear();
        Vector3 position = cIndex.GetPosition(SXZ, 0);
        for (int y = 0, i = 0; y < sy; y++)
        {
            for (var z = 0; z < SXZ; z++)
            {
                for (var x = 0; x < SXZ; x++, i++)
                {
                    var pointPosition = new Vector3(x, y, z) + position;
                    if (!generateCallback.PointIsPossibleGenerate(pointPosition, out var distanceToFloor)) continue;
                    
                    pointPosition.y -= distanceToFloor;
                    pointPosition.y += DISTANCE_TO_FLOOR;
                    Points.Set(i, new GridPoint(this, pointPosition));
                    Indices.Add((short)i);
                }
            }
        }
        _pointsIsGenerated = true;
        return true;
    }

    public bool CalculateNotVisiblePoints(IGenerateCallback generateCallback, Chunk other)
    {
        if (other == null || _calcPoints.Contains(other.cIndex) || other._calcPoints.Contains(this.cIndex)) return false;
        if (!_pointsIsGenerated || !other._pointsIsGenerated) return false;
        for (var a = 0; a < Indices.Count; a++)
        {
            for (var b = 0; b < other.Indices.Count; b++)
            {
                var pa = Points.Get(Indices[a]);
                var pb = other.Points.Get(other.Indices[b]);
                
                if ((pa.Position - pb.Position).sqrMagnitude > DISTANCE_NVP) continue;
                if (pa == pb || !generateCallback.PointsIsNotVisible(pa.Position, pb.Position)) continue;
                
                pa.NotVisiblePoints.Add(pb);
                pb.NotVisiblePoints.Add(pa);
            }
        }
        
        _calcPoints.Add(other.cIndex);
        if (!Equals(other, this))
        {
            other._calcPoints.Add(cIndex);
        }
        return true;
    }

    public bool CalculateNotVisiblePoints(IGenerateCallback generateCallback)
    {
        return CalculateNotVisiblePoints(generateCallback, this);
    }

    public override bool Equals(object obj)
    {
        return obj is Chunk chunk &&
               EqualityComparer<CIndex>.Default.Equals(cIndex, chunk.cIndex);
    }

    public override int GetHashCode()
    {
        return cIndex.GetHashCode();
    }

    public override string ToString()
    {
        return cIndex + " Points:" + Indices.Count;
    }

    public bool PointsIsCreated()
    {
        return _pointsIsGenerated;
    }

    public static int GetChunkMaxHeight()
    {
        return _chunkMaxHeight;
    }

    public bool NVPIsGenerated(CIndex index)
    {
        return _calcPoints.Contains(index);
    }
}
