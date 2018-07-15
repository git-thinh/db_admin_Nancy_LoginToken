using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace host
{
    public partial class fSystemTray : Form
    {
        public fSystemTray()
        {
            InitializeComponent();
        }

        private void fSystemTray_Load(object sender, System.EventArgs e)
        {
            this.Width = 1;
            this.Height = 1;
            this.Location = new System.Drawing.Point(-90, -90);
             
        }

        private void notifyIcon1_MouseClick(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            ////hostServer.openBrowser();
            //hostModule.CacheRefresh();
            //main.show_notification("Cache OK", 5000);
            main.show_form_Main();
        }
    }
}