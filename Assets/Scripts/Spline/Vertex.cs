using UnityEngine;

namespace Spline
{

    public struct Vertex
    {
        public Vector3 point;
        public Vector3 normal;
        public float uCoord;


        public Vertex(Vector3 point, Vector3 normal, float uCoord)
        {
            this.point = point;
            this.normal = normal;
            this.uCoord = uCoord;
        }
    }
}