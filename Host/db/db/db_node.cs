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
using System.Runtime.Caching;

namespace host
{

    public class db_node
    {
        private static int m_type_device = 99;
        private static string
            path_node = hostServer.pathRoot + @"db_node\",
            path_ID_parent = hostServer.pathRoot + @"db_node_parent\",
            path_ID_node = hostServer.pathRoot + @"db_node_sub\",
            path_ID_device = hostServer.pathRoot + @"db_node_device\";

        private static object lock_node = new object();
        private static object lock_ID_node = new object();
        private static object lock_ID_device = new object();
        private static Dictionary<long, m_node> dic_node = new Dictionary<long, m_node>() { };
        private static Dictionary<string, long> dic_node_code = new Dictionary<string, long>() { };
        private static DictionaryList<long, long> dic_ID_node = new DictionaryList<long, long>() { }; //<node_id, child_ids>
        private static DictionaryList<long, long> dic_ID_device = new DictionaryList<long, long>() { };

        private static object lock_ID_parent = new object();
        private static DictionaryList<long, long> dic_ID_parent = new DictionaryList<long, long>() { }; //<node_id, parent_ids>

        private static object lock_cache = new object();
        private static Dictionary<long, string> dic_ID_cache = new Dictionary<long, string>() { };

        #region /// update, load ...

        public static void update_node(long id, m_node o)
        {
            hostFile.write_MMF<m_node>(o, path_node, id.ToString());
            //dbCache.clear(typeof(m_node).FullName);
        }

        public static void update_ID_parent(long id, List<long> ids)
        {
            ids.Insert(0, id);
            hostFile.write_file_MMF<long>(ids, path_ID_parent, id.ToString());
            ids.RemoveAt(0);
        }

        public static void update_ID_node(long id, List<long> ids)
        {
            ids.Insert(0, id);
            hostFile.write_file_MMF<long>(ids, path_ID_node, id.ToString());
            ids.RemoveAt(0);
        }

        public static void update_ID_device(long id, List<long> ids)
        {
            ids.Insert(0, id);
            hostFile.write_file_MMF<long>(ids, path_ID_device, id.ToString());
            ids.RemoveAt(0);
        }

        public static void load()
        {
            if (!Directory.Exists(path_node))
                Directory.CreateDirectory(path_node);
            else
            {
                var ds = Directory.GetFiles(path_node, "*.mmf");
                for (int k = 0; k < ds.Length; k++)
                {
                    m_node o = hostFile.read_MMF<m_node>(ds[k]);
                    if (o.id != 0)
                    {
                        if (dic_node.ContainsKey(o.id) == false)
                            dic_node.Add(o.id, o);
                        if (dic_node_code.ContainsKey(o.code) == false)
                            dic_node_code.Add(o.code, o.id);
                    }
                }
            }

            if (!Directory.Exists(path_ID_parent))
                Directory.CreateDirectory(path_ID_parent);
            else
            {
                var ds = Directory.GetFiles(path_ID_parent, "*.mmf");
                for (int k = 0; k < ds.Length; k++)
                {
                    List<long> ls = hostFile.read_file_MMF<long>(ds[k]).Distinct().ToList();
                    if (ls.Count > 0)
                    {
                        long id = ls[0];
                        ls = ls.Where(x => x != 0 && x != id).ToList();
                        if (dic_ID_parent.ContainsKey(id) == false)
                            dic_ID_parent.AddListDistinct(id, ls.ToArray());
                    }
                }
            }

            if (!Directory.Exists(path_ID_node))
                Directory.CreateDirectory(path_ID_node);
            else
            {
                var ds = Directory.GetFiles(path_ID_node, "*.mmf");
                for (int k = 0; k < ds.Length; k++)
                {
                    List<long> ls = hostFile.read_file_MMF<long>(ds[k]).Distinct().ToList();
                    if (ls.Count > 0)
                    {
                        long id = ls[0];
                        ls = ls.Where(x => x != 0 && x != id).ToList();
                        if (dic_ID_node.ContainsKey(id) == false)
                            dic_ID_node.AddListDistinct(id, ls.ToArray());
                    }
                }
            }


            if (!Directory.Exists(path_ID_device))
                Directory.CreateDirectory(path_ID_device);
            else
            {
                var ds = Directory.GetFiles(path_ID_device, "*.mmf");
                for (int k = 0; k < ds.Length; k++)
                {
                    List<long> ls = hostFile.read_file_MMF<long>(ds[k]).Distinct().ToList();
                    if (ls.Count > 0)
                    {
                        long id = ls[0];
                        ls = ls.Where(x => x != 0 && x != id).ToList();
                        if (dic_ID_device.ContainsKey(id) == false)
                            dic_ID_device.AddListDistinct(id, ls.ToArray());
                    }
                }
            }

            cache_node_id();
        }

