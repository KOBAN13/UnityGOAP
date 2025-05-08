using System;
using UnityEngine;

public readonly struct CIndex
{
    public static readonly CIndex NULL = new (int.MaxValue, int.MinValue);

    public readonly int X;
    public readonly int Z;


    public CIndex(int x, int z)
    {
        X = x;
        Z = z;
    }

    public override bool Equals(object obj)
    {
        return obj is CIndex index &&
               X == index.X &&
               Z == index.Z;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(X, Z);
    }

    public override string ToString()
    {
        return X + ", " + Z;
    }

    public Vector2 GetSize()
    {
        return new Vector2(Chunk.SXZ, Chunk.SXZ);
    }

    public Vector2 GetPosition()
    {
        return new Vector2(X, Z);
    }

    public Vector3Int GetPosition(int sxz, float y)
    {
        return new Vector3Int(X * sxz, (int)y, Z * sxz);
    }

    public static bool operator ==(CIndex a, CIndex b)
    {
        return a.X == b.X && a.Z == b.Z;
    }

    public static bool operator !=(CIndex a, CIndex b)
    {
        return a.X != b.X || a.Z != b.Z;
    }
}
