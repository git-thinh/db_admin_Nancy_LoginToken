using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Newtonsoft.Json;
using System.IO;
using System.IO.MemoryMappedFiles;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Globalization;
using System.Linq.Dynamic;
using System.Runtime.Caching;
using System.Collections;
using model;
using System.Reflection;

namespace host
{
    public enum e_query_data
    {
        data_detail,    // = pha + data_type + bieu + tech + nsx
        data_type       // = 100k: TSVH | 200K: TSTT | 300k: Load profile | ...
    }

    public class cache_tsvh
    {
        public List<Tuple<int, double[]>> ls = new List<Tuple<int, double[]>>() { };
        public string key_cache { set; get; }
    }

    public class store
    {
        // [0] data_type, [1] file_id, [2] dcu_id, [3] device_id
        // [4] yyyyMMdd, [5] HHmmss
        // [6] phase: 1pha, [7] data, [8] bieu, [9] tech, [10] factory 
        //===========================================================================

        public static string[] cols_index = new string[] {
            "data_type", "file_id", "dcu_id", "device_id",
            "yyyyMMdd", "HHmmss",
            "phase", "data", "tech", "factory"
        };

        #region // Var ...

        private static object lock_db_where = new object();
        private static Dictionary<string, Func<string, string, string, string, string, int, int, Tuple<bool, string, int, int, IList>>>
            dic_db_where = new Dictionary<string, Func<string, string, string, string, string, int, int, Tuple<bool, string, int, int, IList>>>() { };

        private static object lock_db_action = new object();
        private static Dictionary<string, Func<string, string, Tuple<bool, string, dynamic>>> dic_db_action =
            new Dictionary<string, Func<string, string, Tuple<bool, string, dynamic>>>() { };

        private static object lock_db_get_id = new object();
        private static Dictionary<string, Func<string, Tuple<bool, string, dynamic[]>>> dic_db_get_id =
            new Dictionary<string, Func<string, Tuple<bool, string, dynamic[]>>>() { };

        private static object lock_device = new object();
        private static List<long> list_device = new List<long>() { };

        private static int max_size = 366;//new DateTime(2015, 12, 31).DayOfYear;

        //< meter_id, 0|1|2 : 0 ko dung he so, 1: dung so ngoai, 2: dung ca 2 loai he so trong va ngoai >
        private static DictionaryList<long, double> dic_HeSoNgoai = new DictionaryList<long, double>() { };
        private static object lock_hs = new object();

        //< meter_id, data_type, data_type_detail, time(yyMMddHHmm), yy >
        private static DictionaryList<Tuple<long, int, int, UInt32, byte>, double>[] dic_DB = new DictionaryList<Tuple<long, int, int, UInt32, byte>, double>[max_size];
        private static object lock_db = new object();
        private static DictionaryList<Tuple<long, int, int, int>, double> dic_DB_error = new DictionaryList<Tuple<long, int, int, int>, double>() { };

        #endregion

        public static void init()
        {
            for (int i = 0; i < max_size; i++)
                dic_DB[i] = new DictionaryList<Tuple<long, int, int, UInt32, byte>, double>() { };

            blue_update_load_INIT();
        }



        #region // ... query ...

