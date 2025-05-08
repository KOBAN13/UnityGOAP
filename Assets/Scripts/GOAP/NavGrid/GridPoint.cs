using System.Collections.Generic;
using UnityEngine;

public class GridPoint
{
    public readonly Chunk Chunk;
    public readonly List<GridPoint> NotVisiblePoints = new List<GridPoint>();
    public readonly Vector3 Position;
    public bool isFree = true;

    public GridPoint(Chunk chunk, Vector3 position)
    {
        Chunk = chunk;
        Position = position;
    }

    public override string ToString()
    {
        return Chunk + " | " + Position + " | " + isFree;
    }

    public GridPoint FindFreeNvPointMinDistance(Vector3 position)
    {
        float distance = float.MaxValue;
        GridPoint res = null;
        for (int i = 0, l = NotVisiblePoints.Count; i < l; i++)
        {
            GridPoint p = NotVisiblePoints[i];
            if (p == null || !p.isFree)
            {
                continue;
            }
            float dist = (p.Position - position).sqrMagnitude;
            if (dist < distance)
            {
                distance = dist;
                res = p;
            }
        }
        return res;
    }
}