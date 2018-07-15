using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace model
{
    [Serializable]
    [StructLayout(LayoutKind.Sequential, Pack = 1, CharSet = CharSet.Unicode)]
    public struct m_meter
    {
        public int index;
        public bool is_online;
        public int phase_id; // 1000000: 1pha; 3100000: 3pha1bieu; 3300000: 3pha3bieu
        public int tech_id; // RF = 1000, PLC = 2000, BLUETOOTH = 3000, GPRS = 4000,
        public long meter_id_ref;
        
        public long meter_id;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 10)]
        public string meter_id_fix;
        
        public long dcu_id;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 36)]
        public string cus_id;

        public long cus_code;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 10)]
        public string cus_code_fix;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 255)]
        public string name;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 255)]
        public string type_code; // _1pha;rf;psmart

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 50)]
        public string username; //ten tai khoan dang nhap he thong de xem thong tin

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 255)]
        public string fullname;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 15)]
        public string phone;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 50)]
        public string email;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 50)]
        public string address_house;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 255)]
        public string address;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 255)]
        public string note;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 10)]
        public string type; //chung loai Psmart

        public long date_up;//ngay treo
        public long date_down;//ngay thao

        public bool status;

        public override string ToString()
        {
            return cus_code + ":" + meter_id + "," + name + "," + type_code;
        }
    }
}
