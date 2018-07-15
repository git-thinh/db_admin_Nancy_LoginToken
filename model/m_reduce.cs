using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace model
{
    [Serializable]
    [StructLayout(LayoutKind.Sequential, Pack = 1, CharSet = CharSet.Unicode)]
    public struct m_reduce
    {
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 36)]
        public string id;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 255)]
        public string api;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 255)]
        public string name;



        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 8000)]
        public string w_type;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 8000)]
        public string w_date_mon;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 8000)]
        public string w_date_item;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 8000)]
        public string w_dcu;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 8000)]
        public string w_meter;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 8000)]
        public string w_value;
         



        public long date_create;
        public long date_update_lastest;

        public bool status;
    }

    //[Serializable]
    //public struct m_reduce_compile
    //{
    //    public string id { set; get; }

    //    public string api { set; get; }
    //    public string name { set; get; }
    //    public string src { set; get; }

    //    public long date_create { set; get; }
    //    public long date_update_lastest { set; get; }

    //    public bool status { set; get; }

    //    public Func<KeyValuePair<Tuple<long, int, int>, List<double>>, bool> where { set; get; }
         
    //}

}
