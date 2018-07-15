using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nancy;
using System.IO;
using System.Data;
using System.Xml;
using Newtonsoft.Json;
using System.Collections;
using System.Xml.Linq;
using System.Reflection;

using model;
using System.Linq.Dynamic;
using System.Runtime.InteropServices;

namespace host
{
    //var gReduce = e64({
    //    api: 'store', // 0:store, 1: db, 9: reduce key cache data 
    //    key: 'export_excels',
    //    para: 'name_file', //
    //    tem_item: e64('<td>{_index_}</td><td>{device_id}</td><td>[mre_split_column_td@{type_code}|;]</td>'),
    //    tem_default: e64('<td></td><td></td><td></td>'),
    //    tem_body: e64('<table>##tem_body##</table>'),
    //    include: e64('mre_split_column_td|<td>{0}</td><td>{1}</td><td>{2}</td>|||') // e64(reduce_key|reduce_tem_item|reduce_tem_def|reduce_tem_body) | e64(||||) | ... 
    //});

    [Serializable]
    [StructLayout(LayoutKind.Sequential, Pack = 1, CharSet = CharSet.Unicode)]
    public struct m_func
    { 
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 50)]
        public string api;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 255)]
        public string key;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 8000)]
        public string para; //e64('para1|para2|...'),

        public int tem_len; // = 3

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 8000)]
        public string tem_item; //: e64('<tr><td>{0}</td><td>{1}</td><td>{2}</td></tr>'),

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 8000)]
        public string tem_default; //: e64('<tr><td></td><td></td><td></td><td></td></tr>')

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 8000)]
        public string tem_body; //: e64('<table>{##tem_body##}</table>')

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 8000)]
        public string include; // e64('mre_split_column_td|<td>{0}</td><td>{1}</td><td>{2}</td>|||') // e64(reduce_key|reduce_tem_item|reduce_tem_def|reduce_tem_body) | e64(||||) | ... 
    }

    public static class db_func
    {
        private static string tem_body_key = "##tem_body##";
        //---------------------------------- index, config , para[] ,cols_id[],cols_code[],value[] | return string
        private static Dictionary<string, Func<int, m_func, object[], int[], string[], object[], string>> dic_reduce =
            new Dictionary<string, Func<int, m_func, object[], int[], string[], object[], string>>() { };

        public static string invoke()
        {
            string s = "";
            return s;
        }

        public static m_func add(string item)
        {
            m_func f = new m_func();

            if (!string.IsNullOrWhiteSpace(item))
            {
                f = JsonConvert.DeserializeObject<m_func>(item);
                if (!string.IsNullOrWhiteSpace(f.key)) {

                    object[] para = null;
                    if (!string.IsNullOrWhiteSpace(f.para)) para = f.para.Split('|').Select(x => (object)x).ToArray();

                    // index, config , para[] ,cols_id[],cols_code[],value[] | return string
                    Func<int, m_func, object[], int[], string[], object[], string> fn = null;

                }
            }

            return f;
        }

        public static void init()
        {
            

            //dic_reduce.Add("split_column_td", (id, config, para, cols_id, cols_name, vals) =>
            //{
            //    object[] a = new object[] { };
            //    if (vals != null && vals.Length > 0 && para != null && para.Length > 0) 
            //        a = vals[0].ToString().Split(new string[] { para[0].ToString() }, StringSplitOptions.None);

            //    string s = "";
            //    if (a.Length > 0 && !string.IsNullOrWhiteSpace(config.reduce_tem_item))
            //    {
            //        s = string.Format(config.reduce_tem_item, a);
            //        if (!string.IsNullOrWhiteSpace(config.reduce_tem_body))

            //    }

            //    return config.reduce_tem_def;
            //});

            //dic_reduce.Add("split_column", (id, config, cols_id, cols_name, vals) =>
            //{
            //    if (vals != null && vals.Length > 0)
            //    {
            //        int len = config.reduce_tem_len;
            //        object[] a = new object[] { };
            //        string s = config.reduce_tem_item;

            //        for (int k = 0; k < len; k++)
            //        {
            //            // bi.Append(string.Format(config.reduce_tem, models[k]));
            //        }
            //    }
            //    return "";
            //});

        }
    }//end class
}
