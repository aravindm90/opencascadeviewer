using System;
using System.Collections.Generic;
using System.Text;

namespace OpenCascaseViewer
{
    public struct TessalatedShape
    {      
        public TopoDS.Shape topoShape; //the shape that was tessalated
        public gp.Pnt[] points;
        public gp.Pnt2d[] uvpoints;
        public int[] triangles;
    }
}
