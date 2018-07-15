using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace model
{
     

    [Serializable]
    [StructLayout(LayoutKind.Sequential, Pack = 1, CharSet = CharSet.Unicode)]
    public struct m_kh_nhom
    {
        public long id;
        public long id_parent;
        public int id_group; // 1: 1pha, 31: 3pha1bieu, 33: 3pha3bieu

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 36)]
        public string code; // mã dùng cho khách hàng sử dụng

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 255)]
        public string name;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 255)]
        public string tag_code; // 1PHA/3PHA1BIEU/3PHA3BIEU;RF/PLC;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 50)]
        public string name_short;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 8000)]
        public string itemsub_ids; // mảng các id của phần tử con: = ;1;2;3;455;
                
        public int type; // 1: Khach hang(Nhom); 2: Tram,Tuyen(duong); 3: Quyen(So ghi); 4: Cot

        public int level; // level tree
        
        public int item_count; //số phần tử con là điểm đo (meter) của từng loại cụ thể: 1pha có 12 meter, 31 có 15 meter ...
        public int itemsub_count; //số phần tử con là nhóm (KH, Trạm, Tuyến, Quyển, Cột) ...

        public bool status;        
    }
}
