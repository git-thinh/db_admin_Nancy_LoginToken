using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace model
{
     

    [Serializable]
    [StructLayout(LayoutKind.Sequential, Pack = 1, CharSet = CharSet.Unicode)]
    public struct m_meter_plc
    {
        /// <summary>
        /// IMEI cua DCU PLC
        /// </summary>
        public UInt32 imei;
        
        /// <summary>
        /// STT công tơ trên PLC
        /// </summary>
        public Int16 id; //16bits = 2byte

        /// <summary>
        /// Số công tơ tương ứng
        /// </summary>
        public UInt32 so_cong_to;
        /// <summary>
        /// 1: 1 phase; 3 = 3 phase
        /// </summary>
        public byte phase_id;
    }
}
