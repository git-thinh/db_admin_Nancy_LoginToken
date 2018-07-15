using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace model
{
    [Serializable]
    [StructLayout(LayoutKind.Sequential, Pack = 1, CharSet = CharSet.Unicode)]
    public struct mvc_category
    {
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 36)]
        public string parent_id;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 36)]
        public string cat_id;

        public int level;

        public int total_sub;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 50)]
        public string tag;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 50)]
        public string name;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 255)]
        public string note;

        public bool status;
    }
}
