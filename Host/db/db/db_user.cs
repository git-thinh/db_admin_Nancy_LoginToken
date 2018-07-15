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
    public class db_user
    {
        public static Tuple<bool, string, dynamic> add(string s_key, string s_item)
        {
            try
            {
                string json = "";
                m_user m = JsonConvert.DeserializeObject<m_user>(s_item);

                int index = list.FindIndex(o => o.username == m.username);
                if (index == -1)
                {
                    m.user_id = Guid.NewGuid().ToString();
                    m.date_join = DateTime.Now.ToString("yyMMdd").TryParseToInt();
                    m.date_update_lastest = DateTime.Now.ToString("yyMMdd").TryParseToInt();
                    m.status = true;

                    lock (lock_list)
                        list.Add(m);

                    update();

                    dbCache.clear(typeof(m_user).FullName);

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
                m_user m = JsonConvert.DeserializeObject<m_user>(s_item);

                int index = list.FindIndex(o => o.username == m.username);
                if (index != -1)
                {
                    m_user o = list[index];
                    o.date_update_lastest = DateTime.Now.ToString("yyMMdd").TryParseToInt();

                    if (string.IsNullOrEmpty(o.user_id)) o.user_id = Guid.NewGuid().ToString();

                    o.name = m.name;
                    o.email = m.email;
                    o.address = m.address;
                    o.phone = m.phone;
                    o.note = m.note;

                    m = o;

                    lock (lock_list)
                        list[index] = o;

                    update();


                    json = JsonConvert.SerializeObject(o);
                }
                return new Tuple<bool, string, dynamic>(true, json, m);
            }
            catch { }
            return new Tuple<bool, string, dynamic>(false, "", null);
        }

        public static Tuple<bool, string, dynamic> remove(string s_key, string s_item)
        {
            try
            {
                m_user o = new m_user();
                string json = "", user_name = s_item;
                if (user_name == "ifc" || user_name == "admin") 
                    json = "{}";
                else
                {
                    int pos = list.FindIndex(x => x.username == user_name);
                    if (pos != -1)
                    {
                        o = list[pos];
                        json = JsonConvert.SerializeObject(list[pos]);
                        lock (lock_list)
                            list.RemoveAt(pos);

                        update();
                    }
                }
                return new Tuple<bool, string, dynamic>(true, json, o);
            }
            catch { }
            return new Tuple<bool, string, dynamic>(false, "", null);
        }

        //===========================================================================
        private static object lock_list = new object();
        private static List<m_user> list = new List<m_user>() { };
        public static string path = hostServer.pathRoot + @"db_user\", file_name = "user";

        public static Tuple<bool, string, int, int, IList> where_call_dynamic(string s_key, string s_select, string s_where, string s_order_by, string s_distinct, int page_number, int page_size)
        {
            return dbQuery.where<m_user>(list.ToArray(), s_key, s_select, s_where, s_order_by, s_distinct, page_number, page_size);
        }

        public static void update()
        {
            dbCache.clear(typeof(m_user).FullName);

            hostFile.write_file_MMF<m_user>(list, path, file_name);
        }

        public static void load()
        {
            string path_file = path + file_name + ".mmf";
            list = hostFile.read_file_MMF<m_user>(path_file).ToList();

            if (list.Count == 0)
            {
                lock (lock_list)
                {
                    list.AddRange(new m_user[] {
                          new m_user() { user_id = Guid.NewGuid().ToString(), username = "ifc", pass  = "123", status = true }
                        , new m_user() { user_id = Guid.NewGuid().ToString(), username = "admin", pass  = "admin", status = true }
                        , new m_user() { user_id = Guid.NewGuid().ToString(), username = "nga", pass  = "nga", status =true }
                        , new m_user() { user_id = Guid.NewGuid().ToString(), username = "tuan", pass  = "tuan", status = true }
                        , new m_user() { user_id = Guid.NewGuid().ToString(), username = "thanh", pass  = "thanh", status = true }
                        , new m_user() { user_id = Guid.NewGuid().ToString(), username = "thinh", pass  = "thinh", status = true }
                        , new m_user() { user_id = Guid.NewGuid().ToString(), username = "duy", pass  = "duy", status = true }
                    });
                }

                update();
            }
        }

        #region // login ...

        public static m_user login(string username, string password)
        {
            m_user u = new m_user();
            List<m_user> dt = new List<m_user>() { };

            if (username == "ifc" || username == "admin")
                dt = list.Where(x => x.username == username && x.pass == password).ToList();
            else
                dt = list.Where(x => x.username == username && x.pass == password && x.status == true).ToList();

            if (dt.Count > 0)
            {
                u = dt[0];
                if (username == "ifc" || username == "admin") u.status = true;
            }

            return u;
        }

        public static m_user login(m_user o)
        {
            m_user u = new m_user();
            if (o.username == null || o.pass == null) return u;
            u = login(o.username, o.pass);
            return u;
        }

        #endregion

        #region // get all, search items ...

        public static Tuple<int, string> get_all(int page_number, int page_size)
        {
            string json = get_allJson(page_number, page_size);
            return new Tuple<int, string>(list.Count, json);
        }

        public static string get_allJson(int page_number, int page_size)
        {
            int startRowIndex = page_size * (page_number - 1);
            var dt = list.OrderByDescending(x => x.date_join).Skip(startRowIndex).Take(page_size).ToList();
            string json = JsonConvert.SerializeObject(dt);

            return json;
        }

        public static Tuple<int, string> get_ItemJsonBy_search(string keyword, int page_number, int page_size)
        {
            var ls = list.Where(x =>
                     (x.username != null && x.username.Contains(keyword))
                    || (x.name != null && x.name.Contains(keyword))
                    || (x.phone != null && x.phone.Contains(keyword))
                    || (x.address != null && x.address.Contains(keyword))
                    || (x.note != null && x.note.Contains(keyword))
                    || (x.email != null && x.email.Contains(keyword))
                ).OrderByDescending(x => x.date_join).ToList();

            //int startRowIndex = page_size * (page_number - 1);
            //var dt = ls.Skip(startRowIndex).Take(page_size).ToList();

            List<m_user> dt = new List<m_user>() { };
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

        public static string get_ItemJsonBy_user_id(string user_id, string msg_default = "")
        {
            string json = msg_default;

            int pos = list.FindIndex(o => o.user_id == user_id);
            if (pos != -1)
            {
                json = JsonConvert.SerializeObject(list[pos]);
                return json;
            }

            return json;
        }

        public static string get_ItemJsonBy_find_user_name(string user_name, string msg_default = "")
        {
            string json = msg_default;

            var ls = list.Where(x => x.username == user_name).ToList();
            for (int k = 0; k < ls.Count; k++)
            {
                m_user m = ls[k];
                m.pass = "";
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
                m_user m = JsonConvert.DeserializeObject<m_user>(item_json);

                int index = list.FindIndex(o => o.username == m.username);
                if (index == -1)
                {
                    m.user_id = Guid.NewGuid().ToString();
                    m.date_join = DateTime.Now.ToString("yyMMdd").TryParseToInt();
                    m.date_update_lastest = DateTime.Now.ToString("yyMMdd").TryParseToInt();
                    m.status = false;

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
                m_user m = JsonConvert.DeserializeObject<m_user>(item_json);

                int index = list.FindIndex(o => o.username == m.username);
                if (index != -1)
                {
                    m_user o = list[index];
                    o.date_update_lastest = DateTime.Now.ToString("yyMMdd").TryParseToInt();

                    if (string.IsNullOrEmpty(o.user_id)) o.user_id = Guid.NewGuid().ToString();

                    o.name = m.name;
                    o.email = m.email;
                    o.address = m.address;
                    o.phone = m.phone;
                    o.note = m.note;

                    lock (lock_list)
                        list[index] = o;

                    update();

                    json = JsonConvert.SerializeObject(o);
                }
            }
            catch { }

            return json;
        }

        public static string remove_Item(string user_name, string msg_default = "")
        {
            if (user_name == "ifc" || user_name == "admin") return "{}";

            string json = msg_default;

            int pos = list.FindIndex(x => x.username == user_name);
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

        #region // permission update ....


        public static bool ok_remove(string user_name)
        {
            return true;

            if (user_name == "ifc" || user_name == "admin") return true;

            int pos = list.FindIndex(m => m.username == user_name);

            if (pos != -1)
            {
                bool rs = list[pos].p_remove;
                if (rs == null) return false;
                else return rs;
            }

            return false;
        }

        public static bool ok_edit(string user_name)
        {
            return true;


            if (user_name == "ifc" || user_name == "admin") return true;

            int pos = list.FindIndex(m => m.username == user_name);

            if (pos != -1)
            {
                bool rs = list[pos].p_edit;
                if (rs == null) return false;
                else return rs;
            }

            return false;
        }


        public static string changePermissionView(string key, string msg_default = "")
        {

            string json = msg_default;
            try
            {
                int index = list.FindIndex(o => o.username == key);
                if (index != -1)
                {
                    m_user u = list[index];
                    bool status = false;
                    if (u.p_view)
                        status = false;
                    else
                        status = true;
                    u.p_view = status;

                    lock (lock_list)
                        list[index] = u;

                    update();

                    json = JsonConvert.SerializeObject(u);
                }
            }
            catch { }
            return json;
        }

        public static string changePermissionEdit(string key, string msg_default = "")
        {

            string json = msg_default;
            try
            {
                int index = list.FindIndex(o => o.username == key);
                if (index != -1)
                {
                    m_user u = list[index];
                    bool status = false;
                    if (u.p_edit)
                        status = false;
                    else
                        status = true;
                    u.p_edit = status;

                    lock (lock_list)
                        list[index] = u;

                    update();

                    json = JsonConvert.SerializeObject(u);
                }
            }
            catch { }
            return json;
        }

        public static string changePermissionRemove(string key, string msg_default = "")
        {

            string json = msg_default;
            try
            {
                int index = list.FindIndex(o => o.username == key);
                if (index != -1)
                {
                    m_user u = list[index];
                    bool status = false;
                    if (u.p_remove)
                        status = false;
                    else
                        status = true;
                    u.p_remove = status;

                    lock (lock_list)
                        list[index] = u;

                    update();

                    json = JsonConvert.SerializeObject(u);
                }
            }
            catch { }
            return json;
        }





















        public static string changeStatus(string key, string msg_default = "")
        {
            string json = msg_default;
            try
            {
                int index = list.FindIndex(o => o.username == key);
                if (index != -1)
                {
                    m_user u = list[index];
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
                }
            }
            catch { }
            return json;
        }



        #endregion







    }
}
