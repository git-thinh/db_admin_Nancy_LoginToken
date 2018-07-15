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

using System.Linq.Dynamic;

using model;

namespace host
{
    public class db_meter
    {
        public static Tuple<bool, string, dynamic> add(string s_key, string s_item)
        {
            try
            {
                string json = "";

                m_meter m = JsonConvert.DeserializeObject<m_meter>(s_item);

                //100000000014

                int index = list.FindIndex(o => o.cus_code == m.cus_code || o.meter_id == m.meter_id);
                if (index == -1)
                {
                    //m.dcu_id = m.dcu_id;
                    //m.meter_id = m.meter_id;

                    m.cus_id = Guid.NewGuid().ToString();
                    m.date_up = DateTime.Now.ToString("yyMMdd").TryParseToInt();
                    m.is_online = false;
                    if (m.index == 0)
                        m.index = list.Count + 1; 
                    m.status = true;

                    lock (lock_list)
                        list.Add(m);

                    //store.f_meter_set_by_dcu(m.dcu_id, m.meter_id);

                    update();



                    json = JsonConvert.SerializeObject(m);
                }

                return new Tuple<bool, string, dynamic>(true, json, m);
            }

            catch { }

            return new Tuple<bool, string, dynamic>(false, "", null);
        }

        public static Tuple<bool, string, dynamic> edit(string s_key, string s_item)
        {
            try
            {
                string json = "";

                m_meter m = JsonConvert.DeserializeObject<m_meter>(s_item);

                int index = list.FindIndex(o => o.cus_id == m.cus_id);
                if (index != -1)
                {
                    m_meter u = list[index];



                    u.address = m.address;
                    u.cus_code = m.cus_code;
                    u.cus_code_fix = m.cus_code_fix;

                    u.date_down = m.date_down;
                    u.date_up = m.date_up;
                    u.email = m.email;
                    u.fullname = m.fullname; 
                    u.meter_id = m.meter_id;
                    u.name = m.name;
                    u.note = m.note;
                    u.phone = m.phone;
                    u.type = m.type;

                    if (m.index == 0) 
                        u.index = list.Count + 1;
                    else
                        u.index = m.index;

                    u.type_code = m.type_code;
                    u.phase_id = m.phase_id;
                    u.dcu_id = m.dcu_id;
                    lock (lock_list)
                        list[index] = u;

                    update();



                    json = JsonConvert.SerializeObject(u);
                    long dcu_id = u.dcu_id;
                    string s = db_dcu.get_ItemJson(dcu_id);
                    json = json.Replace(@"""dcu_id"":" + dcu_id.ToString() + ",", @"""dcu_id"":" + s + ",");

                    return new Tuple<bool, string, dynamic>(true, json, m);
                }
            }

            catch { }
            return new Tuple<bool, string, dynamic>(false, "", null);
        }

        public static Tuple<bool, string, dynamic> remove(string s_key, string s_item)
        {
            try
            {
                string json = "";


                //int pos = list.FindIndex(x => x.cus_id == cus_id);
                //if (pos != -1)
                //{
                //    lock (lock_list)
                //        list.RemoveAt(pos);
                //    update();
                //}

                int pos = list.FindIndex(x => x.cus_id == s_item);
                if (pos != -1)
                {
                    var o = list[pos];
                    if (o.meter_id > 0)
                    {
                        o.status = false;

                        o.address = "";
                        o.cus_code = 0;
                        o.cus_code_fix = "";
                        o.date_down = 0;
                        o.date_up = 0;
                        o.email = "";
                        o.fullname = "";
                        o.index = 0;
                        o.is_online = false;
                        //o.meter_id =
                        o.name = "";
                        o.note = "";
                        o.phone = "";
                        o.type = "";
                        o.username = "";

                        lock (lock_list)
                            list[pos] = o;

                    }
                    else
                    {
                        lock (lock_list)
                            list.RemoveAt(pos);
                    }

                    update();


                }

                return new Tuple<bool, string, dynamic>(true, json, null);
            }

            catch { }
            return new Tuple<bool, string, dynamic>(false, "", null);
        }

        //===========================================================================
        private static object lock_list = new object();
        private static List<m_meter> list = new List<m_meter>() { };

        public static string path = hostServer.pathRoot + @"db_meter\", file_name = "meter";

        public static Tuple<bool, string, int, int, IList> where_call_dynamic(
            string s_key, string s_select, string s_where, string s_order_by, string s_distinct,
            int page_number, int page_size)
        {
            return dbQuery.where<m_meter>(list.ToArray(), s_key, s_select, s_where, s_order_by, s_distinct, page_number, page_size);
        }

        public static void update()
        {
            hostFile.write_file_MMF<m_meter>(list, path, file_name);
            dbCache.clear(typeof(m_meter).FullName);
        }

        public static void load()
        {
            string path_file = path + file_name + ".mmf";
            list = hostFile.read_file_MMF<m_meter>(path_file).ToList();
        }

        /// <summary>
        /// Thêm mới các thiết bị meter từ dcu_id, meter_id, meter_id_ref, type_code
        /// </summary>
        /// <param name="a_dcu_meter_ref_type_code"></param>
        public static void add_Items(Tuple<long, long, long, string>[] a_dcu_meter_ref_type_code, d1_pha phase_id)
        {
            var ls = a_dcu_meter_ref_type_code
                .Where(x => !list.Any(o => o.meter_id == x.Item2))
                .GroupBy(x => new { meter = x.Item2 })
                .Where(x => x.Key.meter > 0)
                .Select(x => new Tuple<long, long, long, string>(x.First().Item1, x.Key.meter, x.First().Item3, x.First().Item4))
                .Select(x => new m_meter()
                {
                    phase_id = (int)phase_id,
                    dcu_id = x.Item1,
                    meter_id = x.Item2,
                    meter_id_ref = x.Item3,
                    type_code = x.Item4,
                    type = "",
                    name = "",
                    cus_id = Guid.NewGuid().ToString(),
                    cus_code = 0,
                    cus_code_fix = "",
                    date_up = 0,
                    date_down = 0,
                    is_online = false,
                    status = false
                }).ToArray();

            if (ls.Length > 0)
            {
                lock (lock_list)
                {
                    list.AddRange(ls);
                    update();
                }
            }
        }

        public static bool check_exist(long meter_id)
        {
            int pos = list.FindIndex(x => x.meter_id == meter_id);
            if (pos == -1) return false;
            return true;
        }

        public static void add_Items(m_meter item)
        {
            int pos = list.FindIndex(x => x.meter_id == item.meter_id);
            if (pos == -1)
            {
                lock (lock_list)
                {
                    list.Add(item);
                    update();
                }
            }
        }

        public static void add_Items(m_meter[] items)
        {
            bool has_update = false;

            lock (lock_list)
            {
                foreach (var item in items)
                {
                    int pos = list.FindIndex(x => x.meter_id == item.meter_id);
                    if (pos == -1)
                    {
                        list.Add(item);
                        has_update = true;
                    }
                }
            }

            if (has_update)
                update();
        }

        /// <summary>
        /// Lấy về mảng m_meter[] từ mảng meter_id[]
        /// </summary>
        /// <param name="a_meter"></param>
        /// <returns></returns>
        public static m_meter[] get_Items(long[] a_meter)
        {
            if (a_meter.Length == 0) return new m_meter[] { };
            return list.Where(x => a_meter.Any(o => o == x.meter_id)).ToArray();
        }


        public static long[] get_AllItemID()
        {
            return list.Select(x => x.meter_id).Distinct().ToArray();
        }


        //public static Tuple<int, int, string> where(string where_lambda, int page_number, int page_size)
        //{
        //    var ls = list.AsQueryable().Where(where_lambda, new object[] { }).ToList();

        //    m_meter[] a = new m_meter[] { };
        //    if (ls.Count > page_size)
        //    {
        //        int startRowIndex = page_size * (page_number - 1);
        //        a = ls.Skip(startRowIndex).Take(page_size).ToArray();
        //    }
        //    else
        //        a = ls.ToArray();

        //    string json = JsonConvert.SerializeObject(a);

        //    return new Tuple<int, int, string>(list.Count, ls.Count, json);
        //}































        #region // get all, search items ...

        public static Tuple<int, string> get_all(int page_number, int page_size)
        {
            string json = get_allJson(page_number, page_size);
            return new Tuple<int, string>(list.Count, json);
        }

        public static Tuple<int, string> get_all_true(int page_number, int page_size)
        {
            string json = get_allJson_true(page_number, page_size);
            return new Tuple<int, string>(list.Count, json);
        }

        public static string get_allJson(long[] vals)
        {
            var ls = list.Where(x => vals.Any(o => o == x.meter_id)).ToList();
            string json = JsonConvert.SerializeObject(ls);
            return json;
        }



        public static string get_allJson(int page_number, int page_size)
        {
            int startRowIndex = page_size * (page_number - 1);
            var dt = list.OrderByDescending(x => x.date_up).Skip(startRowIndex).Take(page_size).ToList();
            string json = JsonConvert.SerializeObject(dt);

            var a_dcu = dt.Select(x => x.dcu_id).Distinct().ToArray();

            for (int k = 0; k < a_dcu.Length; k++)
            {
                long dcu_id = a_dcu[k];
                string s = db_dcu.get_ItemJson(dcu_id);
                json = json.Replace(@"""dcu_id"":" + dcu_id.ToString() + ",", @"""dcu_id"":" + s + ",");
            }

            return json;
        }

        public static string get_allJson_true(int page_number, int page_size)
        {
            int startRowIndex = page_size * (page_number - 1);
            var dt = list.Where(o => o.status == true).OrderByDescending(x => x.date_up).Skip(startRowIndex).Take(page_size).ToList();
            string json = JsonConvert.SerializeObject(dt);

            var a_dcu = dt.Select(x => x.dcu_id).Distinct().ToArray();

            for (int k = 0; k < a_dcu.Length; k++)
            {
                long dcu_id = a_dcu[k];
                string s = db_dcu.get_ItemJson(dcu_id);
                json = json.Replace(@"""dcu_id"":" + dcu_id.ToString() + ",", @"""dcu_id"":" + s + ",");
            }

            return json;
        }

        public static Tuple<int, string> get_ItemJsonBy_search(string keyword, int page_number, int page_size)
        {
            var ls = list.Where(o => o.status == true).Where(x =>
                  (x.meter_id != 0 && x.meter_id.ToString().Contains(keyword))
                      // || (x.dcu_id != 0 && x.dcu_id.ToString().Contains(keyword))
                    || (x.cus_code != 0 && x.cus_code.ToString().Contains(keyword))
                    || (x.name != null && x.name.Contains(keyword))
                    || (x.phone != null && x.phone.Contains(keyword))
                    || (x.note != null && x.note.Contains(keyword))
                    || (x.email != null && x.email.Contains(keyword))
                    || (x.date_down != null && x.date_down.ToString().Contains(keyword))
                    || (x.date_up != null && x.date_up.ToString().Contains(keyword))
                ).OrderByDescending(x => x.date_up).ToList();

            //int startRowIndex = page_size * (page_number - 1);
            //var dt = ls.Skip(startRowIndex).Take(page_size).ToList();

            List<m_meter> dt = new List<m_meter>() { };
            if (ls.Count > page_size)
            {
                int startRowIndex = page_size * (page_number - 1);
                dt = ls.Skip(startRowIndex).Take(page_size).ToList();
            }
            else
                dt = ls;

            string json = JsonConvert.SerializeObject(dt);
            var a_dcu = dt.Select(x => x.dcu_id).Distinct().ToArray();

            for (int k = 0; k < a_dcu.Length; k++)
            {
                long dcu_id = a_dcu[k];
                string s = db_dcu.get_ItemJson(dcu_id);
                json = json.Replace(@"""dcu_id"":" + dcu_id.ToString() + ",", @"""dcu_id"":" + s + ",");
            }

            //return json;

            return new Tuple<int, string>(ls.Count, json);
        }

        #endregion

        #region // get item by id, find item by ...

        public static string get_ItemJsonBy_cus_id(string cus_id, string msg_default = "")
        {
            string json = msg_default;

            int pos = list.FindIndex(o => o.cus_id == cus_id);
            if (pos != -1)
            {
                json = JsonConvert.SerializeObject(list[pos]);

                long dcu_id = list[pos].dcu_id;
                string s = db_dcu.get_ItemJson(dcu_id);
                json = json.Replace(@"""dcu_id"":" + dcu_id.ToString() + ",", @"""dcu_id"":" + s + ",");

                return json;
            }

            return json;
        }

        public static Tuple<int, int, string> get_ItemJsonBy_find_cus_none(string keyword, int page_number, int page_size)
        {
            var ls = list.Where(x => (x.cus_code == 0) && x.meter_id.ToString().Contains(keyword)).ToList();

            int startRowIndex = page_size * (page_number - 1);
            var dt = ls.Skip(startRowIndex).Take(page_size).ToList();
            string json = JsonConvert.SerializeObject(dt);

            var a_dcu = dt.Select(x => x.dcu_id).Distinct().ToArray();

            for (int k = 0; k < a_dcu.Length; k++)
            {
                long dcu_id = a_dcu[k];
                string s = db_dcu.get_ItemJson(dcu_id);
                json = json.Replace(@"""dcu_id"":" + dcu_id.ToString() + ",", @"""dcu_id"":" + s + ",");
            }

            return new Tuple<int, int, string>(list.Count, ls.Count, json);
        }

        public static Tuple<int, int, string> get_ItemJsonBy_cus_none(int page_number, int page_size)
        {
            var ls = list.Where(x => x.cus_code == 0).ToList();

            int startRowIndex = page_size * (page_number - 1);
            var dt = ls.Skip(startRowIndex).Take(page_size).ToList();
            string json = JsonConvert.SerializeObject(dt);

            var a_dcu = dt.Select(x => x.dcu_id).Distinct().ToArray();

            for (int k = 0; k < a_dcu.Length; k++)
            {
                long dcu_id = a_dcu[k];
                string s = db_dcu.get_ItemJson(dcu_id);
                json = json.Replace(@"""dcu_id"":" + dcu_id.ToString() + ",", @"""dcu_id"":" + s + ",");
            }

            return new Tuple<int, int, string>(list.Count, ls.Count, json);
        }

        public static string get_ItemJsonBy_find_meter_id(long meter_id, string msg_default = "")
        {
            string json = msg_default;

            var ls = list.Where(x => x.meter_id == meter_id).ToList();
            json = JsonConvert.SerializeObject(list);

            var a_dcu = list.Select(x => x.dcu_id).Distinct().ToArray();

            for (int k = 0; k < a_dcu.Length; k++)
            {
                long dcu_id = a_dcu[k];
                string s = db_dcu.get_ItemJson(dcu_id);
                json = json.Replace(@"""dcu_id"":" + dcu_id.ToString() + ",", @"""dcu_id"":" + s + ",");
            }

            return json;
        }

        public static string get_ItemJsonBy_find_cus_code(long cus_code, string msg_default = "")
        {
            string json = msg_default;

            var dt = list.Where(x => x.cus_code == 0).ToList();
            json = JsonConvert.SerializeObject(dt);

            var a_dcu = dt.Select(x => x.dcu_id).Distinct().ToArray();

            for (int k = 0; k < a_dcu.Length; k++)
            {
                long dcu_id = a_dcu[k];
                string s = db_dcu.get_ItemJson(dcu_id);
                json = json.Replace(@"""dcu_id"":" + dcu_id.ToString() + ",", @"""dcu_id"":" + s + ",");
            }

            return json;
        }

        public static string get_ItemJsonBy_meter_id(long meter_id, string msg_default = "")
        {
            string json = msg_default;

            int pos = list.FindIndex(o => o.meter_id == meter_id);
            if (pos != -1)
            {
                json = JsonConvert.SerializeObject(list[pos]);
                long dcu_id = list[pos].dcu_id;
                string s = db_dcu.get_ItemJson(dcu_id);
                json = json.Replace(@"""dcu_id"":" + dcu_id.ToString() + ",", @"""dcu_id"":" + s + ",");
            }

            return json;
        }

        public static string get_ItemsID_Json_true(long[] meter_ids)
        {
            var dt = list.Where(o => o.status == true).Where(m => meter_ids.Any(o => o == m.meter_id))
                .Select(x => x.meter_id)
                .ToArray();

            string json = JsonConvert.SerializeObject(dt);
            return json;
        }

        public static string get_allItemsID_Json_true()
        {
            var dt = list.Where(o => o.status == true)
                .Select(x => x.meter_id)
                .ToArray();

            string json = JsonConvert.SerializeObject(dt);
            return json;
        }


        public static string get_ItemsJson_true(long[] meter_ids)
        {
            var dt = list.Where(o => o.status == true).Where(m => meter_ids.Any(o => o == m.meter_id))
                .ToArray();
            string json = JsonConvert.SerializeObject(dt);
            var a_dcu = dt.Select(x => x.dcu_id).Distinct().ToArray();

            for (int k = 0; k < a_dcu.Length; k++)
            {
                long dcu_id = a_dcu[k];
                string s = db_dcu.get_ItemJson(dcu_id);
                json = json.Replace(@"""dcu_id"":" + dcu_id.ToString() + ",", @"""dcu_id"":" + s + ",");
            }
            return json;
        }

        public static string get_ItemsJson(long[] meter_ids)
        {
            var dt = list.Where(m => meter_ids.Any(o => o == m.meter_id))
                .ToArray();
            string json = JsonConvert.SerializeObject(dt);
            var a_dcu = dt.Select(x => x.dcu_id).Distinct().ToArray();

            for (int k = 0; k < a_dcu.Length; k++)
            {
                long dcu_id = a_dcu[k];
                string s = db_dcu.get_ItemJson(dcu_id);
                json = json.Replace(@"""dcu_id"":" + dcu_id.ToString() + ",", @"""dcu_id"":" + s + ",");
            }
            return json;
        }

        #endregion

        #region // add, edit, remove ...

        public static void edit_dcu_id(Tuple<long, long>[] dic_meter_dcu)
        {
            long[] a_meter = dic_meter_dcu.Select(x => x.Item1).ToArray();
            bool ok = false;
            for (int k = 0; k < a_meter.Length; k++)
            {
                long mid = a_meter[k];
                int pos = list.FindIndex(x => x.meter_id == mid);
                if (pos != -1)
                {
                    ok = true;
                    m_meter m = list[pos];
                    m.dcu_id = dic_meter_dcu[k].Item2;
                    lock (lock_list)
                        list[pos] = m;
                }
            }

            if (ok)
                update();
        }

        public static void edit_dcu_id(Dictionary<long, long> dic_meter_dcu)
        {
            long[] a_meter = dic_meter_dcu.Keys.ToArray();
            bool ok = false;
            for (int k = 0; k < a_meter.Length; k++)
            {
                long mid = a_meter[k];
                int pos = list.FindIndex(x => x.meter_id == mid);
                if (pos != -1)
                {
                    ok = true;
                    m_meter m = list[pos];
                    m.dcu_id = dic_meter_dcu[mid];
                    lock (lock_list)
                        list[pos] = m;
                }
            }

            if (ok)
                update();
        }

        public static void add_Items(long dcu_id, Tuple<long, long>[] dcuREF_meter)
        {
            var ls = dcuREF_meter.Where(x => !list.Any(o => o.meter_id == x.Item2))
                .Select(x => new m_meter()
                {
                    meter_id = x.Item2,
                    dcu_id = dcu_id,
                    meter_id_ref = x.Item1,
                    name = "",
                    cus_id = Guid.NewGuid().ToString(),
                    cus_code = 0,
                    cus_code_fix = "",
                    date_up = 0,
                    date_down = 0,
                    is_online = false,
                    status = true
                }).ToArray();

            if (ls.Length > 0)
            {
                lock (lock_list)
                {
                    list.AddRange(ls);
                    update();
                }
            }
        }

        public static void add_Items(long[] a_meter_id)
        {
            var ls = a_meter_id.Where(x => !list.Any(o => o.meter_id == x))
                .Select(x => new m_meter()
                {
                    meter_id = x,
                    name = "",
                    cus_id = Guid.NewGuid().ToString(),
                    cus_code = 0,
                    cus_code_fix = "",
                    date_up = 0,
                    date_down = 0,
                    is_online = false,
                    status = true
                }).ToArray();

            if (ls.Length > 0)
            {
                lock (lock_list)
                {
                    list.AddRange(ls);
                    update();
                }
            }
        }

        public static string add_ItemJson(string item_json, string msg_default = "")
        {
            string json = msg_default;

            //try
            //{
            //    m_meter m = JsonConvert.DeserializeObject<m_meter>(item_json);

            //    //100000000014

            //    int index = list.FindIndex(o => o.cus_code == m.cus_code);
            //    if (index == -1)
            //    {
            //        //m.dcu_id = m.dcu_id;
            //        //m.meter_id = m.meter_id;

            //        m.cus_id = Guid.NewGuid().ToString();
            //        m.date_up = DateTime.Now.ToString("yyMMdd").TryParseToInt();
            //        m.is_online = false;
            //        m.status = false;

            //        lock (lock_list)
            //            list.Add(m);

            //        //store.f_meter_set_by_dcu(m.dcu_id, m.meter_id);

            //        update();

            //        json = JsonConvert.SerializeObject(m);
            //    }
            //}
            //catch { }

            return json;
        }

        public static string changeStatus(string key, string msg_default = "")
        {
            string json = msg_default;
            long id = key.TryParseToLong();
            if (id > 0)
            {
                try
                {
                    int index = list.FindIndex(o => o.meter_id == id);
                    if (index != -1)
                    {
                        m_meter u = list[index];
                        bool status = false;
                        if (u.status)
                            status = false;
                        else
                            status = true;
                        u.status = status;

                        lock (lock_list)
                            list[index] = u;

                        update();

                        json = JsonConvert.SerializeObject(u);
                        long dcu_id = u.dcu_id;
                        string s = db_dcu.get_ItemJson(dcu_id);
                        json = json.Replace(@"""dcu_id"":" + dcu_id.ToString() + ",", @"""dcu_id"":" + s + ",");
                    }
                }
                catch { }
            }
            return json;
        }

        public static string edit_ItemChange_meter(string cus_id_string, string msg_default = "")
        {
            string json = msg_default;
            string[] a = cus_id_string.Split('|');

            int pos0 = list.FindIndex(m => m.cus_id == a[0]);
            int pos1 = list.FindIndex(m => m.meter_id == a[1].TryParseToLong());

            if (pos0 != -1 && pos1 != -1)
            {
                m_meter m0 = list[pos0];
                m_meter m1 = list[pos1];

                long mid = m0.meter_id;
                m0.meter_id = m1.meter_id;
                m1.meter_id = mid;

                lock (lock_list)
                {
                    list[pos0] = m0;
                    list[pos1] = m1;
                }

                update();

                json = JsonConvert.SerializeObject(m0);
                long dcu_id = m0.dcu_id;
                string s = db_dcu.get_ItemJson(dcu_id);
                json = json.Replace(@"""dcu_id"":" + dcu_id.ToString() + ",", @"""dcu_id"":" + s + ",");
            }

            return json;
        }

        public static string edit_ItemJsonString(string item_json, string msg_default = "")
        {
            string json = msg_default;
            try
            {
                m_meter m = JsonConvert.DeserializeObject<m_meter>(item_json);

                int index = list.FindIndex(o => o.cus_id == m.cus_id);
                if (index != -1)
                {
                    m_meter u = list[index];


                    u.address = m.address;
                    u.cus_code = m.cus_code;
                    u.cus_code_fix = m.cus_code_fix;

                    u.date_down = m.date_down;
                    u.date_up = m.date_up;
                    u.email = m.email;
                    u.fullname = m.fullname;
                    u.index = m.index;
                    u.meter_id = m.meter_id;
                    u.name = m.name;
                    u.note = m.note;
                    u.phone = m.phone;
                    u.type = m.type;
                    u.type_code = m.type_code;

                    lock (lock_list)
                        list[index] = u;

                    update();

                    json = JsonConvert.SerializeObject(u);
                    long dcu_id = u.dcu_id;
                    string s = db_dcu.get_ItemJson(dcu_id);
                    json = json.Replace(@"""dcu_id"":" + dcu_id.ToString() + ",", @"""dcu_id"":" + s + ",");
                }
            }
            catch { }

            return json;
        }


        public static m_meter[] get_All()
        {
            return list.ToArray();
        }

        public static string remove_Customer(string cus_id, string msg_default = "")
        {
            string json = msg_default;
            //int pos = list.FindIndex(x => x.cus_id == cus_id);
            //if (pos != -1)
            //{
            //    lock (lock_list)
            //        list.RemoveAt(pos);
            //    update();
            //}

            int pos = list.FindIndex(x => x.cus_id == cus_id);
            if (pos != -1)
            {
                var o = list[pos];
                if (o.meter_id > 0)
                {
                    o.status = false;

                    o.address = "";
                    o.cus_code = 0;
                    o.cus_code_fix = "";
                    o.date_down = 0;
                    o.date_up = 0;
                    o.email = "";
                    o.fullname = "";
                    o.index = 0;
                    o.is_online = false;
                    //o.meter_id =
                    o.name = "";
                    o.note = "";
                    o.phone = "";
                    o.type = "";
                    o.username = "";

                    lock (lock_list)
                        list[pos] = o;
                }
                else
                {
                    lock (lock_list)
                        list.RemoveAt(pos);
                }

                update();
            }

            return json;
        }


        #endregion

    }

}
