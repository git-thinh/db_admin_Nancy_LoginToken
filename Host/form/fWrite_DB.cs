using model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace host
{
    public partial class fWrite_DB : Form
    {
        public fWrite_DB()
        {
            InitializeComponent();
        }

        private m_meter_plc[] ds = new m_meter_plc[] { };

        private void browser_files_csv_Click(object sender, EventArgs e)
        {
            OpenFileDialog d = new System.Windows.Forms.OpenFileDialog();
            d.DefaultExt = "mmf";
            d.Filter = "MMF files (*.mmf)|*.mmf";
            d.InitialDirectory = AppDomain.CurrentDomain.BaseDirectory;
            //Environment.SpecialFolder.MyComputer.ToString();
            d.FileName = null;

            // Display the openFile dialog.
            DialogResult result = d.ShowDialog();

            // OK button was pressed.
            if (result == DialogResult.OK)
            {
                string file = d.FileName;
                path_file_textBox.Text = file;
            }
            // Cancel button was pressed.
            else if (result == DialogResult.Cancel)
            {
                return;
            }

        }

        private void b_update_db_Click(object sender, EventArgs e)
        {
            try
            {
                var data = hostFile.read_file_MMF<Tuple<Tuple<long, int, int, UInt32, byte>, int, double[]>>(path_file_textBox.Text.Trim());
                store.update(data);
                lb_result.Text = "Update thành công " + data.Length + " dữ liệu";
            }
            catch (Exception ex)
            {
                lb_result.Text = "Update lỗi: " + ex.Message;
            }
           
        }

        private void fPLC_export_mmf_Load(object sender, EventArgs e)
        {

        }
    }
}
