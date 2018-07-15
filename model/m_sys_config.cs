using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace model
{
    [Serializable]
    [StructLayout(LayoutKind.Sequential, Pack = 1, CharSet = CharSet.Unicode)]
    public struct m_sys_config
    {
        /// <summary>
        /// Tên cấu hình
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 255)]
        public string name;

        /// <summary>
        /// IP localhost: 127.0.0.1; 192.168.1.123... 
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 255)]
        public string ip_local;
        /// <summary>
        /// IP NAT (IP tĩnh) trỏ ngoại mạng để cho phép truy cập vào IP Local
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 255)]
        public string ip_nat;

        /// <summary>
        /// IP socket truyền dữ liệu
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 255)]
        public string socket_ip;
        /// <summary>
        /// Domain socket truyền dữ liệu
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 255)]
        public string socket_domain;        
        /// <summary>
        /// PORT của kết nối socket dữ liệu
        /// </summary>
        public int socket_port;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 255)]
        public string login_ip;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 255)]
        public string login_domain;        
        public int login_port;

        /// <summary>
        /// IP truy cập site chính
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 255)]
        public string site_ip;
        /// <summary>
        /// Domain sẽ ánh xạ vào site_ip
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 255)]
        public string site_domain;        
        public int site_port;
        
        /// <summary>
        /// Đường dẫn nguồn file dữ liệu phân tích
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 1000)]
        public string path_file;
        /// <summary>
        /// Đường dẫn lưu trữ cơ sở dữ liệu file
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 1000)]
        public string path_db;
        /// <summary>
        /// Đường dẫn các page của site
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 1000)]
        public string path_site;  
    }
}
