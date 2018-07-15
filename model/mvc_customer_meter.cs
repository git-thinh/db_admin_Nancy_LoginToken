using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace model
{
    [Serializable]
    [StructLayout(LayoutKind.Sequential, Pack = 1, CharSet = CharSet.Unicode)]
    public struct mvc_customer_meter
    {
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 36)] 
        public string cus_id;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 50)]
        public string meter_id;

        public int time1;
        public int time2;
        public int date1;
        public int date2;

    }
}
