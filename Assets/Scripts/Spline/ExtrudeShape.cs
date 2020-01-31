namespace Spline
{
    public class ExtrudeShape
    {
        public Vertex[] verts2Ds;
        public int[] lines;

        public ExtrudeShape(Vertex[] vert2Ds, int[] lines)
        {
            this.verts2Ds = vert2Ds;
            this.lines = lines;
        }
    }
}