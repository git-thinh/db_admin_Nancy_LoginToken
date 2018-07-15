using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace host
{

    [Serializable]
    public class msgPara
    {
        public string guid_id { set; get; }
         
        public string modkey { set; get; }
        public string api { set; get; }
        public string callback { set; get; }
        public string data_type { set; get; }
        public string data { set; get; }
        public string config { set; get; }

        public int page_number { set; get; }
        public int page_size { set; get; }
        public int row_total { set; get; }
        public int row_count { set; get; }
    }
}
