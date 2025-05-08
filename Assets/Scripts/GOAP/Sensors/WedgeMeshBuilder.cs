using UnityEngine;

namespace GOAP
{
    public class WedgeMeshBuilder
    {
        private float _angle;
        private float _distance;
        private float _height;
        private int _segments;

        public WedgeMeshBuilder WithAngle(float angle)
        {
            _angle = angle;
            return this;
        }
        
        public WedgeMeshBuilder WithDistance(float distance)
        {
            _distance = distance;
            return this;
        }
        
        public WedgeMeshBuilder WithHeight(float height)
        {
            _height = height;
            return this;
        }
        
        public WedgeMeshBuilder WithSegment(int segment)
        {
            _segments = segment;
            return this;
        }

        public Mesh BuildMesh()
        {
            var mesh = new Mesh();

            var segments = 10;
            var numberTriangles = segments * 4 + 2 + 2;
            var numberVertices = numberTriangles * 3;

            var vertices = new Vector3[numberVertices];
            var triangles = new int[numberVertices];
            
            Vector3 bottomCenter = Vector3.zero;
            Vector3 bottomRight = Quaternion.Euler(0f, _angle, 0f) * Vector3.forward * _distance;
            Vector3 bottomLeft = Quaternion.Euler(0f, -_angle, 0f) * Vector3.forward * _distance;

            Vector3 topCenter = bottomCenter + Vector3.up * _height;
            Vector3 topRight = bottomRight + Vector3.up * _height;
            Vector3 topLeft = bottomLeft + Vector3.up * _height;
            
            FillVertices(vertices, bottomCenter, bottomLeft, topLeft, topCenter, topRight, bottomRight);
            
            for (int i = 0; i < numberVertices; i++)
            {
                triangles[i] = i;
            }

            mesh.vertices = vertices;
            mesh.triangles = triangles;
            mesh.RecalculateNormals();
            return mesh;
        }

        private void FillVertices(Vector3[] vertices, Vector3 bottomCenter, Vector3 bottomLeft, Vector3 topLeft,
            Vector3 topCenter, Vector3 topRight, Vector3 bottomRight)
        {
            var vert = 0;

            vertices[vert++] = bottomCenter;
            vertices[vert++] = bottomLeft;
            vertices[vert++] = topLeft;
            vertices[vert++] = topLeft;
            vertices[vert++] = topCenter;
            vertices[vert++] = bottomCenter;
            
            vertices[vert++] = bottomCenter;
            vertices[vert++] = topCenter;
            vertices[vert++] = topRight;
            vertices[vert++] = topRight;
            vertices[vert++] = bottomRight;
            vertices[vert++] = bottomCenter;

            var currentAngle = -_angle;
            var deltaAngle = _angle * 2 / _segments;

            for (int i = 0; i < _segments; i++)
            {
                bottomRight = Quaternion.Euler(0f, currentAngle, 0f) * Vector3.forward * _distance;
                bottomLeft = Quaternion.Euler(0f, currentAngle + deltaAngle, 0f) * Vector3.forward * _distance;
                
                topRight = bottomRight + Vector3.up * _height;
                topLeft = bottomLeft + Vector3.up * _height;
                currentAngle += deltaAngle;
                
                vertices[vert++] = bottomLeft;
                vertices[vert++] = bottomRight;
                vertices[vert++] = topRight;
                vertices[vert++] = topRight;
                vertices[vert++] = topLeft;
                vertices[vert++] = bottomLeft;
            
                vertices[vert++] = topCenter;
                vertices[vert++] = topLeft;
                vertices[vert++] = topRight;
            
                vertices[vert++] = bottomCenter;
                vertices[vert++] = bottomRight;
                vertices[vert++] = bottomLeft;
            }
        }
            
    }
}