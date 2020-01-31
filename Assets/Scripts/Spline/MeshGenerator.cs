using System;
using UnityEngine;

/*
 * Code heavily inspired by 
 * Unite 2015 - A coder's guide to spline-based procedural geometry
 * by Joachim (now Freya) Holmer
 */
namespace Spline
{
    public class MeshGenerator {
        // Using a simple line as 2D shape for the extrusion (for now)
        private static ExtrudeShape GenerateShapeToExtrude(int width)
        {
            Vertex[] verts2D = {new Vertex(
                new Vector3(-width/2f,0,0), Vector3.up, 0)};
            int[] lines = {0};

            for (int i = 1, j = 1; i <= width; i++, j+=2)
            {
                Array.Resize(ref verts2D, verts2D.Length + 1);
                verts2D[i] = new Vertex(
                    new Vector3(i+(-width/2f),0,0), Vector3.up ,0);
                if (i < width) {
                    Array.Resize(ref lines, lines.Length + 2);
                    lines[j] = i;
                    lines[j + 1] = i;
                }
                else
                {
                    Array.Resize(ref lines, lines.Length + 1);
                    lines[j] = i;
                }
            }

            return new ExtrudeShape(verts2D, lines);
        }
        
        /*
         * Generates needed elements for the mesh (vertices, uvs...)
         * by using the shape to extrude and the path of the spline
         * and creates the mesh
         */
        private static Mesh GenerateSplineMesh(ExtrudeShape shape, OrientedPoint[] path)
        {
            int vertsInShape = shape.verts2Ds.Length;
            int edgeLoops = path.Length;
            int vertCount = vertsInShape * edgeLoops;

            Mesh mesh = new Mesh();

            Vector3[] vertices = new Vector3[ vertCount ];
            Vector3[] normals = new Vector3[ vertCount ];
            Vector2[] uvs = new Vector2[ vertCount ];
        
            for(int i = 0; i < path.Length; i++) {
                int offset = i * vertsInShape;
                for( int j = 0; j < vertsInShape; j++ ) {
                    int id = offset + j;
                    vertices[id] = path[i].LocalToWorld(shape.verts2Ds[j].point);
                    normals[id] = path[i].LocalToWorldDirection( shape.verts2Ds[j].normal );
                    uvs[id] = new Vector2( shape.verts2Ds[j].uCoord, i / ((float)edgeLoops) );
                }
            }
            
            mesh.vertices = vertices;
            mesh.triangles = GetTrianglesIndexes(shape, path.Length - 1, vertsInShape);
            mesh.normals = normals;
            mesh.uv = uvs;

            return mesh;
        }

        private static int[] GetTrianglesIndexes(ExtrudeShape shape, int segments,
            int vertsInShape)
        {
            int triCount = shape.lines.Length * segments;
            int triIndexCount = triCount * 3;
            int[] triangleIndices = new int[ triIndexCount ];
            int ti = 0;

            for (int i = 0; i < segments; i++)
            {
                int offset = i * vertsInShape;

                for (int l = 0; l < shape.lines.Length; l += 2)
                {
                    int a = offset + shape.lines[l] + vertsInShape;
                    int b = offset + shape.lines[l];
                    int c = offset + shape.lines[l + 1];
                    int d = offset + shape.lines[l + 1] + vertsInShape;

                    // First triangle
                    triangleIndices[ti++] = a;
                    triangleIndices[ti++] = b;
                    triangleIndices[ti++] = c;

                    // Second triangle
                    triangleIndices[ti++] = c;
                    triangleIndices[ti++] = d;
                    triangleIndices[ti++] = a;
                }
            }

            return triangleIndices;
        }

        public static Mesh ExtrudeMeshAlongSpline(BezierSpline spline, 
            int width, int steps = 100)
        {
            ExtrudeShape shape = GenerateShapeToExtrude(width);
        
            // Retrieving the path from the spline and storing it in oriented points
            OrientedPoint[] path = new OrientedPoint[steps + 1];
        
            for (int i = 0; i <= steps; i++)
            {
                Vector3 point = spline.GetPoint((float) i / steps);
                Quaternion rot = spline.GetOrientation((float) i / steps);
                path[i] = new OrientedPoint(point, rot);
            }

            return GenerateSplineMesh(shape, path);
        }
    }
}