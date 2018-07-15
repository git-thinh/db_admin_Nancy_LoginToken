using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace model
{
     

    [Serializable]
    [StructLayout(LayoutKind.Sequential, Pack = 1, CharSet = CharSet.Unicode)]
    public struct m_kh_danhmuc
    {
        public int id;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 50)]
        public string code;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 255)]
        public string name;
   
    }
}
