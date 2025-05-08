using UnityEngine;

public interface IGenerateCallback
{
    public int GetChunkHeight(Vector2 position, Vector2 size);
    public bool PointIsPossibleGenerate(Vector3 position, out float distanceToFloor);
    public bool PointsIsNotVisible(Vector3 positionA, Vector3 positionB);
}
