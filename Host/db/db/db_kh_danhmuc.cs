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
using Newtonsoft.Json.Linq;
using System.Linq.Dynamic;

namespace host
{
    public class db_kh_danhmuc
    {
        public static Tuple<bool, string, dynamic> add(string s_key, string s_item)
        {
            var json_obj = JsonConvert.DeserializeObject<m_kh_danhmuc>(s_item);

            try
            {
                string json = "";

                m_kh_danhmuc m = JsonConvert.DeserializeObject<m_kh_danhmuc>(s_item);

                int index = list.FindIndex(o => o.id == m.id);
                if (index == -1)
                {


                    m.id = (list.Count + 1);
                    m.name = json_obj.name;

                    lock (lock_list)
                        list.Add(m);

                    update();

                    json = JsonConvert.SerializeObject(m);
                }

                return new Tuple<bool, string, dynamic>(true, json, m);
            }

            catch { }

            return new Tuple<bool, string, dynamic>(false, "",null);
        }

        public static Tuple<bool, string, dynamic> edit(string s_key, string s_item)
        {
           
            try
            {
                string json = "";

                m_kh_danhmuc m = JsonConvert.DeserializeObject<m_kh_danhmuc>(s_item);

                int index = list.FindIndex(o => o.id == m.id);
                if (index != -1)
                {
                    m_kh_danhmuc o = list[index];


                    o.name = m.name;

                    lock (lock_list)
                        list[index] = o;

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

                int id_ = Int32.Parse(s_item);
                int pos = list.FindIndex(x => x.id == id_);
                if (pos != -1)
                {

                    json = JsonConvert.SerializeObject(list[pos]);
                    lock (lock_list)
                        list.RemoveAt(pos);

                    update();
                }

                return new Tuple<bool, string, dynamic>(true, json, null);
            }

            catch { }
            return new Tuple<bool, string, dynamic>(false, "",null);
        }

        //===========================================================================
        private static DictionaryList<long, long> dic_DienLuc_Item = new DictionaryList<long, long>() { };

        private static object lock_list = new object();
        private static List<m_kh_danhmuc> list = new List<m_kh_danhmuc>() { };
        public static string path = hostServer.pathRoot + @"db_kh_loaidanhmuc\", file_name = "loaidanhmuc";

        public static Tuple<bool, string, int, int, IList> where_call_dynamic(string s_key, string s_select, string s_where, string s_order_by, string s_distinct, int page_number, int page_size)
        {
            return dbQuery.where<m_kh_danhmuc>(list.ToArray(), s_key, s_select, s_where, s_order_by, s_distinct, page_number, page_size);
        }

        public static void update()
        {
            hostFile.write_file_MMF<m_kh_danhmuc>(list, path, file_name);
            dbCache.clear(typeof(m_kh_danhmuc).FullName);
        }

        public static void load()
        {
            if (!Directory.Exists(path)) Directory.CreateDirectory(path);

            string path_file = path + file_name + ".mmf";
            list = hostFile.read_file_MMF<m_kh_danhmuc>(path_file).ToList();

            if (list.Count == 0)
            {
                lock (lock_list)
                {
                    list.AddRange(new m_kh_danhmuc[] {
                            new m_kh_danhmuc() { id = 1, name = "Khách hàng" }
                          , new m_kh_danhmuc() { id = 2, name = "Tuyến"}
                          , new m_kh_danhmuc() { id = 3, name = "Sổ ghi" }
                          , new m_kh_danhmuc() { id = 4, name = "Cột" }
                    });
                }

                update();
            }
        }

        #region // get all, search items ...

        public static Tuple<int, string> get_All(int page_number, int page_size)
        {
            string json = get_AllJson(page_number, page_size);
            return new Tuple<int, string>(list.Count, json);
        }

        public static string get_AllJson(int page_number, int page_size)
        {
            int startRowIndex = page_size * (page_number - 1);
            var dt = list.Skip(startRowIndex).Take(page_size).ToList();
            string json = JsonConvert.SerializeObject(dt);

            return json;
        }

        public static Tuple<int, int, string> get_ItemJsonByWhere(Func<m_kh_danhmuc, bool> where, int page_number, int page_size)
        {
            var ls = list.Where(where).ToList();

            //int startRowIndex = page_size * (page_number - 1);
            //var dt = ls.Skip(startRowIndex).Take(page_size).ToList();

            List<m_kh_danhmuc> dt = new List<m_kh_danhmuc>() { };
            if (ls.Count > page_size)
            {
                int startRowIndex = page_size * (page_number - 1);
                dt = ls.Skip(startRowIndex).Take(page_size).ToList();
            }
            else
                dt = ls;

            string json = JsonConvert.SerializeObject(dt);

            //return json;

            return new Tuple<int, int, string>(list.Count, ls.Count, json);
        }

        public static string get_ItemJsonBy_id(string id, string msg_default = "")
        {
            string json = msg_default;
            int id_ = Int32.Parse(id);
            var ls = list.Where(x => x.id == id_).ToList();
            for (int k = 0; k < ls.Count; k++)

                json = JsonConvert.SerializeObject(ls);
            return json;
        }

        #endregion

        #region // add, edit, remove ...

        public static string add_ItemJson(string item_json, string msg_default = "")
        {
            string json = msg_default;

            var json_obj = JsonConvert.DeserializeObject<m_kh_danhmuc>(item_json);

            try
            {
                m_kh_danhmuc m = JsonConvert.DeserializeObject<m_kh_danhmuc>(item_json);

                int index = list.FindIndex(o => o.id == m.id);
                if (index == -1)
                {


                    m.id = (list.Count + 1);
                    m.name = json_obj.name;

                    lock (lock_list)
                        list.Add(m);

                    update();

                    json = JsonConvert.SerializeObject(m);
                }
            }
            catch { }

            return json;
        }

        public static string edit_ItemJsonString(string item_json, string msg_default = "")
        {
            string json = msg_default;
            try
            {
                m_kh_danhmuc m = JsonConvert.DeserializeObject<m_kh_danhmuc>(item_json);

                int index = list.FindIndex(o => o.id == m.id);
                if (index != -1)
                {
                    m_kh_danhmuc o = list[index];


                    o.name = m.name;

                    lock (lock_list)
                        list[index] = o;

                    update();

                    json = JsonConvert.SerializeObject(o);
                }
            }
            catch { }

            return json;
        }

        public static string remove_Item(string id, string msg_default = "")
        {

            string json = msg_default;
            int id_ = Int32.Parse(id);
            int pos = list.FindIndex(x => x.id == id_);
            if (pos != -1)
            {

                json = JsonConvert.SerializeObject(list[pos]);
                lock (lock_list)
                    list.RemoveAt(pos);

                update();
            }

            return json;
        }

        #endregion

    }
}
