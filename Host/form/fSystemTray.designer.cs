namespace host
{
    partial class fSystemTray
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(fSystemTray));
            this.notifyIcon1 = new System.Windows.Forms.NotifyIcon(this.components);
            this.SuspendLayout();
            // 
            // notifyIcon1
            // 

            this.notifyIcon1.Icon = host.ResourceData.icon_amiss;
                // ((System.Drawing.Icon)(resources.GetObject("notifyIcon1.Icon")));
            this.notifyIcon1.Text = "IFC Amiss v1.0";
            this.notifyIcon1.Visible = true;
            this.notifyIcon1.MouseClick += new System.Windows.Forms.MouseEventHandler(this.notifyIcon1_MouseClick);
            // 
            // menuStrip1
            // 
            // 
            // TrayMinimizerForm
            // 
            this.Name = "IFC_Amiss";
            this.Text = "IFC Amiss";
            this.ShowInTaskbar = false;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(292, 266);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Width = 1;
            this.Height = 1;
            this.Location = new System.Drawing.Point(-90, -90);
            this.Top = System.Windows.Forms.Screen.PrimaryScreen.WorkingArea.Height + 1000;
            this.Load+=fSystemTray_Load;
            //this.Activated +=fSystemTray_Activated;
            //this.Resize += new System.EventHandler(this.TrayMinimizerForm_Resize);
            this.ResumeLayout(false);
            this.PerformLayout();

        }


        private void fSystemTray_Activated(object sender, System.EventArgs e)
        {
            //hostServer.openBrowser();
        }


        #endregion

        private System.Windows.Forms.NotifyIcon notifyIcon1;
    }
}

