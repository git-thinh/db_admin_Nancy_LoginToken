using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace model
{
     

    [Serializable]
    [StructLayout(LayoutKind.Sequential, Pack = 1, CharSet = CharSet.Unicode)]
    public struct m_node
    { 
        public int type; // 0: dienluc, 1: Khach hang(Nhom); 2: Tram,Tuyen(duong); 3: Quyen(So ghi); 4: Cot
        public int level; // level tree

        public long id;
        public long id_root0;
        public long id_root1;
        public long id_parent;
        public int id_group; // 0: all, 1: 1pha, 31: 3pha1bieu, 33: 3pha3bieu

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 36)]
        public string code;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 255)]
        public string name;         

        public bool online;
    }
}
