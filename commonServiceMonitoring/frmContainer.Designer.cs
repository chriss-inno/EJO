namespace commonServiceMonitoring
{
    partial class frmContainer
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.mnuContainer = new System.Windows.Forms.MenuStrip();
            this.mainToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.servicesStatusToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.serviceSettingsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.manageServicesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ftstatus = new System.Windows.Forms.StatusStrip();
            this.mnuContainer.SuspendLayout();
            this.SuspendLayout();
            // 
            // mnuContainer
            // 
            this.mnuContainer.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mainToolStripMenuItem,
            this.servicesStatusToolStripMenuItem,
            this.serviceSettingsToolStripMenuItem,
            this.manageServicesToolStripMenuItem});
            this.mnuContainer.Location = new System.Drawing.Point(0, 0);
            this.mnuContainer.Name = "mnuContainer";
            this.mnuContainer.Size = new System.Drawing.Size(809, 24);
            this.mnuContainer.TabIndex = 1;
            this.mnuContainer.Text = "menuStrip1";
            // 
            // mainToolStripMenuItem
            // 
            this.mainToolStripMenuItem.Name = "mainToolStripMenuItem";
            this.mainToolStripMenuItem.Size = new System.Drawing.Size(76, 20);
            this.mainToolStripMenuItem.Text = "Dashboard";
            // 
            // servicesStatusToolStripMenuItem
            // 
            this.servicesStatusToolStripMenuItem.Name = "servicesStatusToolStripMenuItem";
            this.servicesStatusToolStripMenuItem.Size = new System.Drawing.Size(96, 20);
            this.servicesStatusToolStripMenuItem.Text = "Services Status";
            // 
            // serviceSettingsToolStripMenuItem
            // 
            this.serviceSettingsToolStripMenuItem.Name = "serviceSettingsToolStripMenuItem";
            this.serviceSettingsToolStripMenuItem.Size = new System.Drawing.Size(101, 20);
            this.serviceSettingsToolStripMenuItem.Text = "Service Settings";
            // 
            // manageServicesToolStripMenuItem
            // 
            this.manageServicesToolStripMenuItem.Name = "manageServicesToolStripMenuItem";
            this.manageServicesToolStripMenuItem.Size = new System.Drawing.Size(107, 20);
            this.manageServicesToolStripMenuItem.Text = "Manage Services";
            // 
            // ftstatus
            // 
            this.ftstatus.Location = new System.Drawing.Point(0, 504);
            this.ftstatus.Name = "ftstatus";
            this.ftstatus.Size = new System.Drawing.Size(809, 22);
            this.ftstatus.TabIndex = 3;
            this.ftstatus.Text = "statusStrip1";
            // 
            // frmContainer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(809, 526);
            this.Controls.Add(this.ftstatus);
            this.Controls.Add(this.mnuContainer);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.IsMdiContainer = true;
            this.MainMenuStrip = this.mnuContainer;
            this.Name = "frmContainer";
            this.Text = "Central Service Monitoring Panel ";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.Load += new System.EventHandler(this.frmContainer_Load);
            this.mnuContainer.ResumeLayout(false);
            this.mnuContainer.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip mnuContainer;
        private System.Windows.Forms.ToolStripMenuItem mainToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem servicesStatusToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem serviceSettingsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem manageServicesToolStripMenuItem;
        private System.Windows.Forms.StatusStrip ftstatus;
    }
}

