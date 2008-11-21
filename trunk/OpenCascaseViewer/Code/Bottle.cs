using System;
using OpenCascade; // this is the main namespace for all OpenCascade classes

namespace OpenCascaseViewer
{
   /// <summary>
   /// Class to implement the static MakeBottle method in C# as provideded as an
   /// example in the OpenCascade Tutorial.
   /// </summary>
   public class Bottle
   {
      /// <summary>
      /// Adopted from the OpenCascade Tutorial "My First Application"
      /// </summary>
      /// <param name="myWidth">width of the bottle</param>
      /// <param name="myHeight">height of the bottle</param>
      /// <param name="myThickness">thickness of the bottle skin</param>
      /// <returns>the shape of the bottle</returns>
      static public TopoDS.Shape MakeBottle(double myWidth, double myHeight, double myThickness)
      {
         //Profile : Define Support Points
         gp.Pnt aPnt1 = new gp.Pnt(-myWidth / 2.0, 0, 0);
         gp.Pnt aPnt2 = new gp.Pnt(-myWidth / 2.0, -myThickness / 4.0, 0);
         gp.Pnt aPnt3 = new gp.Pnt(0, -myThickness / 2.0, 0);
         gp.Pnt aPnt4 = new gp.Pnt(myWidth / 2.0, -myThickness / 4.0, 0);
         gp.Pnt aPnt5 = new gp.Pnt(myWidth / 2.0, 0, 0);
         //Profile : Define the Geometry
         Geom.TrimmedCurve aArcOfCircle = (new GC.MakeArcOfCircle(ref aPnt2, ref aPnt3, ref aPnt4)).Value();
         // note: all constructors have to be called in C# with the new keyword in C#
         Geom.TrimmedCurve aSegment1 = (new GC.MakeSegment(ref aPnt1, ref aPnt2)).Value();
         Geom.TrimmedCurve aSegment2 = (new GC.MakeSegment(ref aPnt4, ref aPnt5)).Value();
         //Profile : Define the Topology
         TopoDS.Edge aEdge1 = (new BRepBuilderAPI.MakeEdge(aSegment1)).Edge();
         TopoDS.Edge aEdge2 = (new BRepBuilderAPI.MakeEdge(aArcOfCircle)).Edge();
         TopoDS.Edge aEdge3 = (new BRepBuilderAPI.MakeEdge(aSegment2)).Edge();
         TopoDS.Wire aWire = (new BRepBuilderAPI.MakeWire(aEdge1, aEdge2, aEdge3)).Wire();
         //Complete Profile
         gp.Ax1 xAxis = gp.General.OX();
         gp.Trsf aTrsf = new gp.Trsf();
         aTrsf.SetMirror(ref xAxis);
         BRepBuilderAPI.Transform aBRepTrsf = new BRepBuilderAPI.Transform(aWire, aTrsf, false);
         TopoDS.Shape aMirroredShape = aBRepTrsf.Shape();
         TopoDS.Wire aMirroredWire = TopoDS.General.Wire(aMirroredShape);
         // note: General is the key to use static namespace methods (TopoDS is a namespace), 
         // since there is no such thing in C#
         BRepBuilderAPI.MakeWire mkWire = new BRepBuilderAPI.MakeWire();

         mkWire.Add(aWire);
         mkWire.Add(aMirroredWire);
         TopoDS.Wire myWireProfile = mkWire.Wire();
         //Body : Prism the Profile
         TopoDS.Face myFaceProfile = (new BRepBuilderAPI.MakeFace(myWireProfile, false)).Face();
         gp.Vec aPrismVec = new gp.Vec(0, 0, myHeight);
         TopoDS.Shape myBody = (new BRepPrimAPI.MakePrism(myFaceProfile, ref aPrismVec, false, true)).Shape();
         //Body : Apply Fillets
         BRepFilletAPI.MakeFillet mkFillet = new BRepFilletAPI.MakeFillet(myBody, ChFi3d.FilletShape.Rational);
         TopExp.Explorer aEdgeExplorer = new TopExp.Explorer(myBody, TopAbs.ShapeEnum.EDGE, TopAbs.ShapeEnum.SHAPE);
         while (aEdgeExplorer.More())
         {
            TopoDS.Edge aEdge = TopoDS.General.Edge(aEdgeExplorer.Current());
            //Add edge to fillet algorithm
            mkFillet.Add(myThickness / 12.0, aEdge);
            aEdgeExplorer.Next();
         }
         myBody = mkFillet.Shape();
         //Body : Add the Neck
         gp.Pnt neckLocation = new gp.Pnt(0, 0, myHeight);
         gp.Dir neckNormal = new gp.Dir(0, 0, 1);
         gp.Ax3 neckAx3; 
         neckAx3.axis.loc = neckLocation;
         neckAx3.axis.vdir = neckNormal;
         neckAx3.vxdir = new gp.Dir(1, 0, 0);
         neckAx3.vydir = new gp.Dir(0, 1, 0);
         gp.Ax2 neckAx2; 
         neckAx2.axis.loc = neckLocation;
         neckAx2.axis.vdir = neckNormal;
         neckAx2.vxdir = new gp.Dir(1, 0, 0);
         neckAx2.vydir = new gp.Dir(0, 1, 0);

         double myNeckRadius = myThickness / 4.0;
         double myNeckHeight = myHeight / 10;
         TopoDS.Shape myNeck = (new BRepPrimAPI.MakeCylinder(ref neckAx2, myNeckRadius, myNeckHeight)).Shape();
         myBody = (new BRepAlgoAPI.Fuse(myBody, myNeck)).Shape();

         //Body : Create a Hollowed Solid
         TopoDS.Face faceToRemove = null;
         double zMax = -1;
         for (TopExp.Explorer aFaceExplorer = new TopExp.Explorer(myBody, TopAbs.ShapeEnum.FACE, TopAbs.ShapeEnum.SHAPE);
            aFaceExplorer.More(); aFaceExplorer.Next())
         {
            TopoDS.Face aFace = TopoDS.General.Face(aFaceExplorer.Current());

            //Check if <aFace> is the top face of the bottle’s neck
            Geom.Surface aSurface = BRep.Tool.Surface(aFace);
            if (aSurface is Geom.Plane)
            {
               Geom.Plane aPlane = aSurface as Geom.Plane;
               // note: the powerful as an is operators also work with the OpenCascade class hierarchy (polymorphism)
               gp.Pnt aPnt = aPlane.Location();
               double aZ = aPnt.z;
               if (aZ > zMax)
               {
                  zMax = aZ;
                  faceToRemove = aFace;
               }
            }
         }
         TopTools.ListOfShape facesToRemove = new TopTools.ListOfShape();
         facesToRemove.Append(faceToRemove);
         myBody = (new BRepOffsetAPI.MakeThickSolid(myBody, facesToRemove, -myThickness / 60, 1.0e-3, BRepOffset.Mode.Skin, false, false, GeomAbs.JoinType.Arc)).Shape();
         //Threading : Create Surfaces
         Geom.CylindricalSurface aCyl1 = new Geom.CylindricalSurface(ref neckAx3, myNeckRadius * 0.99);
         Geom.CylindricalSurface aCyl2 = new Geom.CylindricalSurface(ref neckAx3, myNeckRadius * 1.05);
         //Threading : Define 2D Curves
         gp.Pnt2d bPnt = new gp.Pnt2d(2.0 * Math.PI, myNeckHeight / 2.0);
         gp.Dir2d aDir = new gp.Dir2d(2.0 * Math.PI, myNeckHeight / 4.0);

         gp.Ax22d aAx22d = new gp.Ax22d();
         aAx22d.point = bPnt;
         aAx22d.vxdir = aDir;
         aAx22d.vydir = new gp.Dir2d(-aDir.y, aDir.x);

         double aMajor = 2.0 * Math.PI;
         double aMinor = myNeckHeight / 10;
         Geom2d.Ellipse anEllipse1 = new Geom2d.Ellipse(ref aAx22d, aMajor, aMinor);
         Geom2d.Ellipse anEllipse2 = new Geom2d.Ellipse(ref aAx22d, aMajor, aMinor / 4);
         Geom2d.TrimmedCurve aArc1 = new Geom2d.TrimmedCurve(anEllipse1, 0, Math.PI, true);
         Geom2d.TrimmedCurve aArc2 = new Geom2d.TrimmedCurve(anEllipse2, 0, Math.PI, true);
         gp.Pnt2d anEllipsePnt1 = anEllipse1.Value(0);
         gp.Pnt2d anEllipsePnt2 = anEllipse1.Value(Math.PI);
         Geom2d.TrimmedCurve aSegment = (new GCE2d.MakeSegment(ref anEllipsePnt1, ref anEllipsePnt2)).Value();
         //Threading : Build Edges and Wires
         TopoDS.Edge aEdge1OnSurf1 = (new BRepBuilderAPI.MakeEdge(aArc1, aCyl1)).Edge();
         TopoDS.Edge aEdge2OnSurf1 = (new BRepBuilderAPI.MakeEdge(aSegment, aCyl1)).Edge();
         TopoDS.Edge aEdge1OnSurf2 = (new BRepBuilderAPI.MakeEdge(aArc2, aCyl2)).Edge();
         TopoDS.Edge aEdge2OnSurf2 = (new BRepBuilderAPI.MakeEdge(aSegment, aCyl2)).Edge();
         TopoDS.Wire threadingWire1 = (new BRepBuilderAPI.MakeWire(aEdge1OnSurf1, aEdge2OnSurf1)).Wire();
         TopoDS.Wire threadingWire2 = (new BRepBuilderAPI.MakeWire(aEdge1OnSurf2, aEdge2OnSurf2)).Wire();
         BRepLib.General.BuildCurves3d(threadingWire1);
         BRepLib.General.BuildCurves3d(threadingWire2);
         //Create Threading
         BRepOffsetAPI.ThruSections aTool = new BRepOffsetAPI.ThruSections(true, false, 1e-6);
         aTool.AddWire(threadingWire1);
         aTool.AddWire(threadingWire2);
         aTool.CheckCompatibility(false);
         TopoDS.Shape myThreading = aTool.Shape();
         //Building the Resulting Compound
         TopoDS.Compound aRes = new TopoDS.Compound();
         BRep.Builder aBuilder = new BRep.Builder();
         aBuilder.MakeCompound(aRes);
         aBuilder.Add(aRes, myBody);
         aBuilder.Add(aRes, myThreading);
         return aRes;
      }
   }
}
