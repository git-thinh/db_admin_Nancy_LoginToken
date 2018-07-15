using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using model;

namespace host
{
    public partial class fColumn_Edit : Form
    {
        private static int data_type = 0;
        private static int index = 0;

        public static bool update_ok = false;

        #region // ... form load ...

        public fColumn_Edit()
        {
            InitializeComponent();
        }

        public fColumn_Edit(int data_type_, int index_)
        {
            InitializeComponent();
            data_type = data_type_;
            index = index_;
        }

        private void fColumn_Edit_Load(object sender, EventArgs e)
        {
            m_column co = db_column.get_ItemByID(data_type, index);
            if (!string.IsNullOrEmpty(co.col_id))
            {
                lbl_data_type.Text = co.data_type.ToString();
                lbl_index.Text = co.index.ToString();

                txt_code.Text = co.code;
                txt_div10n.Text = co.div10n.ToString();
                txt_name.Text = co.name;

                txt_note.Text = co.note;
                txt_title.Text = co.title;

                txt_value_max.Text = co.value_max.ToString();
                txt_value_min.Text = co.value_min.ToString();

                check_index_date.Checked = co.index_date;
                check_index_time.Checked = co.index_time;
                check_progressive.Checked = co.progressive;

                string s_data_join = co.data_join;
                if (!string.IsNullOrEmpty(s_data_join))
                {
                    if (s_data_join.IndexOf("meter") != -1)
                        check_data_join_meter.Checked = true;

                    if (s_data_join.IndexOf("updown") != -1)
                        check_data_join_meter_updown.Checked = true;
                }
            }
        }

        #endregion


        public event EventHandler<Column_Edit_EventArgs> eventUpdated;
        private void OnEventUpdated(Column_Edit_EventArgs e)
        {
            EventHandler<Column_Edit_EventArgs> handler = eventUpdated;
            if (handler != null)
            {
                handler(null, e);
            }
        }









        private void txt_div10n_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = !char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar);
        }

        private void txt_value_min_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && (e.KeyChar != '.'))
            {
                e.Handled = true;
            }

            // only allow one decimal point
            if ((e.KeyChar == '.') && ((sender as TextBox).Text.IndexOf('.') > -1))
            {
                e.Handled = true;
            }
        }

        private void txt_value_max_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && (e.KeyChar != '.'))
            {
                e.Handled = true;
            }

            // only allow one decimal point
            if ((e.KeyChar == '.') && ((sender as TextBox).Text.IndexOf('.') > -1))
            {
                e.Handled = true;
            }
        }


        private void bUpdate_Config_Click(object sender, EventArgs e)
        {
            m_column co = db_column.get_ItemByID(data_type, index);
            if (!string.IsNullOrEmpty(co.col_id))
            {
                co.code = txt_code.Text.ToAscii().ToLower().Trim();
                co.name = txt_name.Text.ToAscii().ToLower().Trim();
                co.title = txt_title.Text.Trim();

                co.div10n = txt_div10n.Text.Trim().TryParseToInt();
                co.note = txt_note.Text;

                co.value_max = txt_value_max.Text.TryParseToInt();
                co.value_min = txt_value_min.Text.TryParseToInt();

                co.index_date = check_index_date.Checked;
                co.index_time = check_index_time.Checked;
                co.progressive = check_progressive.Checked;

                string s_data_join = "";
                if (check_data_join_meter.Checked) s_data_join = "meter";
                if (check_data_join_meter_updown.Checked)
                    if (s_data_join == "") s_data_join = "updown";
                    else s_data_join += ";updown";
                co.data_join = s_data_join;
            }

            var rs = db_column.edit_Item(co);
            if (rs)
            {
                update_ok = true;
                OnEventUpdated(new Column_Edit_EventArgs() { result = true, data_type = data_type });
                this.Close();
            }
        }


    } // end class
}
