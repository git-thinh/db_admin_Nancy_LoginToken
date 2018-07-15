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
using System.Dynamic;
using System.Linq.Dynamic;

namespace host
{
    public class db_kh_nhom
    {

        public static Tuple<bool, string, dynamic> add(string s_key, string s_item)
        {
           
             try
            {
                string json = "";

                var json_obj = JsonConvert.DeserializeObject<m_kh_nhom>(s_item);

                m_kh_nhom m = JsonConvert.DeserializeObject<m_kh_nhom>(s_item);

                int index = list.FindIndex(o => o.code == m.code);
                if (index == -1)
                {

                    bool c = false;
                    int id_new = 0;
                    while (c == false)
                    {
                        Random rnd = new Random();
                        id_new = rnd.Next(1, int.MaxValue);
                        int idx = list.FindIndex(o => id_new == m.id);
                        if (idx == -1) { c = true; };
                    }

                    int phase_type = json_obj.id_group;
                    long id_parent = json_obj.id_parent;

                    m.id = id_new;
                    m.id_group = phase_type;
                    m.id_parent = id_parent;

                    m.level = json_obj.level;
                    m.name = json_obj.name;
                    m.code = json_obj.code;

                    m.status = true;

                    string array_subID = "";
                    dic_Nhom_Index.TryGetValue(id_parent, out array_subID);
                    if (array_subID == null) array_subID = ";" + id_new.ToString() + ";";
                    else array_subID = array_subID + id_new.ToString() + ";";

                    lock (lock_list)
                    {
                        list.Add(m);
                    }

                    lock (lock_index)
                    {
                        if (dic_Nhom_Index.ContainsKey(id_parent))
                            dic_Nhom_Index[id_parent] = array_subID;

                        update_index(id_parent, array_subID);
                    }

                    update();

                    json = JsonConvert.SerializeObject(m);
                }
                else
                {
                    json = "trung_ma";
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

                m_kh_nhom m = JsonConvert.DeserializeObject<m_kh_nhom>(s_item);

                int index = list.FindIndex(o => o.id == m.id);
                if (index != -1)
                {
                    m_kh_nhom o = list[index];
                    o.name = m.name;
                    o.type = m.type;

                    long id_parent_new = m.id_parent, id_parent_old = o.id_parent;
                    if (id_parent_new != id_parent_old)
                    {
                        string array_subID_new = "", array_subID_old = "";

                        dic_Nhom_Index.TryGetValue(id_parent_new, out array_subID_new);
                        dic_Nhom_Index.TryGetValue(id_parent_old, out array_subID_old);

                        //join new id parent
                        if (string.IsNullOrEmpty(array_subID_new)) array_subID_new = ";" + o.id.ToString() + ";";
                        else array_subID_new += o.id.ToString() + ";";

                        //remove id parent old
                        if (!string.IsNullOrEmpty(array_subID_old))
                            array_subID_old = array_subID_old.Replace(";" + o.id.ToString() + ";", ";");
                        try
                        {
                            lock (lock_index)
                            {
                                update_index(id_parent_new, array_subID_new);
                                update_index(id_parent_old, array_subID_old);
                            }
                        }
                        catch
                        {

                        }
                    }
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
        private static object lock_index = new object();
        private static object lock_item = new object();

        private static Dictionary<long, string> dic_Nhom_Index = new Dictionary<long, string>() { }; // id_nhom  = { ma các item }; // lưu index các mã nhóm con của 1 nhóm cha
        private static DictionaryList<long, long> dic_Nhom_Item = new DictionaryList<long, long>() { }; // id_nhom = { danh sách mã item(meter) }; // lưu danh sách điểm đo của 1 nhóm

        private static object lock_list = new object();
        private static List<m_kh_nhom> list = new List<m_kh_nhom>() { };
        public static string path = hostServer.pathRoot + @"db_kh_nhom\", file_name = "nhom";

        public static Tuple<bool, string, int, int, IList> where_call_dynamic(string s_key, string s_select, string s_where, string s_order_by, string s_distinct, int page_number, int page_size)
        {
            return dbQuery.where<m_kh_nhom>(list.ToArray(), s_key, s_select, s_where, s_order_by, s_distinct, page_number, page_size);
        }

        public static Tuple<bool, string, int, int, IList> where_index_call_dynamic(string s_index, string s_select, string s_where, string s_order_by, string s_distinct, int page_number, int page_size)
        {
            int rs_total = dic_Nhom_Item.Count,
                rs_count = 0;

            try
            {
                long[] a_index = new long[] { };
                if (!string.IsNullOrEmpty(s_index))
                    a_index = s_index.Split(';').Select(x => x.TryParseToLong()).Where(x => x > 0).ToArray();

                List<long> ls = new List<long>() { };
                if (a_index.Length > 0)
                {
                    for (int k = 0; k < a_index.Length; k++)
                    {
                        long key = a_index[k];
                        List<long> li = new List<long>() { };
                        dic_Nhom_Item.TryGetValue(key, out li);
                        if (li != null && li.Count > 0)
                            ls.AddRange(li);
                    }
                }

                var ds = db_meter.get_Items(ls.ToArray());
                rs_count = ds.Length;

                if (rs_count > 0) 
                    return dbQuery.where<m_meter>(ds, s_index, s_select, s_where, s_order_by, s_distinct, page_number, page_size);
            }
            catch (Exception ex)
            {
                return new Tuple<bool, string, int, int, IList>(true, ex.Message, rs_total, 0, null);
            }

            return new Tuple<bool, string, int, int, IList>(true, "", rs_total, 0, null);
        }


        public static Tuple<long, long, long[]> query_byNhomID(long id_key, int page_number, int page_size)
        {
            List<long> ls = new List<long>() { };
            long[] a = new long[] { };
            long count = 0;

            dic_Nhom_Item.TryGetValue(id_key, out ls);
            if (ls != null)
            {
                if (ls.Count > 0)
                {
                    count = ls.Count;
                    if (count > page_size)
                    {
                        int startRowIndex = page_size * (page_number - 1);
                        a = ls.Skip(startRowIndex).Take(page_size).ToArray();
                    }
                    else
                        a = ls.ToArray();
                }
            }
            return new Tuple<long, long, long[]>(count, list.Count, a);
        }

        public static int join_item_add(long id_key, long[] vals)
        {
            int k = 0;

            lock (lock_item)
            {
                k = dic_Nhom_Item.AddListDistinct(id_key, vals);
            }
            var ss =  new List<long>() { };
            dic_Nhom_Item.TryGetValue(id_key, out ss);

            string s = string.Join(";", ss);
            update_item(id_key, s);
            dbCache.clear(typeof(m_meter).FullName);
            //db_kh_dienluc.join_ItemSub_Add(id_key, vals);

            return k;
        }

        public static long[] join_item_get(long id_key)
        {
            List<long> vals = new List<long>() { };

            dic_Nhom_Item.TryGetValue(id_key, out vals);
            if (vals == null) vals = new List<long>() { };

            return vals.ToArray();
        }

        public static int join_item_remove(long id_key, long[] vals)
        {
            int k = 0;
            string s = "";
            List<long> ls = new List<long>() { };

            if (dic_Nhom_Item.TryGetValue(id_key, out ls))
            {
                var lso = ls.Where(x => !vals.Any(o => o == x)).ToList();
                lock (lock_item)
                    dic_Nhom_Item[id_key] = lso;
                k = lso.Count;
                s = string.Join(";", lso);

                //db_kh_dienluc.join_Item_Remove(id_key, vals);
            }

            update_item(id_key, s);
            dbCache.clear(typeof(m_meter).FullName);
            return k;
        }

        #region // ... update ...

        public static void update()
        {
            hostFile.write_file_MMF<m_kh_nhom>(list, path, file_name);
            dbCache.clear(typeof(m_kh_nhom).FullName);
        }

        public static void update_index(long id, string array_index)
        {
            string key = id.ToString();
            string fpath = hostServer.pathRoot + @"db_kh_nhom_index\", fname = key + ".mmf";

            if (!array_index.StartsWith(key + ";"))
                array_index = (key + ";" + array_index).Replace(";;", ";");

            hostFile.writeFilfe(fpath, fname, array_index, Encoding.ASCII);
        }

        public static void update_item(long id, string array_item)
        {
            string key = id.ToString();
            string fpath = hostServer.pathRoot + @"db_kh_nhom_item\", fname = key + ".mmf";

            if (!array_item.StartsWith(key + ";"))
                array_item = (key + ";" + array_item).Replace(";;", ";");

            hostFile.writeFilfe(fpath, fname, array_item, Encoding.ASCII);
        }

        #endregion

        public static void load()
        {
            if (!Directory.Exists(path)) Directory.CreateDirectory(path);

            string path_file = path + file_name + ".mmf";
            list = hostFile.read_file_MMF<m_kh_nhom>(path_file).ToList();

            string p_index = hostServer.pathRoot + @"db_kh_nhom_index\";

            if (!Directory.Exists(p_index))
                Directory.CreateDirectory(p_index);
            else
            {
                var ds = Directory.GetFiles(p_index, "*.mmf");
                for (int k = 0; k < ds.Length; k++)
                {
                    string arr = hostFile.readFile(ds[k], Encoding.ASCII);
                    string key = arr.Split(';')[0];
                    long id = key.TryParseToLong();
                    arr = arr.Substring(key.Length, arr.Length - key.Length);
                    if (!dic_Nhom_Index.ContainsKey(id)) dic_Nhom_Index.Add(id, arr);
                }
            }

            string p_item = hostServer.pathRoot + @"db_kh_nhom_item\";
            if (!Directory.Exists(p_item))
                Directory.CreateDirectory(p_item);
            else
            {
                var ds = Directory.GetFiles(p_item, "*.mmf");
                for (int k = 0; k < ds.Length; k++)
                {
                    string arr = hostFile.readFile(ds[k], Encoding.ASCII);
                    string key = arr.Split(';')[0];
                    List<long> aids = arr.Split(';').Select(x => x.TryParseToLong()).ToList();
                    if (aids.Count > 0)
                    {
                        aids.RemoveAt(0);
                        if (aids.Count > 0)
                        {
                            long id = key.TryParseToLong();
                            arr = arr.Substring(key.Length, arr.Length - key.Length);
                            if (!dic_Nhom_Item.ContainsKey(id)) dic_Nhom_Item.Add(id, aids);
                        }
                    }
                }
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

        public static Tuple<int, int, string> get_ItemJsonByWhere(Func<m_kh_nhom, bool> where, int page_number, int page_size)
        {
            var ls = list.Where(where).ToList();

            //int startRowIndex = page_size * (page_number - 1);
            //var dt = ls.Skip(startRowIndex).Take(page_size).ToList();

            List<m_kh_nhom> dt = new List<m_kh_nhom>() { };
            if (ls.Count > page_size)
            {
                int startRowIndex = page_size * (page_number - 1);
                dt = ls.Skip(startRowIndex).Take(page_size).ToList();
            }
            else
                dt = ls;

            for (int k = 0; k < dt.Count; k++)
            {
                m_kh_nhom m = dt[k];
                List<long> lsi = new List<long>() { };
                dic_Nhom_Item.TryGetValue(m.id, out lsi);
                if (lsi != null && lsi.Count > 0)
                    m.item_count = lsi.Count;
                dt[k] = m;
            }

            string json = JsonConvert.SerializeObject(dt);

            //return json;

            return new Tuple<int, int, string>(list.Count, ls.Count, json);
        }

        #endregion

        #region // add, edit, remove ...
        // check code da co hay chua
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


        // them 1 nhom moi
        public static string add_ItemJson(string item_json, string msg_default = "")
        {
            string json = msg_default;

            var json_obj = JsonConvert.DeserializeObject<m_kh_nhom>(item_json);
            try
            {
                m_kh_nhom m = JsonConvert.DeserializeObject<m_kh_nhom>(item_json);

                int index = list.FindIndex(o => o.code == m.code);
                if (index == -1)
                {

                    bool c = false;
                    int id_new = 0;
                    while (c == false)
                    {
                        Random rnd = new Random();
                        id_new = rnd.Next(1, int.MaxValue);
                        int idx = list.FindIndex(o => id_new == m.id);
                        if (idx == -1) { c = true; };
                    }

                    int phase_type = json_obj.id_group;
                    long id_parent = json_obj.id_parent;

                    m.id = id_new;
                    m.id_group = phase_type;
                    m.id_parent = id_parent;

                    m.level = json_obj.level;
                    m.name = json_obj.name;
                    m.code = json_obj.code;

                    m.status = true;

                    string array_subID = "";
                    dic_Nhom_Index.TryGetValue(id_parent, out array_subID);
                    if (array_subID == null) array_subID = ";" + id_new.ToString() + ";";
                    else array_subID = array_subID + id_new.ToString() + ";";

                    lock (lock_list)
                    {
                        list.Add(m);
                    }

                    lock (lock_index)
                    {
                        if (dic_Nhom_Index.ContainsKey(id_parent))
                            dic_Nhom_Index[id_parent] = array_subID;

                        update_index(id_parent, array_subID);
                    }

                    update();

                    json = JsonConvert.SerializeObject(m);
                }
                else
                {
                    json = "Trùng mã";
                }
            }
            catch { }

            return json;
        }

        // update các thông tin của nhóm
        public static string edit_ItemJsonString(string item_json, string msg_default = "")
        {
            string json = msg_default;
            try
            {
                m_kh_nhom m = JsonConvert.DeserializeObject<m_kh_nhom>(item_json);

                int index = list.FindIndex(o => o.id == m.id);
                if (index != -1)
                {
                    m_kh_nhom o = list[index];
                    o.name = m.name;
                    o.type = m.type;

                    long id_parent_new = m.id_parent, id_parent_old = o.id_parent;
                    if (id_parent_new != id_parent_old)
                    {
                        string array_subID_new = "", array_subID_old = "";

                        dic_Nhom_Index.TryGetValue(id_parent_new, out array_subID_new);
                        dic_Nhom_Index.TryGetValue(id_parent_old, out array_subID_old);

                        //join new id parent
                        if (string.IsNullOrEmpty(array_subID_new)) array_subID_new = ";" + o.id.ToString() + ";";
                        else array_subID_new += o.id.ToString() + ";";

                        //remove id parent old
                        if (!string.IsNullOrEmpty(array_subID_old))
                            array_subID_old = array_subID_old.Replace(";" + o.id.ToString() + ";", ";");
                        try
                        {
                            lock (lock_index)
                            {
                                update_index(id_parent_new, array_subID_new);
                                update_index(id_parent_old, array_subID_old);
                            }
                        }
                        catch
                        {

                        }
                    }
                    lock (lock_list)
                        list[index] = o;

                    update();

                    json = JsonConvert.SerializeObject(o);
                }
            }
            catch { }

            return json;
        }

        // xóa 1 nhóm
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



        // xem lại chuyển về hàm demo trên tree
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

    }
}
