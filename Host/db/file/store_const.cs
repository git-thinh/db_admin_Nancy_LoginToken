using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace host
{
    [Serializable]
    public struct d1234
    {
        public d1_pha v1_pha;
        public d2_data v2_data; 
        public d4_tech v3_tech;
        public d5_nsx v4_nsx;

        public d1234(d1_pha p1_pha, d2_data p2_data,   d4_tech p4_tech, d5_nsx p5_nsx)
        {
            v1_pha = p1_pha;
            v2_data = p2_data; 
            v3_tech = p4_tech;
            v4_nsx = p5_nsx;
        }
        
        public override string ToString()
        {
            return v1_pha.ToString() + ";" +    v3_tech.ToString() + ";" + v4_nsx.ToString();
        }
    }

    [Serializable]
    public enum d1_pha
    {
        // (1)-[0]          // loai pha
        // 1,000,000: 1 pha
        // 3,000,000: 3 pha 

        _ = 0,
        _1pha = 1000000,
        _3pha = 3000000
    }

    [Serializable]
    public enum d2_data
    {
        // (2)-[1]          //loai du lieu
        // _,100,000: TSVH
        // _,200,000: TSTT
        // _,300,000: LOAD PROFILE
        // _,400,000: LOAD PROFILE DAY
        // _,500,000: EVEN
        // _,600,000: FIX DAY
        // _,700,000: FIX MONTH 

        _ = 0,
        TSVH = 100000,
        TSTT = 200000,
        LOAD_PROFILE = 300000,
        LOAD_PROFILE_DAY = 400000,
        EVEN = 500000,
        FIX_DAY = 600000,
        FIX_MONTH = 700000,
    }

        // (3)-[2]          // loai bieu
        // 3,_10,000: 3 pha 1 bieu
        // 3,_30,000: 3 pha 3 bieu  

    [Serializable]
    public enum d4_tech
    {
        // [3]              // loai cong nghe
        // _,__1,000: RF
        // _,__2,000: PLC
        // _,__3,000: BLUETOOTH 

        _ = 0,
        RF = 1000,
        PLC = 2000,
        BLUETOOTH = 3000,
        GPRS = 4000,
    }

    [Serializable]
    public enum d5_nsx
    {
        //                  // loai nsx
        // (4)-[a.Length-2][a.Length-1] //2 char lastest
        // _,___,001: PSMART
        // _,___,002: VNSINO
        // _,___,003: OMNI

        _ = 0,
        PSMART = 1,
        VNSINO = 2,
        OMNI = 3,
    }


    // (1)-[0]          // loai pha
    // 1,000,000: 1 pha
    // 3,000,000: 3 pha

    // (2)-[1]          //loai du lieu
    // _,100,000: TSVH
    // _,200,000: TSTT
    // _,300,000: LOAD PROFILE
    // _,400,000: LOAD PROFILE DAY
    // _,500,000: EVEN
    // _,600,000: FIX DAY
    // _,700,000: FIX MONTH

    // (3)-[2]          // loai bieu
    // 3,_10,000: 3 pha 1 bieu
    // 3,_30,000: 3 pha 3 bieu

    // [3]              // loai cong nghe
    // _,__1,000: RF
    // _,__2,000: PLC
    // _,__3,000: BLUETOOTH

    //                  // loai nsx
    // (4)-[a.Length-2][a.Length-1] //2 char lastest
    // _,___,001: PSMART
    // _,___,002: VNSINO
    // _,___,003: OMNI

    public class store_const
    {
        public static d1234 f_data_type
            (string s_line_data, string factory_type )
        {
            s_line_data = s_line_data.Split(new String[] { "\t", " " }, StringSplitOptions.None)[0].Trim();

            d1_pha v1_pha = 0;
            d2_data v2_data = 0; 
            d4_tech v4_tech = 0;
            d5_nsx v5_nsx = 0;

            int k = 0;
            switch (s_line_data)
            {
                case "tsvh": //1pha; 
                    v1_pha = d1_pha._1pha;
                    v2_data = d2_data.TSVH;
                    v4_tech = d4_tech.RF;
                    break;
                case "tsvhvalue": //3pha; 
                    v1_pha = d1_pha._3pha;
                    v2_data = d2_data.TSVH;
                    break;

                case "tstt": //1pha; 
                    v1_pha = d1_pha._1pha;
                    v2_data = d2_data.TSTT;
                    v4_tech = d4_tech.RF;
                    break;
                case "tstt_3pha???"://3pha; 
                    v1_pha = d1_pha._3pha;
                    v2_data = d2_data.TSTT;
                    break;

                case "loadvalue_???"://1pha; 
                    v1_pha = d1_pha._1pha;
                    v2_data = d2_data.LOAD_PROFILE;
                    v4_tech = d4_tech.RF;
                    break;
                case "loadvalue": //3pha; 
                    v1_pha = d1_pha._3pha;
                    v2_data = d2_data.LOAD_PROFILE;
                    break;

                case "load1dayvalue???": //1pha; 
                    v1_pha = d1_pha._1pha;
                    v2_data = d2_data.LOAD_PROFILE_DAY;
                    v4_tech = d4_tech.RF;
                    break;
                case "load1dayvalue": //3pha; 
                    v1_pha = d1_pha._3pha;
                    v2_data = d2_data.LOAD_PROFILE_DAY;
                    break;

                case "even"://1pha; 
                    v1_pha = d1_pha._1pha;
                    v2_data = d2_data.EVEN;
                    v4_tech = d4_tech.RF;
                    break;
                case "eventvalue"://3pha; 
                    v1_pha = d1_pha._3pha;
                    v2_data = d2_data.EVEN;
                    break;

                case "fday": //1pha; 
                    v1_pha = d1_pha._1pha;
                    v2_data = d2_data.FIX_DAY;
                    v4_tech = d4_tech.RF;
                    break;
                case "cscvalue": //3pha; 
                    v1_pha = d1_pha._3pha;
                    v2_data = d2_data.FIX_DAY;
                    break;

                case "fmon": //1pha; 
                    v1_pha = d1_pha._1pha;
                    v2_data = d2_data.FIX_MONTH;
                    v4_tech = d4_tech.RF;
                    break;
                case "fmon_3pha???"://3pha; 
                    v1_pha = d1_pha._3pha;
                    v2_data = d2_data.FIX_MONTH;
                    break;
            }

            if (v2_data == 0 && s_line_data.StartsWith("tsvh"))
            {
                // if PLC is 1 pha - fix day
                v1_pha = d1_pha._1pha;
                v2_data = d2_data.FIX_DAY;
                v4_tech = d4_tech.PLC;
            }

            if (v1_pha == d1_pha._1pha)
            {
                if (factory_type.Contains("psmart")) v5_nsx = d5_nsx.PSMART;
                else if (factory_type.Contains("vnsino")) v5_nsx = d5_nsx.VNSINO;
                else if (factory_type.Contains("omni")) v5_nsx = d5_nsx.OMNI;
            } 


            return new d1234(v1_pha, v2_data,   v4_tech, v5_nsx);
        }


    }
}
