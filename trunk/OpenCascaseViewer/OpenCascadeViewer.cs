using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace OpenCascaseViewer
{

    public partial class OpenCascadeViewer : Form
    {
        public List<TopoDS.Shape> viewedShapes = new List<TopoDS.Shape>();

        public OpenCascadeViewer()
        {
            InitializeComponent();
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);
            if (viewedShapes.Count == 0)
            {
                TopoDS.Shape bottle = OpenCascaseViewer.Code.Bottle.MakeBottle(12, 44, 2);
                gp.Pnt[] points = null;
                gp.Pnt2d[] uvpoints = null;
                int[] triangles = null;
                Utility.Triangulate(bottle, out points, out uvpoints, out triangles);
                drawingControl1.Fill(points, uvpoints, triangles);
                return;
            }

            List<TopoDS.Shape>.Enumerator iterator = viewedShapes.GetEnumerator();
            while (iterator.MoveNext())
            {
                gp.Pnt[] points = null;
                gp.Pnt2d[] uvpoints = null;
                int[] triangles = null;
                Utility.Triangulate(iterator.Current, out points, out uvpoints, out triangles);
                drawingControl1.Fill(points, uvpoints, triangles);
            }
        }

        private void loadToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                string[] parts = openFileDialog1.FileName.Split('.');
                string type = parts[parts.Length - 1];
                type = type.ToUpper();
                switch (type)
                {
                    case "IGES":
                        IGESControl.Reader igesReader = new IGESControl.Reader();
                        if (igesReader.ReadFile(openFileDialog1.FileName) == IFSelect.ReturnStatus.RetDone)
                        {
                            igesReader.TransferRoots();
                            viewedShapes.Clear();
                            viewedShapes.Add(igesReader.OneShape());
                        }
                        else
                            MessageBox.Show("Could not load file " + openFileDialog1.FileName);
                        break;

                    case "STEP":
                        STEPControl.Reader stepReader = new STEPControl.Reader();
                        if (stepReader.ReadFile(openFileDialog1.FileName) == IFSelect.ReturnStatus.RetDone)
                        {
                            viewedShapes.Clear();
                            int numberOfRoots = stepReader.NbRootsForTransfer();
                            for (int n = 1; n <= numberOfRoots; n++)
                            {
                                stepReader.TransferRoot(n);
                                int numberOfShapes = stepReader.NbShapes();
                                for (int i = 1; i <= numberOfShapes; i++)
                                    viewedShapes.Add(stepReader.Shape(i));
                            }
                        }
                        else
                            MessageBox.Show("Could not load file " + openFileDialog1.FileName);
                        break;

                    //case "CSFDB":

                    //    break;
                    case "BREP":
                        TopoDS.Shape shape = new TopoDS.Shape();
                        BRep.Builder builder = new BRep.Builder(); ;
                        if (BRepTools.General.Read(shape, openFileDialog1.FileName, builder))
                        {
                            viewedShapes.Clear();
                            viewedShapes.Add(shape);
                        }
                        else
                            MessageBox.Show("Could not load file " + openFileDialog1.FileName);
                        break;
                    default:
                        MessageBox.Show("Unknown format \"" + type + "\"");
                        break;
                }

            }
        }

    }
}
