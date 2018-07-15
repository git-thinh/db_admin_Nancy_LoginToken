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
    public class db_dcu
    {
        public static Tuple<bool, string, dynamic> add(string s_key, string s_item)
        {
            try
            {
                string json = "";

                var json_obj = JsonConvert.DeserializeObject<m_dcu>(s_item);

                int index = list_DCU.FindIndex(o => o.dcu_id == json_obj.dcu_id);
                if (index == -1)
                {
                    m_dcu item = new m_dcu()
                    {
                        dcu_id = json_obj.dcu_id,
                        name = json_obj.name,
                        status = true,
                        is_online = true
                    };

                    lock (lock_list)
                        list_DCU.Add(item);

                    update();
                }
                return new Tuple<bool, string, dynamic>(true, json, json_obj);
            }

            catch { }

            return new Tuple<bool, string, dynamic>(false, "",null);
        }

        public static Tuple<bool, string, dynamic> edit(string s_key, string s_item)
        {
            string json = "";
            try
            {
                m_dcu m = JsonConvert.DeserializeObject<m_dcu>(s_item);

                int index = list_DCU.FindIndex(o => o.dcu_id == m.dcu_id);
                if (index != -1)
                {
                    m_dcu u = list_DCU[index];
                    u.name = m.name;
                    u.note = m.note;
                    u.address = m.address;

                    lock (lock_list)
                        list_DCU[index] = u;

                    update();

                    json = JsonConvert.SerializeObject(u);
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

                var json_obj = JsonConvert.DeserializeObject<m_dcu>(s_item);

                int pos = list_DCU.FindIndex(x => x.dcu_id == json_obj.dcu_id);
                if (pos != -1)
                {
                    var item = list_DCU[pos];
                    item.status = true;
                    json = "actived";

                    if (json_obj.status == false)
                    {
                        item.status = false;
                        json = "disabled";
                    }

                    lock (lock_list)
                        list_DCU[pos] = item;

                    update();
                }
                return new Tuple<bool, string, dynamic>(true, json, null);
            }

            catch { }
            return new Tuple<bool, string, dynamic>(false, "",null);
        }

        //===========================================================================
        private static object lock_list = new object();
        private static List<m_dcu> list_DCU = new List<m_dcu>() { };

        private static object lock_index = new object();
        private static DictionaryList<long, long> dic_DCU_meter = new DictionaryList<long, long>() { };


        private static string path = hostServer.pathRoot + @"db_dcu\";
        private static string file_name = "dcu";

        public static Tuple<bool, string, int, int, IList> where_call_dynamic(string s_key, string s_select, string s_where, string s_order_by, string s_distinct, int page_number, int page_size)
        {
            return dbQuery.where<m_dcu>(list_DCU.ToArray(), s_key, s_select, s_where, s_order_by, s_distinct, page_number, page_size);
        }

        public static void load()
        {
            if (!Directory.Exists(path)) Directory.CreateDirectory(path);

            string path_file = path + file_name + ".mmf";
            list_DCU = hostFile.read_file_MMF<m_dcu>(path_file).ToList();

            for (int k = 0; k < list_DCU.Count; k++)
            {
                m_dcu it = list_DCU[k];
                it.is_online = false;
                list_DCU[k] = it;
            }             

            path_file = path + file_name + "_meter.mmf";
            var a = hostFile.read_file_MMF<Tuple<long, long[]>>(path_file);
            for (int k = 0; k < a.Length; k++)
            {
                var o = a[k];
                lock (lock_index)
                    dic_DCU_meter.AddListDistinct(o.Item1, o.Item2);
            }
        }

        public static void update()
        {
            hostFile.write_file_MMF<m_dcu>(list_DCU, path, file_name);
            dbCache.clear(typeof(m_dcu).FullName);
        }

        public static void update_index()
        {
            var a = dic_DCU_meter.Select(x => new Tuple<long, long[]>(x.Key, x.Value.ToArray())).ToArray();
            hostFile.write_file_MMF<Tuple<long, long[]>>(a, path, file_name + "_meter");
        }

        /// <summary>
        /// Thêm mới các m_DCU: (dcu_id, tag_code)
        /// </summary>
        /// <param name="a_dcu_type_code"></param>
        public static void add_Items(Tuple<long, string>[] a_dcu_type_code)
        {
            var ls = a_dcu_type_code.Where(x => !list_DCU.Any(o => o.dcu_id == x.Item1))
                .Select(x => new m_dcu()
                {
                    dcu_id = x.Item1,
                    type_code = x.Item2,
                    name = x.ToString(),
                    is_online = true
                }).ToArray();

            if (ls.Length > 0)
            {
                lock (lock_list)
                {
                    list_DCU.AddRange(ls);
                    update();
                }
            }
        }

        /// <summary>
        /// Lưu vào cache Dictionary [dcu_id, List<meter_id>] 
        /// </summary>
        /// <param name="a_dcu_list_meter"></param>
        public static void add_ItemsIndex(Tuple<long, long[]>[] a_dcu_list_meter)
        {
            bool update_dcu = false;

            lock (lock_index)
            {

                for (int k = 0; k < a_dcu_list_meter.Length; k++)
                {
                    var o = a_dcu_list_meter[k];
                    int len = dic_DCU_meter.AddListDistinct(o.Item1, o.Item2);
                    if (len > 0)
                    {
                        update_dcu = true;
                        var di = list_DCU.FindIndex(m => m.dcu_id == o.Item1);
                        var du = list_DCU[di];
                        du.itemsub_count = len;
                        list_DCU[di] = du;
                    }
                }
                update_index();
            }

            if (update_dcu)
            {
                lock (lock_list)
                    update();
            }
        }

        /// <summary>
        /// Lấy về tất cả bảng m_DCU
        /// </summary>
        /// <returns></returns>
        public static List<m_dcu> get_All()
        {
            return list_DCU;
        }

        /// <summary>
        /// Lấy về danh sách meter_id của một DCU, lấy từ Dictionary Index
        /// </summary>
        /// <param name="dcu_id"></param>
        /// <returns></returns>
        public static long[] get_ItemsIndex(long dcu_id)
        {
            List<long> val = new List<long>() { };

            dic_DCU_meter.TryGetValue(dcu_id, out val);

            if (val == null) val = new List<long>() { };

            return val.ToArray();
        }























































        public static bool updateStatusOnline(long dcu_id)
        {
            int index = list_DCU.FindIndex(o => o.dcu_id == dcu_id);
            if (index != -1)
            {
                m_dcu item = list_DCU[index];
                int date = DateTime.Now.ToString("yyMMdd").TryParseToInt();

                if (item.date_update_lastest != date || item.is_online == false)
                {
                    item.is_online = true;
                    item.date_update_lastest = date;

                    list_DCU[index] = item;

                    update();

                    return true;
                }
                else
                {
                    if (item.date_update_lastest == date && item.is_online == false)
                    {
                        item.is_online = true;
                        item.date_update_lastest = date;

                        list_DCU[index] = item;

                        update();

                        return true;
                    }
                }
            }

            return false;
        }




        #region // get all, seach items ...

        public static Tuple<int, string> get_all(int page_number, int page_size)
        {
            string json = get_allJson(page_number, page_size);
            return new Tuple<int, string>(list_DCU.Count, json);
        }

        public static Tuple<int, string> get_all_true(int page_number, int page_size)
        {
            string json = get_allJson_true(page_number, page_size);
            return new Tuple<int, string>(list_DCU.Count, json);
        }

        private static string get_allJson(int page_number, int page_size)
        {
            int startRowIndex = page_size * (page_number - 1);
            var dt = list_DCU.Skip(startRowIndex).Take(page_size).ToList();

            for (int k = 0; k < dt.Count; k++)
            {
                m_dcu item = dt[k];
                //////item.itemsub_count = store.f_dcu_meter_count(list[k].dcu_id);
                dt[k] = item;
            }

            return JsonConvert.SerializeObject(dt);
        }

        private static string get_allJson_true(int page_number, int page_size)
        {
            int startRowIndex = page_size * (page_number - 1);
            var dt = list_DCU.Where(o => o.status == true).Skip(startRowIndex).Take(page_size).ToList();

            for (int k = 0; k < dt.Count; k++)
            {
                m_dcu item = dt[k];
                //item.itemsub_count = store.f_dcu_meter_count(list[k].dcu_id);
                dt[k] = item;
            }

            return JsonConvert.SerializeObject(dt);
        }

        public static Tuple<int, string> get_ItemJsonBy_search(string keyword, int page_number, int page_size)
        {
            var ls = list_DCU.Where(o => o.status == true).Where(x =>
                  (x.dcu_id.ToString().Contains(keyword))
                    || (x.name != null && x.name.Contains(keyword))
                    || (x.note != null && x.note.Contains(keyword))
                    || (x.address != null && x.address.Contains(keyword))
                ).ToList();

            dynamic dt;

            if (ls.Count > page_size)
            {
                int startRowIndex = page_size * (page_number - 1);
                dt = ls.Skip(startRowIndex).Take(page_size).ToList();
            }
            else
                dt = ls;

            for (int k = 0; k < dt.Count; k++)
            {
                m_dcu item = dt[k];
                //item.itemsub_count = store.f_dcu_meter_count(list[k].dcu_id);
                dt[k] = item;
            }

            string json = JsonConvert.SerializeObject(dt);
            return new Tuple<int, string>(ls.Count, json);
        }

        #endregion

        public static string get_ItemJson(long dcu_id, string msg_default = "")
        {
            string json = msg_default;

            int pos = list_DCU.FindIndex(o => o.dcu_id == dcu_id);
            if (pos != -1)
            {
                m_dcu o = list_DCU[pos];
                //o.cat_array = store.f_meter_get_by_dcu_array(dcu_id);

                json = JsonConvert.SerializeObject(o);
            }

            return json;
        }

        public static string add_ItemJson(long dcu_id, string name, string msg_default = "")
        {
            string json = msg_default;

            int index = list_DCU.FindIndex(o => o.dcu_id == dcu_id);
            if (index == -1)
            {
                m_dcu item = new m_dcu()
                {
                    dcu_id = dcu_id,
                    name = name,
                    status = false,
                    is_online = true
                };

                lock (lock_list)
                    list_DCU.Add(item);

                update();

                json = JsonConvert.SerializeObject(list_DCU);
            }

            return json;
        }
        public static void add_Item(long dcu_id, string name, string type_code)
        {
            int index = list_DCU.FindIndex(o => o.dcu_id == dcu_id);
            if (index == -1)
            {
                m_dcu item = new m_dcu()
                {
                    dcu_id = dcu_id,
                    name = name,
                    status = true,
                    is_online = true
                };

                lock (lock_list)
                    list_DCU.Add(item);

                update();
            }
        }


        public static string changeStatus(string key, string msg_default = "")
        {
            string json = msg_default;
            long id = key.TryParseToLong();
            if (id > 0)
            {
                try
                {
                    int index = list_DCU.FindIndex(o => o.dcu_id == id);
                    if (index != -1)
                    {
                        m_dcu u = list_DCU[index];
                        bool status = false;
                        if (u.status)
                            status = false;
                        else
                            status = true;
                        u.status = status;

                        lock (lock_list)
                            list_DCU[index] = u;

                        update();

                        json = JsonConvert.SerializeObject(u);
                    }
                }
                catch { }
            }
            return json;
        }

        public static string edit_ItemJsonString(string item_json, string msg_default = "")
        {
            string json = msg_default;

            try
            {
                m_dcu m = JsonConvert.DeserializeObject<m_dcu>(item_json);

                int index = list_DCU.FindIndex(o => o.dcu_id == m.dcu_id);
                if (index != -1)
                {
                    m_dcu u = list_DCU[index];
                    u.name = m.name;
                    u.note = m.note;
                    u.address = m.address;

                    lock (lock_list)
                        list_DCU[index] = u;

                    update();

                    json = JsonConvert.SerializeObject(u);
                }
            }
            catch { }

            return json;
        }

        public static string edit_ItemJson(long dcu_id, string name, bool status, string msg_default = "")
        {
            string json = msg_default;

            int index = list_DCU.FindIndex(o => o.dcu_id == dcu_id);
            if (index != -1)
            {
                m_dcu item = new m_dcu()
                {
                    dcu_id = dcu_id,
                    name = name,
                    status = status
                };
                lock (lock_list)
                    list_DCU.Add(item);

                update();

                json = JsonConvert.SerializeObject(list_DCU);
            }

            return json;
        }

        public static string remove_ItemJson(long dcu_id, string status, string msg_default = "")
        {
            string json = msg_default;

            int pos = list_DCU.FindIndex(x => x.dcu_id == dcu_id);
            if (pos != -1)
            {
                var item = list_DCU[pos];
                item.status = true;
                json = "actived";

                if (status == "0")
                {
                    item.status = false;
                    json = "disabled";
                }

                lock (lock_list)
                    list_DCU[pos] = item;

                update();
            }

            return json;
        }



    }

}
