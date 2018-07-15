using model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Windows.Forms;

namespace host
{
    public partial class fSysConfig : Form
    {
        private Dictionary<string, m_sys_config> dic = new Dictionary<string, m_sys_config>() { };
        private object config_Current = null;

        public fSysConfig()
        {
            InitializeComponent();
        }

        private void button_save_config_Click(object sender, EventArgs e)
        {
            string name = c_name_comboBox.Text.Trim().ToString_CacheID();
            f_save(name);
        }

        private void fSysConfig_Load(object sender, EventArgs e)
        {
            int w = System.Windows.Forms.Screen.PrimaryScreen.WorkingArea.Width;
            int h = System.Windows.Forms.Screen.PrimaryScreen.WorkingArea.Height;

            int top = (h - this.Height) / 2;
            top = top / 2;

            this.Left = (w - this.Width) / 2;
            this.Top = top;

            //--------------------------------------------------------

            string root = AppDomain.CurrentDomain.BaseDirectory;

            c_path_file_TextBox.Text = root + "db_file";
            c_path_db_TextBox.Text = root;
            c_path_site_TextBox.Text = root + @"views\www";

            c_ip_local_comboBox.Text = "127.0.0.1";
            c_ip_local_comboBox.Items.Add("127.0.0.1");

            foreach (var a in Dns.GetHostByName(Dns.GetHostName()).AddressList)
                c_ip_local_comboBox.Items.Add(a.ToString());

            //--------------------------------------------------------

            if (Directory.Exists(hostServer.pathConfig))
            {
                var fs = Directory.GetFiles(hostServer.pathConfig, "*.mmf");
                for (int k = 0; k < fs.Length; k++)
                {
                    try
                    {
                        var o = hostFile.read_MMF<m_sys_config>(fs[k]);
                        if (dic.ContainsKey(o.name) == false)
                            dic.Add(o.name, o);
                        if (o.name == "DEFAULT")
                            config_Current = o;
                    }
                    catch { }
                }
            }

            foreach (string key in dic.Keys)
                c_name_comboBox.Items.Add(key);

            if (config_Current != null)
            {
                var o = (m_sys_config)config_Current;

                c_name_comboBox.Text = o.name;

                c_ip_local_comboBox.Text = o.ip_local;
                c_ip_nat_TextBox.Text = o.ip_nat;

                c_login_domain_comboBox.Text = o.login_domain;
                c_login_ip_textBox.Text = o.login_ip;
                c_login_port_textBox.Text = o.login_port.ToString();

                c_name_comboBox.Text = o.name;

                c_path_db_TextBox.Text = o.path_db;
                c_path_file_TextBox.Text = o.path_file;
                c_path_site_TextBox.Text = o.path_site;

                c_site_domain_comboBox.Text = o.site_domain;
                c_site_ip_textBox.Text = o.site_ip;
                c_site_port_textBox.Text = o.site_port.ToString();

                c_socket_domain_comboBox.Text = o.socket_domain;
                c_socket_ip_textBox.Text = o.socket_ip;
                c_socket_port_textBox.Text = o.socket_port.ToString();
            }


        }//end function

        #region // ... browser files ...

        private void path_file_Button_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog d = new FolderBrowserDialog();
            d.Description = "Chọn đường dẫn chứa các tệp tin dữ liệu nhận về hệ thống";
            // Do not allow the user to create new files via the FolderBrowserDialog.
            d.ShowNewFolderButton = false;
            d.RootFolder = Environment.SpecialFolder.MyComputer;

            // Show the FolderBrowserDialog.
            DialogResult result = d.ShowDialog();
            if (result == DialogResult.OK)
                c_path_file_TextBox.Text = d.SelectedPath;
        }

