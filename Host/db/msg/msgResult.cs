using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace host
{
    public class msgResult
    {
        public bool ok = true;
        public string msg = "";
        
        public string api = "";
        public string modkey = "";
        public string callback = "";
        public string data_type = "";

        public string config = "";
        public string query = "";

        public long row_total = 0;
        public long row_count = 0;

        //"cols_part":[],
        //"cols_join":[],
        //"cols_query":[],
        //"cols_ok":[],
        //"cols_all":[],
        //"cols_join_val":[],
        //"config":{},
        //"part_ok":[],
        //"part_query":[],
        //"part_all":[],
        //"ok":false,
        //"msg":null,
        //"cache_key":null,
        //"query_id":null,
        //"connect_id":null,
        //"tab_code":null,
        //"col_code_key":null,
        //"key_id":0.0,
        //"page_size":0,
        //"page_number":0,
        //"row_total":0,
        //"row_count":0, 
        //"data":[

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }

    }
}
