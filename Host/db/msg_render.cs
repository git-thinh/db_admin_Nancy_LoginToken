using model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace host
{
    public enum msg_type
    {
        json = 0,
        xml = 1,
        table_td = 2
    }

    public class msg_render
    {
        public static string col_tbody_key = "##bodyitems##";
        public static string col_index_key = "{{index_}}";

        public static List<string> store(
            msg_type msg_type_result,
            List<decimal[]> dta,
            Dictionary<int, m_column[]> dic_cols,
            string tem_item = "", string tem_item_def = "")
        {
            //Dictionary<int, m_column[]> dic_type_col = new Dictionary<int, m_column[]>() { };
            List<string> ls_json = new List<string>() { };


            var a_type_col = dta.Select(x => x[0]).Distinct().ToArray();

            //foreach (int id_type in a_type_col)
            //{
            //    m_column[] ad_col = new m_column[] { };
            //    if (dic_cols.TryGetValue(id_type, out ad_col))
            //    {
            //        var a_col_duple = ad_col.GroupBy(x => x.code)
            //            .Select(x => new { k = x.Count(), code = x.Key })
            //            .Where(x => x.k > 1).Select(x => x.code).ToList();
            //        if (a_col_duple.Count == 0)
            //            dic_type_col.Add(id_type, ad_col);
            //        else
            //        {
            //            for (int ki = 0; ki < ad_col.Length; ki++)
            //            {
            //                string code_i = ad_col[ki].code;
            //                if (a_col_duple.IndexOf(code_i) != -1)
            //                    ad_col[ki].code = code_i + "_" + ki.ToString();
            //            }
            //        }
            //    }
            //}

            bool xxx = false;

            string[] cols_select = Regex.Matches(tem_item, @"{(.+?)}").Cast<Match>()
                .Select(o => o.Groups[1].Value).ToArray();

            ls_json = dta.Select((ar, id) =>
            {
                // [0] data_type, [1] file_id, [2] dcu_id, [3] device_id
                // [4] yyyyMMdd, [5] HHmmss
                // [6] phase: 1pha, [7] data, [8] bieu, [9] tech, [10] factory 
                switch (msg_type_result)
                {
                    default:
                        return convert_Item2Json(cols_select, dic_cols, ar, id);
                    case msg_type.table_td:
                        return convert_Item2TD(tem_item, tem_item_def, cols_select, dic_cols, ar, id);
                }
            }).Where(x => x != "").ToList();

            return ls_json;
        }

        private static string convert_Item2TD(
            string tem_item,
            string tem_item_def,
            string[] cols_select,
            Dictionary<int, m_column[]> dic_type_col,
            decimal[] ar, int id)
        {
            string tr = tem_item_def;

            if(id == 252)
                tr = tem_item_def;

            int d_type = (int)ar[0];

            d2_data data_type = d2_data._;
            switch (d_type.ToString().Length)
            {
                case 6:
                    data_type = (d2_data)(d_type.ToString().Substring(0, 1).TryParseToInt() * 100000);
                    break;
                case 7:
                    data_type = (d2_data)(d_type.ToString().Substring(1, 1).TryParseToInt() * 100000);
                    break;
            }

            m_column[] a_col = new m_column[] { };
            if (dic_type_col.TryGetValue(d_type, out a_col))
            {
                tr = tem_item;
                string ss = "";
                int len = ar.Length;
                for (int k = 0; k < a_col.Length; k++)
                {
                    try
                    {
                        string val = "-";
                        int index = a_col[k].index;
                        if (index < len)
                        {
                            val = ar[k].ToString();
                            int div10n = a_col[k].div10n;
                            if (div10n > 0)
                                val = ((decimal)ar[k] / div10n).ToString("0,0.00");
                        }


                        string code = a_col[k].code;
                        switch (code)
                        {
                            case "device_id":
                            case "meter_id":
                                tr = tr.Replace("{{device_id}}", val)
                                    .Replace("{{meter_id}}", val);
                                break;
                            case "yyyyMMdd":
                                tr = tr.Replace("{{yyyyMMdd}}", "&nbsp;" + f_yyyyMMdd(data_type, val));
                                break;
                            case "HHmmss":
                                tr = tr.Replace("{{HHmmss}}", "&nbsp;" + f_HHmmss(data_type, val));
                                break;
                            default:
                                tr = tr.Replace("{{" + a_col[k].code + "}}", val);
                                break;
                        }
                    }
                    catch (Exception ex)
                    {
                        ss = ex.Message + k.ToString();
                    }
                }

                tr = tr.Replace(col_index_key, (id + 1).ToString());
            }

            return tr;
        }


        private static string convert_Item2Json(
            string[] cols_select, Dictionary<int, m_column[]> dic_type_col,
            decimal[] ar, int id)
        {
            try
            {
                #region

                long me_id = (long)ar[3];
                if (me_id < 1000000) return "";

                int d_type = (int)ar[0];
                m_column[] a_col = new m_column[] { };
                if (dic_type_col.TryGetValue(d_type, out a_col))
                {
                    string error_Parse = "";
                    string[] lii = new string[] { };

                    #region /// process cols ...

                    d4_tech tech = d4_tech._;
                    if (ar.Length > 9) tech = (d4_tech)ar[9];

                    int len_val = a_col.Length - ar.Length;
                    string col_miss = "";
                    if (len_val > 0)
                    {
                        for (int x = 0; x < len_val; x++)
                        {
                            m_column cx = a_col[x + ar.Length];
                            col_miss += @",""" + cx.code + @""":""-""";
                        }
                    }

                    lii = ar.Zip(a_col, (val, col) =>
                    {
                        #region // ...

                        string sii = "";
                        object rso = "";

                        if (col.code.Length == 2)
                        {
                            error_Parse = "config data type " + d_type.ToString();
                            return @"""_x" + col.code + @""":" + val.ToString();
                        }
                        else
                        {
                            #region //...

                            try
                            {
                                if (d_type > 999)
                                {
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
                                                rso = (decimal)val / div10n;
                                            else
                                                rso = (decimal)val;
                                            break;
                                    }
                                }
                                else
                                    rso = (decimal)val;

                                sii = @"""" + col.code + @""":" + rso.ToString();
                            }
                            catch (Exception ex)
                            {
                                error_Parse = ex.ToString();
                                return @"""" + col.code + @"_error"":""" + val.ToString() + "|" + error_Parse + @"""";
                            }

                            #endregion
                        }
                        return sii;

                        #endregion

                    }).ToArray();

                    #endregion

                    string s_stt_export = "";
                    if (error_Parse == "")
                        s_stt_export = @"""__ok"":true, ";
                    else
                        s_stt_export = @"""__ok"":false, ""__msg"":""" + error_Parse + @""", ";

                    string json = "";
                    if (lii.Length > 0) json = "{" + s_stt_export + string.Join(",", lii) + col_miss + "}";
                    return json;
                }

                #endregion
            }
            catch (Exception ex)
            {
                return "{" +
                    @"""msg"":""" +
                        ex.Message.Replace(":", "").Replace("\\", "").Replace(@"""", "") + @""", " +
                        @"""data"":" + JsonConvert.SerializeObject(ar) +
                    "}";
            }

            return "";
        }


        public static string f_yyyyMMdd(d2_data data_type, string s)
        {
            switch (s.Length)
            {
                case 6:
                    if (data_type == d2_data.FIX_MONTH)
                        return s[2].ToString() + s[3].ToString() + "/20" + s[0].ToString() + s[1].ToString();
                    else
                        return s[4].ToString() + s[5].ToString() + "/" + s[2].ToString() + s[3].ToString() + "/20" + s[0].ToString() + s[1].ToString();
                case 8:
                    if (data_type == d2_data.FIX_MONTH)
                        return s[4].ToString() + s[5].ToString() + "/20" + s[2].ToString() + s[3].ToString();
                    else
                        return s[6].ToString() + s[7].ToString() + "/" + s[4].ToString() + s[5].ToString() + "/20" + s[2].ToString() + s[3].ToString();
            }
            return "";
        }

        public static string f_HHmmss(d2_data data_type, string s)
        {
            if (data_type == d2_data.FIX_DAY || data_type == d2_data.FIX_MONTH)
                return "00:00";
            else
            {
                switch (s.Length)
                {
                    case 5:
                        return "0" + s[0].ToString() + ":" + s[1].ToString() + s[2].ToString() + ":" + s[3].ToString() + s[4].ToString();
                    case 6:
                        return s[0].ToString() + s[1].ToString() + ":" + s[2].ToString() + s[3].ToString() + ":" + s[4].ToString() + s[5].ToString();
                }
            }

            return "";
        }


    }// end class
}
