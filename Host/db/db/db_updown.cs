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
    public class db_updown
    {
        public static Tuple<bool, string, dynamic> add(string s_key, string s_item)
        {
            try
            {
                string json = "";
                return new Tuple<bool, string, dynamic>(true, json, null);
            }
            catch { }

            return new Tuple<bool, string, dynamic>(false, "", null);
        }

        public static Tuple<bool, string, dynamic> edit(string s_key, string s_item)
        {
            try
            {
                string json = "";
                return new Tuple<bool, string, dynamic>(true, json, null);
            }
            catch { }
            return new Tuple<bool, string, dynamic>(false, "", null);
        }

        public static Tuple<bool, string, dynamic> remove(string s_key, string s_item)
        {
            try
            {
                string json = "";
                return new Tuple<bool, string, dynamic>(true, json, null);
            }
            catch { }
            return new Tuple<bool, string, dynamic>(false, "", null);
        }

        //===========================================================================
        private static object lock_write = new object();
        // < meter_id,cus_code,yyMMdd,HHmmss, [1:up;0:down] >
        private static List<Tuple<long, int, int, UInt32, byte>> list = new List<Tuple<long, int, int, UInt32, byte>>() { };
        private static string path = hostServer.pathRoot + @"db_updown\";

        public static Tuple<bool, string, int, int, IList> where_call_dynamic(string s_key, string s_select, string s_where, string s_order_by, string s_distinct, int page_number, int page_size)
        {
            return dbQuery.where<Tuple<long, int, int, UInt32, byte>>(list.ToArray(), s_key, s_select, s_where, s_order_by, s_distinct, page_number, page_size);
        }

        public static void load()
        {
            if (!Directory.Exists(path)) Directory.CreateDirectory(path);

            var dis = Directory.GetFiles(path, "*.mmf");
            foreach (var fi in dis)
            {
                var ls = hostFile.read_file_MMF<Tuple<long, int, int, UInt32, byte>>(fi);
                list.AddRange(ls);
            }
        }

        private static void update(int yyDDmm)
        {
            var a_col = get_Items(yyDDmm);
            hostFile.write_file_MMF<Tuple<long, int, int, UInt32, byte>>(a_col, path, yyDDmm.ToString());
        }

        public static Tuple<long, int, int, UInt32, byte>[] get_All()
        {
            return list.ToArray();
        }

        public static Tuple<long, int, int, UInt32, byte>[] get_ItemByCusCode(int[] a_cus_code)
        {
            return list.Where(x => a_cus_code.Any(o => o == x.Item2))
                .OrderByDescending(x => new { x.Item3, x.Item4 })
                .ToArray();
        }

        public static Tuple<long, int, int, UInt32, byte>[] get_Items(int yyDDmm)
        {
            return list.Where(x => x.Item3 == yyDDmm)
                .OrderByDescending(x => new { x.Item3, x.Item4 })
                .ToArray();
        }

        public static Tuple<long, int, int, UInt32, byte>[] get_ItemByMeterID(int meter_id)
        {
            return list.Where(x => x.Item1 == meter_id)
                .OrderByDescending(x => new { x.Item3, x.Item4 })
                .ToArray();
        }

        public static Tuple<long, int, int, UInt32, byte>[] get_ItemByCusCode(int cus_code)
        {
            return list.Where(x => x.Item2 == cus_code)
                .OrderByDescending(x => new { x.Item3, x.Item4 })
                .ToArray();
        }

        public static Tuple<long, int, int, UInt32, byte>[] get_Item(int meter_id, int cus_code)
        {
            return list.Where(x => x.Item1 == meter_id && x.Item2 == cus_code)
                .OrderByDescending(x => new { x.Item3, x.Item4 })
                .ToArray();
        }

        public static void add_Item(int meter_id, int cus_code, int yyDDmm, UInt32 HHmmss, byte up_down)
        {
            lock (lock_write)
            {
                list.Add(new Tuple<long, int, int, UInt32, byte>(meter_id, cus_code, yyDDmm, HHmmss, up_down));
                update(yyDDmm);
            }
        }


    }//end class
}
