using model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace host 
{
    public class store_plc
    {
        //< dcu_id, index> , <meter_id, phase>
        private static Dictionary<Tuple<long, long>, Tuple<long, d1_pha>> dic_plc_Phase = new Dictionary<Tuple<long, long>, Tuple<long, d1_pha>>() { };

        public static void load() 
        {
            string path = AppDomain.CurrentDomain.BaseDirectory + "store_plc";

            if (!Directory.Exists(path)) Directory.CreateDirectory(path);

            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
            else
            {
                var ds = Directory.GetFiles(path, "*.mmf");
                for (int k = 0; k < ds.Length; k++)
                {
                    try
                    {
                        var arr = hostFile.read_file_MMF<m_meter_plc>(ds[k]);
                        for (int n = 0; n < arr.Length; n++) {
                            m_meter_plc o = arr[n];
                            Tuple<long, long> key = new Tuple<long, long>(o.imei, o.id);
                            Tuple<long, d1_pha> val = new Tuple<long, d1_pha>(o.so_cong_to, o.phase_id == 1 ? d1_pha._1pha : d1_pha._3pha);
                            if (dic_plc_Phase.ContainsKey(key) == false)
                                dic_plc_Phase.Add(key, val);
                            else
                                dic_plc_Phase[key] = val;
                        }
                    }
                    catch { }
                }
            }
        }

        public static Tuple<long, d1_pha> getValue(Tuple<long, long> key) {
            Tuple<long, d1_pha> val = new Tuple<long, d1_pha>(0, d1_pha._);
            dic_plc_Phase.TryGetValue(key, out val);
            return val;
        }



    }
}
