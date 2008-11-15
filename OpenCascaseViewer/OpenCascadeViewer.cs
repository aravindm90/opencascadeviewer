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
        public OpenCascadeViewer()
        {
            InitializeComponent();

            TopoDS.Shape s = OpenCascaseViewer.Code.Bottle.MakeBottle(12, 44, 2);
      
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }

    }
}