using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace model
{
    [Serializable]
    [StructLayout(LayoutKind.Sequential, Pack = 1, CharSet = CharSet.Unicode)]
    public struct m_column
    {
        public int data_type;
        public int index;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 36)]
        public string col_id;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 255)]
        public string code;


        public int div10n; //chia 10xn

        public bool index_date;
        public bool index_time;
                
        public long value_min;//giá trị tối thiểu để ra cảnh báo
        public long value_max;//giá trị tối đa để ra cảnh báo

        public bool progressive;//lũy tiến: giá trị sau lớn hơn giá trị trước.

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 255)]
        public string data_join;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 255)]
        public string name;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 255)]
        public string title;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 255)]
        public string note;


        public override string ToString()
        {
            return index.ToString() + ":" + code + ":" + name;
        }
    }
}
