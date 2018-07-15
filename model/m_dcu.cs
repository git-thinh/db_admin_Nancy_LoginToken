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
    public struct m_dcu
    {
        public long dcu_id;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 255)]
        public string name;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 8000)]
        public string cat_array;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 255)]
        public string note;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 255)]
        public string address;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 255)]
        public string type_code;

        public int date_join;
        public int date_update_lastest;
        public int date_stop;

        public int itemsub_count;

        public bool is_online;

        public bool status;

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
    
}
