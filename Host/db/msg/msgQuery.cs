using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace host
{

    public static class msgQuery
    {
        public static string jsonError(string key, string func_name, string msg)
        {
            var o = new { ok = false, msg = msg, func_name = func_name, key = key };
            return JsonConvert.SerializeObject(o);
        }

        public static Tuple<long, long, long[], string> f_db_query_group(Dictionary<string, string> config)
        {
            string s_data_type = "", s_meter = "", s_date = "", s_type = "", s_page_number = "", s_page_size = "";

            config.TryGetValue("p_data_type", out s_data_type);
            config.TryGetValue("p_meter", out s_meter);
            config.TryGetValue("p_date", out s_date);

            int data_type = s_data_type.TryParseToInt();

            config.TryGetValue("page_number", out s_page_number);
            config.TryGetValue("page_size", out s_page_size);
            int page_number = s_page_number.TryParseToInt(), page_size = s_page_size.TryParseToInt();

            if (!string.IsNullOrEmpty(s_date))
            {
                long[] meter = s_meter.Split(';').Select(x => x.TryParseToLong()).Where(x => x > 0).ToArray();
                int date_id = msgConverter.f_DateKeyToIndexCache(s_date.TryParseToInt());

                string key_cache = s_date + s_meter + s_type;
                return store.f_db_query_group(key_cache, 
                    data_type, 
                    date_id, meter, 
                    page_number, page_size);
            }

            return null;
        }

        public static Tuple<long, int, string> f_db_query_item(Dictionary<string, string> config)
        {
            string s_data_type = "", s_meter = "", s_date = "", s_type = "", s_page_number = "", s_page_size = "";
            config.TryGetValue("p_meter", out s_meter);
            config.TryGetValue("p_date", out s_date);
            config.TryGetValue("p_data_type", out s_data_type);

            int data_type = s_data_type.TryParseToInt();

            config.TryGetValue("page_number", out s_page_number);
            config.TryGetValue("page_size", out s_page_size);
            int page_number = s_page_number.TryParseToInt(), page_size = s_page_size.TryParseToInt();


            if (!string.IsNullOrEmpty(s_date))
            {
                long meter_id = s_meter.TryParseToLong();
                int[] date = msgConverter.f_DateKeyArrayToIndexCache(s_date).Where(x => x > 0).ToArray(); //s_date.Split(';').Select(x => msgConverter.f x.TryParseToInt()).ToArray();

                string key_cache = s_date + s_meter + s_type;
                return store.f_db_query_item(key_cache, data_type, date, meter_id, page_number, page_size);
            }

            return null;
        }

         

        
          

        #region /// api, load_ ...

        public static string load_Json(Dictionary<string, string> config)
        {
            string json = "", path_key = "";
            config.TryGetValue("path", out path_key);
            if (!string.IsNullOrEmpty(path_key))
                json = hostModule.get_json_module(path_key);

            return json;
        }

        public static string load_Window(Dictionary<string, string> config)
        {
            string log_func_name = System.Reflection.MethodBase.GetCurrentMethod().Name;

            string s_para = "", s_html = "", s_css = "", s_js = "", s_message = "", s_config = config.ToString();
            bool hasError = false;

            string module_id = "", theme_key = "", device_type = "", window_id = "";
            config.TryGetValue("theme_key", out theme_key);
            config.TryGetValue("device_type", out device_type);
            config.TryGetValue("window_id", out window_id);
            config.TryGetValue("module_id", out module_id);

            if (string.IsNullOrWhiteSpace(theme_key)) theme_key = hostUser.theme_Default;
            if (string.IsNullOrWhiteSpace(device_type)) theme_key = hostUser.device_Default;

            module_id = module_id.Replace('/','\\');

            string path = hostServer.pathModule + @"\.window\" + module_id + @"\";

            string file_htm = path + "index_.html",
                file_css = path + "bin_" + theme_key + "_" + device_type + ".css",
                file_js = path + "bin_" + theme_key + "_" + device_type + ".js";

            if (File.Exists(file_htm))
                s_html = File.ReadAllText(file_htm);

            if (File.Exists(file_css))
                s_css = File.ReadAllText(file_css);

            if (File.Exists(file_js))
                s_js = File.ReadAllText(file_js);

            string modkey = module_id.Replace('-', '_').Replace('.', '_');
            s_html = s_html.Replace("@modkey", window_id).Replace("___modkey", window_id);
            s_js = s_js.Replace("@modkey", window_id).Replace("___modkey", window_id);
            s_css = s_css.Replace("@modkey", window_id).Replace("___modkey", window_id);

            //hostUser.onWindow(this.Context.ConnectionId, module_id, s_config, s_para, s_html, s_css, s_js, s_message);

            string htm = s_html + Environment.NewLine + @" <style type=""text/css""> " + Environment.NewLine + s_css + Environment.NewLine + @" </style><script type=""text/javascript""> " + Environment.NewLine + s_js + Environment.NewLine + @" </script>";
            return htm;
        }

        #endregion

    }//end class
}
