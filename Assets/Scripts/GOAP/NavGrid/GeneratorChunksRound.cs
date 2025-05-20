using System;
using UnityEngine;

//TODO REFACTOR NAV GRID ALL
public class GeneratorChunksRound
{
    private readonly PointMap _map;
    private readonly Transform _position;
    private readonly float _distanceChunkGenerate;
    private CIndex _prevCIndex = new (int.MaxValue, int.MaxValue);

    public GeneratorChunksRound(PointMap map, Transform position, float distanceChunkGenerate)
    {
        _map = map;
        _position = position;
        _distanceChunkGenerate = Math.Max(distanceChunkGenerate, 1);
    }

    public void Update()
    {
        var index = _map.GetCIndexAtPosition(_position.position);
        
        if (index.Equals(_prevCIndex)) return;
        _prevCIndex = index;
        GenerateRoundChunks();
    }

    private void GenerateRoundChunks()
    {
        var countChunks = (int)Math.Max(_distanceChunkGenerate / Chunk.SXZ, 2);
        for (var z = _prevCIndex.Z - countChunks; z <= _prevCIndex.Z + countChunks; z++)
        {
            for (var x = _prevCIndex.X - countChunks; x <= _prevCIndex.X + countChunks; x++)
            {
                _map.CreateGenerateChunkTack(new CIndex(x, z));
            }
        }
    }
}