        public static Tuple<long, int, string> f_db_query_item(
            string key_cache,
            int data_type,
            int[] date_index, long item_id, int page_number, int page_size)
        {
            date_index = date_index.OrderByDescending(x => x).ToArray();
            string json = "";
            long rs_count_all = dic_DB.Select(k => k.Count).Sum();
            int rs_count = 0;

            List<double[]> ls_cache = new List<double[]>() { };

            ////ObjectCache cache = MemoryCache.Default;
            ////if (cache[key_cache] == null)
            ////{

            for (int k = 0; k < date_index.Length; k++)
            {
                int id = date_index[k];
                //int date = msgConverter.f_DayOfYearToDate_yyMMdd(id);
                var ls0 = dic_DB[id]
                    .Where(d => d.Key.Item1 == item_id && d.Key.Item2 == data_type)
                    .Select(x => x.Value.ToArray())
                    .ToArray();

                if (ls0.Length > 0)
                {
                    ls_cache.AddRange(ls0);
                }
            }

            ////    Task.Factory.StartNew((object obj) =>
            ////    {
            ////        cache_tsvh data = obj as cache_tsvh;

            ////        ObjectCache ca = MemoryCache.Default;
            ////        CacheItemPolicy policy = new CacheItemPolicy();
            ////        policy.Priority = System.Runtime.Caching.CacheItemPriority.Default;
            ////        policy.AbsoluteExpiration = DateTimeOffset.Now.AddMinutes(30);

            ////        //Muốn tạo sự kiện thì tạo ở đây còn k thì hoy
            ////        //(MyCacheItemPriority == MyCachePriority.Default) ? CacheItemPriority.Default : CacheItemPriority.NotRemovable;
            ////        ///Globals.policy.RemovedCallback = callback; 

            ////        ca.Set(data.key_cache, data.ls, policy, null);

            ////        //    object test = objCache.Get(KeyRequest, null);
            ////    }, new cache_tsvh() { key_cache = key_cache, ls = ls });
            ////}
            ////else
            ////{
            ////    var data = cache[key_cache] as cache_tsvh;

            ////}

            if (ls_cache.Count > 0)
            {
                var l0 = ls_cache.Select(x => new Tuple<int, int, double[]>((int)x[2], (int)x[3], x)).ToList();
                ls_cache = l0
                    //.GroupBy(x => new { x.Item1, x.Item2 })
                    //.Select(x => x.First().Item3)
                    .Select(x => x.Item3)
                    .ToList();

                if (page_size > ls_cache.Count) page_size = ls_cache.Count;

                int startRowIndex = page_size * (page_number - 1);
                var dt_pager = ls_cache.Skip(startRowIndex).Take(page_size).ToList();

                json = JsonConvert.SerializeObject(dt_pager);
                rs_count = ls_cache.Count;
            }

            if (json != "")
                json = json.Replace(@"""Key"":", @"""").Replace(@",""Value"":[", @""":[").Replace(@".0""", @"""");

            return new Tuple<long, int, string>(rs_count_all, rs_count, json);
        }


        public static Tuple<long, long, long[], string> f_db_query_group(
            string key_cache,
            int data_type,
            int date_id, long[] meter_a,
            int page_number, int page_size)
        {
            string json = "";
            long rs_count_all = dic_DB.Select(k => k.Count).Sum();
            long rs_count = 0;

            ////ObjectCache cache = MemoryCache.Default;
            ////if (cache[key_cache] == null)
            ////{

            List<Tuple<long, double, double[]>> dt = new List<Tuple<long, double, double[]>>() { };
            long[] meter_result = new long[] { };


            #region // ...

            if (meter_a.Length == 0)
            {
                dt = dic_DB[date_id]
                    .Where(d => d.Key.Item2 == data_type)
                    .Select(x => new Tuple<long, double, double[]>(x.Key.Item1, x.Key.Item3, x.Value.ToArray()))
                    .OrderByDescending(x => x.Item2).ToList();
            }
            else
            {
                dt = dic_DB[date_id].Where(d =>
                        d.Key.Item2 == data_type
                        && meter_a.Any(o => o == d.Key.Item1))
                    .Select(x => new Tuple<long, double, double[]>(x.Key.Item1, x.Key.Item3, x.Value.ToArray()))
                    .OrderByDescending(x => x.Item2).ToList();
            }

            #endregion

            if (dt.Count > 0)
            {
                var dto = dt
                    //.GroupBy(x => x.Item1)
                    //.Select(x => x.First())
                    .Select(x => new Tuple<double, double[]>(x.Item2, x.Item3))
                    .ToList();

                rs_count = dto.Count;

                int startRowIndex = page_size * (page_number - 1);
                var dt_pager = dto.Skip(startRowIndex).Take(page_size).ToList();
                meter_result = dt_pager.Select(x => (long)x.Item1).ToArray();

                json = JsonConvert.SerializeObject(dt_pager);
            }


            ////    Task.Factory.StartNew((object obj) =>
            ////    {
            ////        cache_tsvh data = obj as cache_tsvh;

            ////        ObjectCache ca = MemoryCache.Default;
            ////        CacheItemPolicy policy = new CacheItemPolicy();
            ////        policy.Priority = System.Runtime.Caching.CacheItemPriority.Default;
            ////        policy.AbsoluteExpiration = DateTimeOffset.Now.AddMinutes(30);

            ////        //Muốn tạo sự kiện thì tạo ở đây còn k thì hoy
            ////        //(MyCacheItemPriority == MyCachePriority.Default) ? CacheItemPriority.Default : CacheItemPriority.NotRemovable;
            ////        ///Globals.policy.RemovedCallback = callback; 

            ////        ca.Set(data.key_cache, data.ls, policy, null);

            ////        //    object test = objCache.Get(KeyRequest, null);
            ////    }, new cache_tsvh() { key_cache = key_cache, ls = ls });
            ////}
            ////else
            ////{
            ////    var data = cache[key_cache] as cache_tsvh;

            ////}

            if (json != "")
                json = json.Replace(@"""Item1"":", @"""").Replace(@",""Item2"":[", @""":[").Replace(@".0""", @"""");

            return new Tuple<long, long, long[], string>(rs_count_all, rs_count, meter_result, json);
        }

        #endregion


        #region // ... data: HeSoNhan ...

        public static void add_HeSoNhan(int level, long mid, List<double> vals)
        {
            //List<double> vals = new List<double>(){};
            //dic_HeSoNgoai.TryGetValue(mid, out vals);
            //double[][] list = dic_HeSoNgoai.Values.Select(x=>x.ToArray()).ToArray();            
            //key, item = {meter_id, ap_dung(0|1|2), yyMMdd, HHmmss, |> he_so_1,he_so_2,he_so_3, ... }  

            int HHmmss = DateTime.Now.ToString("HHmmss").TryParseToInt();
            int yyMMdd = DateTime.Now.ToString("yyMMdd").TryParseToInt();

            vals.Insert(0, HHmmss);
            vals.Insert(0, yyMMdd);
            vals.Insert(0, level);
            vals.Insert(0, mid);

            lock (lock_hs)
            {
                if (dic_HeSoNgoai.ContainsKey(mid) == false)
                    dic_HeSoNgoai.Add(mid, vals);
                else
                    dic_HeSoNgoai[mid] = vals;
            }

            update_HeSoNhan(mid, vals);
        }

        public static void update_HeSoNhan(long mid, List<double> vals)
        {
            //List<double> vals = new List<double>(){};
            //dic_HeSoNgoai.TryGetValue(mid, out vals);
            //double[][] list = dic_HeSoNgoai.Values.Select(x=>x.ToArray()).ToArray();            

            string path = hostServer.pathRoot + "store_heso";
            string file_name = mid.ToString();
            //key, item = {meter_id, ap_dung(0|1|2), yyMMdd, HHmmss, |> he_so_1,he_so_2,he_so_3, ... }            
            hostFile.write_file_MMF<double>(vals, path, file_name);
        }

        public static void load_HeSoNhan()
        {
            string path = hostServer.pathRoot + "store_heso";
            if (Directory.Exists(path))
            {
                var fs = Directory.GetFiles(path, "*.mmf");
                if (fs.Length > 0)
                {
                    foreach (var fi in fs)
                    {
                        try
                        {
                            double[] li = hostFile.read_file_MMF<double>(fi);
                            if (li.Length > 0)
                            {
                                long mid = (long)li[0];
                                if (dic_HeSoNgoai.ContainsKey(mid) == false)
                                    dic_HeSoNgoai.AddList(mid, li.ToArray());
                            }
                        }
                        catch { }
                    }
                }
            }
        }

        public static string query_heso(long[] m_ids)
        {
            string json = "";

            List<double[]> ls = new List<double[]>() { };
            for (int k = 0; k < m_ids.Length; k++)
            {
                List<double> li = new List<double>() { };
                if (dic_HeSoNgoai.TryGetValue(m_ids[k], out li))
                    ls.Add(li.ToArray());
            }

            json = JsonConvert.SerializeObject(ls);

            return json;
        }

        #endregion

        #region // ... data: bluetooth ....

        public static void blue_update_add(m_blue_data m)
        {
            device_update(m.device_id);

            double[] vals = new double[] { 
                (int)m.data_type + (int)d4_tech.BLUETOOTH, // [0] data_type_detail
                (int)m.data_type,                          // [1] data_type
                0,                                         // [2] dcu_id
                m.device_id,                               // [3] device_id
                m.yyMMdd,                                  // [4] yyMMdd
                m.HHmmss,                                  // [5] HHmmss
                0, (int)m.data_type, 0,                    // [6] phase, [7] data, [8] bieu
                (int)d4_tech.BLUETOOTH,                    // [9] tech, [10] factory
                m.yyMMdd,                                  //     date_meter
                m.HHmmss,                                  //     time_meter
                m.value                                    //     p_giao_tong
            };

            Tuple<long, int, int, UInt32, byte> key = new Tuple<long, int, int, UInt32, byte>(
                m.device_id,                                      // device_id
                (int)m.data_type,                                 // data_type
                (int)m.data_type + (int)d4_tech.BLUETOOTH,        // data_type_detail
                m.yyMMddHHmm,                                     // yyMMddHHmm
                m.yy);                                            // yy

            lock (lock_db)
            {
                if (dic_DB[m.index_date].ContainsKey(key))
                    dic_DB[m.index_date][key] = vals.ToList();
                else
                    dic_DB[m.index_date].Add(key, vals.ToList());
            }
        }

        public static void blue_update_tsvh(long id, double val)
        {
            device_update(id);

            int yyMMdd = DateTime.Now.ToString("yyMMdd").TryParseToInt();
            int HHmmss = DateTime.Now.ToString("HHmmss").TryParseToInt();
            UInt32 yyMMddHHmm = DateTime.Now.ToString("yyMMddHHmm").TryParseToUInt32();

            byte yy = DateTime.Now.ToString("yy").TryParseToByte();

            int index = DateTime.Now.DayOfYear; if (index > 0) index--;

            blue_update_add(new m_blue_data(
                d2_data.TSVH,
                id, val,
                index, yy,
                yyMMdd, HHmmss, yyMMddHHmm));
        }

        public static void blue_update_fday(long id, double val)
        {
            device_update(id);

            int yyMMdd = DateTime.Now.ToString("yyMMdd").TryParseToInt();
            int HHmmss = DateTime.Now.ToString("HHmmss").TryParseToInt();
            UInt32 yyMMddHHmm = DateTime.Now.ToString("yyMMddHHmm").TryParseToUInt32();

            byte yy = DateTime.Now.ToString("yy").TryParseToByte();

            int index = DateTime.Now.DayOfYear;
            if (index > 0) index--;

            blue_update_add(new m_blue_data(
                d2_data.FIX_DAY,
                id, val,
                index, yy,
                yyMMdd, HHmmss, yyMMddHHmm));
        }

        public static void blue_update_fmon(long id, double val)
        {
            device_update(id);

            int yyMMdd = DateTime.Now.ToString("yyMMdd").TryParseToInt();
            int HHmmss = DateTime.Now.ToString("HHmmss").TryParseToInt();
            UInt32 yyMMddHHmm = DateTime.Now.ToString("yyMMddHHmm").TryParseToUInt32();

            byte yy = DateTime.Now.ToString("yy").TryParseToByte();

            int index = DateTime.Now.DayOfYear; if (index > 0) index--;

            blue_update_add(new m_blue_data(
                d2_data.FIX_MONTH,
                id, val,
                index, yy,
                yyMMdd, HHmmss, yyMMddHHmm));
        }

        public static void blue_update_load_INIT()
        {
            //100000000024-20160112-021303-73.88
            List<long> li_device = new List<long>() { };
            List<m_blue_data> dt = new List<m_blue_data>() { };

            #region // TSVH ...

            if (Directory.Exists("blue_update"))
            {
                var ars = Directory.GetFiles("blue_update", "*.txt")
                    .Select(x => x.Replace(@".txt", "")
                        .Replace(@"blue_update\", "").Trim().Replace(@"tstt-", "")
                        )
                        .Where(x => x.Split('-').Length == 4)
                    .ToArray();
                if (ars.Length > 0)
                {
                    List<m_blue_data> li = new List<m_blue_data>() { };
                    foreach (var fi in ars)
                    {
                        try
                        {
                            string[] a = fi.Split('-');
                            string s_val = a[a.Length - 1];

                            m_blue_data m = new m_blue_data();
                            m.data_type = d2_data.TSVH;

                            m.device_id = a[0].Trim().TryParseToLong();
                            m.HHmmss = a[2].TryParseToInt();
                            m.yyMMdd = a[1].Substring(2, 6).TryParseToInt();
                            m.index_date = m.yyMMdd.IntDateToDayOfYear();
                            m.yyMMddHHmm = (a[1] + a[2]).Substring(2, 10).Trim().TryParseToUInt32();
                            m.yy = a[1].Substring(2, 2).TryParseToByte();
                            m.value = s_val.TryParseToDouble();

                            li_device.Add(m.device_id);
                            li.Add(m);
                        }
                        catch { }
                    }//end for
                    dt.AddRange(li);
                }
            }

            if (Directory.Exists("blue_sync_tstt"))
            {
                var ars = Directory.GetFiles("blue_sync_tstt", "*.txt")
                    .Select(x => x.Replace(@".txt", "")
                        .Replace(@"blue_sync_tstt\", "").Trim().Replace(@"tstt-", ""))
                        .Where(x => x.Split('-').Length == 4)
                    .ToArray();
                if (ars.Length > 0)
                {
                    List<m_blue_data> li = new List<m_blue_data>() { };
                    foreach (var fi in ars)
                    {
                        try
                        {
                            string[] a = fi.Split('-');
                            string s_val = a[a.Length - 1];

                            m_blue_data m = new m_blue_data();
                            m.data_type = d2_data.TSVH;

                            m.device_id = a[0].Trim().TryParseToLong();
                            m.HHmmss = a[2].TryParseToInt();
                            m.yyMMdd = a[1].Substring(2, 6).TryParseToInt();
                            m.index_date = m.yyMMdd.IntDateToDayOfYear();
                            m.yyMMddHHmm = (a[1] + a[2]).Substring(2, 10).Trim().TryParseToUInt32();
                            m.yy = a[1].Substring(2, 2).TryParseToByte();
                            m.value = s_val.TryParseToDouble();

                            li_device.Add(m.device_id);
                            li.Add(m);
                        }
                        catch { }
                    }//end for
                    dt.AddRange(li);
                }
            }

            #endregion

            #region // FDAY ...

            if (Directory.Exists("blue_update_fday"))
            {
                var ars = Directory.GetFiles("blue_update_fday", "*.txt")
                    .Select(x => x.Replace(@".txt", "")
                        .Replace(@"blue_update_fday\", "").Trim().Replace(@"fday-", ""))
                        .Where(x => x.Split('-').Length == 4)
                    .ToArray();
                if (ars.Length > 0)
                {
                    List<m_blue_data> li = new List<m_blue_data>() { };
                    foreach (var fi in ars)
                    {
                        try
                        {
                            string[] a = fi.Split('-');
                            string s_val = a[a.Length - 1];

                            m_blue_data m = new m_blue_data();
                            m.data_type = d2_data.FIX_DAY;

                            m.device_id = a[0].Trim().TryParseToLong();
                            m.HHmmss = a[2].TryParseToInt();
                            m.yyMMdd = a[1].Substring(2, 6).TryParseToInt();
                            m.index_date = m.yyMMdd.IntDateToDayOfYear();
                            m.yyMMddHHmm = (a[1] + a[2]).Substring(2, 10).Trim().TryParseToUInt32();
                            m.yy = a[1].Substring(2, 2).TryParseToByte();
                            m.value = s_val.TryParseToDouble();

                            li_device.Add(m.device_id);
                            li.Add(m);
                        }
                        catch(Exception ex) { string msg = ex.Message; }
                    }//end for
                    dt.AddRange(li);
                }
            }

            if (Directory.Exists("blue_sync_fday"))
            {
                var ars = Directory.GetFiles("blue_sync_fday", "*.txt")
                    .Select(x => x.Replace(@".txt", "")
                        .Replace(@"blue_sync_fday\", "").Trim().Replace(@"fday-", ""))
                        .Where(x => x.Split('-').Length == 4)
                    .ToArray();
                if (ars.Length > 0)
                {
                    List<m_blue_data> li = new List<m_blue_data>() { };
                    foreach (var fi in ars)
                    {
                        try
                        {
                            string[] a = fi.Split('-');
                            string s_val = a[a.Length - 1];

                            m_blue_data m = new m_blue_data();
                            m.data_type = d2_data.FIX_DAY;

                            m.device_id = a[0].Trim().TryParseToLong();
                            m.HHmmss = a[2].TryParseToInt();
                            m.yyMMdd = a[1].Substring(2, 6).TryParseToInt();
                            m.index_date = m.yyMMdd.IntDateToDayOfYear();
                            m.yyMMddHHmm = (a[1] + a[2]).Substring(2, 10).Trim().TryParseToUInt32();
                            m.yy = a[1].Substring(2, 2).TryParseToByte();
                            m.value = s_val.TryParseToDouble();

                            li_device.Add(m.device_id);
                            li.Add(m);
                        }
                        catch { }
                    }//end for
                    dt.AddRange(li);
                    
                }
            }
            //string meter = "";
            //for (int i = 0; i < dt.Count; i++)
            //{
            //    meter = meter + dt[i].device_id.ToString() + "\n";
            //}
            #endregion

            #region // FMON ...

            if (Directory.Exists("blue_update_fmon"))
            {
                var ars = Directory.GetFiles("blue_update_fmon", "*.txt")
                    .Select(x => x.Replace(@".txt", "")
                        .Replace(@"blue_update_fmon\", "").Trim().Replace(@"fmon-", ""))
                        .Where(x => x.Split('-').Length == 4)
                    .ToArray();
                if (ars.Length > 0)
                {
                    List<m_blue_data> li = new List<m_blue_data>() { };
                    foreach (var fi in ars)
                    {
                        try
                        {
                            string[] a = fi.Split('-');
                            string s_val = a[a.Length - 1];

                            m_blue_data m = new m_blue_data();
                            m.data_type = d2_data.FIX_MONTH;

                            m.device_id = a[0].Trim().TryParseToLong();
                            m.HHmmss = a[2].TryParseToInt();
                            m.yyMMdd = a[1].Substring(2, 6).TryParseToInt();
                            m.index_date = m.yyMMdd.IntDateToDayOfYear();
                            m.yyMMddHHmm = (a[1] + a[2]).Substring(2, 10).Trim().TryParseToUInt32();
                            m.yy = a[1].Substring(2, 2).TryParseToByte();
                            m.value = s_val.TryParseToDouble();

                            li_device.Add(m.device_id);
                            li.Add(m);
                        }
                        catch { }
                    }//end for
                    dt.AddRange(li);

                }
            }

            if (Directory.Exists("blue_sync_fmon"))
            {
                var ars = Directory.GetFiles("blue_sync_fmon", "*.txt")
                    .Select(x => x.Replace(@".txt", "")
                        .Replace(@"blue_sync_fmon\", "").Trim().Replace(@"fmon-", ""))
                        .Where(x => x.Split('-').Length == 4)
                    .ToArray();
                if (ars.Length > 0)
                {
                    List<m_blue_data> li = new List<m_blue_data>() { };
                    foreach (var fi in ars)
                    {
                        try
                        {
                            string[] a = fi.Split('-');
                            string s_val = a[a.Length - 1];

                            m_blue_data m = new m_blue_data();
                            m.data_type = d2_data.FIX_MONTH;

                            m.device_id = a[0].Trim().TryParseToLong();
                            m.HHmmss = a[2].TryParseToInt();
                            m.yyMMdd = a[1].Substring(2, 6).TryParseToInt();
                            m.index_date = m.yyMMdd.IntDateToDayOfYear();
                            m.yyMMddHHmm = (a[1] + a[2]).Substring(2, 10).Trim().TryParseToUInt32();
                            m.yy = a[1].Substring(2, 2).TryParseToByte();
                            m.value = s_val.TryParseToDouble();

                            li_device.Add(m.device_id);
                            li.Add(m);
                        }
                        catch { }
                    }//end for
                    dt.AddRange(li);
                }
            }
            #endregion


            if (dt.Count > 0)
            {
                dt = dt.Distinct().ToList();

                lock (lock_db)
                {
                    foreach (var m in dt)
                        blue_update_add(m);
                }
            }

            device_addItems(li_device);
        }

        public static void device_addItems(List<long> li_device)
        {        
            if (li_device.Count > 0)
            {
                li_device = li_device.Distinct().ToList();
                device_update(li_device.ToArray());

                db_meter.add_Items(li_device.ToArray());
            }
        }

        #endregion


        #region // ... data: tsvh, tstt, fday, fmon ...


        private delegate Tuple<bool, int, int, string> where(
            string s_select, string s_where, string s_order_by, string s_distinct,
            int page_number, int page_size);


        public static Tuple<bool, string, int, int, IList> db_where_index(
            string s_index, string s_tab, string s_select, string s_where, string s_order_by, string s_distinct,
            int page_number, int page_size)
        {
            string key = "index." + s_tab;
            Func<string, string, string, string, string, int, int, Tuple<bool, string, int, int, IList>> fun = null;
            dic_db_where.TryGetValue(key, out fun);

            if (fun == null)
            {
                Type ti = Type.GetType("host." + s_tab + ", host");
                if (ti != null)
                {
                    try
                    {
                        MethodInfo fu = ti.GetMethod("where_index_call_dynamic", BindingFlags.Static | BindingFlags.Public);
                        if (fu != null)
                        {
                            fun = (Func<string, string, string, string, string, int, int, Tuple<bool, string, int, int, IList>>)
                                Delegate.CreateDelegate(typeof(
                                Func<string, string, string, string, string, int, int,
                                Tuple<bool, string, int, int, IList>>), fu);
                            lock (lock_db_where)
                                if (dic_db_where.ContainsKey(key) == false) dic_db_where.Add(key, fun);
                        }
                    }
                    catch (Exception ex)
                    {
                        string s_error = ex.Message;
                    }
                }
            }

            if (fun != null)
            {
                try
                {
                    var rs = fun(s_index, s_select, s_where, s_order_by, s_distinct, page_number, page_size);
                    if (rs != null)
                        return rs;
                }
                catch (Exception ex)
                {
                    return new Tuple<bool, string, int, int, IList>(false, ex.Message, 0, 0, null);
                }
            }

            return new Tuple<bool, string, int, int, IList>(false, "", 0, 0, null);
        }

        public static void device_update(long[] vals)
        {
            var ar = vals.Where(x => !list_device.Any(o => o == x)).ToArray();
            lock (lock_device)
                list_device.AddRange(ar);
        }

        public static void device_update(long val)
        {
            if (list_device.IndexOf(val) == -1)
            {
                lock (lock_device)
                    list_device.Add(val);
            }
        }

        public static long[] device_find(string text)
        {
            return list_device.Where(x => x.ToString().Contains(text)).ToArray();
        }


        public static void error_checking(m_column[] data_index)
        {
        }

        public static void error_add_record(long meter_id, int data_type, int yyMMdd, int HHmmss, List<double> ls_val)
        {
            Tuple<long, int, int, int> key = new Tuple<long, int, int, int>(meter_id, data_type, yyMMdd, HHmmss);

            if (dic_DB_error.ContainsKey(key))
                dic_DB_error.Add(key, ls_val);
            else
                dic_DB_error[key] = ls_val;
        }


        /// <summary>
        /// Lấy về danh sách các ngày có dữ liệu
        /// </summary>
        /// <returns></returns>
        public static int[] get_DateHasData()
        {
            var a = dic_DB.Select((item, id) => new { id = id, item = item }).Where(x => x.item.Count > 0).Select(x => x.id).ToArray();
            return a;
        }


        public static List<Tuple<int, long, int, int, UInt32, byte>> query_index(
            string key_cache,
            List<Tuple<byte, int>> w_date, long[] w_meter,
            int[] w_type, e_query_data query_data_type,
            string w_value, List<d2_data> ls_type = null)
        {
            int k_today = DateTime.Now.DayOfYear - 1;
            List<Tuple<int, long, int, int, UInt32, byte>> ls_index = new List<Tuple<int, long, int, int, UInt32, byte>>() { };

            byte yy_today = DateTime.Now.ToString("yy").TryParseToByte();
            int k_exist = w_date.IndexOf(new Tuple<byte, int>(yy_today, k_today));

            key_cache = "";

            //if (k_exist == -1)
            //{
            //    key_cache = "store." + string.Join(";", w_date) + "|" + string.Join(";", w_type) + "|" + string.Join(";", w_meter);

            //    ObjectCache cache = MemoryCache.Default;
            //    if (cache[key_cache] != null)
            //    {
            //        ls_index = cache[key_cache] as List<Tuple<int, long, int, int, UInt32, byte>>;
            //        return ls_index;
            //    }
            //}

            int len_meter = w_meter.Length;
            for (int i = 0; i < w_date.Count; i++)
            {
                List<Tuple<int, long, int, int, UInt32, byte>> ls_item = new List<Tuple<int, long, int, int, UInt32, byte>>() { };
                int k = w_date[i].Item2;
                byte yy = w_date[i].Item1;

                //if (k > k_today) break;

                int kkk = 0;
                if (k == k_today)
                    kkk = 9999;

                if (string.IsNullOrEmpty(w_value))
                {
                    #region // ... Tìm kiếm bên trong dữ liệu Keys(index):  ...

                    Tuple<int, long, int, int, UInt32, byte>[] arr_index = new Tuple<int, long, int, int, UInt32, byte>[] { };

                    switch (query_data_type)
                    {
                        case e_query_data.data_detail:
                            if (len_meter == 0)
                                arr_index = dic_DB[k].Keys
                                    .Where(x => w_type.Any(o => o == x.Item3))
                                    .Select(x => new Tuple<int, long, int, int, UInt32, byte>(k, x.Item1, x.Item2, x.Item3, x.Item4, x.Item5))
                                    .ToArray();
                            else
                                arr_index = dic_DB[k].Keys
                                    .Where(x => w_meter.Any(o => o == x.Item1) && w_type.Any(o => o == x.Item3))
                                    .Select(x => new Tuple<int, long, int, int, UInt32, byte>(k, x.Item1, x.Item2, x.Item3, x.Item4, x.Item5))
                                    .ToArray();
                            break;
                        case e_query_data.data_type:
                            if (len_meter == 0)
                                arr_index = dic_DB[k].Keys
                                    .Where(x => w_type.Any(o => o == x.Item2))
                                    .Select(x => new Tuple<int, long, int, int, UInt32, byte>(k, x.Item1, x.Item2, x.Item3, x.Item4, x.Item5))
                                    .ToArray();
                            else
                                arr_index = dic_DB[k].Keys
                                    .Where(x => w_meter.Any(o => o == x.Item1) && w_type.Any(o => o == x.Item2) && w_date.Exists(dd=>dd.Item1==x.Item5))
                                    .Select(x => new Tuple<int, long, int, int, UInt32, byte>(k, x.Item1, x.Item2, x.Item3, x.Item4, x.Item5))
                                    .ToArray();
                            break;
                    }

                    ls_item.AddRange(arr_index);

                    #endregion
                }
                else
                {
                    #region // ... Tìm kiếm bên trong dữ liệu Values ...

                    double[] a_ids_w_Value = new double[] { };

                    switch (query_data_type)
                    {
                        case e_query_data.data_type:
                            if (len_meter == 0)
                                a_ids_w_Value = dic_DB[k]
                                    .Select((x, id) =>
                                    {
                                        List<double> vs = x.Value.ToList();
                                        vs[0] = id;
                                        return new KeyValuePair<Tuple<long, int, int, UInt32, byte>, List<double>>(x.Key, vs);
                                    })
                                    .Where(x => w_type.Any(o => o == x.Key.Item2)) //tìm kiếm theo loại dữ liệu: TSVH, TSTH ...
                                    .Select(x => x.Value.ToArray())
                                    .Where(w_value)
                                    .Cast<double[]>()
                                    .Select(x => x[0])
                                    .ToArray();
                            else
                                a_ids_w_Value = dic_DB[k]
                                    .Select((x, id) =>
                                    {
                                        List<double> vs = x.Value.ToList();
                                        vs[0] = id;
                                        return new KeyValuePair<Tuple<long, int, int, UInt32, byte>, List<double>>(x.Key, vs);
                                    })
                                    .Where(x => w_meter.Any(o => o == x.Key.Item1) && w_type.Any(o => o == x.Key.Item2)) //tìm kiếm theo loại dữ liệu: TSVH, TSTH ...
                                    .Select(x => x.Value.ToArray())
                                    .Where(w_value)
                                    .Cast<double[]>()
                                    .Select(x => x[0])
                                    .ToArray();
                            break;
                        case e_query_data.data_detail:
                            if (len_meter == 0)
                                a_ids_w_Value = dic_DB[k]
                                    .Select((x, id) =>
                                    {
                                        List<double> vs = x.Value.ToList();
                                        vs[0] = id;
                                        return new KeyValuePair<Tuple<long, int, int, UInt32, byte>, List<double>>(x.Key, vs);
                                    })
                                    .Where(x => w_type.Any(o => o == x.Key.Item3)) //tìm kiếm dữ liệu chi tiết
                                    .Select(x => x.Value.ToArray())
                                    .Where(w_value)
                                    .Cast<double[]>()
                                    .Select(x => x[0])
                                    .ToArray();
                            else
                                a_ids_w_Value = dic_DB[k]
                                    .Select((x, id) =>
                                    {
                                        List<double> vs = x.Value.ToList();
                                        vs[0] = id;
                                        return new KeyValuePair<Tuple<long, int, int, UInt32, byte>, List<double>>(x.Key, vs);
                                    })
                                    .Where(x => w_meter.Any(o => o == x.Key.Item1) && w_type.Any(o => o == x.Key.Item3)) //tìm kiếm dữ liệu chi tiết
                                    .Select(x => x.Value.ToArray())
                                    .Where(w_value)
                                    .Cast<double[]>()
                                    .Select(x => x[0])
                                    .ToArray();
                            break;
                    }

                    var li = dic_DB[k].Keys.Where((x, id) => a_ids_w_Value.Any(o => o == id))
                        .Select(x => new Tuple<int, long, int, int, UInt32, byte>(k, x.Item1, x.Item2, x.Item3, x.Item4, x.Item5))
                        .ToArray();

                    ls_item.AddRange(li);

                    #endregion
                }

                if (ls_item.Count > 0)
                {
                    if (ls_type != null && ls_type.IndexOf(d2_data.FIX_MONTH) != -1)
                    {
                        if (w_meter.Length == 1) // 1 diem do
                            ls_index.AddRange(ls_item);
                        else
                        { // nhom diem do
                            byte b_yy = w_date[0].Item1;
                            string s_yyMM = w_date[0].ToString().Substring(0, 4);
                            UInt32 m_min = (s_yyMM + "000000").TryParseToUInt32(),
                                m_max = (s_yyMM + "330000").TryParseToUInt32();
                            ls_item = ls_item.Where(x => x.Item6 == b_yy).ToList();
                            ls_index.AddRange(ls_item);
                        }
                    }
                    else
                    {
                        ls_item = ls_item.Where(x => x.Item6 == yy).ToList();
                        if (ls_item.Count > 0)
                            ls_index.AddRange(ls_item);
                    }
                }
            }

            if (ls_index.Count > 0) ls_index = ls_index.Where(x => x.Item2 > 1000000).ToList();

            if (key_cache != "")
            {
                ObjectCache ca = MemoryCache.Default;
                CacheItemPolicy policy = new CacheItemPolicy();
                policy.Priority = System.Runtime.Caching.CacheItemPriority.Default;
                policy.AbsoluteExpiration = DateTimeOffset.Now.AddMinutes(200);

                //Muốn tạo sự kiện thì tạo ở đây còn k thì hoy
                //(MyCacheItemPriority == MyCachePriority.Default) ? CacheItemPriority.Default : CacheItemPriority.NotRemovable;
                ///Globals.policy.RemovedCallback = callback; 

                ca.Set(key_cache, ls_index, policy, null);
                //    object test = objCache.Get(KeyRequest, null);
            }

            return ls_index;
        }

        public static Tuple<long, long, int[], long[], List<decimal[]>> query(
            string key_cache,
            List<Tuple<byte, int>> w_date, long[] w_meter,
            int[] w_type, e_query_data query_data_type,
            string w_value,
            int page_number, int page_size)
        {


            byte yy_today = DateTime.Now.ToString("yy").TryParseToByte();
            bool distinct_group_item = false;

            w_date = w_date.OrderByDescending(x => x).ToList();
            int len_meter = w_meter.Length;
            if (len_meter > 1)
                distinct_group_item = true;

            bool is_fday = false;
            if (w_type.ToList().IndexOf((int)d2_data.FIX_DAY) != -1) is_fday = true;

            List<d2_data> ls_type = new List<d2_data>() { };
            try
            {
                switch (query_data_type)
                {
                    case e_query_data.data_detail:
                        ls_type = w_type.Select(x => (d2_data)(x.ToString()[1].TryToInt() * 100000)).Distinct().ToList();
                        break;
                    case e_query_data.data_type:
                        ls_type = w_type.Select(x => (d2_data)x).ToList();
                        break;
                }
            }
            catch { }

            //if (ls_type != null && ls_type.IndexOf(d2_data.FIX_MONTH) != -1) w_date = new List<Tuple<byte, int>>() { };

            if (w_date.Count == 0)
                for (int kd = 0; kd < max_size; kd++)
                    w_date.Add(new Tuple<byte, int>(yy_today, kd));

            int[] a_type_result = new int[] { };
            List<Tuple<int, long, int, int, UInt32, byte>> ls_index = new List<Tuple<int, long, int, int, UInt32, byte>>() { };

            int k_today = DateTime.Now.DayOfYear - 1;
            List<Tuple<byte, int>> w_date_cache = new List<Tuple<byte, int>>() { };
            int k_exist = w_date.IndexOf(new Tuple<byte, int>(yy_today, k_today));


            if (k_exist == -1)
                w_date_cache = w_date;
            else
            {
                var ls_today = query_index(key_cache, new List<Tuple<byte, int>> { new Tuple<byte, int>(yy_today, k_today) }, w_meter, w_type, query_data_type, w_value, ls_type);
                ls_index.AddRange(ls_today);

                var d0 = w_date;
                d0.RemoveAt(k_exist);
                w_date_cache = d0;
            }

            if (w_date_cache.Count > 0)
            {
                try
                {
                    var ls0 = query_index(key_cache, w_date_cache, w_meter, w_type, query_data_type, w_value, ls_type);
                    ls_index.AddRange(ls0);
                }
                catch (Exception ex)
                {
                    string sadad = ex.Message;
                }
            }

            long[] a_deviceID = new long[] { };
            List<decimal[]> data = new List<decimal[]>() { };
            if (ls_index.Count > 0)
            {
                bool isTSVH = false, is3Phase = false;
                char c_3phase = ((int)d1_pha._3pha).ToString()[0];

                switch (query_data_type)
                {
                    case e_query_data.data_detail:
                        char c_datatype_tsvh = ((int)d2_data.TSVH).ToString()[0];
                        is3Phase = w_type.Where(x => x.ToString()[0] == c_3phase).Count() > 0;
                        isTSVH = w_type.Where(x => x.ToString()[1] == c_datatype_tsvh).Count() > 0;
                        break;
                    case e_query_data.data_type:
                        is3Phase = ls_index[0].Item4.ToString()[0] == c_3phase;
                        isTSVH = w_type.Where(x => x == (int)d2_data.TSVH).Count() > 0;
                        break;
                }

                #region ...


                ls_index = ls_index.Distinct().ToList();
                a_type_result = ls_index.Select(x => x.Item4).Distinct().ToArray();

                if (ls_type != null && ls_type.IndexOf(d2_data.FIX_MONTH) != -1)
                {
                    //date,meter,data_type,HHmm
                    ls_index = ls_index.OrderByDescending(x => x.Item5).ToList();
                    ls_index = ls_index.GroupBy(x => new { meter = x.Item2 })
                          .Select(group => new Tuple<int, long, int, int, UInt32, byte>(group.First().Item1, group.Key.meter, group.First().Item3, group.First().Item4, group.First().Item5, group.First().Item6))
                          .ToList();

                }
                else
                {
                    #region //................

                    if (is_fday)
                    {
                        //date,meter,data_type,HHmm
                        ls_index = ls_index
                            .SortMultiple(new List<Tuple<string, string>>() {
                                  new Tuple<string, string>("Item6", "desc"),
                                  new Tuple<string, string>("Item1", "desc"),
                                  new Tuple<string, string>("Item5", "desc") })
                            .ToList();
                        ls_index = ls_index.GroupBy(x => new { date = x.Item1, meter = x.Item2 })
                              .Select(group => new Tuple<int, long, int, int, UInt32, byte>(group.Key.date, group.Key.meter, group.First().Item3, group.First().Item4, group.First().Item5, group.First().Item6))
                              .ToList();
                    }
                    else
                    {
                        if (isTSVH == false || w_meter.Length == 1)
                        {
                            //xem 1 điểm đo cụ thể
                            if (is3Phase)
                            {
                                //date,meter,data_type,HHmm
                                ls_index = ls_index.GroupBy(x => new { date = x.Item1, meter = x.Item2, HHmm = x.Item5 })
                                          .Select(group => new Tuple<int, long, int, int, UInt32, byte>(group.Key.date, group.Key.meter, group.First().Item3, group.First().Item4, group.First().Item5, group.First().Item6))
                                          .SortMultiple(new List<Tuple<string, string>>() {
                                  new Tuple<string, string>("Item6", "desc"),
                                  new Tuple<string, string>("Item1", "desc"),
                                  new Tuple<string, string>("Item5", "desc") })
                                          .ToList();
                            }
                            else
                            {
                                //date,meter,data_type,HHmm
                                ls_index = ls_index
                                    .SortMultiple(new List<Tuple<string, string>>() {
                                  new Tuple<string, string>("Item6", "desc"),
                                  new Tuple<string, string>("Item1", "desc"),
                                  new Tuple<string, string>("Item5", "desc") })
                                    .ToList();
                                ls_index = ls_index.GroupBy(x => new { date = x.Item1, meter = x.Item2 })
                                      .Select(group => new Tuple<int, long, int, int, UInt32, byte>(group.Key.date, group.Key.meter, group.First().Item3, group.First().Item4, group.First().Item5, group.First().Item6))
                                      .ToList();
                            }
                        }
                        else
                        {
                            //xem 1 nhóm nhiều điểm đo
                            //date,meter,data_type,HHmm
                            ls_index = ls_index
                                .SortMultiple(new List<Tuple<string, string>>() {
                                  new Tuple<string, string>("Item6", "desc"),
                                  new Tuple<string, string>("Item1", "desc"),
                                  new Tuple<string, string>("Item5", "desc") })
                                .ToList();
                            ls_index = ls_index.GroupBy(x => new { date = x.Item1, meter = x.Item2 })
                                  .Select(group => new Tuple<int, long, int, int, UInt32, byte>(group.Key.date, group.Key.meter, group.First().Item3, group.First().Item4, group.First().Item5, group.First().Item6))
                                  .ToList();
                        }
                    }
                    #endregion
                }

                int startRowIndex = page_size * (page_number - 1);
                int page_number_max = ls_index.Count / page_size;
                if (ls_index.Count % page_size != 0) page_number_max++;
                if (page_number > page_number_max)
                {
                    page_number = 1;
                    startRowIndex = 0;
                }
                var p_index = ls_index.Skip(startRowIndex).Take(page_size).ToArray();
                a_deviceID = p_index.Select(x => x.Item2).ToArray();

                for (int k = 0; k < p_index.Length; k++)
                {
                    //date,meter,data_type,HHmm
                    var o = p_index[k];
                    int date_index = o.Item1;

                    Tuple<long, int, int, UInt32, byte> key = new Tuple<long, int, int, UInt32, byte>(o.Item2, o.Item3, o.Item4, o.Item5, o.Item6);
                    List<double> val;

                    dic_DB[date_index].TryGetValue(key, out val);
                    if (val != null && val.Count > 0)
                        data.Add(val.Select(x => (decimal)x).ToArray());
                }

                #endregion
            }


            long rs_count_all = dic_DB.Select(k => k.Count).Sum();
            return new Tuple<long, long, int[], long[], List<decimal[]>>(rs_count_all, ls_index.Count, a_type_result, a_deviceID, data);
        }











































        public static void update(Tuple<Tuple<long, int, int, UInt32, byte>, int, double[]>[] data)
        {
            long[] a_device = data.Select(x => x.Item1.Item1).Distinct().ToArray();
            device_update(a_device);

            //< < meter_id,data_type,time(hh|hhmm) > , date_yyMMdd , value[] >
            lock (lock_db)
            {
                for (int k = 0; k < data.Length; k++)
                {
                    try
                    {
                        var o = data[k];
                        int index = o.Item2.IntDateToDayOfYear();

                        if (dic_DB[index].ContainsKey(o.Item1))
                            dic_DB[index][o.Item1] = o.Item3.ToList();
                        else
                            dic_DB[index].AddList(o.Item1, o.Item3);
                    }
                    catch
                    {
                        int kk = 0;
                    }
                }

                ////296
                //int k_2410 = 151024.IntDateToDayOfYear();
                //var db_2410 = dic_DB[k_2410];
                //var db_2410_1001 = db_2410.Where(x => x.Key.Item2 == 1001).ToArray();


                //var db_1023 = dic_DB[295];
                //var db_0602 = dic_DB[152];
                //var db_0603 = dic_DB[153];
            }
        }

        #endregion


        #region ... where, add, edit, remove ....


        public static Tuple<bool, string, int, int, IList> db_where(

            string s_key, string s_tab, string s_select, string s_where, string s_order_by, string s_distinct,
            int page_number, int page_size)
        {
            string key = "db." + s_tab;
            Func<string, string, string, string, string, int, int, Tuple<bool, string, int, int, IList>> fun = null;
            dic_db_where.TryGetValue(key, out fun);

            if (fun == null)
            {
                Type ti = Type.GetType("host." + s_tab + ", host");
                if (ti != null)
                {
                    try
                    {
                        MethodInfo fu = ti.GetMethod("where_call_dynamic", BindingFlags.Static | BindingFlags.Public);
                        if (fu != null)
                        {
                            fun = (Func<string, string, string, string, string, int, int, Tuple<bool, string, int, int, IList>>)
                                Delegate.CreateDelegate(typeof(
                                Func<string, string, string, string, string, int, int,
                                Tuple<bool, string, int, int, IList>>), fu);

                            lock (lock_db_where)
                                if (dic_db_where.ContainsKey(key) == false) dic_db_where.Add(key, fun);
                        }
                    }
                    catch (Exception ex)
                    {
                        string s_error = ex.Message;
                    }
                }
            }

            if (fun != null)
            {
                try
                {
                    var rs = fun(s_key, s_select, s_where, s_order_by, s_distinct, page_number, page_size);
                    if (rs != null)
                        return rs;
                }
                catch (Exception ex)
                {
                    return new Tuple<bool, string, int, int, IList>(false, ex.Message, 0, 0, null);
                }
            }

            return new Tuple<bool, string, int, int, IList>(false, "", 0, 0, null);
        }


        public static Tuple<bool, string, dynamic[]> db_get_id(string s_key, string s_tab)
        {
            string action = "get_id";
            string key = "db." + s_tab + "/" + action;
            Func<string, Tuple<bool, string, dynamic[]>> fun = null;
            dic_db_get_id.TryGetValue(key, out fun);

            if (fun == null)
            {
                Type ti = Type.GetType("host." + s_tab + ", host");
                if (ti != null)
                {
                    try
                    {
                        MethodInfo fu = ti.GetMethod(action, BindingFlags.Static | BindingFlags.Public);
                        if (fu != null)
                        {
                            fun = (Func<string, Tuple<bool, string, dynamic[]>>)
                                Delegate.CreateDelegate(typeof(Func<string, Tuple<bool, string, dynamic[]>>), fu);

                            lock (lock_db_get_id)
                                if (dic_db_get_id.ContainsKey(key) == false) dic_db_get_id.Add(key, fun);
                        }
                    }
                    catch (Exception ex)
                    {
                        string s_error = ex.Message;
                    }
                }
            }

            if (fun != null)
            {
                try
                {
                    var rs = fun(s_key);
                    if (rs != null)
                        return rs;
                }
                catch (Exception ex)
                {
                    return new Tuple<bool, string, dynamic[]>(false, ex.Message, null);
                }
            }

            return new Tuple<bool, string, dynamic[]>(false, "", null);
        }


        public static Tuple<bool, string, dynamic> db_add(string s_key, string s_tab, string s_item)
        {
            string action = "add";
            string key = "db." + s_tab + "/" + action;
            Func<string, string, Tuple<bool, string, dynamic>> fun = null;
            dic_db_action.TryGetValue(key, out fun);

            if (fun == null)
            {
                Type ti = Type.GetType("host." + s_tab + ", host");
                if (ti != null)
                {
                    try
                    {
                        MethodInfo fu = ti.GetMethod(action, BindingFlags.Static | BindingFlags.Public);
                        if (fu != null)
                        {
                            fun = (Func<string, string, Tuple<bool, string, dynamic>>)
                                Delegate.CreateDelegate(typeof(Func<string, string, Tuple<bool, string, dynamic>>), fu);

                            lock (lock_db_action)
                                if (dic_db_action.ContainsKey(key) == false) dic_db_action.Add(key, fun);
                        }
                    }
                    catch (Exception ex)
                    {
                        string s_error = ex.Message;
                    }
                }
            }

            if (fun != null)
            {
                try
                {
                    var rs = fun(s_key, s_item);
                    if (rs != null)
                        return rs;
                }
                catch (Exception ex)
                {
                    return new Tuple<bool, string, dynamic>(false, ex.Message, null);
                }
            }

            return new Tuple<bool, string, dynamic>(false, "", null);
        }

        public static Tuple<bool, string, dynamic> db_edit(string s_key, string s_tab, string s_item)
        {
            string action = "edit";
            string key = "db." + s_tab + "/" + action;
            Func<string, string, Tuple<bool, string, dynamic>> fun = null;
            dic_db_action.TryGetValue(key, out fun);

            if (fun == null)
            {
                Type ti = Type.GetType("host." + s_tab + ", host");
                if (ti != null)
                {
                    try
                    {
                        MethodInfo fu = ti.GetMethod(action, BindingFlags.Static | BindingFlags.Public);
                        if (fu != null)
                        {
                            fun = (Func<string, string, Tuple<bool, string, dynamic>>)
                                Delegate.CreateDelegate(typeof(Func<string, string, Tuple<bool, string, dynamic>>), fu);

                            lock (lock_db_action)
                                if (dic_db_action.ContainsKey(key) == false) dic_db_action.Add(key, fun);
                        }
                    }
                    catch (Exception ex)
                    {
                        string s_error = ex.Message;
                    }
                }
            }

            if (fun != null)
            {
                try
                {
                    var rs = fun(s_key, s_item);
                    if (rs != null)
                        return rs;
                }
                catch (Exception ex)
                {
                    return new Tuple<bool, string, dynamic>(false, ex.Message, null);
                }
            }

            return new Tuple<bool, string, dynamic>(false, "", null);
        }

        public static Tuple<bool, string, dynamic> db_remove(string s_key, string s_tab, string s_item)
        {
            string action = "remove";
            string key = "db." + s_tab + "/" + action;
            Func<string, string, Tuple<bool, string, dynamic>> fun = null;
            dic_db_action.TryGetValue(key, out fun);

            if (fun == null)
            {
                Type ti = Type.GetType("host." + s_tab + ", host");
                if (ti != null)
                {
                    try
                    {
                        MethodInfo fu = ti.GetMethod(action, BindingFlags.Static | BindingFlags.Public);
                        if (fu != null)
                        {
                            fun = (Func<string, string, Tuple<bool, string, dynamic>>)
                                Delegate.CreateDelegate(typeof(Func<string, string, Tuple<bool, string, dynamic>>), fu);

                            lock (lock_db_action)
                                if (dic_db_action.ContainsKey(key) == false) dic_db_action.Add(key, fun);
                        }
                    }
                    catch (Exception ex)
                    {
                        string s_error = ex.Message;
                    }
                }
            }

            if (fun != null)
            {
                try
                {
                    var rs = fun(s_key, s_item);
                    if (rs != null)
                        return rs;
                }
                catch (Exception ex)
                {
                    return new Tuple<bool, string, dynamic>(false, ex.Message, null);
                }
            }

            return new Tuple<bool, string, dynamic>(false, "", null);
        }


        #endregion
    }//end class


    public class m_blue_data
    {
        public m_blue_data()
        {

        }

        public m_blue_data(d2_data data_type_,
                long device_id_, double val_,
                int index_, byte yy_,
                int yyMMdd_, int HHmmss_, UInt32 yyMMddHHmm_)
        {
            data_type = data_type_;
            device_id = device_id_;

            value = val_;
            index_date = index_;

            yy = yy_;
            yyMMdd = yyMMdd_;
            HHmmss = HHmmss_;
            yyMMddHHmm = yyMMddHHmm_;
        }

        /// <summary>
        /// TSVH, FIX_DAY, FIX_MONTH, EVEN ...
        /// </summary>
        public d2_data data_type { set; get; }

        /// <summary>
        /// meter_id
        /// </summary>
        public long device_id { set; get; }

        /// <summary>
        /// Value: p_giao_tong
        /// </summary>
        public double value { set; get; }

        /// <summary>
        /// index = yyMMdd.IntDateToDayOfYear();
        /// int index = DateTime.Now.DayOfYear; if (index > 0) index--;
        /// </summary>
        public int index_date { set; get; }


        public byte yy { set; get; }
        public int yyMMdd { set; get; }
        public int HHmmss { set; get; }
        public UInt32 yyMMddHHmm { set; get; }

        public override string ToString()
        {
            return this.data_type.ToString() + ";" + device_id.ToString() + ";" + value.ToString() + ";" + yyMMddHHmm.ToString();
        }
    }
}