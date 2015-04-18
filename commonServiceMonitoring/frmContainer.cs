using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace commonServiceMonitoring
{
    public partial class frmContainer : Form
    {
        Form frm = new frmDashBoard();

        public frmContainer()
        {
            InitializeComponent();
            frm.WindowState = FormWindowState.Maximized;
            frm.MdiParent = this;
            
        }

        private void frmContainer_Load(object sender, EventArgs e)
        {
            
            
            frm.Show();
            frm.Focus();
                      
        }
    }
}