        private void path_db_Button_Click(object sender, EventArgs e)
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
                c_path_db_TextBox.Text = d.SelectedPath;
        }

        private void path_site_Button_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog d = new FolderBrowserDialog();
            // Set the help text description for the FolderBrowserDialog.
            d.Description = "Chọn đường dẫn đến các trang web của hệ thống";

            // Do not allow the user to create new files via the FolderBrowserDialog.
            d.ShowNewFolderButton = true;
            d.RootFolder = Environment.SpecialFolder.MyComputer;

            // Show the FolderBrowserDialog.
            DialogResult result = d.ShowDialog();
            if (result == DialogResult.OK)
                c_path_site_TextBox.Text = d.SelectedPath;
        }

        #endregion

        private void c_name_comboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            string name = c_name_comboBox.Text;

            m_sys_config o = new m_sys_config();
            dic.TryGetValue(name, out o);
            if (!string.IsNullOrEmpty(o.name))
            {
                c_ip_local_comboBox.Text = o.ip_local;
                c_ip_nat_TextBox.Text = o.ip_nat;

                c_login_domain_comboBox.Text = o.login_domain;
                c_login_ip_textBox.Text = o.login_ip;
                c_login_port_textBox.Text = o.login_port.ToString();

                c_name_comboBox.Text = o.name;

                c_path_db_TextBox.Text = o.path_db;
                c_path_file_TextBox.Text = o.path_file;
                c_path_site_TextBox.Text = o.path_site;

                c_site_domain_comboBox.Text = o.site_domain;
                c_site_ip_textBox.Text = o.site_ip;
                c_site_port_textBox.Text = o.site_port.ToString();

                c_socket_domain_comboBox.Text = o.socket_domain;
                c_socket_ip_textBox.Text = o.socket_ip;
                c_socket_port_textBox.Text = o.socket_port.ToString();
            }
        }

        private void b_set_default_Click(object sender, EventArgs e)
        {
            string name = c_name_comboBox.Text;
            f_save("default");
            label_msg.Text = "Set config default is " + name + " succesfull";
        } // function


        private void f_save(string name)
        {
            name = name.ToUpper();
            c_name_comboBox.Text = name;

            string ip_local = c_ip_local_comboBox.Text.Trim();
            string ip_nat = c_ip_nat_TextBox.Text.Trim();

            string socket_ip = c_socket_ip_textBox.Text.Trim();
            string socket_domain = c_socket_domain_comboBox.Text.Trim();
            int socket_port = c_socket_port_textBox.Text.Trim().TryParseToInt();

            string login_ip = c_login_ip_textBox.Text.Trim();
            string login_domain = c_login_domain_comboBox.Text.Trim();
            int login_port = c_login_port_textBox.Text.Trim().TryParseToInt();

            string site_ip = c_site_ip_textBox.Text.Trim();
            string site_domain = c_site_domain_comboBox.Text.Trim();
            int site_port = c_site_port_textBox.Text.Trim().TryParseToInt();

            string path_file = c_path_file_TextBox.Text.Trim();
            string path_db = c_path_db_TextBox.Text.Trim();
            string path_site = c_path_site_TextBox.Text.Trim();

            #region // ... validate ...

            foreach (Control o in this.Controls)
            {
                if (o.Tag != null)
                {
                    string tag = o.Tag.ToString();
                    if (!string.IsNullOrEmpty(tag))
                    {
                        string val = o.Text;
                        if (tag.Contains("null") && string.IsNullOrEmpty(val))
                        {
                            MessageBox.Show("Vui lòng nhập chính xác thông tin");
                            o.Select();
                            o.Focus();
                            return;
                        }

                        if (tag.Contains("number"))
                        {
                            if (val.TryParseToInt() == 0)
                            {
                                MessageBox.Show("Vui lòng nhập chính xác thông tin");
                                o.Select();
                                o.Focus();
                                return;
                            }
                        }
                    }
                }
            }

            #endregion

            // Begin updating ...

            m_sys_config m = new m_sys_config();

            m.ip_local = ip_local;
            m.ip_nat = ip_nat;

            m.login_domain = login_domain;
            m.login_ip = login_ip;
            m.login_port = login_port;

            m.name = name;

            m.path_db = path_db;
            m.path_file = path_file;
            m.path_site = path_site;

            m.site_domain = site_domain;
            m.site_ip = site_ip;
            m.site_port = site_port;

            m.socket_domain = socket_domain;
            m.socket_ip = socket_ip;
            m.socket_port = socket_port;

            string path_folder = hostServer.pathConfig;
            hostFile.write_MMF<m_sys_config>(m, path_folder, name);

            if (dic.ContainsKey(name))
                dic[name] = m;
            else
                dic.Add(name, m);

            label_msg.Text = "Lưu cấu hình [ " + name.ToUpper() + " ] thành công";
        }

        private void b_reset_Click(object sender, EventArgs e)
        {
            main.reset();
        }

        private void apply_all_checkBox_CheckStateChanged(object sender, EventArgs e)
        {
            if (apply_all_checkBox.Checked)
            {
                string ip = c_ip_local_comboBox.Text;
                c_socket_ip_textBox.Text = ip;
                c_ip_nat_TextBox.Text = ip;
                c_login_ip_textBox.Text = ip;
                c_site_ip_textBox.Text = ip;
            }
            else
            {
                c_socket_ip_textBox.Text = "";
                c_ip_nat_TextBox.Text = "";
                c_login_ip_textBox.Text = "";
                c_site_ip_textBox.Text = "";
            }
        }

    }//end class
}
