using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace model
{
    [Serializable]
    [StructLayout(LayoutKind.Sequential, Pack = 1, CharSet = CharSet.Unicode)]
    public struct m_canhbao_cauhinh
    {       
        public int cauhinh_id;
        public int type; // 100000: cảnh báo vận hành; 500000: cảnh báo sự kiện
        public int tech; // 1000: rf; 2000: plc; 3000: blue; 4000: gprs
        public int column;
        public int oper; // 0: > ; 1: < ; 2: = ;3 >=; 4: <=  
        public int value;

        public float default_value;
        public bool use_percentage; 

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 255)]
        public string name;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 1000)]
        public string note;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 255)]
        public string title;

    }
}
