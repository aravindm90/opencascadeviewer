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
        private ModelData data = new ModelData();

        public OpenCascadeViewer()
        {
            InitializeComponent();
            drawingControl1.Model = data;
        }

        private void loadToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                data.Clear();

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
                            data.Add(igesReader.OneShape());
                        }
                        else
                            MessageBox.Show("Could not load file " + openFileDialog1.FileName);
                        break;

                    case "STEP":
                        STEPControl.Reader stepReader = new STEPControl.Reader();
                        if (stepReader.ReadFile(openFileDialog1.FileName) == IFSelect.ReturnStatus.RetDone)
                        {
                            int numberOfRoots = stepReader.NbRootsForTransfer();
                            for (int n = 1; n <= numberOfRoots; n++)
                            {
                                stepReader.TransferRoot(n);
                                int numberOfShapes = stepReader.NbShapes();
                                for (int i = 1; i <= numberOfShapes; i++)
                                    data.Add(stepReader.Shape(i));
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
                            data.Add(shape);
                        }
                        else
                            MessageBox.Show("Could not load file " + openFileDialog1.FileName);
                        break;
                    default:
                        MessageBox.Show("Unknown format \"" + type + "\"");
                        break;
                }

                drawingControl1.SyncBuffers();
            }
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void sampleShapeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            data.Clear();
            data.Add(Bottle.MakeBottle(12, 44, 2));
            drawingControl1.SyncBuffers();
        }

    }
}
