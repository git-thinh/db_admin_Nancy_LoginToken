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
    public class db_meter_heso
    {
        public static Tuple<bool, string, dynamic> add(string s_key, string s_item)
        {
            try
            {
                List<double> vals = s_item.Split(';').Select(x => x.TryParseToDouble()).ToList();
                long mid = (long)vals[0];
                int level = (int)vals[1];

                vals.RemoveAt(0);
                vals.RemoveAt(0);

                store.add_HeSoNhan(level, mid, vals);

                dbCache.clear(typeof(m_meter_heso).FullName);

                return new Tuple<bool, string, dynamic>(true, "ok", null);
            }
            catch { }

            return new Tuple<bool, string, dynamic>(false, "",null);
        }
        
        public static Tuple<bool, string, dynamic> edit(string s_key, string s_item)
        {
            try {

                dbCache.clear(typeof(m_meter_heso).FullName);
            }
            catch { }
            return new Tuple<bool, string, dynamic>(false, "",null);
        }

        public static Tuple<bool, string, dynamic> remove(string s_key, string s_item)
        {
            try
            {

                dbCache.clear(typeof(m_meter_heso).FullName);
            }
            catch { }
            return new Tuple<bool, string, dynamic>(false, "",null);
        }
         
    }

}