        #endregion

        #region /// Add, Edit, Remove ...

        public static Tuple<bool, string, dynamic> add(string s_key, string s_item)
        {
            try
            {
                string json = "";

                var o = JsonConvert.DeserializeObject<m_node>(s_item);
                m_node m = JsonConvert.DeserializeObject<m_node>(s_item);

                if (dic_node_code.ContainsKey(m.code) == false)
                {
                    long id_new = 0;
                    bool c = true;
                    while (c == true)
                    {
                        id_new = DateTime.Now.ToString("yyyyMMddHHmmssfff").TryParseToLong() + new Random().Next(10, 99);
                        c = dic_node.ContainsKey(id_new);
                    }

                    int phase_type = o.id_group;
                    long id_parent = o.id_parent;

                    m.type = o.type;

                    m.id = id_new;
                    m.id_group = phase_type;
                    m.id_parent = id_parent;

                    m.id_root0 = o.id_root0;
                    m.id_root1 = o.id_root1;

                    m.level = o.level;
                    m.name = o.name;
                    m.code = o.code;

                    lock (lock_node)
                    {
                        dic_node_code.Add(m.code, id_new);
                        dic_node.Add(id_new, m);
                        cache_node_id();
                    }
                    update_node(id_new, m);

                    lock (lock_ID_node)
                        dic_ID_node.AddDistinct(id_parent, id_new);

                    List<long> ls = new List<long>() { };
                    if (dic_ID_node.TryGetValue(id_parent, out ls) == false) ls = new List<long>() { };
                    update_ID_node(id_parent, ls);

                    Task.Factory.StartNew(() => { cache_node_set(id_parent); });

                    json = JsonConvert.SerializeObject(m);

                    List<long> ls_parent = new List<long>() { };
                    if (dic_ID_parent.TryGetValue(id_parent, out ls_parent) == false) ls_parent = new List<long>() { };
                    ls_parent.Add(id_parent);
                    lock (lock_ID_parent)
                        dic_ID_parent.AddListDistinct(m.id, ls_parent);
                    update_ID_parent(m.id, ls_parent);
                }
                else
                {
                    json = "trung_ma";
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

                m_node m = JsonConvert.DeserializeObject<m_node>(s_item);

                if (dic_node.ContainsKey(m.id) == true && m.id != 0)
                {
                    m_node o = new m_node();
                    dic_node.TryGetValue(m.id, out o);

                    if (o.name != m.name || o.type != m.type)
                    {
                        o.name = m.name;
                        o.type = m.type;
                        if (dic_node.ContainsKey(o.id))
                        {
                            lock (lock_node)
                                dic_node[o.id] = o;

                            update_node(o.id, o);
                            Task.Factory.StartNew(() => { cache_node_set(o.id_parent); });
                        }
                    }
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
                string json = "";
                long id_ = s_item.TryParseToLong();
                if (id_ > 0)
                {
                    m_node m = new m_node();
                    if (dic_node.TryGetValue(id_, out m) == true)
                    {
                        json = JsonConvert.SerializeObject(m);

                        lock (lock_ID_device)
                        {
                            if (dic_ID_device.ContainsKey(id_) == true)
                            {
                                if (dic_ID_device.ContainsKey(id_))
                                    dic_ID_device.Remove(id_);

                                string f = path_ID_device + @"\" + id_.ToString() + ".mmf";
                                f = f.Replace(@"\\", @"\");
                                if (File.Exists(f))
                                    File.Delete(f);
                            }
                        }

                        lock (lock_ID_node)
                        {
                            List<long> ls = new List<long>() { };
                            dic_ID_node.TryGetValue(m.id_parent, out ls);
                            if (ls.IndexOf(id_) != -1)
                            {
                                ls.Remove(id_);
                                update_ID_node(m.id_parent, ls);
                            }
                        }

                        lock (lock_node)
                        {
                            if (dic_node_code.ContainsKey(m.code)) dic_node_code.Remove(m.code);
                            if (dic_node.ContainsKey(id_)) dic_node.Remove(id_);
                            cache_node_id();
                        }
                        string fo = path_node + @"\" + id_.ToString() + ".mmf";
                        fo = fo.Replace(@"\\", @"\");
                        if (File.Exists(fo))
                            File.Delete(fo);

                        lock (lock_ID_parent)
                        {
                            if (dic_ID_device.ContainsKey(id_) == true)
                            {
                                if (dic_ID_parent.ContainsKey(id_))
                                    dic_ID_parent.Remove(id_);

                                string f = path_ID_device + @"\" + id_.ToString() + ".mmf";
                                f = f.Replace(@"\\", @"\");
                                if (File.Exists(f))
                                    File.Delete(f);
                            }

                            var ls_parent = dic_ID_parent.Where(x => x.Value.IndexOf(id_) != -1).Select(x => x).ToList();
                            foreach (var ke in ls_parent)
                            {
                                List<long> ls = ke.Value;
                                ls.Remove(id_);
                                dic_ID_parent[ke.Key] = ls;
                                update_ID_parent(ke.Key, ls);

                                cache_node_set(ke.Key);
                            }

                        }

                        cache_node_id();
                    }
                }
                return new Tuple<bool, string, dynamic>(true, json, null);
            }
            catch { }
            return new Tuple<bool, string, dynamic>(false, "", null);
        }

        #endregion

        #region /// cache ...

        private static void cache_node_id()
        {
            string data = string.Join("|", dic_node.Keys.ToArray());

            ObjectCache cache = MemoryCache.Default;
            CacheItemPolicy policy = new CacheItemPolicy();
            policy.Priority = System.Runtime.Caching.CacheItemPriority.Default;
            policy.AbsoluteExpiration = DateTimeOffset.Now.AddDays(30);
            //Muốn tạo sự kiện thì tạo ở đây còn k thì hoy
            //(MyCacheItemPriority == MyCachePriority.Default) ? CacheItemPriority.Default : CacheItemPriority.NotRemovable;
            ///Globals.policy.RemovedCallback = callback;             
            cache.Set("node", data, policy, null);
        }

        public static string cache_node_get(long id)
        {
            return cache_node_set(id);

            //ObjectCache cache = MemoryCache.Default;
            //string data = cache["node." + id.ToString()] as string;
            //if (data == null)
            //{
            //    return cache_node_set(id);
            //}
            //else
            //    return data;
        }

        public static string cache_node_set(long id)
        {
            List<long> ids_node = new List<long>() { };
            List<long> ids_device = new List<long>() { };

            string data = "", key = "node." + id.ToString();
            if (dic_ID_node.TryGetValue(id, out ids_node) || dic_ID_device.TryGetValue(id, out ids_device))
            {
                if (ids_node == null) ids_node = new List<long>() { };
                if (ids_device == null) ids_device = new List<long>() { };
                if (ids_node.Count > 0 || ids_device.Count > 0)
                {
                    ids_node = ids_node.Where(x => x != id).ToList();
                    ids_device = ids_device.Where(x => x != id).ToList();

                    if (ids_node.Count > 0)
                    {
                        for (int k = 0; k < ids_node.Count; k++)
                        {
                            long id_ = ids_node[k];
                            m_node o = new m_node();
                            if (dic_node.TryGetValue(id_, out o))
                            {
                                string len_sub_node = "|0";
                                List<long> ls_sub_node = new List<long>() { };
                                if (dic_ID_node.TryGetValue(id_, out ls_sub_node))
                                    len_sub_node = "|" + ls_sub_node.Where(x => x != id_).Distinct().Count().ToString();

                                string len_sub_device = "|0";
                                List<long> ls_sub_device = new List<long>() { };
                                if (dic_ID_device.TryGetValue(id_, out ls_sub_device))
                                {
                                    int leni = ls_sub_device.Where(x => x != 0 && x != id_).Distinct().Count();
                                    len_sub_device = "|" + leni.ToString();
                                }

                                if (data == "")
                                    data = o.type.ToString() + "|1|" + o.id + "|" + o.name + len_sub_node + len_sub_device + Environment.NewLine;
                                else
                                    data += "@" + o.type.ToString() + "|1|" + o.id + "|" + o.name + len_sub_node + len_sub_device + Environment.NewLine;
                            }
                        }
                    }

                    if (ids_device.Count > 0)
                    {
                        var ls = db_meter.get_Items(ids_device.ToArray());
                        for (int k = 0; k < ls.Length; k++)
                        {
                            var o = ls[k];
                            if (data == "")
                                data = m_type_device.ToString() + "|1|" + o.meter_id + "|" + o.name + "|0|0" + Environment.NewLine;
                            else
                                data += "@" + m_type_device.ToString() + "|1|" + o.meter_id + "|" + o.name + "|0|0" + Environment.NewLine;
                        }
                    }
                }

                ObjectCache cache = MemoryCache.Default;
                CacheItemPolicy policy = new CacheItemPolicy();
                policy.Priority = System.Runtime.Caching.CacheItemPriority.Default;
                policy.AbsoluteExpiration = DateTimeOffset.Now.AddDays(3);
                //Muốn tạo sự kiện thì tạo ở đây còn k thì hoy
                //(MyCacheItemPriority == MyCachePriority.Default) ? CacheItemPriority.Default : CacheItemPriority.NotRemovable;
                ///Globals.policy.RemovedCallback = callback;             
                cache.Set(key, data, policy, null);
            }

            return data;
        }

        #endregion

        #region // where, where_index ...

        public static Tuple<bool, string, dynamic[]> get_id(string s_key)
        {
            long id = s_key.TryParseToLong();
            m_node o = new m_node();
            if (dic_node.TryGetValue(id, out o))
            {
                m_node p = new m_node();
                if (o.id_parent != 0 && dic_node.TryGetValue(o.id_parent, out p))
                    return new Tuple<bool, string, dynamic[]>(true, "", new dynamic[] { o, p });
                else
                    return new Tuple<bool, string, dynamic[]>(true, "", new dynamic[] { o });
            }

            return new Tuple<bool, string, dynamic[]>(false, "", null);
        }

        public static Tuple<bool, string, int, int, IList> where_call_dynamic(string s_key, string s_select, string s_where, string s_order_by, string s_distinct, int page_number, int page_size)
        {
            return dbQuery.where<m_node>(dic_node.Values.ToArray(), s_key, s_select, s_where, s_order_by, s_distinct, page_number, page_size);
        }

        public static Tuple<bool, string, int, int, IList> where_index_call_dynamic(string s_index, string s_select, string s_where, string s_order_by, string s_distinct, int page_number, int page_size)
        {
            int rs_total = dic_node.Count,
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
                        if (dic_ID_device.TryGetValue(key, out li) == false) li = new List<long>() { };
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

        #endregion

        #region // join device, unjoin device to Node ...

        public static long[] get_IDs_node(long id_parent)
        {
            List<long> ls = new List<long>() { };
            if (dic_ID_node.TryGetValue(id_parent, out ls) == false) ls = new List<long>() { };
            return ls.ToArray();
        }

        public static long[] join_device_get(long id_key)
        {
            List<long> ls = new List<long>() { };
            if (dic_ID_device.TryGetValue(id_key, out ls) == false) ls = new List<long>() { };
            return ls.ToArray();
        }

        public static long[] get_IDs_device(long id_parent)
        {
            List<long> ls = new List<long>() { };
            if (dic_ID_device.TryGetValue(id_parent, out ls) == false) ls = new List<long>() { };
            return ls.ToArray();
        }

        public static int join_device_add(long id_parent, long id_key, long[] vals)
        {
            vals = vals.Where(x => x > 1000000).ToArray();
            List<long> ls = new List<long>() { };
            if (vals.Length > 0)
            {

                lock (lock_ID_device)
                    ls = dic_ID_device.AddListDistinctItem(id_key, vals);
                update_ID_device(id_key, ls);
                cache_node_set(id_key);

                List<long> ls_parent = new List<long>() { };
                var dp = dic_ID_parent
                    .Where(x => x.Value.IndexOf(id_parent) != -1)
                    .Select(x => x.Value.ToArray()).ToArray();
                foreach (var ids in dp)
                    ls_parent.AddRange(ids);
                ls_parent = ls_parent.Where(x => x != id_key).Distinct().ToArray().Reverse().ToList();
                foreach (long id in ls_parent)
                {
                    if (id > 0)
                    {
                        List<long> lsi = new List<long>() { };

                        lock (lock_ID_device)
                            lsi = dic_ID_device.AddListDistinctItem(id, ls.ToArray());
                        lsi = lsi.Where(x => x > 1000000).ToList();

                        update_ID_device(id, lsi);
                    }
                }

                foreach (long id in ls_parent)
                    cache_node_set(id);

                dbCache.clear(typeof(m_meter).FullName);
            }

            return ls.Count;
        }

        public static int join_device_remove(long id_parent, long id_key, long[] vals)
        {
            int k = 0;

            List<long> ls = new List<long>() { };
            if (dic_ID_device.TryGetValue(id_key, out ls))
            {
                var lso = ls.Where(x => !vals.Any(o => o == x)).ToList();
                if (lso.Count != vals.Length)
                {
                    lock (lock_ID_device)
                        dic_ID_device[id_key] = lso;
                    update_ID_device(id_key, lso);
                    cache_node_set(id_key);

                    List<long> ls_parent = new List<long>() { };
                    var dp = dic_ID_parent
                        .Where(x => x.Value.IndexOf(id_parent) != -1)
                        .Select(x => x.Value.ToArray()).ToArray();
                    foreach (var ids in dp)
                        ls_parent.AddRange(ids);
                    ls_parent = ls_parent.Where(x => x != 0 && x != id_key).Distinct().ToList();
                    foreach (long id in ls_parent)
                    {
                        List<long> lsi = new List<long>() { };
                        if (dic_ID_device.TryGetValue(id, out lsi))
                        {
                            var lxi = lsi.Where(x => !vals.Any(o => o == x)).ToList();
                            lock (lock_ID_device)
                                dic_ID_device[id] = lxi;

                            update_ID_device(id, lxi);
                            cache_node_set(id);
                        }
                    }
                    dbCache.clear(typeof(m_meter).FullName);
                }
            }

            return k;
        }


        #endregion
    }
}
