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
    public partial class fPLC_export_mmf : Form
    {
        public fPLC_export_mmf()
        {
            InitializeComponent();
        }

        private m_meter_plc[] ds = new m_meter_plc[] { };

        private void browser_files_csv_Click(object sender, EventArgs e)
        {
            OpenFileDialog d = new System.Windows.Forms.OpenFileDialog();
            d.DefaultExt = "csv";
            d.Filter = "CSV files (*.csv)|*.csv";
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

                string data = hostFile.readFile(file, Encoding.ASCII).Trim();
                ds = data
                    .Split(new string[] { Environment.NewLine, "\r", "\n" }, StringSplitOptions.None)
                    .Select(x => x.Trim())
                    .Where(x => x != "")
                    .Select(x => x.Split(','))
                    .Where(x => x.Length == 4)
                    .Select(x => x.Select(i => i.TryParseToLong()).Where(i => i > 0).ToArray())
                    .Where(x => x.Length == 4)
                    .Select(x => new m_meter_plc { imei = (UInt32)x[0], id = (Int16)x[1], so_cong_to = (UInt32)x[2], phase_id = (byte)x[3] })
                    .ToArray();
                grid_file.DataSource = ds.Select(x => new { IMEI = x.imei, STT = x.id, CongTo = x.so_cong_to, phase_id = x.phase_id }).ToArray();
            }
            // Cancel button was pressed.
            else if (result == DialogResult.Cancel)
            {
                return;
            }

        }

        private void b_export_to_file_mmf_Click(object sender, EventArgs e)
        {

            FolderBrowserDialog d = new FolderBrowserDialog();
            // Set the help text description for the FolderBrowserDialog.
            d.Description = "Chọn đường dẫn lưu dữ liệu hệ thống";

            // Do not allow the user to create new files via the FolderBrowserDialog.
            d.ShowNewFolderButton = true;
            d.RootFolder = Environment.SpecialFolder.MyComputer;

            // Show the FolderBrowserDialog.
            DialogResult result = d.ShowDialog();
            if (result == DialogResult.OK)
            {
                string[] a = path_file_textBox.Text.Split('\\');
                string filename = a[a.Length - 1];
                filename = filename.Substring(0, filename.Length - 4);
                string file = filename + ".mmf";
                hostFile.write_file_MMF<m_meter_plc>(ds, d.SelectedPath, filename);
                MessageBox.Show("Export file MMF ok \n " + d.SelectedPath + file);
            }
        }

        private void fPLC_export_mmf_Load(object sender, EventArgs e)
        {

        }
    }
}
