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
    public class db_canhbao_cauhinh
    {
        public static Tuple<bool, string, dynamic> add(string s_key, string s_item)
        {
            try
            {
                string json = "";

                var m = JsonConvert.DeserializeObject<m_canhbao_cauhinh>(s_item);

                int index = list.FindIndex(o => o.cauhinh_id == m.cauhinh_id);
                if (index == -1)
                {
                    var random = new Random();
                    int randomNumber = random.Next(0, int.MaxValue);

                    m.cauhinh_id = randomNumber;

                    lock (list) list.Add(m);

                    update();

                    json = JsonConvert.SerializeObject(m);
                }

                return new Tuple<bool, string, dynamic>(true, json, m);
            }

            catch
            {
                
            }

            return new Tuple<bool, string, dynamic>(false, "",null);
        }

        public static Tuple<bool, string, dynamic> edit(string s_key, string s_item)
        {
            try
            {
                string json = "";

                var m = JsonConvert.DeserializeObject<m_canhbao_cauhinh>(s_item);

                int index = list.FindIndex(o => o.cauhinh_id == m.cauhinh_id);
                if (index != -1)
                {
                    m_canhbao_cauhinh o = list[index];

                    o.name = m.name;
                    o.note = m.note;

                    lock (list) list[index] = o;

                    update();

                    json = JsonConvert.SerializeObject(o);
                }

                return new Tuple<bool, string, dynamic>(true, json, m);
            }

            catch { }
            return new Tuple<bool, string, dynamic>(false, "",null);
        }

        public static Tuple<bool, string, dynamic> remove(string s_key, string s_item)
        {
            try
            {
                string json = "";

                int ch_id = Int32.Parse(s_item);
                int pos = list.FindIndex(x => x.cauhinh_id == ch_id);
                if (pos != -1)
                {
                    json = JsonConvert.SerializeObject(list[pos]);
                    lock (list)
                        list.RemoveAt(pos);

                    update();
                }

                return new Tuple<bool, string, dynamic>(true, json, null);
            }

            catch { }
            return new Tuple<bool, string, dynamic>(false, "",null);
        }

        //===========================================================================
        private static object lock_list = new object();
        private static List<m_canhbao_cauhinh> list = new List<m_canhbao_cauhinh>() { };
        public static string path = hostServer.pathRoot + @"db_canhbao\", file_name = "canhbao_cauhinh";

        public static Tuple<bool, string, int, int, IList> where_call_dynamic(string s_key, string s_select, string s_where, string s_order_by, string s_distinct, int page_number, int page_size)
        {
            return dbQuery.where<m_canhbao_cauhinh>(list.ToArray(), s_key, s_select, s_where, s_order_by, s_distinct, page_number, page_size);
        }

        public static void update()
        {
            hostFile.write_file_MMF<m_canhbao_cauhinh>(list, path, file_name);
            dbCache.clear(typeof(m_dcu).FullName);
        }

        public static void load()
        {
            string path_file = path + file_name + ".mmf";
            list = hostFile.read_file_MMF<m_canhbao_cauhinh>(path_file).ToList();

            if (list.Count == 0)
            {
                lock (list)
                {
                   
                }

                update();
            }
        }

        #region // get all, search items ...

        public static Tuple<int, string> get_All(int page_number, int page_size)
        {
            string json = get_allJson(page_number, page_size);
            return new Tuple<int, string>(list.Count, json);
        }

        public static string get_allJson(int page_number, int page_size)
        {
            int startRowIndex = page_size * (page_number - 1);
            var dt = list.Skip(startRowIndex).Take(page_size).ToList();
            var json = JsonConvert.SerializeObject(dt);

            return json;
        }

        public static Tuple<int, string> get_ItemJsonBy_search(string keyword, int page_number, int page_size)
        {
            var ls = list.Where(x => (x.name != null && x.name.Contains(keyword))
                ).ToList();

            //int startRowIndex = page_size * (page_number - 1);
            //var dt = ls.Skip(startRowIndex).Take(page_size).ToList();

            List<m_canhbao_cauhinh> dt = new List<m_canhbao_cauhinh>() { };
            if (ls.Count > page_size)
            {
                int startRowIndex = page_size * (page_number - 1);
                dt = ls.Skip(startRowIndex).Take(page_size).ToList();
            }
            else
                dt = ls;

            string json = JsonConvert.SerializeObject(dt);

            //return json;

            return new Tuple<int, string>(ls.Count, json);
        }

        #endregion


        #region // get item by id, find item by ...

        public static string get_ItemJsonBy_id(string cauhinh_id, string msg_default = "")
        {
            int ch_id = Int32.Parse(cauhinh_id);

            string json = msg_default;

            int pos = list.FindIndex(o => o.cauhinh_id == ch_id);
            if (pos != -1)
            {
                json = JsonConvert.SerializeObject(list[pos]);
                return json;
            }

            return json;
        }

        public static string get_ItemJsonBy_find_name(string name, string msg_default = "")
        {
            string json = msg_default;

            var ls = list.Where(x => x.name == name).ToList();
            for (int k = 0; k < ls.Count; k++)
            {
                m_canhbao_cauhinh m = ls[k];
                ls[k] = m;
            }

            json = JsonConvert.SerializeObject(ls);
            return json;
        }


        #endregion

        #region // add, edit, remove ...

        public static string add_ItemJson(string item_json, string msg_default = "")
        {
            string json = msg_default;

            try
            {
                var m = JsonConvert.DeserializeObject<m_canhbao_cauhinh>(item_json);

                int index = list.FindIndex(o => o.cauhinh_id == m.cauhinh_id);
                if (index == -1)
                {
                    var random = new Random();
                    int randomNumber = random.Next(0, int.MaxValue);

                    m.cauhinh_id = randomNumber;

                    lock (list) list.Add(m);

                    update();

                    json = JsonConvert.SerializeObject(m);
                }
            }
            catch
            {
                
            }

            return json;
        }



        public static string edit_ItemJsonString(string item_json, string msg_default = "")
        {
            string json = msg_default;
            try
            {
                m_canhbao_cauhinh m = JsonConvert.DeserializeObject<m_canhbao_cauhinh>(item_json);

                int index = list.FindIndex(o => o.cauhinh_id == m.cauhinh_id);
                if (index != -1)
                {
                    m_canhbao_cauhinh o = list[index];

                    o.name = m.name;
                    o.note = m.note;

                    lock (list)
                        list[index] = o;

                    update();

                    json = JsonConvert.SerializeObject(o);
                }
            }
            catch { }

            return json;
        }

        public static string remove_Item(string cauhinh_id, string msg_default = "")
        {
            string json = msg_default;
            int ch_id = Int32.Parse(cauhinh_id);
            int pos = list.FindIndex(x => x.cauhinh_id == ch_id);
            if (pos != -1)
            {
                json = JsonConvert.SerializeObject(list[pos]);
                lock (list)
                    list.RemoveAt(pos);

                update();
            }

            return json;
        }

        #endregion








    }
}
