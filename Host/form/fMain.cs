using model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Newtonsoft.Json;
using System.Text.RegularExpressions;
using System.Runtime.InteropServices;
using System.Configuration;

namespace host
{
    public partial class fMain : Form
    {
        #region // ... Variable ...

        public const int WM_NCLBUTTONDOWN = 0xA1;
        public const int HT_CAPTION = 0x2;

        [DllImportAttribute("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
        [DllImportAttribute("user32.dll")]
        public static extern bool ReleaseCapture();

        public static string reduce_select_id = "";

        //-------------------------------------------------------------------

        private static string s_key = "", s_date_mon = "", s_date_item = "", s_dcu = "", s_meter = "", s_type = "";

        private static List<int> ls_mon = new List<int>() { };
        private static List<int> ls_date_item = new List<int>() { };

        //private static string id = "";
        private static List<long> ls_dcu = new List<long>() { };
        private static List<long> ls_meter = new List<long>() { };

        private static List<long> ls_dcu_where = new List<long>() { };
        private static List<long> ls_meter_where = new List<long>() { };

        #endregion

        #region // ... Form load ...

        public fMain()
        {
            InitializeComponent();
        }

        private void fReduce_Query_Load(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Maximized;
            this.TopMost = false;
            this.ShowInTaskbar = true;

            int left = (System.Windows.Forms.Screen.PrimaryScreen.WorkingArea.Width - this.Width) / 2;
            int top = (System.Windows.Forms.Screen.PrimaryScreen.WorkingArea.Height - this.Height) / 2;

            this.Left = left;
            this.Top = top;

            var dt = db_reduce.get_All();
            gridView_Redure.AutoGenerateColumns = false;
            gridView_Redure.DataSource = dt;

            if (gridView_Redure.RowCount > 0)
            {
                reduce_select_id = gridView_Redure.Rows[gridView_Redure.CurrentCell.RowIndex].Cells[0].Value.ToString();
                lblReduce_select_name.Text = gridView_Redure.Rows[gridView_Redure.CurrentCell.RowIndex].Cells[2].Value.ToString();
            }

            f_form_load();
            tabConfig_Active(false);

            string pj_name = ConfigurationManager.AppSettings["project_name"];
            project_lable.Text = pj_name;
        }

        #endregion

        #region // ... Tab reduce manage ...


        private void bCache_Click(object sender, EventArgs e)
        {
            ////hostServer.openBrowser();
            this.WindowState = FormWindowState.Minimized;
            hostModule.CacheRefresh();
            main.show_notification("Cache OK", 5000);
        }

        private void bOpenBrowser_Click(object sender, EventArgs e)
        {
            hostServer.openBrowser();
        }



        public void reduce_reload()
        {
            var dt = db_reduce.get_All();
            gridView_Redure.DataSource = dt;
        }


        private void bReduce_search_Click(object sender, EventArgs e)
        {
            string keyword = txtReduce_Search.Text.Trim();
            var dt = db_reduce.get_Search(keyword);
            gridView_Redure.AutoGenerateColumns = false;
            gridView_Redure.DataSource = dt;

            gridView_Redure.CurrentCell = gridView_Redure[1, 0];

            //if (gridView_Redure.SelectedCells.Count > 0)
            //{
            //    var ri = gridView_Redure.SelectedCells[0].RowIndex;
            //    reduce_select_id = gridView_Redure.Rows[ri].Cells[0].Value.ToString();
            //    lblReduce_select_name.Text = gridView_Redure.Rows[ri].Cells[2].Value.ToString();
            //}
        }

        private void bClose_Click(object sender, EventArgs e)
        {
            //this.Hide();
            this.WindowState = FormWindowState.Minimized;
        }

        private void bReduce_Add_Click(object sender, EventArgs e)
        {
            tabMain.SelectedIndex = 1;
        }

        private void panelBar_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
            }
        }

