using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.AI.Navigation;
using UnityEngine;

public class NavGrid : MonoBehaviour
{
    [SerializeField] private Transform _mainPoint;
    [SerializeField] private Vector3 _lastMainPosition;
    [SerializeField] private GameObject _debugPrefab;
    private NavMeshSurface _navMesh;
    private bool _debug = true;
    private string _debugTag = "NavGridDebugSphere";
    private GameObject _debugContainer;
    public PointMap _pointMap;
    private GeneratorChunksRound _generatorChunksRound;

    private HashSet<CIndex> _createdDebugPoints = new ();

    private int _obstaclesLayers = 0;

    private class GridCallback : IGenerateCallback
    {
        private readonly int _layer;
        private readonly float _maxDistanceToFloor = 1.5F;
        private readonly Collider[] _tempColliders = new Collider[6];
        private readonly Collider[] _allColliders = null;

        public GridCallback(int layers)
        {
            _layer = layers; //1 << LayerMask.NameToLayer("Ground");
        }

        public int GetChunkHeight(Vector2 position, Vector2 size)
        {
            if (_allColliders == null)
            {
                 Physics.OverlapBoxNonAlloc(Vector3.zero, new Vector3(int.MaxValue, int.MaxValue, int.MaxValue), _allColliders);
            }
            
            return 10;
        }

        public bool PointIsPossibleGenerate(Vector3 position, out float distanceToFloor)
        {
            distanceToFloor = -1;
            var countColliders = Physics.OverlapSphereNonAlloc(position, 0.2F, _tempColliders, _layer);
            if (countColliders > 0)
            {
                return false;
            }
            
            Ray ray = new(position, Vector3.down);

            if (!Physics.Raycast(ray, out var raycastHit, _maxDistanceToFloor, _layer)) return false;
            distanceToFloor = raycastHit.distance;
            return true;

        }

        public bool PointsIsNotVisible(Vector3 positionA, Vector3 positionB)
        {
            Ray ray = new Ray(positionA, positionB - positionA);
            return Physics.Raycast(ray, (positionB - positionA).magnitude, _layer);
        }
    }

    public void RemoveObstaclesLayer(string layer)
    {
        RemoveObstaclesLayer(LayerMask.NameToLayer(layer));
    }

    private void Awake()
    {
        AddObstaclesLayer("Wall");
        AddObstaclesLayer("Ground");
        AddObstaclesLayer("Obstacle");

        _pointMap = new PointMap(new GridCallback(_obstaclesLayers));

        _generatorChunksRound = new GeneratorChunksRound(_pointMap, _mainPoint, 20);

        if (_debug)
        {
            _debugContainer = GameObject.Find("DebugContainer");
        }

        _navMesh = GetComponent<NavMeshSurface>();
        _lastMainPosition = _mainPoint.position;
    }

    private void Update()
    {
        _pointMap.Update();
        _generatorChunksRound.Update();

        if (_debug)
        {
            var v = _pointMap.GetPointAtPosition(_mainPoint.transform.position + new Vector3(0.0f, 0.0f, 0.0f));
        }
    }
    
    private void AddObstaclesLayer(int layer)
    {
        _obstaclesLayers |= (1 << layer);
    }

    private void AddObstaclesLayer(string layer)
    {
        AddObstaclesLayer(LayerMask.NameToLayer(layer));
    }

    private void RemoveObstaclesLayer(int layer)
    {
        _obstaclesLayers &= ~(1 << layer);
    }

    private void FixedUpdate()
    {
        if (!_debug) return;

        foreach (var index in _pointMap.map.Keys.Where(index => !_createdDebugPoints.Contains(index) && _pointMap.map[index].PointsIsCreated()))
        {
            _createdDebugPoints.Add(index);
            ChunkCreatePoints(_pointMap.map[index]);
        }
    }

    private GameObject CreateDebugPoint(Vector3 pos)
    {
        GameObject p = Instantiate(_debugPrefab, _debugContainer.transform, true);
        p.transform.position = pos;
        p.layer = 0;
        p.tag = _debugTag;
        return p;
    }

    private void CreateDebugPoint(Vector3 pos, Color color)
    {
        var p = CreateDebugPoint(pos);
        var m = p.GetComponent<MeshRenderer>();
        m.material.color = color;
    }

    private void ChunkCreatePoints(Chunk chunk)
    {
        foreach (var i in chunk.Indices)
        {
            var p = chunk.Points.Get(i);
            CreateDebugPoint(p.Position);
        }
    }

    public PointMap GetPointMap()
    {
        return _pointMap;
    }
}
