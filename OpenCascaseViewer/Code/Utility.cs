using System;
using System.Collections.Generic;
using System.Text;

namespace OpenCascaseViewer
{
    public class Utility
    {
        public static void Triangulate(TopoDS.Shape topoShape, out gp.Pnt[] points, out gp.Pnt2d[] uvpoints, out int[] triangles)
        {
            TopExp.Explorer ex = new TopExp.Explorer(topoShape, TopAbs.ShapeEnum.FACE, TopAbs.ShapeEnum.SHAPE);

            BRepMesh.General.Mesh(topoShape, 0.01);

            points = null;
            uvpoints = null;
            triangles = null;

            // Loop all faces
            while (ex.More())
            {

                TopoDS.Face F = TopoDS.General.Face(ex.Current());
                TopLoc.Location location = new TopLoc.Location();

                Poly.Triangulation triangulation = BRep.Tool.Triangulation(F, location);

                Poly.Array1OfTriangle arrayOfTriangle = triangulation.Triangles();

                TColgp.Array1OfPnt p3d = triangulation.Nodes();

                gp.Pnt average = new gp.Pnt(0.0, 0.0, 0.0);
                points = new gp.Pnt[p3d.Length()];
                for (int i = 0; i < points.Length; ++i)
                {
                    points[i] = p3d.Value(i + 1);
                    average.x += points[i].x;
                    average.y += points[i].y;
                    average.z += points[i].z;
                }
                average.x /= points.Length;
                average.y /= points.Length;
                average.z /= points.Length;
                //if (location != null)
                if (location != null)
                {
                    gp.Trsf trsf = location.Transformation();
                    for (int i = 0; i < points.Length; ++i)
                    {
                        trsf.Transforms(out points[i].x, out points[i].y, out points[i].z);
                        points[i].x -= average.x;
                        points[i].y -= average.y;
                        points[i].z -= average.z;
                    }
                }
                TColgp.Array1OfPnt2d p2d = triangulation.UVNodes();
                if (p2d != null)
                {
                    uvpoints = new gp.Pnt2d[p2d.Length()];
                    for (int i = 0; i < uvpoints.Length; ++i)
                    {
                        uvpoints[i] = p2d.Value(i + 1);
                    }
                }
                else
                {
                    uvpoints = null;
                }
                triangles = new int[arrayOfTriangle.Length() * 3];
                int l = arrayOfTriangle.Length();
                for (int i = 0; i < l; ++i)
                {
                    Poly.Triangle t = arrayOfTriangle.Value(i + 1);
                    triangles[3 * i] = t.Value(1) - 1;
                    triangles[3 * i + 1] = t.Value(2) - 1;
                    triangles[3 * i + 2] = t.Value(3) - 1;
                }

                ex.Next();
            }
        }
    }
}
