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
    public class db_kh_dienluc
    {
        public static Tuple<bool, string, dynamic> add(string s_key, string s_item)
        {
            try
            {
                string json = "";

                var json_obj = JsonConvert.DeserializeObject<m_kh_dienluc>(s_item);

                m_kh_dienluc m = JsonConvert.DeserializeObject<m_kh_dienluc>(s_item);

                int index = list.FindIndex(o => o.code == m.code);
                if (index == -1)
                {

                    bool c = false;
                    int new_rnd = 0;
                    while (c == false)
                    {
                        Random rnd = new Random();
                        new_rnd = rnd.Next(1, int.MaxValue);
                        int idx = list.FindIndex(o => new_rnd == m.id);
                        if (idx == -1) { c = true; };
                    }

                    m.id = new_rnd;
                    m.name = json_obj.name;
                    m.code = json_obj.code;
                    m.id_parent = json_obj.id_parent;
                    m.status = true;

                    lock (lock_list)
                        list.Add(m);

                    update();

                    json = JsonConvert.SerializeObject(m);

                    return new Tuple<bool, string, dynamic>(true, json, m);
                } else
                {
                    json = "trung_ma";
                    return new Tuple<bool, string, dynamic>(true, json, null);
                }
            }

            catch { }

            return new Tuple<bool, string, dynamic>(false, "",null);
        }

        public static Tuple<bool, string, dynamic> edit(string s_key, string s_item)
        {
           
            try
            {
                string json = "";

                m_kh_dienluc m = JsonConvert.DeserializeObject<m_kh_dienluc>(s_item);

                int index = list.FindIndex(o => o.id == m.id);
                if (index != -1)
                {
                    m_kh_dienluc o = list[index];


                    o.name = m.name;
                    //o.code = m.code;
                    o.id_parent = m.id_parent;

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
        private static object lock_itemSub = new object();
        private static object lock_item = new object();
        private static DictionaryList<long, long> dic_DienLuc_ItemSub = new DictionaryList<long, long>() { }; // Mã điện lực = { mã các nhóm: trạm, tuyến, ... };
        private static Dictionary<long, long> dic_DienLuc_ItemSubCount = new Dictionary<long, long>() { }; // Mã điện lực, Số lượng các nhóm

        private static DictionaryList<long, long> dic_DienLuc_Item = new DictionaryList<long, long>() { }; // Mã điện lực = { mã các điểm đo };
        private static Dictionary<long, long> dic_DienLuc_ItemCount = new Dictionary<long, long>() { }; // Mã điện lực, Số lượng các điểm đo

        private static object lock_list = new object();
        private static List<m_kh_dienluc> list = new List<m_kh_dienluc>() { };
        public static string path = hostServer.pathRoot + @"db_kh_dienluc\", file_name = "dienluc";

        public static Tuple<bool, string, int, int, IList> where_call_dynamic(string s_key, string s_select, string s_where, string s_order_by, string s_distinct, int page_number, int page_size)
        {
            return dbQuery.where<m_kh_dienluc>(list.ToArray(), s_key, s_select, s_where, s_order_by, s_distinct, page_number, page_size);
        }


        public static void update()
        {
            hostFile.write_file_MMF<m_kh_dienluc>(list, path, file_name);
            dbCache.clear(typeof(m_kh_dienluc).FullName);
        }

        public static void load()
        {
            if (!Directory.Exists(path)) Directory.CreateDirectory(path);

            string path_file = path + file_name + ".mmf";
            list = hostFile.read_file_MMF<m_kh_dienluc>(path_file).ToList();
        }

        #region // ... task join ItemSub, Item: nhom, tram, tuyen... diem do ....
        //các nghiệp vụ này được sử dụng khi tạo, sửa, xóa các nhóm: trạm, tuyến, nhóm... và xóa điểm đo ...

        public static void join_update_item(long id, string array_index)
        {
            try
            {
                string key = id.ToString();
                string fpath = hostServer.pathRoot + @"db_kh_dienluc_item\", fname = key + ".mmf";

                if (!array_index.StartsWith(key + ";"))
                    array_index = (key + ";" + array_index).Replace(";;", ";");

                hostFile.writeFilfe(fpath, fname, array_index, Encoding.ASCII);
            }
            catch { }
        }

        public static void join_update_itemsub(long id, string array_item)
        {
            try
            {
                string key = id.ToString();
                string fpath = hostServer.pathRoot + @"db_kh_dienluc_itemsub\", fname = key + ".mmf";

                if (!array_item.StartsWith(key + ";"))
                    array_item = (key + ";" + array_item).Replace(";;", ";");

                hostFile.writeFilfe(fpath, fname, array_item, Encoding.ASCII);
            }
            catch { }
        }

        public static void join_Item_Add(long id_key, long[] vals)
        {
            Task.Factory.StartNew(() =>
            {
                int k = 0;

                lock (lock_item)
                {
                    k = dic_DienLuc_Item.AddListDistinct(id_key, vals);
                }

                string s = string.Join(";", vals);
                join_update_item(id_key, s); 
            }).Start();
        }

        public static void join_Item_Remove(long id_key, long[] vals)
        {
            Task.Factory.StartNew(() =>
            {
                int k = 0;
                string s = "";
                List<long> ls = new List<long>() { };

                if (dic_DienLuc_Item.TryGetValue(id_key, out ls))
                {
                    var lso = ls.Where(x => !vals.Any(o => o == x)).ToList();
                    lock (lock_item)
                        dic_DienLuc_Item[id_key] = lso;
                    k = lso.Count;
                    s = string.Join(";", lso);
                }

                join_update_item(id_key, s); 
            }).Start();
        }

        public static void join_ItemSub_Add(long id_key, long[] vals)
        {
            Task.Factory.StartNew(() =>
            {
                int k = 0;

                lock (lock_itemSub)
                {
                    k = dic_DienLuc_ItemSub.AddListDistinct(id_key, vals);
                }

                string s = string.Join(";", vals);
                join_update_itemsub(id_key, s); 
            }).Start();
        }

        public static void join_ItemSub_Remove(long id_key, long[] vals)
        {
            Task.Factory.StartNew(() =>
            {
                int k = 0;
                string s = "";
                List<long> ls = new List<long>() { };

                if (dic_DienLuc_ItemSub.TryGetValue(id_key, out ls))
                {
                    var lso = ls.Where(x => !vals.Any(o => o == x)).ToList();
                    lock (lock_itemSub)
                        dic_DienLuc_ItemSub[id_key] = lso;
                    k = lso.Count;
                    s = string.Join(";", lso);
                }

                join_update_itemsub(id_key, s); 
            }).Start();
        }

        #endregion

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

        public static Tuple<int, int, string> get_ItemJsonByWhere(Func<m_kh_dienluc, bool> where, int page_number, int page_size)
        {
            var ls = list.Where(where).ToList();

            //int startRowIndex = page_size * (page_number - 1);
            //var dt = ls.Skip(startRowIndex).Take(page_size).ToList();

            List<m_kh_dienluc> dt = new List<m_kh_dienluc>() { };
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

        public static Tuple<int, string> get_ItemJsonBySearch(string keyword, int page_number, int page_size)
        {
            var ls = list.Where(x =>
                     (x.code.Contains(keyword))
                    || (x.name_short != null && x.name_short.Contains(keyword))
                    || (x.name != null && x.name.Contains(keyword))
                ).ToList();

            //int startRowIndex = page_size * (page_number - 1);
            //var dt = ls.Skip(startRowIndex).Take(page_size).ToList();

            List<m_kh_dienluc> dt = new List<m_kh_dienluc>() { };
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

        public static string get_check_code(string code, string msg_default = "")
        {
            string json = "1";
            int index = list.FindIndex(o => o.code == code);
            if (index == -1)
            {
                return json;
            }
            json = "0";
            return json;
        }

        public static string add_ItemJson(string item_json, string msg_default = "")
        {
            string json = msg_default;

            var json_obj = JsonConvert.DeserializeObject<m_kh_dienluc>(item_json);

            try
            {
                m_kh_dienluc m = JsonConvert.DeserializeObject<m_kh_dienluc>(item_json);

                int index = list.FindIndex(o => o.code == m.code);
                if (index == -1)
                {

                    bool c = false;
                    int new_rnd = 0;
                    while (c == false)
                    {
                        Random rnd = new Random();
                        new_rnd = rnd.Next(1, int.MaxValue);
                        int idx = list.FindIndex(o => new_rnd == m.id);
                        if (idx == -1) { c = true; };
                    }

                    m.id = new_rnd;
                    m.name = json_obj.name;
                    m.code = json_obj.code;
                    m.id_parent = json_obj.id_parent;
                    m.status = true;

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
                m_kh_dienluc m = JsonConvert.DeserializeObject<m_kh_dienluc>(item_json);

                int index = list.FindIndex(o => o.id == m.id);
                if (index != -1)
                {
                    m_kh_dienluc o = list[index];


                    o.name = m.name;
                    //o.code = m.code;
                    o.id_parent = m.id_parent;

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
