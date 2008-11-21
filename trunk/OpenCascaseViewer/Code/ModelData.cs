using System;
using System.Collections.Generic;
using System.Text;

namespace OpenCascaseViewer
{
    public class ModelData
    {
        private List<TopoDS.Shape> viewedShapes = new List<TopoDS.Shape>();
        private List<TessalatedShape> triangulations = new List<TessalatedShape>();

        public void Add(TopoDS.Shape shape)
        {
            viewedShapes.Add(shape);
            triangulations.Add(Utility.Triangulate(shape));
        }

        public void Clear()
        {
            viewedShapes.Clear();
            triangulations.Clear();
        }

        public List<TessalatedShape> Tessations
        {
            get { return triangulations; }
        }

    }
}
