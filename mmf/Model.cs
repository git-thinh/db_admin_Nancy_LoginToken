//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Runtime.InteropServices;
//using System.Text;

//namespace mmf
//{
//    [StructLayout(LayoutKind.Sequential)]
//    unsafe public struct blob
//    {
//        //[FieldOffset(12)]
//        //[MarshalAs(UnmanagedType.LPStr)]
//        //public string name;

//        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 255)]
//        public byte[] cval;

//        public Int32 b0;

//        public Int32 b1;

//        public Int32 b2;
//    }

//    [Serializable]
//    [StructLayout(LayoutKind.Sequential, Pack = 1)]
//    public struct r_item_o
//    {
//        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 100)] // Max length of string
//        public string key;
//    }


//    [Serializable]
//    [StructLayout(LayoutKind.Sequential)]
//    unsafe public struct r_item
//    {
//        const int MAXLENGTH = 100;

//        fixed char name_[MAXLENGTH];

//        public r_item(string key_) {
//            key = key_;
//        }

//        public string key
//        {
//            get
//            {
//                fixed (char* n = name_)
//                {
//                    return new String(n);
//                }
//            }
//            set
//            {
//                fixed (char* n = name_)
//                {
//                    int indx = 0;
//                    foreach (char c in value)
//                    {
//                        *(n + indx) = c;
//                        indx++;
//                        if (indx >= MAXLENGTH)
//                            break;
//                    }
//                }
//            }
//        }
//    }

//    //[Serializable]
//    //[StructLayout(LayoutKind.Sequential)]
//    //unsafe public struct r_item
//    //{
//    //    const int MAXLENGTH = 100;

//    //    fixed char name_[MAXLENGTH];

//    //    public int c0;
//    //    public int c1;
//    //    public int c2;

//    //    public string key
//    //    {
//    //        get
//    //        {
//    //            fixed (char* n = name_)
//    //            {
//    //                return new String(n);
//    //            }
//    //        }
//    //        set
//    //        {
//    //            fixed (char* n = name_)
//    //            {
//    //                int indx = 0;
//    //                foreach (char c in value)
//    //                {
//    //                    *(n + indx) = c;
//    //                    indx++;
//    //                    if (indx >= MAXLENGTH)
//    //                        break;
//    //                }
//    //            }
//    //        }
//    //    }
//    //}

//    //[Serializable]
//    //[StructLayout(LayoutKind.Sequential)]
//    //unsafe public struct r_item
//    //{
//    //    const int MAXLENGTH = 100;

//    //    fixed char name_[MAXLENGTH];

//    //    public int c0;
//    //    public int c1;
//    //    public int c2;

//    //    public string key
//    //    {
//    //        get
//    //        {
//    //            fixed (char* n = name_)
//    //            {
//    //                return new String(n);
//    //            }
//    //        }
//    //        set
//    //        {
//    //            fixed (char* n = name_)
//    //            {
//    //                int indx = 0;
//    //                foreach (char c in value)
//    //                {
//    //                    *(n + indx) = c;
//    //                    indx++;
//    //                    if (indx >= MAXLENGTH)
//    //                        break;
//    //                }
//    //            }
//    //        }
//    //    }
//    //}

//}
