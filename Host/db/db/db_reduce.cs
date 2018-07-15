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
    public class db_reduce
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
        private static Dictionary<string, m_reduce> dic_reduce = new Dictionary<string, m_reduce>() { };

        public static string path = hostServer.pathRoot + @"db_reduce\";

        public static Tuple<bool, string, int, int, IList> where_call_dynamic(string s_key, string s_select, string s_where, string s_order_by, string s_distinct, int page_number, int page_size)
        {
            return dbQuery.where<m_reduce>(dic_reduce.Values.ToArray(), s_key, s_select, s_where, s_order_by, s_distinct, page_number, page_size);
        }

        public static void load()
        {
            if (!Directory.Exists(path)) Directory.CreateDirectory(path);

            List<MethodInfo> list = new List<MethodInfo>() { };
            var dis = Directory.GetFiles(path, "*.mmf");
            foreach (var fi in dis)
            {
                m_reduce o = hostFile.read_MMF<m_reduce>(fi);
                if (!dic_reduce.ContainsKey(o.api))
                    lock (lock_write)
                        dic_reduce.Add(o.api, o);
            }
        }

        public static void update(m_reduce item)
        { 
            hostFile.write_MMF<m_reduce>(item, path, item.api);
        }

        public static dynamic get_All()
        {
            return dic_reduce
                .Select(x => new  
                {
                    id = x.Value.id,
                    api = x.Value.api,
                    name = x.Value.name,
                    date_create = x.Value.date_create,
                    date_update_lastest = x.Value.date_update_lastest
                })
                .OrderByDescending(x => x.date_update_lastest)
                .ToArray();
        }

        public static dynamic get_Search(string keyword)
        {
            return dic_reduce
                .Where(x => x.Key.Contains(keyword) || x.Value.name.Contains(keyword))
                .Select(x => new 
                {
                    id = x.Value.id,
                    api = x.Value.api,
                    name = x.Value.name,
                    date_create = x.Value.date_create,
                    date_update_lastest = x.Value.date_update_lastest
                })
                .OrderByDescending(x => x.date_update_lastest)
                .ToArray();
        }

        public static bool check_ExistAPI(string api)
        {
            return dic_reduce.ContainsKey(api);
        }

        public static m_reduce get_ItemByAPI(string api)
        {
            m_reduce o;
            if (dic_reduce.TryGetValue(api, out o)) return o;
            return o;
        }

        public static m_reduce get_ItemByID(string id)
        {
            m_reduce o = new m_reduce();

            var ls = dic_reduce.Where(x => x.Value.id == id).Select(x => x.Value).ToArray();
            if (ls.Length > 0) return ls[0];

            return o;
        }

        public static string add_Item(m_reduce item)
        {
            lock (lock_write)
            {
                if (dic_reduce.ContainsKey(item.api) == false)
                {
                    item.id = Guid.NewGuid().ToString();
                    item.date_create = DateTime.Now.ToString("yyMMddHHmmss").TryParseToLong();
                    item.date_update_lastest = DateTime.Now.ToString("yyMMddHHmmss").TryParseToLong();

                    dic_reduce.Add(item.api, item);

                    update(item);
                    return item.id;
                }
            }
            return "";
        }

        public static bool edit_Item(m_reduce item)
        {
            lock (lock_write)
            {
                if (dic_reduce.ContainsKey(item.api))
                {
                    item.date_update_lastest = DateTime.Now.ToString("yyMMddHHmmss").TryParseToLong();
                    dic_reduce[item.api] = item;
                    update(item);
                    return true;
                }
            }
            return false;
        }

    }//end class
}
