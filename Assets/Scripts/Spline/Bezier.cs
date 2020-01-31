using UnityEngine;

namespace Spline
{
    public static class Bezier
    {
        /**
     * Returns the point position in the cubic bezier curve
     * resulting of the linear interpolation between p0, p1, p2, p3
     * at a given time t
     */
        public static Vector3 GetPoint(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t)
        {
            float leftTime = 1f-t;
            float leftTimeSquared = leftTime * leftTime;
            float timeSquared = t * t;
        
            return	p0 * ( leftTimeSquared * leftTime ) +
                    p1 * ( 3f * leftTimeSquared * t ) +
                    p2 * ( 3f * leftTime * timeSquared ) +
                    p3 * ( timeSquared * t );
        }
    
        // Get Tangent of the cubic bezier curve
        public static Vector3 GetTangent(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t) {
            float leftTime = 1f-t;
            float leftTimeSquared = leftTime * leftTime;
            float timeSquared = t * t;
            Vector3 tangent = 
                p0 * ( -leftTimeSquared ) +
                p1 * ( 3 * leftTimeSquared - 2 * leftTime ) +
                p2 * ( -3 * timeSquared + 2 * t ) +
                p3 * ( timeSquared );
        
            return tangent.normalized;
        }

        public static Vector3 GetNormal(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t, Vector3 up) {
            Vector3 tangent = GetTangent(p0, p1, p2, p3, t);
            Vector3 biNormal = Vector3.Cross(up, tangent).normalized;

            return Vector3.Cross(tangent, biNormal);
        }

        public static Quaternion GetOrientation(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t, Vector3 up) {
            Vector3 tangent = GetTangent(p0, p1, p2, p3, t);
            Vector3 normal = GetNormal(p0, p1, p2, p3, t, up);

            return Quaternion.LookRotation(tangent, normal);
        }

        /**
     * Returns a position tangent to the cubic bezier curve at a given time t
     */
        public static Vector3 GetFirstDerivative(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t)
        {
            t = Mathf.Clamp01(t);
            return 3f * (1f - t) * (1f - t) * (p1 - p0) +
                   6f * (1f - t) * t * (p2 - p1) +
                   3f * t * t * (p3 - p2);
        }
    }
}