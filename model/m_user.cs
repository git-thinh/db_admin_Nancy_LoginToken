using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace model
{
     

    [Serializable]
    [StructLayout(LayoutKind.Sequential, Pack = 1, CharSet = CharSet.Unicode)]
    public struct m_user
    {
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 36)]
        public string user_id;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 50)]
        public string username;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 50)]
        public string pass;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 50)]
        public string name;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 50)]
        public string phone;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 50)]
        public string email;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 50)]
        public string avatar;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 50)]
        public string address;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 255)]
        public string note;


        public int date_join;
        public int date_update_lastest;

        public bool p_view;
        public bool p_edit;
        public bool p_remove;

        public bool status;
    }
}
