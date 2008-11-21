using System;
using System.Collections.Generic;
using System.Text;

namespace OpenCascaseViewer
{
    public class Utility
    {

        public static List<TessalatedShape> Triangulate(TopoDS.Shape topoShape)
        {
            TopExp.Explorer ex = new TopExp.Explorer(topoShape, TopAbs.ShapeEnum.FACE, TopAbs.ShapeEnum.SHAPE);

            BRepMesh.General.Mesh(topoShape, 0.1);

            List<TessalatedShape> faces = new List<TessalatedShape>();

            // Loop all faces
            while (ex.More())
            {

                TessalatedShape shape = new TessalatedShape();

                TopoDS.Face F = TopoDS.General.Face(ex.Current());
                TopLoc.Location location = new TopLoc.Location();

                Poly.Triangulation triangulation = BRep.Tool.Triangulation(F, location);

                Poly.Array1OfTriangle arrayOfTriangle = triangulation.Triangles();

                TColgp.Array1OfPnt p3d = triangulation.Nodes();

                shape.points = new gp.Pnt[p3d.Length()];
                for (int i = 0; i < shape.points.Length; ++i)
                {
                    shape.points[i] = p3d.Value(i + 1);
                }
                
                if (location != null)
                {
                    gp.Trsf trsf = location.Transformation();
                    for (int i = 0; i < shape.points.Length; ++i)
                    {
                        trsf.Transforms(out shape.points[i].x, out shape.points[i].y, out shape.points[i].z);
                    }
                }
                TColgp.Array1OfPnt2d p2d = triangulation.UVNodes();
                if (p2d != null)
                {
                    shape.uvpoints = new gp.Pnt2d[p2d.Length()];
                    for (int i = 0; i < shape.uvpoints.Length; ++i)
                    {
                        shape.uvpoints[i] = p2d.Value(i + 1);
                    }
                }
                else
                {
                    shape.uvpoints = null;
                }
                shape.triangles = new int[arrayOfTriangle.Length() * 3];
                int l = arrayOfTriangle.Length();
                for (int i = 0; i < l; ++i)
                {
                    Poly.Triangle t = arrayOfTriangle.Value(i + 1);
                    shape.triangles[3 * i] = t.Value(1) - 1;
                    shape.triangles[3 * i + 1] = t.Value(2) - 1;
                    shape.triangles[3 * i + 2] = t.Value(3) - 1;
                }

                faces.Add(shape);

                ex.Next();
            }

            return faces;

        }
    }
}
