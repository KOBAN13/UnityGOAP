using System;
using UnityEngine;

public class Array3d<T>
{
    public readonly T[] array;
    public readonly int sx, sy, sz;
    private readonly int _sxz;

    public Array3d(int sx, int sy, int sz)
    {
        if (sx <= 0 || sy <= 0 || sz <= 0)
        {
            throw new Exception("sx<=0 || sy<=0 || sz <= 0");
        }
        this.sx = sx;
        this.sy = sy;
        this.sz = sz;

        _sxz = sx * sz;

        array = new T[sx * sy * sz];
    }

    public int GetIndex(int x, int y, int z)
    {
        return x + z * sx + y * _sxz;
    }

    public T Set(int index, T value)
    {
        T res = array[index];
        array[index] = value;
        return res;
    }
    public T Set(int x, int y, int z, T value)
    {
        return Set(GetIndex(x, y, z), value);
    }
    
    public T Get(int index) { return array[index]; }
    
    public T Get(int x, int y, int z) { return Get(GetIndex(x, y, z)); }
    
    public T Get(int index, T def) { return (index >= 0 && index < array.Length) ? array[index] : def; }

    public T Get(Vector3Int vector)
    {
        return Get(GetIndex(vector.x, vector.y, vector.z));
    }
}
