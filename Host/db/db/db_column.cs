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

namespace host
{
    public class db_column
    {
        public static Tuple<bool, string, dynamic> add(string s_key, string s_item)
        {
            try
            {
                string json = "";
                return new Tuple<bool, string, dynamic>(true, json, null);
            }
            catch { }

            return new Tuple<bool, string, dynamic>(false, "",null);
        }

        public static Tuple<bool, string, dynamic> edit(string s_key, string s_item)
        {
            try
            {
                string json = "";
                return new Tuple<bool, string, dynamic>(true, json, null);
            }
            catch { }
            return new Tuple<bool, string, dynamic>(false, "",null);
        }

        public static Tuple<bool, string, dynamic> remove(string s_key, string s_item)
        {
            try
            {
                string json = "";
                return new Tuple<bool, string, dynamic>(true, json, null);
            }
            catch { }
            return new Tuple<bool, string, dynamic>(false, "",null);
        }

        //===========================================================================
        private static object lock_write = new object();
        // < data_type,index>
        private static Dictionary<Tuple<int, int>, m_column> dic_column = new Dictionary<Tuple<int, int>, m_column>() { };
        public static string path = hostServer.pathRoot + @"db_column\";

        public static Tuple<bool, string, int, int, IList> where_call_dynamic(string s_key, string s_select, string s_where, string s_order_by, string s_distinct, int page_number, int page_size)
        {
            return dbQuery.where<m_column>(dic_column.Values.ToArray(), s_key, s_select, s_where, s_order_by, s_distinct, page_number, page_size);
        }

        public static void load()
        {
            if (!Directory.Exists(path)) Directory.CreateDirectory(path);

            List<MethodInfo> list = new List<MethodInfo>() { };
            var dis = Directory.GetFiles(path, "*.mmf");
            foreach (var fi in dis)
            {
                m_column[] a_col = hostFile.read_file_MMF<m_column>(fi);
                if (a_col.Length > 0)
                {
                    lock (lock_write)
                    {
                        for (int k = 0; k < a_col.Length; k++)
                        {
                            var o = a_col[k];
                            Tuple<int, int> key = new Tuple<int, int>(o.data_type, o.index);

                            if (!dic_column.ContainsKey(key))
                                dic_column.Add(key, o);
                        }
                    }
                }
            }
        }

        private static void update(int data_type)
        {
            string f_name = data_type.ToString();
            var a_col = get_Items(data_type);
            hostFile.write_file_MMF<m_column>(a_col, path, f_name);
        }

        public static int[] get_AllDataType()
        {
            return dic_column.Keys.Select(x => x.Item1).Distinct().ToArray();
        }

        public static dynamic get_All()
        {
            return dic_column.Values
                .OrderBy(x => x.data_type)
                .ToArray();
        }

        public static m_column[] get_Items(int data_type)
        {
            return dic_column
                .Where(x => x.Key.Item1 == data_type)
                .Select(x => x.Value)
                .OrderBy(x => x.index)
                .ToArray();
        }

        public static m_column get_ItemByID(int data_type, int colunm_index)
        {
            m_column o = new m_column();

            Tuple<int, int> key = new Tuple<int, int>(data_type, colunm_index);
            dic_column.TryGetValue(key, out o);

            return o;
        }

        public static void save_Items(int data_type, m_column[] a)
        {
            lock (lock_write)
            {
                for (int k = 0; k < a.Length; k++)
                {
                    var o = a[k];
                    o.col_id = Guid.NewGuid().ToString();
                    Tuple<int, int> key = new Tuple<int, int>(o.data_type, o.index);
                    if (dic_column.ContainsKey(key) == false)
                        dic_column.Add(key, o);
                    else
                        dic_column[key] = o;
                }

                update(data_type);
            }
        }

        public static bool add_Item(m_column o)
        {
            Tuple<int, int> key = new Tuple<int, int>(o.data_type, o.index);

            if (dic_column.ContainsKey(key) == false)
            {
                lock (lock_write)
                {
                    o.col_id = Guid.NewGuid().ToString();
                    dic_column.Add(key, o);
                    update(o.data_type);
                }
                return true;
            }
            return false;
        }

        public static bool edit_Item(m_column o)
        {
            Tuple<int, int> key = new Tuple<int, int>(o.data_type, o.index);
            if (dic_column.ContainsKey(key))
            {
                lock (lock_write)
                {
                    dic_column[key] = o;
                    update(o.data_type);
                }
                return true;
            }
            return false;
        }


    }//end class
}