        private void gridView_Redure_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                var cell = gridView_Redure.Rows[e.RowIndex].Cells[e.ColumnIndex];
                string id = gridView_Redure.Rows[e.RowIndex].Cells[0].Value.ToString();
                string name = gridView_Redure.Rows[e.RowIndex].Cells[2].Value.ToString();
                lblReduce_select_name.Text = name;
                reduce_select_id = id;
            }
            catch { }
        }

        private void bRedure_Edit_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(reduce_select_id))
            {
                fReduce_Edit f = new fReduce_Edit(reduce_select_id);
                f.Show();
            }
        }

        private void bRedure_Query_Click(object sender, EventArgs e)
        {
            //if (!string.IsNullOrEmpty(reduce_select_id))
            //{
            //main.hide_form_Main();
            //fReduce_Query f = new fReduce_Query(reduce_select_id);
            //f.Show();
            //}

            tabMain.SelectedIndex = 1;
        }

        private void gridView_Redure_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            var cell = gridView_Redure.Rows[e.RowIndex].Cells[e.ColumnIndex];
            string id = gridView_Redure.Rows[e.RowIndex].Cells[0].Value.ToString();
            string name = gridView_Redure.Rows[e.RowIndex].Cells[2].Value.ToString();
            lblReduce_select_name.Text = name;
            reduce_select_id = id;

            main.hide_form_Main();

            tabMain.SelectedIndex = 0;
            //fReduce_Query f = new fReduce_Query(reduce_select_id);
            //f.Show();
        }

        #endregion

        #region // ... Tab Query: button Compile, query ...

        private void bClear_Click(object sender, EventArgs e)
        {
            txt_key.Text = "";
            ls_dcu_where.Clear();
            ls_meter_where.Clear();

            s_key = "";
            s_date_mon = "";
            s_date_item = "";
            s_dcu = "";
            s_meter = "";
            s_type = "";
        }

        private void bClone_Click(object sender, EventArgs e)
        {
            reduce_select_id = "";
            f_save();
        }

        private void bUpdateRedure_Click(object sender, EventArgs e)
        {
            f_save();
        }

        private void bQuery_Click(object sender, EventArgs e)
        {
            f_query();
        }

        private void bCompile_Click(object sender, EventArgs e)
        {
            f_compile();
        }

        private void type__ItemCheck(object sender, ItemCheckEventArgs e)
        {
            //CheckedListBox o = (sender as CheckedListBox);
            // Get combobox selection (in handler)
            //string value = ((KeyValuePair<string, string>)o.SelectedItem).Key;

            //MessageBox.Show(value);
        }
        #endregion

        #region // ... Tab Query: GridView DCU, Meter ...

        private void f_item_search_by_dcu_select()
        {

            gridView_Item.AutoGenerateColumns = false;
            gridView_Item.DataSource = null;

            lbl_Item_count.Text = "(0)";

            Int32 selectedCellCount = gridView_DCU.GetCellCount(DataGridViewElementStates.Selected);
            if (selectedCellCount > 0)
            {
                //if (gridView_DCU.AreAllCellsSelected(true))
                //{
                //    // MessageBox.Show("All cells are selected", "Selected Cells");
                //}
                //else
                //{
                List<dynamic> ls = new List<dynamic>() { };
                List<int> rs = new List<int>() { };
                for (int i = 0; i < selectedCellCount; i++)
                    rs.Add(gridView_DCU.SelectedCells[i].RowIndex);
                rs = rs.Distinct().ToList();

                string keyword = txtItem_search.Text.Trim();

                for (int i = 0; i < rs.Count; i++)
                {
                    int r = rs[i];
                    long dcu_id = gridView_DCU.Rows[r].Cells[1].Value.ToString().TryParseToLong();
                    if (dcu_id > 0)
                    {
                        long[] a_meter = db_dcu.get_ItemsIndex(dcu_id);
                        if (a_meter.Length > 0)
                        {
                            ls_dcu.Add(dcu_id);
                            ls_meter.AddRange(a_meter);

                            if (string.IsNullOrEmpty(keyword))
                            {
                                var dt = db_meter.get_Items(a_meter).Select(x => new { meter_id = x.meter_id, name = x.name }).ToArray();
                                ls.AddRange(dt);
                            }
                            else
                            {
                                var dt = db_meter.get_Items(a_meter).Where(x => x.meter_id.ToString().Contains(keyword)).Select(x => new { meter_id = x.meter_id, name = x.name }).ToArray();
                                ls.AddRange(dt);
                            }
                        }
                    }
                }

                ls = ls.Distinct().ToList();


                //if (ls.Count > 0)
                //{
                gridView_Item.AutoGenerateColumns = false;
                gridView_Item.DataSource = ls;
                lbl_Item_count.Text = "(" + ls.Count.ToString() + ")";
                //}
                //}
            }
        }

        private void gridView_DCU_CellMouseUp(object sender, DataGridViewCellMouseEventArgs e)
        {
            f_item_search_by_dcu_select();
        }

        private void bItem_search_Click(object sender, EventArgs e)
        {
            f_item_search_by_dcu_select();
        }

        private void bDCU_where_Click(object sender, EventArgs e)
        {
            Int32 selectedCellCount = gridView_DCU.GetCellCount(DataGridViewElementStates.Selected);
            if (selectedCellCount > 0)
            {
                if (gridView_DCU.AreAllCellsSelected(true))
                {
                    // MessageBox.Show("All cells are selected", "Selected Cells");
                }
                else
                {
                    List<long> rs = new List<long>() { };
                    for (int i = 0; i < selectedCellCount; i++)
                    {
                        int r = gridView_DCU.SelectedCells[i].RowIndex;
                        long dcu_id = gridView_DCU.Rows[r].Cells[1].Value.ToString().TryParseToLong();
                        rs.Add(dcu_id);
                    }
                    rs = rs.Distinct().ToList();
                    ls_dcu_where.AddRange(rs);
                }
            }

            f_compile();
        }

        /// <summary>
        /// Double click meter_id to remove or addnew clause where: *.dcu_id;meter_id_1;meter_id_2;...
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void bMeter_where_Click(object sender, EventArgs e)
        {
            Int32 selectedCellCount = gridView_Item.GetCellCount(DataGridViewElementStates.Selected);
            if (selectedCellCount > 0)
            {
                List<long> rs = new List<long>() { };
                for (int i = 0; i < selectedCellCount; i++)
                {
                    int r = gridView_Item.SelectedCells[i].RowIndex;
                    long mid = gridView_Item.Rows[r].Cells[1].Value.ToString().TryParseToLong();
                    rs.Add(mid);
                }
                rs = rs.Distinct().ToList();
                ls_meter_where.AddRange(rs);

                f_compile();
            }
        }

        #endregion



        private void bQueryPage_Click(object sender, EventArgs e)
        {
            f_query();
        }

        List<decimal[]> dt_query = new List<decimal[]>() { };

        private void f_query()
        {
            f_compile();
            txt_query_json_view.Text = "";
            tabMain.SelectedIndex = 2;

            int[] w_type = s_type.Split(';').Where(x => x != "").Select(x => x.TryParseToInt()).ToArray();
            byte yy_today = DateTime.Now.ToString("yy").TryParseToByte();
            List<Tuple<byte, int>> w_date = s_date_item.Split(';').Where(x => x != "").Select(x => new Tuple<byte, int>(yy_today, x.TryParseToInt())).ToList();
            long[] w_meter = s_meter.Split(';').Where(x => x != "").Select(x => x.TryParseToLong()).ToArray();

            string s_value = "";

            e_query_data data_type = e_query_data.data_type;
            if (Data_type_detail_checkBox.Checked) data_type = e_query_data.data_detail;

            var rs = store.query("",
                w_date, w_meter,
                w_type, data_type,
                s_value,
                1, 1);

            f_query_bind(rs);
        }

        private void f_query_bind(Tuple<long, long, int[], long[], List<decimal[]>> rs)
        {
            tabResultQuery.TabPages.Clear();

            int[] a_type = rs.Item3;
            int type_first = 0;

            if (a_type.Length > 0)
            {
                type_first = a_type[0];

                foreach (var d_type in a_type)
                {
                    TabPage tab = new TabPage();
                    tab.Text = d_type.ToString();

                    Label lbl = new Label();
                    lbl.AutoSize = false;
                    lbl.Height = 30;
                    lbl.TextAlign = ContentAlignment.MiddleLeft;
                    lbl.Dock = DockStyle.Top;
                    lbl.Text = d_type.dataType_ToText();

                    DataGridView gridResult = new DataGridView();
                    gridResult.Dock = DockStyle.Fill;

                    tab.Controls.Add(lbl);
                    tab.Controls.Add(gridResult);
                    gridResult.BringToFront();

                    tabResultQuery.TabPages.Add(tab);

                    int page_number = txt_page_number.Text.Trim().TryParseToInt();
                    int page_size = txt_page_size.Text.Trim().TryParseToInt();

                    byte yy_today = DateTime.Now.ToString("yy").TryParseToByte();
                    List<Tuple<byte, int>> w_date = s_date_item.Split(';').Where(x => x != "").Select(x => new Tuple<byte, int>(yy_today, x.TryParseToInt())).ToList();
                    //int[] w_date = s_date_item.Split(';').Where(x => x != "").Select(x => x.TryParseToInt()).ToArray();
                    long[] w_meter = s_meter.Split(';').Where(x => x != "").Select(x => x.TryParseToLong()).ToArray();

                    string s_value = "";

                    var rsi = store.query("",
                        w_date, w_meter,
                        new int[] { d_type }, e_query_data.data_detail,
                        s_value,
                        page_number, page_size);

                    tab.Tag = "Total: " + rsi.Item2.ToString() + " / " + rsi.Item1.ToString() + " record";

                    //grid.DataSource = rsi.Item4;

                    gridResult.Columns.Clear();

                    var dta = rsi.Item5;
                    if (dta.Count > 0)
                    {
                        //var col_max = dt_query.Select(x => x.Length).Max();
                        //int d_type = (int)dt_query.Select(x => x[0]).Distinct().Take(1).SingleOrDefault();
                        //f_config_load(d_type, col_max);

                        int col_data_len = dta[0].Length;

                        var a_col = db_column.get_Items(d_type);
                        int col_len = a_col.Length;
                        if (col_len == 0)
                        {
                            int col_max_result = 0;
                            if (dta.Count > 0) col_max_result = dta[0].Length;
                            if (col_max_result > 0)
                            {
                                a_col = Enumerable.Range(0, col_max_result)
                                    .Select(x => new m_column()
                                    {
                                        data_type = d_type,
                                        index = x,
                                        data_join = "",
                                        code = x < store.cols_index.Length ? store.cols_index[x] : x.ToString(),
                                        name = x < store.cols_index.Length ? store.cols_index[x] : x.ToString(),
                                        div10n = 0,
                                        index_date = false,
                                        index_time = false,
                                        value_min = 0,
                                        value_max = 0,
                                        note = "",
                                        title = x < store.cols_index.Length ? store.cols_index[x] : x.ToString(),
                                        progressive = false
                                    })
                                    .ToArray();
                                db_column.save_Items(d_type, a_col);
                                col_len = a_col.Length;
                            }
                        }

                        if (col_data_len > col_len)
                        {
                            List<m_column> lmax = a_col.ToList();
                            for (int k = 0; k < col_data_len - col_len; k++)
                            {
                                int x = k + col_len + 1;
                                lmax.Insert(k + col_len, new m_column()
                                    {
                                        data_type = d_type,
                                        index = x,
                                        data_join = "",
                                        code = x < store.cols_index.Length ? store.cols_index[x] : x.ToString(),
                                        name = x < store.cols_index.Length ? store.cols_index[x] : x.ToString(),
                                        div10n = 0,
                                        index_date = false,
                                        index_time = false,
                                        value_min = 0,
                                        value_max = 0,
                                        note = "",
                                        title = x < store.cols_index.Length ? store.cols_index[x] : x.ToString(),
                                        progressive = false
                                    });
                            }
                            a_col = lmax.ToArray();

                            db_column.save_Items(d_type, a_col);
                            col_len = col_data_len;
                        }

                        if (col_len > 0)
                        {
                            gridResult.Columns.Add("STT", "STT");
                            for (int k = 0; k < col_len; k++)
                            {
                                if (a_col.Length == col_len)
                                    gridResult.Columns.Add("Col_" + k.ToString(), a_col[k].code);
                                else
                                    gridResult.Columns.Add("Col_" + k.ToString(), k.ToString());
                            }

                            for (int k = 0; k < col_len + 1; k++)
                                gridResult.Columns[k].SortMode = DataGridViewColumnSortMode.NotSortable;

                            char c_rf = d_type.ToString()[3]; //1,001,000
                            for (int i = 0; i < dta.Count; i++)
                            {

                                // [0] data_type, [1] file_id, [2] dcu_id, [3] device_id
                                // [4] yyyyMMdd, [5] HHmmss
                                // [6] phase: 1pha, [7] data, [8] bieu, [9] tech, [10] factory 

                                var li = dta[i].Select((x, id) =>
                                {
                                    object rso = new object();
                                    switch (id)
                                    {
                                        case 6:
                                            rso = ((d1_pha)x).ToString();
                                            break;
                                        case 7:
                                            rso = ((d2_data)x).ToString();
                                            break;
                                        case 8:
                                            rso = ((d4_tech)x).ToString();
                                            break;
                                        case 9:
                                            rso = ((d5_nsx)x).ToString();
                                            break;
                                        default:
                                            if (c_rf == '1')
                                            {
                                                int div10n = a_col[id].div10n;
                                                if (div10n > 0)
                                                    rso = x / div10n;
                                                else
                                                    rso = x;
                                            }
                                            else
                                                rso = x;

                                            break;
                                    }
                                    return rso;
                                }).Cast<object>().ToList();

                                //.Cast<object>().ToList();
                                li.Insert(0, i + 1);

                                gridResult.Rows.Add(li.ToArray());
                            }

                            gridResult.AutoResizeColumns();
                            gridResult.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;

                        }//end if col_len > 0
                    }

                }
            }

        }

        private void f_form_load()
        {
            this.Text = "//Tạo mới thủ tục";

            if (!string.IsNullOrEmpty(reduce_select_id))
            {
                var o = db_reduce.get_ItemByID(reduce_select_id);
                if (o.api != null)
                {
                    this.Text = "API: " + o.api + " | " + o.name;
                    txt_API.Text = o.api;
                    txt_Name.Text = o.name;

                    s_date_item = o.w_date_item;
                    s_date_mon = o.w_date_mon;
                    s_dcu = o.w_dcu;
                    s_meter = o.w_meter;
                    s_type = o.w_type;

                    txt_where_value.Text = o.w_value;

                    f_print();
                }
            }

            var dt = db_dcu.get_All().Select(x => new { dcu_id = x.dcu_id, name = x.name, itemsub_count = x.itemsub_count }).ToArray();
            gridView_DCU.AutoGenerateColumns = false;
            gridView_DCU.DataSource = dt;

            Dictionary<string, string> dt_date = new Dictionary<string, string>();
            var date = store.get_DateHasData();
            if (date.Length > 0)
            {
                for (int k = 0; k < date.Length; k++)
                {
                    int id_date = date[k];
                    string date_s = msgConverter.f_DayOfYearToDate_dd_MM_yyyy(id_date);
                    if (!dt_date.ContainsKey(id_date.ToString())) dt_date.Add(id_date.ToString(), date_s);
                }
            }
            if (dt_date.Count > 0)
            {
                list_date.DataSource = new BindingSource(dt_date, null);
                list_date.DisplayMember = "Value";
                list_date.ValueMember = "Key";
                list_date.ItemCheck += type__ItemCheck;
            }

            Dictionary<string, string> dt_loai_pha = new Dictionary<string, string>();
            dt_loai_pha.Add("0,000,000", "");
            dt_loai_pha.Add("1,000,000", "1 Pha");
            dt_loai_pha.Add("3,000,000", "3 Pha");
            dt_loai_pha.Add("4,000,000", "Chieu sang");
            type_01_Loai_Pha.DataSource = new BindingSource(dt_loai_pha, null);
            type_01_Loai_Pha.DisplayMember = "Value";
            type_01_Loai_Pha.ValueMember = "Key";
            //type_01_Loai_Pha.SetItemChecked(0, true);
            type_01_Loai_Pha.ItemCheck += type__ItemCheck;

            Dictionary<string, string> dt_loai_data = new Dictionary<string, string>();
            dt_loai_data.Add("100,000", "TSVH");
            dt_loai_data.Add("200,000", "TSTT");
            dt_loai_data.Add("300,000", "LOAD PROFILE");
            dt_loai_data.Add("400,000", "LOAD PROFILE DAY");
            dt_loai_data.Add("500,000", "EVEN");
            dt_loai_data.Add("600,000", "FIX DAY");
            dt_loai_data.Add("700,000", "FIX MONTH");
            type_02_Loai_Du_Lieu.DataSource = new BindingSource(dt_loai_data, null);
            type_02_Loai_Du_Lieu.DisplayMember = "Value";
            type_02_Loai_Du_Lieu.ValueMember = "Key";
            type_02_Loai_Du_Lieu.SetItemChecked(0, true);
            type_02_Loai_Du_Lieu.ItemCheck += type__ItemCheck;


            Dictionary<string, string> dt_loai_congnghe = new Dictionary<string, string>();
            dt_loai_congnghe.Add("0,000", "");
            dt_loai_congnghe.Add("1,000", "RF");
            dt_loai_congnghe.Add("2,000", "PLC");
            dt_loai_congnghe.Add("3,000", "BLUETOOTH");
            dt_loai_congnghe.Add("4,000", "GPRS");
            dt_loai_congnghe.Add("5,000", "WIFI");
            dt_loai_congnghe.Add("6,000", "ETHERNET");
            type_04_Cong_Nghe.DataSource = new BindingSource(dt_loai_congnghe, null);
            type_04_Cong_Nghe.DisplayMember = "Value";
            type_04_Cong_Nghe.ValueMember = "Key";
            //type_04_Cong_Nghe.SetItemChecked(0, true);
            //type_04_Cong_Nghe.SetItemChecked(1, true);
            //type_04_Cong_Nghe.SetItemChecked(2, true);
            //type_04_Cong_Nghe.SetItemChecked(3, true);
            type_04_Cong_Nghe.ItemCheck += type__ItemCheck;

            Dictionary<string, string> dt_loai_nsx = new Dictionary<string, string>();
            dt_loai_nsx.Add("000", "");
            dt_loai_nsx.Add("001", "PSMART");
            dt_loai_nsx.Add("002", "VNSINO");
            dt_loai_nsx.Add("003", "OMNI");
            type_05_Loai_NSX.DataSource = new BindingSource(dt_loai_nsx, null);
            type_05_Loai_NSX.DisplayMember = "Value";
            type_05_Loai_NSX.ValueMember = "Key";
            //type_05_Loai_NSX.SetItemChecked(0, true);
            //type_05_Loai_NSX.SetItemChecked(1, true);
            //type_05_Loai_NSX.SetItemChecked(2, true);
            //type_05_Loai_NSX.SetItemChecked(3, true);
            type_05_Loai_NSX.ItemCheck += type__ItemCheck;
        }

        private void f_print()
        {
            s_key = s_date_mon + s_date_item + s_dcu + s_meter + s_type;
            string text =
                "key: " + s_key.Length.ToString() + " length\n\n" + s_key + "\n\n" +
                "--------------------------------------------\n" +
                "month: [" + s_date_mon.Split(';').Length.ToString() + "]\n\n" + s_date_mon + "\n\n" +
                "--------------------------------------------\n" +
                "date: [" + s_date_item.Split(';').Length.ToString() + "]\n\n" + s_date_item + "\n\n" +
                "--------------------------------------------\n" +
                "dcu: [" + ls_dcu_where.Count.ToString() + "]\n\n" + s_dcu + "\n\n" +
                "--------------------------------------------\n" +
                "meter: " + ls_meter_where.Count.ToString() + "\n\n" + s_meter + "\n\n" +
                "--------------------------------------------\n" +
                "data_type: \n\n" + s_type + "\n";

            text = text.Replace("\n", Environment.NewLine);
            txt_key.Text = text;
        }

        private void f_compile()
        {
            s_key = "";
            s_date_mon = "";
            s_date_item = "";
            s_dcu = "";
            s_meter = "";
            s_type = "";

            #region // ... data_type ...

            var dates = list_date.CheckedItems.Cast<KeyValuePair<string, string>>()
                    .Select(x => x.Key.Replace(",", "").TryParseToInt())
                    .ToArray();
            var loai1_phas = type_01_Loai_Pha.CheckedItems.Cast<KeyValuePair<string, string>>()
                .Select(x => x.Key.Replace(",", "").TryParseToInt())
                .ToArray();
            var loai2_du_lieus = type_02_Loai_Du_Lieu.CheckedItems.Cast<KeyValuePair<string, string>>()
                .Select(x => x.Key.Replace(",", "").TryParseToInt())
                .ToArray();
            var loai4_congnghes = type_04_Cong_Nghe.CheckedItems.Cast<KeyValuePair<string, string>>()
                .Select(x => x.Key.Replace(",", "").TryParseToInt())
                .ToArray();
            var loai5_nsx = type_05_Loai_NSX.CheckedItems.Cast<KeyValuePair<string, string>>()
                .Select(x => x.Key.Replace(",", "").TryParseToInt())
                .ToArray();

            if (loai1_phas.Length > 0)
            {
                if (loai1_phas[0] == 0 || (loai1_phas.Length == 1 && loai1_phas[0] == 0))
                    loai1_phas = type_01_Loai_Pha.Items.Cast<KeyValuePair<string, string>>()
                        .Select(x => x.Key.Replace(",", "").TryParseToInt())
                        .Where(x => x > 0)
                        .ToArray();
            }


            if (loai4_congnghes.Length == 1 && loai4_congnghes[0] == 0)
                loai4_congnghes = type_04_Cong_Nghe.Items.Cast<KeyValuePair<string, string>>()
                    .Select(x => x.Key.Replace(",", "").TryParseToInt())
                    .ToArray();

            if (loai5_nsx.Length == 1 && loai5_nsx[0] == 0)
                loai5_nsx = type_05_Loai_NSX.Items.Cast<KeyValuePair<string, string>>()
                    .Select(x => x.Key.Replace(",", "").TryParseToInt())
                    .ToArray();


            if (loai2_du_lieus.Length == 0)
            {
                MessageBox.Show("Vui lòng chọn loại dữ liệu");
                return;
            }

            if (loai1_phas.Length == 0) loai1_phas = new int[] { 0 };
            if (loai4_congnghes.Length == 0) loai4_congnghes = new int[] { 0 };
            if (loai5_nsx.Length == 0) loai5_nsx = new int[] { 0 };

            List<int> ls_type = new List<int>() { };
            foreach (var pha in loai1_phas)
            {
                foreach (var data in loai2_du_lieus)
                {
                    foreach (var congnghe in loai4_congnghes)
                    {
                        foreach (var nsx in loai5_nsx)
                        {
                            int data_type = pha + data + congnghe + nsx;
                            ls_type.Add(data_type);
                        }
                    }
                }
            }//end for each 

            //Data_type_detail_checkBox.Checked = ls_type.Where(x => x.ToString().Length == 6).Select(x => x.ToString().Replace("0", "").Length).Where(x => x == 1).Count() == 0 ? true : false;

            s_type = string.Join(";", ls_type.ToArray());

            #endregion

            #region // ... dcu ...

            ls_dcu_where = ls_dcu_where.Distinct().ToList();
            ls_meter_where = ls_meter_where.Distinct().ToList();

            s_dcu = string.Join(";", ls_dcu_where.ToArray());

            List<long> ls_dcu_mid = new List<long>() { };
            for (int k = 0; k < ls_dcu_where.Count; k++)
            {
                long dcu_id = ls_dcu_where[k];
                long[] a_meter = db_dcu.get_ItemsIndex(dcu_id);
                ls_dcu_mid.AddRange(a_meter);
            }

            #endregion

            // meter 
            ls_meter_where = ls_meter_where.Where(x => !ls_dcu_mid.Any(o => o == x)).ToList();
            s_meter = string.Join(";", ls_meter_where.ToArray());

            #region // ... date ...

            if (date_SelectAll.Checked)
            {
                s_date_item = "";
                s_date_mon = "";
            }
            else
            {
                foreach (var r in date_month.CheckedItems)
                {
                    int value = r.ToString().TryParseToInt();
                    ls_mon.Add(value);
                }
                ls_mon = ls_mon.Distinct().ToList();
                s_date_mon = string.Join(";", ls_mon);

                foreach (var r in list_date.CheckedItems)
                {
                    int value = ((KeyValuePair<string, string>)r).Key.TryParseToInt();
                    ls_date_item.Add(value);
                }
                ls_date_item = ls_date_item.Distinct().ToList();
                s_date_item = string.Join(";", ls_date_item);
            }

            #endregion

            f_print();
        }//end function compile

        private void f_save()
        {
            string api = txt_API.Text.Trim().ToLower();
            string name = txt_Name.Text.Trim();

            if (api == "")
            {
                MessageBox.Show("Nhập mã truy vấn thủ tục");
                return;
            }

            if (name == "")
            {
                MessageBox.Show("Nhập tên thủ tục");
                return;
            }

            m_reduce o = new m_reduce();
            o.api = api;
            o.name = name;

            o.w_date_item = s_date_item;
            o.w_date_mon = s_date_mon;
            o.w_dcu = s_dcu;
            o.w_meter = s_meter;
            o.w_type = s_type;
            o.w_value = txt_where_value.Text.Trim();

            if (string.IsNullOrEmpty(reduce_select_id))
            {
                if (db_reduce.check_ExistAPI(api))
                {
                    MessageBox.Show("API: " + api + " >> Đã tồn tại. Vui lòng sửa lại tên API.");
                    txt_API.Focus();
                    return;
                }

                var rs = db_reduce.add_Item(o);
                if (!string.IsNullOrEmpty(rs))
                {
                    reduce_select_id = rs;
                    this.Text = "API:" + api + " | " + name;

                    main.main_redure_reload();
                    main.show_notification("Thêm mới thủ tục thành công", 3000);
                    main.show_form_Main();
                }
                else
                    main.show_notification("Thêm mới thủ tục không thành công. Kiểm tra lại hệ thống.", 3000);
            }
            else
            {
                var m = db_reduce.get_ItemByAPI(api);
                if (m.id == null || m.id == reduce_select_id)
                {
                    o.id = reduce_select_id;
                    var rs = db_reduce.edit_Item(o);
                    if (rs)
                    {
                        main.main_redure_reload();
                        main.show_notification("Cập nhật thủ tục thành công", 3000);
                        //this.Close();
                    }
                    else
                        main.show_notification("Cập nhật thủ tục không thành công. Kiểm tra lại hệ thống.", 3000);
                }
                else
                {
                    MessageBox.Show("API: " + api + " >> Đã tồn tại. Vui lòng sửa lại tên API.");
                    txt_API.Focus();
                    return;
                }

            }
        }


        #region // ... json view result ...

        private void bQueryJsonView_Click(object sender, EventArgs e)
        {
            if (tabResultQuery.TabPages.Count > 0)
            {
                int d_type = tabResultQuery.SelectedTab.Text.TryParseToInt();

                int page_number = txt_page_number.Text.Trim().TryParseToInt();
                int page_size = txt_page_size.Text.Trim().TryParseToInt();

                byte yy_today = DateTime.Now.ToString("yy").TryParseToByte();
                List<Tuple<byte, int>> w_date = s_date_item.Split(';').Where(x => x != "").Select(x => new Tuple<byte, int>(yy_today, x.TryParseToInt())).ToList();
                //int[] w_date = s_date_item.Split(';').Where(x => x != "").Select(x => x.TryParseToInt()).ToArray();

                long[] w_meter = s_meter.Split(';').Where(x => x != "").Select(x => x.TryParseToLong()).ToArray();

                string s_value = "";

                var rsi = store.query("",
                    w_date, w_meter,
                    new int[] { d_type }, e_query_data.data_detail,
                    s_value,
                    page_number, page_size);

                var dta = rsi.Item5;
                if (dta.Count > 0)
                {
                    var a_col = db_column.get_Items(d_type);
                    char c_rf = data_type.ToString().ToCharArray()[3]; //1,000 = rf
                    var l0 = dta.Select((ar, id) =>
                    {
                        string[] lii = new string[] { };

                        lii = ar.Zip(a_col, (val, col) =>
                        {
                            string sii = "";
                            object rso = "";

                            switch (col.index)
                            {
                                case 6:
                                    rso = @"""" + ((d1_pha)val).ToString() + @"""";
                                    break;
                                case 7:
                                    rso = @"""" + ((d2_data)val).ToString() + @"""";
                                    break;
                                case 8:
                                    rso = @"""" + ((d4_tech)val).ToString() + @"""";
                                    break;
                                case 9:
                                    rso = @"""" + ((d5_nsx)val).ToString() + @"""";
                                    break;
                                default:
                                    int div10n = col.div10n;
                                    if (div10n > 0)
                                        rso = val / div10n;
                                    else
                                        rso = val;

                                    break;
                            }

                            sii = @"""" + col.code + @""":" + rso.ToString();

                            return sii;
                        }).ToArray();

                        return "{" + string.Join(",", lii) + "}";
                    }).ToArray();

                    string json = @" ""data"":[" + Environment.NewLine + string.Join("," + Environment.NewLine, l0) + Environment.NewLine + "] ";

                    var a_meter0 = dt_query.Select(x => (long)x[3]).Distinct().ToArray();
                    var a_meter1 = db_meter.get_Items(a_meter0);
                    string json1 = Environment.NewLine + @",""meter"":" + Environment.NewLine + JsonConvert.SerializeObject(a_meter1);

                    var a_kh = a_meter1.Select(x => x.cus_code.ToString())
                        .Select(x => Regex.Replace(x, "[^0-9a-zA-Z]+", string.Empty, RegexOptions.IgnoreCase).Trim().TryParseToInt())
                        .Where(x => x > 0)
                        .Distinct().ToArray();
                    var a_treothao = db_updown.get_ItemByCusCode(a_kh);
                    string json2 = Environment.NewLine + @",""updown"":" + Environment.NewLine + JsonConvert.SerializeObject(a_treothao);

                    string jo = "[{ " + Environment.NewLine + json + json1 + json2 + Environment.NewLine + " }]";
                    txt_query_json_view.Text = jo;
                }
            }
        }

        #endregion

        private void bDCU_search_Click(object sender, EventArgs e)
        {

        }

        #region // ... tab config .....



        public void tabConfig_Active(bool enable)
        {
            foreach (Control ctl in tabConfig.Controls)
                ctl.Enabled = enable;
        }

        private void f_config_load(int data_type, int col_max_result)
        {
            gridConfig.AutoGenerateColumns = false;
            gridConfig.DataSource = null;

            var ls = db_column.get_Items(data_type);

            if (ls.Length < col_max_result)
            {
                var a_new = Enumerable.Range(0, col_max_result)
                    .Where(x => !ls.Any(o => o.index == x))
                    .ToArray();

                var a0 = a_new
                    .Select(x => new m_column()
                    {
                        data_type = data_type,
                        index = x,
                        data_join = "",
                        code = x < store.cols_index.Length ? store.cols_index[x] : x.ToString(),
                        name = x < store.cols_index.Length ? store.cols_index[x] : x.ToString(),
                        div10n = 0,
                        index_date = false,
                        index_time = false,
                        value_min = 0,
                        value_max = 0,
                        note = "",
                        title = x < store.cols_index.Length ? store.cols_index[x] : x.ToString(),
                        progressive = false
                    })
                    .ToArray();
                db_column.save_Items(data_type, a0);
            }

            f_config_load_grid(data_type);
        }

        private void f_config_load_grid(int data_type)
        {
            tabConfig_Active(true);

            var a_col = db_column.get_Items(data_type)
                    .Select(x => new
                    {
                        data_type = x.data_type,
                        index = x.index,
                        data_join = x.data_join == null ? "" : x.data_join,
                        code = x.code,
                        name = x.name,
                        div10n = x.div10n,
                        index_date = x.index_date,
                        index_time = x.index_time,
                        progressive = x.progressive,
                        value_min = x.value_min,
                        value_max = x.value_max,
                        note = x.note,
                        title = x.title
                    }).ToArray();

            gridConfig.DataSource = a_col;
            gridConfig.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.DisplayedCells;
        }

        private void gridConfig_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            int r = gridConfig.CurrentCell.RowIndex;
            int data_type = gridConfig.Rows[r].Cells[0].Value.ToString().TryParseToInt();
            int index = gridConfig.Rows[r].Cells[1].Value.ToString().TryParseToInt();
            if (data_type > 0)
            {
                fColumn_Edit f = new fColumn_Edit(data_type, index);
                f.Show();
                f.eventUpdated += f_eventUpdated;
            }
        }

        private void f_eventUpdated(object sender, Column_Edit_EventArgs e)
        {
            if (e.result)
            {
                f_config_load_grid(e.data_type);
            }
        }



        private void tabResultQuery_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (tabResultQuery.SelectedTab != null)
                lbl_query_total_record.Text = tabResultQuery.SelectedTab.Tag.ToString();
        }

        private void b_config_Click(object sender, EventArgs e)
        {
            if (tabResultQuery.SelectedTab != null)
            {
                int d_type = tabResultQuery.SelectedTab.Text.TryParseToInt();

                tabMain.SelectedIndex = 3;
                tabConfig_Active(true);

                label_data_type_text.Text = d_type.dataType_ToText();

                f_config_load_grid(d_type);
            }
        }

        #endregion


        #region // ... Config System ...

        private void b_sys_config_Click(object sender, EventArgs e)
        {
            fSysConfig f = new fSysConfig();
            f.ShowDialog();
        }

        #endregion

        private void b_plc_export_Click(object sender, EventArgs e)
        {
            fPLC_export_mmf f = new fPLC_export_mmf();
            f.ShowDialog();
        }

        private void button_write_DB_Click(object sender, EventArgs e)
        {
            fWrite_DB form = new fWrite_DB();
            form.Show();
        }



    }//end class
}
