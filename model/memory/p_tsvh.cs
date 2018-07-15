using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

using Newtonsoft.Json;

namespace model.memory
{

    [StructLayout(LayoutKind.Sequential)]
    public struct p1_tsvh
    {
        public byte pkey;       // primakey store: only on system ... 
        public byte type;       // type device
        public long msg;        // file id; message id
        public long id;         // device id: meter_id
        public long fid;        // father id: dcu_id, imei

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this, Formatting.Indented);
        }

        public int i0;
        public int i1;
        public int i2;
        public int i3;
        public int i4;
        public int i5;
        public int i6;
        public int i7;
        public int i8;
        public int i9;

        public int i10;
        public int i11;
        public int i12;
        public int i13;
        public int i14;
        public int i15;
        public int i16;
        public int i17;
        public int i18;
        public int i19;
    }

    [Serializable]
    [StructLayout(LayoutKind.Sequential)]
    unsafe public struct p3_tsvh
    {
        public byte pkey;       // primakey store: only on system ... 
        public byte type;       // type device
        public long msg;        // file id; message id
        public long id;         // device id: meter_id
        public long fid;        // father id: dcu_id, imei

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this, Formatting.Indented);
        }

        public long Q1;
        public long Q2;
        public long Q3;
        public long Q4;

        public long CD1;
        public long CD2;
        public long CD3;

        public long PGiao1;
        public long PGiao2;
        public long PGiao3;

        public long PNhan1;
        public long PNhan2;
        public long PNhan3;

        public int TimerMeter;
        public int TIME;
        public int PGiaoTong;
        public int PNhanTong;
        public int STong;

        public int UA;
        public int UB;
        public int UC;

        public int IA;
        public int IB;
        public int IC;

        public int FregA;
        public int FregB;
        public int FregC;

        public int AngleA;
        public int AngleB;
        public int AngleC;

        public int CosA;
        public int CosB;
        public int CosC;

        public int PA;
        public int PB;
        public int PC;

        public int QA;
        public int QB;
        public int QC;

        public int SA;
        public int SB;
        public int SC;

        public int MDS1;
        public int MDS1_Time;
        public int MDS2;
        public int MDS2_Time;
        public int MDS3;
        public int MDS3_Time;
        public int MDS4;
        public int MDS4_Time;
        public int MDS5;
        public int MDS5_Time;
        public int MDS6;
        public int MDS6_Time;
        public int MDS7;
        public int MDS7_Time;
        public int MDS8;
        public int MDS8_Time;

        public int TU;
        public int TI;
        public int ThutuPha;
        public int HSN;
    }


    //[Serializable]
    //[StructLayout(LayoutKind.Sequential, Pack = 1)]
    //public struct r_item_file
    //{
    //    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 100)] // Max length of string
    //    public string key;

    //    public int c0;
    //    public int c1;
    //    public int c2;
    //}

    //[Serializable]
    //[StructLayout(LayoutKind.Sequential)]
    //unsafe public struct r_item_ram
    //{
    //    const int MAXLENGTH = 100;

    //    fixed char name_[MAXLENGTH];

    //    public int c0;
    //    public int c1;
    //    public int c2;

    //    public string key
    //    {
    //        get
    //        {
    //            fixed (char* n = name_)
    //            {
    //                return new String(n);
    //            }
    //        }
    //        set
    //        {
    //            fixed (char* n = name_)
    //            {
    //                int indx = 0;
    //                foreach (char c in value)
    //                {
    //                    *(n + indx) = c;
    //                    indx++;
    //                    if (indx >= MAXLENGTH)
    //                        break;
    //                }
    //            }
    //        }
    //    }
    //}


}
