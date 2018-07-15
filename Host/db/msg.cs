using System.Linq.Dynamic;
using System.Windows.Forms;
using host.websocket;
using model;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace host
{
    public class msg
    {
        private static string msg_login_permission = JsonConvert.SerializeObject(new { ok = false, msg = "Tài khoản chưa được phân quyền." });

        static Func<object, object[], object> Wrap(MethodInfo method)
        {
            var dm = new DynamicMethod(method.Name, typeof(object), new Type[] {
            typeof(object), typeof(object[])
        }, method.DeclaringType, true);
            var il = dm.GetILGenerator();

            if (!method.IsStatic)
            {
                il.Emit(OpCodes.Ldarg_0);
                il.Emit(OpCodes.Unbox_Any, method.DeclaringType);
            }
            var parameters = method.GetParameters();
            for (int i = 0; i < parameters.Length; i++)
            {
                il.Emit(OpCodes.Ldarg_1);
                il.Emit(OpCodes.Ldc_I4, i);
                il.Emit(OpCodes.Ldelem_Ref);
                il.Emit(OpCodes.Unbox_Any, parameters[i].ParameterType);
            }
            il.EmitCall(method.IsStatic || method.DeclaringType.IsValueType ?
                OpCodes.Call : OpCodes.Callvirt, method, null);
            if (method.ReturnType == null || method.ReturnType == typeof(void))
            {
                il.Emit(OpCodes.Ldnull);
            }
            else if (method.ReturnType.IsValueType)
            {
                il.Emit(OpCodes.Box, method.ReturnType);
            }
            il.Emit(OpCodes.Ret);
            return (Func<object, object[], object>)dm.CreateDelegate(typeof(Func<object, object[], object>));
        }

        public static Tuple<bool, string, msgPara, IList, m_meter[], m_meter_heso[]> ProcessMessage
            (string message, int page_number, int page_size,
            msg_type msg_type_result = msg_type.json,
            string tem_item = "",
            string tem_item_def = "")
        {
            msgPara para = new msgPara() { data_type = "json" };
            string rs_string = "";
            try
            {
                #region //....

                bool ok = true;
                string user_name = "", api = "", modkey = "", callback = "", data_in = "", token_key = "";
                IList rs_data = null;
                m_meter[] rs_info = null;
                m_meter_heso[] rs_heso = null;

                int row_total = 0, row_count = 0;

                string guid_id = "";
                if (!string.IsNullOrEmpty(message))
                {
                    if (message[0] == '#')
                    {
                        guid_id = message.Split('|')[0].Replace("#", string.Empty);
                        message = message.Substring(guid_id.Length + 2, message.Length - (guid_id.Length + 2)).Trim();
                        para.guid_id = guid_id;
                    }

                    api = message.Split('|')[0].ToLower();
                    data_in = message.Substring(api.Length + 1, message.Length - (api.Length + 1));
                    string[] a_data = data_in.Split('|');
                    token_key = a_data[a_data.Length - 1];
                    switch (api)
                    {
                        // api | callback | data_base64 | token

                        #region // ... bluetooth update fday...

                        case "blue_sync_fday":
                            #region
                            try
                            {
                                string[] a_blue_sync_fday = data_in.Split('|').Where(o => o.Trim() != "").ToArray(); ;
                                if (a_blue_sync_fday.Length > 0)
                                {
                                    if (!Directory.Exists("blue_sync_fday")) Directory.CreateDirectory("blue_sync_fday");
                                    string[] dt_sync = a_blue_sync_fday[a_blue_sync_fday.Length - 1].Trim()
                                        .Split(new string[] { Environment.NewLine, "\n", "\r" }, StringSplitOptions.None);

                                    List<m_blue_data> li = new List<m_blue_data>() { };
                                    List<long> li_device = new List<long>() { };

                                    foreach (string si in dt_sync)
                                    {
                                        var av = si.Split('-');//.Select(x => x.TryParseToDouble()).ToArray();
                                        if (av.Length == 4)
                                        {

                                            try
                                            {
                                                string[] a = av;
                                                string s_val = a[a.Length - 1];

                                                m_blue_data m = new m_blue_data();
                                                m.data_type = d2_data.FIX_DAY;

                                                m.device_id = a[0].Trim().TryParseToLong();
                                                m.HHmmss = a[2].TryParseToInt();
                                                m.yyMMdd = a[1].Substring(2, 6).TryParseToInt();
                                                m.index_date = m.yyMMdd.IntDateToDayOfYear();
                                                m.yyMMddHHmm = (a[1] + a[2]).Substring(2, 10).Trim().TryParseToUInt32();
                                                m.yy = a[1].Substring(2, 2).TryParseToByte();
                                                m.value = s_val.TryParseToDouble();

                                                li_device.Add(m.device_id);
                                                li.Add(m);
                                            }
                                            catch { }

                                            Task.Factory.StartNew((object obj) =>
                                            {
                                                File.WriteAllText(@"blue_update_fday\" + obj.ToString(), "", Encoding.ASCII);
                                            }, si);
                                        }
                                    }//end for

                                    foreach (var m in li) store.blue_update_add(m);
                                    store.device_addItems(li_device);

                                    rs_string = "ok";
                                }
                            }
                            catch { }
                            break;
                            #endregion
                        case "blue_update_fday":
                            #region
                            try
                            {
                                string[] a_blue_update_fday = data_in.Split('|').Where(x => x.Trim() != "").ToArray();
                                if (a_blue_update_fday.Length > 1)
                                {
                                    long mid = a_blue_update_fday[0].Trim().TryParseToLong();
                                    double val = a_blue_update_fday[1].Trim().TryParseToDouble();

                                    store.blue_update_fday(mid, val);

                                    if (!Directory.Exists("blue_update_fday")) Directory.CreateDirectory("blue_update_fday");
                                    string fi = a_blue_update_fday[0].Trim() + DateTime.Now.ToString("-yyyyMMdd-HHmmss-") + a_blue_update_fday[1].Trim() + ".txt";

                                    Task.Factory.StartNew((object obj) =>
                                    {
                                        File.WriteAllText(@"blue_update_fday\" + obj.ToString(), a_blue_update_fday[1], Encoding.ASCII);
                                    }, fi);

                                    rs_string = "ok";
                                }

                            }
                            catch { }
                            break;
                            #endregion
                        #endregion

                        #region // ... bluetooth update fmon...

                        case "blue_sync_fmon":
                            #region

                            try
                            {
                                string[] a_blue_sync_fmon = data_in.Split('|').Where(o => o.Trim() != "").ToArray(); ;
                                if (a_blue_sync_fmon.Length > 0)
                                {
                                    string[] dt_sync = a_blue_sync_fmon[a_blue_sync_fmon.Length - 1].Trim().Split(new string[] { Environment.NewLine, "\n", "\r" }, StringSplitOptions.None);

                                    List<m_blue_data> li = new List<m_blue_data>() { };
                                    List<long> li_device = new List<long>() { };

                                    if (!Directory.Exists("blue_sync_fmon")) Directory.CreateDirectory("blue_sync_fmon");

                                    foreach (string si in dt_sync)
                                    {
                                        var av = si.Split('-');//.Select(x => x.TryParseToDouble()).ToArray();
                                        if (av.Length == 4)
                                        {
                                            try
                                            {
                                                string[] a = av;
                                                string s_val = a[a.Length - 1];

                                                m_blue_data m = new m_blue_data();
                                                m.data_type = d2_data.FIX_MONTH;

                                                m.device_id = a[0].Trim().TryParseToLong();
                                                m.HHmmss = a[2].TryParseToInt();
                                                m.yyMMdd = a[1].Substring(2, 6).TryParseToInt();
                                                m.index_date = m.yyMMdd.IntDateToDayOfYear();
                                                m.yyMMddHHmm = (a[1] + a[2]).Substring(2, 10).Trim().TryParseToUInt32();
                                                m.yy = a[1].Substring(2, 2).TryParseToByte();
                                                m.value = s_val.TryParseToDouble();

                                                li_device.Add(m.device_id);
                                                li.Add(m);
                                            }
                                            catch { }

                                            Task.Factory.StartNew((object obj) =>
                                            {
                                                File.WriteAllText(@"blue_update_fmon\" + obj.ToString(), "", Encoding.ASCII);
                                            }, si);
                                        }
                                    }//end for

                                    foreach (var m in li) store.blue_update_add(m);
                                    store.device_addItems(li_device);

                                    rs_string = "ok";
                                }
                            }
                            catch { }

                            #endregion
                            break;
                        case "blue_update_fmon":
                            #region

                            string[] a_blue_update_fmon = data_in.Split('|').Where(o => o.Trim() != "").ToArray(); ;
                            if (a_blue_update_fmon.Length > 1)
                            {
                                long mid = a_blue_update_fmon[0].Trim().TryParseToLong();
                                double val = a_blue_update_fmon[1].Trim().TryParseToDouble();

                                store.blue_update_fmon(mid, val);
                                if (!Directory.Exists("blue_update_fmon")) Directory.CreateDirectory("blue_update_fmon");
                                string fi = a_blue_update_fmon[0].Trim() + DateTime.Now.ToString("-yyyyMMdd-HHmmss-") + a_blue_update_fmon[1].Trim() + ".txt";

                                Task.Factory.StartNew((object obj) =>
                                {
                                    File.WriteAllText(@"blue_update_fmon\" + obj.ToString(), a_blue_update_fmon[1], Encoding.ASCII);
                                }, fi);

                                rs_string = "ok";
                            }

                            #endregion
                            break;

                        #endregion

                        #region // ... bluetooth update tstt, tsvh ...

                        case "blue_sync_tstt":
                            #region
                            try
                            {
                                string[] a_blue_sync_tstt = data_in.Split('|').Where(o => o.Trim() != "").ToArray();
                                if (a_blue_sync_tstt.Length > 0)
                                {
                                    if (!Directory.Exists("blue_sync_tstt")) Directory.CreateDirectory("blue_sync_tstt");
                                    string[] dt_sync = a_blue_sync_tstt[a_blue_sync_tstt.Length - 1].Trim().Split(new string[] { Environment.NewLine, "\n", "\r" }, StringSplitOptions.None);

                                    List<m_blue_data> li = new List<m_blue_data>() { };
                                    List<long> li_device = new List<long>() { };

                                    foreach (string si in dt_sync)
                                    {
                                        var av = si.Split('-');//.Select(x => x.TryParseToDouble()).ToArray();
                                        if (av.Length == 4)
                                        {
                                            try
                                            {
                                                string[] a = av;
                                                string s_val = a[a.Length - 1];

                                                m_blue_data m = new m_blue_data();
                                                m.data_type = d2_data.TSVH;

                                                m.device_id = a[0].Trim().TryParseToLong();
                                                m.HHmmss = a[2].TryParseToInt();
                                                m.yyMMdd = a[1].Substring(2, 6).TryParseToInt();
                                                m.index_date = m.yyMMdd.IntDateToDayOfYear();
                                                m.yyMMddHHmm = (a[1] + a[2]).Substring(2, 10).Trim().TryParseToUInt32();
                                                m.yy = a[1].Substring(2, 2).TryParseToByte();
                                                m.value = s_val.TryParseToDouble();

                                                li_device.Add(m.device_id);
                                                li.Add(m);
                                            }
                                            catch { }

                                            Task.Factory.StartNew((object obj) =>
                                            {
                                                File.WriteAllText(@"blue_update\" + obj.ToString(), "", Encoding.ASCII);
                                            }, si);
                                        }
                                    }//end for

                                    foreach (var m in li) store.blue_update_add(m);
                                    store.device_addItems(li_device);

                                    rs_string = "ok";
                                }
                            }
                            catch { }
                            break;
                            #endregion
                        case "blue_update_tstt":
                            #region
                            try
                            {
                                string[] a_blue_update_tstt = data_in.Split('|').Where(x => x.Trim() != "").ToArray();
                                if (a_blue_update_tstt.Length > 1)
                                {
                                    long mid = a_blue_update_tstt[0].Trim().TryParseToLong();
                                    double val = a_blue_update_tstt[1].Trim().TryParseToDouble();

                                    store.blue_update_tsvh(mid, val);

                                    if (!Directory.Exists("blue_update")) Directory.CreateDirectory("blue_update");
                                    string fi = a_blue_update_tstt[0].Trim() + DateTime.Now.ToString("-yyyyMMdd-HHmmss-") + a_blue_update_tstt[1].Trim() + ".txt";

                                    Task.Factory.StartNew((object obj) =>
                                    {
                                        File.WriteAllText(@"blue_update\" + obj.ToString(), a_blue_update_tstt[1], Encoding.ASCII);
                                    }, fi);

                                    rs_string = "ok";
                                }
                            }
                            catch { }
                            break;

                            #endregion

                        #endregion

                        #region // ping, token_encode, login ...

                        case "ping":
                            rs_string = "Tiếng việt: demo at " + DateTime.Now.ToString();
                            break;
                        case "token_encode":
                            #region
                            para.data_type = "text";
                            if (a_data.Length > 1)
                            {
                                callback = a_data[0];
                                rs_string = tokenLogin.decode_get_username(token_key);
                            }
                            break;
                            #endregion
                        case "login":
                            #region
                            string[] v_a = data_in.Split('|');
                            if (v_a.Length > 2)
                            {
                                string v_username = v_a[1].Trim(), v_password = v_a[2].Trim();
                                var mlogin = db_user.login(v_username, v_password);
                                rs_string = JsonConvert.SerializeObject(mlogin);
                            }
                            break;
                            #endregion

                        #endregion

                        default:
                            #region ...

                            Dictionary<string, string> config = new Dictionary<string, string>() { };
                            para = JsonConvert.DeserializeObject<msgPara>(data_in);
                            if (string.IsNullOrEmpty(para.data_type)) para.data_type = "json";
                            api = para.api;
                            modkey = para.modkey;
                            callback = para.callback;
                            data_in = para.data;

                            string config_s = para.config;

                            #region /// Set Config ...

                            //Encode: btoa(string);	
                            //Decode: atob(string);
                            //Encode: System.Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(plainTextBytes));	
                            //Decode: System.Text.Encoding.UTF8.GetString(System.Convert.FromBase64String(base64EncodedData));

                            if (!string.IsNullOrEmpty(data_in)) data_in = HttpUtility.UrlDecode(System.Text.Encoding.UTF8.GetString(System.Convert.FromBase64String(data_in)));

                            if (!string.IsNullOrEmpty(config_s))
                            {
                                config_s = config_s.f_Base64ToString();
                                para.config = config_s.f_StringToBase64();

                                try
                                {
                                    config = JsonConvert.DeserializeObject<JObject>(config_s).ToObject<Dictionary<string, string>>();
                                }
                                catch
                                {
                                    rs_string = "[Config] query error format json";
                                    ok = false;
                                }
                            }

                            #endregion

                            string p_db_data = "";
                            config.TryGetValue("data", out p_db_data);
                            if (!string.IsNullOrWhiteSpace(p_db_data)) p_db_data = p_db_data.f_Base64ToString();

                            string s_cache = "";
                            bool cache = true;
                            config.TryGetValue("cache", out s_cache);
                            if (!string.IsNullOrWhiteSpace(s_cache))
                            {
                                s_cache = s_cache.ToLower().Trim();
                                if (s_cache == "0" || s_cache == "false") cache = false;
                            }

                            if (ok && !string.IsNullOrEmpty(api))
                            {
                                #region // page_number; page_size ............................

                                config.TryGetValue("user_name", out user_name);

                                if (page_size == 0)
                                {
                                    string s_page_number = "", s_page_size = "";
                                    config.TryGetValue("page_number", out s_page_number);
                                    config.TryGetValue("page_size", out s_page_size);
                                    page_number = s_page_number.TryParseToInt();
                                    page_size = s_page_size.TryParseToInt();
                                }

                                if (page_number == 0) page_number = 1;
                                if (page_size == 0) page_size = 10;

                                string[] arr0 = api.Split('/');
                                api = arr0[0];
                                string api_sub = "";
                                if (arr0.Length > 1) api_sub = arr0[1];

                                #endregion

                                switch (api)
                                {
                                    case "node_cache":
                                        #region

                                        switch (api_sub)
                                        {
                                            default:
                                                string s_id_parent = "";
                                                config.TryGetValue("id_parent", out s_id_parent);
                                                long node_id = s_id_parent.TryParseToLong();
                                                ok = true;
                                                rs_string = db_node.cache_node_get(node_id);
                                                break;
                                            case "join_ids_add":
                                                string join_ids_add = "";
                                                config.TryGetValue("ids", out join_ids_add);
                                                long[] ids_meter = join_ids_add.Split(';').Select(x => x.TryParseToLong()).ToArray();

                                                string s_id_nhom_join = "";
                                                config.TryGetValue("id_nhom", out s_id_nhom_join);
                                                long id_nhom_join = s_id_nhom_join.TryParseToLong();

                                                string s_id_parent_join = "";
                                                config.TryGetValue("id_parent", out s_id_parent_join);
                                                long id_parent_join = s_id_parent_join.TryParseToLong();

                                                //int k_join = db_kh_nhom.join_item_add(id_nhom_join, ids_meter);
                                                int k_join = db_node.join_device_add(id_parent_join, id_nhom_join, ids_meter);

                                                rs_string = k_join.ToString();
                                                para.data_type = "text";

                                                //rs.row_total = k_join;
                                                //rs.row_count = ids_meter.Length;

                                                break;
                                            case "join_ids_remove":
                                                string join_ids_del = "";
                                                config.TryGetValue("ids", out join_ids_del);
                                                long[] ids_meter_del = join_ids_del.Split(';').Select(x => x.TryParseToLong()).ToArray();

                                                string s_id_nhom_join_del = "";
                                                config.TryGetValue("id_nhom", out s_id_nhom_join_del);
                                                long id_nhom_join_del = s_id_nhom_join_del.TryParseToLong();

                                                string s_id_parent_join_del = "";
                                                config.TryGetValue("id_parent", out s_id_parent_join_del);
                                                long id_parent_join_del = s_id_parent_join_del.TryParseToLong();

                                                //int k_join_del = db_kh_nhom.join_item_remove(id_nhom_join_del, ids_meter_del);
                                                int k_join_del = db_node.join_device_remove(id_parent_join_del, id_nhom_join_del, ids_meter_del);

                                                rs_string = k_join_del.ToString();
                                                para.data_type = "text";

                                                break;
                                        }
                                        break;
                                        #endregion
                                    case "index":
                                        #region // ...
                                        string pi_db_tab = "";
                                        config.TryGetValue("db_tab", out pi_db_tab);
                                        string pi_db_where = "";
                                        config.TryGetValue("db_where", out pi_db_where);
                                        string pi_db_select = "";
                                        config.TryGetValue("db_select", out pi_db_select);
                                        string pi_db_orderby = "";
                                        config.TryGetValue("db_orderby", out pi_db_orderby);
                                        string pi_db_distinct = "";
                                        config.TryGetValue("db_distinct", out pi_db_distinct);
                                        string pi_db_index = "";
                                        config.TryGetValue("db_index", out pi_db_index);

                                        var rs_dbix = store.db_where_index(pi_db_index, pi_db_tab, pi_db_select, pi_db_where, pi_db_orderby, pi_db_distinct, page_number, page_size);
                                        if (rs_dbix.Item1)
                                        {
                                            ok = true;
                                            row_total = rs_dbix.Item3;
                                            row_count = rs_dbix.Item4;
                                            rs_data = rs_dbix.Item5;
                                        }
                                        else
                                        {
                                            ok = false;
                                            rs_string = rs_dbix.Item2;
                                        }
                                        break;
                                        #endregion
                                    case "db":
                                        #region // ... query, update ...

                                        string p_db_item = "";
                                        config.TryGetValue("db_item", out p_db_item);
                                        if (!string.IsNullOrEmpty(p_db_item))
                                            p_db_item = p_db_item.f_Base64ToString();


                                        switch (api_sub)
                                        {
                                            default:
                                                #region // ... where ...

                                                string p_db_tab = "";
                                                config.TryGetValue("db_tab", out p_db_tab);
                                                string p_db_key = "";
                                                config.TryGetValue("db_key", out p_db_key);

                                                string p_db_where = "";
                                                config.TryGetValue("db_where", out p_db_where);
                                                string p_db_select = "";
                                                config.TryGetValue("db_select", out p_db_select);
                                                string p_db_orderby = "";
                                                config.TryGetValue("db_orderby", out p_db_orderby);
                                                string p_db_distinct = "";
                                                config.TryGetValue("db_distinct", out p_db_distinct);

                                                var rs_dbW = store.db_where(p_db_key, p_db_tab, p_db_select, p_db_where, p_db_orderby, p_db_distinct, page_number, page_size);
                                                if (rs_dbW.Item1)
                                                {
                                                    ok = true;
                                                    row_total = rs_dbW.Item3;
                                                    row_count = rs_dbW.Item4;
                                                    rs_data = rs_dbW.Item5;

                                                    //// var dt_ex = rs_dbW.Item5;
                                                    //// convert 
                                                    //if (is_export)
                                                    //{
                                                    //    var dynamicArray = rs_dbW.Item5.ToDynamicArray();
                                                    //    List<Object> listTarget = new List<Object>();
                                                    //    foreach (var model in dynamicArray)
                                                    //    {
                                                    //        Object newObject = model;
                                                    //        listTarget.Add(model);
                                                    //    }
                                                    //    rs_data = (IList)listTarget;
                                                    //}
                                                }
                                                else
                                                {
                                                    ok = false;
                                                    //rs_string = rs_dbW.Item4;
                                                }
                                                break;

                                                #endregion
                                            case "id":
                                                string p_db_tab_id = "";
                                                config.TryGetValue("db_tab", out p_db_tab_id);
                                                string p_db_key_id = "";
                                                config.TryGetValue("db_key", out p_db_key_id);

                                                var rsDB_id = store.db_get_id(p_db_key_id, p_db_tab_id);
                                                ok = rsDB_id.Item1;
                                                rs_string = rsDB_id.Item2;
                                                rs_data = rsDB_id.Item3;
                                                break;
                                            case "add":
                                                string p_db_tab_add = "";
                                                config.TryGetValue("db_tab", out p_db_tab_add);
                                                string p_db_key_add = "";
                                                config.TryGetValue("db_key", out p_db_key_add);

                                                var rsDB_add = store.db_add(p_db_key_add, p_db_tab_add, p_db_item);
                                                ok = rsDB_add.Item1;
                                                rs_string = rsDB_add.Item2;
                                                rs_data = new dynamic[] { rsDB_add.Item3 };
                                                break;
                                            case "edit":
                                                string p_db_tab_edit = "";
                                                config.TryGetValue("db_tab", out p_db_tab_edit);
                                                string p_db_key_edit = "";
                                                config.TryGetValue("db_key", out p_db_key_edit);

                                                var rsDB_edit = store.db_edit(p_db_key_edit, p_db_tab_edit, p_db_item);
                                                ok = rsDB_edit.Item1;
                                                rs_string = rsDB_edit.Item2;
                                                rs_data = new dynamic[] { rsDB_edit.Item3 };
                                                break;
                                            case "remove":
                                                string p_db_tab_remove = "";
                                                config.TryGetValue("db_tab", out p_db_tab_remove);
                                                string p_db_key_remove = "";
                                                config.TryGetValue("db_key", out p_db_key_remove);

                                                var rsDB_remove = store.db_remove(p_db_key_remove, p_db_tab_remove, p_db_item);
                                                ok = rsDB_remove.Item1;
                                                rs_string = rsDB_remove.Item2;
                                                rs_data = new dynamic[] { rsDB_remove.Item3 };
                                                break;
                                        }
                                        break;
                                        #endregion
                                    case "store":
                                        #region

                                        string s_id_nhom_db = "";
                                        config.TryGetValue("p_nhom", out s_id_nhom_db);
                                        long id_nhom_db = s_id_nhom_db.TryParseToLong();

                                        string s_tree_item_type = "";
                                        config.TryGetValue("item_type", out s_tree_item_type);

                                        long[] aids_data = new long[] { };
                                        if (id_nhom_db > 0)
                                        {
                                            if (s_tree_item_type == "item")
                                            {
                                                if (id_nhom_db.ToString().Length < 5)
                                                    aids_data = store.device_find(id_nhom_db.ToString());
                                                else
                                                    aids_data = new long[] { id_nhom_db };
                                            }
                                            else
                                                //aids_data = db_kh_nhom.query_byNhomID(id_nhom_db, 1, 10000).Item3;
                                                aids_data = db_node.get_IDs_device(id_nhom_db);
                                        }

                                        string s_data_type_db = "", s_data_type_detail = "", s_date = "";

                                        config.TryGetValue("p_data_type", out s_data_type_db);
                                        config.TryGetValue("p_data_type_detail", out s_data_type_detail);
                                        config.TryGetValue("p_date", out s_date);

                                        int[] data_type_db = new int[] { };
                                        int[] data_type_detail = new int[] { };

                                        if (!string.IsNullOrEmpty(s_data_type_db))
                                            data_type_db = s_data_type_db.Split(';').Select(x => x.TryParseToInt()).Where(x => x > 0).ToArray();
                                        if (!string.IsNullOrEmpty(s_data_type_detail))
                                            data_type_detail = s_data_type_detail.Split(';').Select(x => x.TryParseToInt()).Where(x => x > 0).ToArray();



                                        List<Tuple<byte, int>> date = new List<Tuple<byte, int>>() { };
                                        if (!string.IsNullOrEmpty(s_date))
                                            date = msgConverter.f_DateKeyArrayToIndexCacheYY(s_date);

                                        string key_cache = s_data_type_db + s_date + string.Join("", aids_data);

                                        Tuple<long, long, int[], long[], List<decimal[]>> rs_dt_query = new Tuple<long, long, int[], long[], List<decimal[]>>(0, 0, new int[] { }, new long[] { }, new List<decimal[]>() { });

                                        if (data_type_detail.Length > 0)
                                            rs_dt_query = store.query(
                                                key_cache,
                                                date, aids_data,
                                                data_type_detail, e_query_data.data_detail,
                                                null,
                                                page_number, page_size);
                                        else
                                            rs_dt_query = store.query(
                                                key_cache,
                                                date, aids_data,
                                                data_type_db, e_query_data.data_type,
                                                null,
                                                page_number, page_size);


                                        Dictionary<int, m_column[]> dic_cols = new Dictionary<int, m_column[]>() { };
                                        foreach (int d_type in rs_dt_query.Item3)
                                        {
                                            m_column[] a_cols = db_column.get_Items(d_type);
                                            if (!dic_cols.ContainsKey(d_type))
                                                dic_cols.Add(d_type, a_cols);
                                        }

                                        row_total = (int)rs_dt_query.Item1;
                                        row_count = (int)rs_dt_query.Item2;

                                        var dta = rs_dt_query.Item5;
                                        var dta_ids = rs_dt_query.Item4;
                                        if (dta.Count > 0)
                                        {
                                            rs_info = db_meter.get_Items(dta_ids);
                                            ///data_info = JsonConvert.SerializeObject(rs_info);
                                        }

                                        string json_rs = "";
                                        if (dta.Count > 0)
                                        {
                                            List<string> ls_json = new List<string>() { };
                                            ls_json = msg_render.store(msg_type_result, dta, dic_cols, tem_item, tem_item_def);

                                            switch (msg_type_result)
                                            {
                                                case msg_type.table_td:
                                                    para.data_type = msg_type_result.ToString();
                                                    rs_string = string.Join(Environment.NewLine, ls_json.ToArray());
                                                    break;
                                                case msg_type.xml:
                                                    break;
                                                case msg_type.json:
                                                    rs_string = "[" + string.Join(",", ls_json.ToArray()) + "]";
                                                    break;
                                            }
                                        }

                                        //int stt_end_item = page_size * page_number;

                                        //if (s_tree_item_type == "group" && stt_end_item > dta.Count)
                                        //{
                                        //    row_count = aids_data.Length;
                                        //    row_total = aids_data.Length;                                            

                                        //    if (stt_end_item > dta.Count)
                                        //    {
                                        //        json_rs = json_rs.Replace("{#####}", "      ");
                                        //    }
                                        //}

                                        //if (json_rs != "") 
                                        //{
                                        //    rs_string = "[" + json_rs.Replace("{#####}","") + "]";
                                        //}

                                        //rs_string = JsonConvert.SerializeObject(lsout);

                                        break;

                                        #endregion
                                    case "load_json":
                                        rs_string = msgQuery.load_Json(config);
                                        break;
                                    case "load_window":
                                        para.data_type = "html";
                                        rs_string = msgQuery.load_Window(config);
                                        break;
                                    case "query_meter":
                                        switch (api_sub)
                                        {
                                            case "change_meter":
                                                string data_out = db_meter.edit_ItemChange_meter(p_db_data);
                                                break;
                                        }
                                        break;
                                }
                            }
                            break;

                            #endregion
                    }
                }

                if (ok)
                {
                    para.row_count = row_count;
                    para.row_total = row_total;
                    para.page_number = page_number;
                    para.page_size = page_size;

                    return new Tuple<bool, string, msgPara, IList, m_meter[], m_meter_heso[]>(true, rs_string, para, rs_data, rs_info, rs_heso);
                }

                return new Tuple<bool, string, msgPara, IList, m_meter[], m_meter_heso[]>(false, rs_string, para, null, null, null);

                #endregion
            }
            catch (Exception ex)
            {
                rs_string = "msg.cs >> ProcessMessage() >> Error: " + ex.Message;
                return new Tuple<bool, string, msgPara, IList, m_meter[], m_meter_heso[]>(false, rs_string, para, null, null, null);
            }
        }


        public static void ProcessMessage(IWebSocketConnection client, string message)
        {
            var ds = ProcessMessage(message, 0, 0);
            bool ok = ds.Item1;
            string rs_string = ds.Item2.Trim();
            msgPara pr = ds.Item3;
            IList rs_data = ds.Item4;
            m_meter[] rs_info = ds.Item5;
            m_meter_heso[] rs_heso = ds.Item6;

            string data_type_out = pr.data_type;
            //if (rs_string.IndexOf('[') == 0 || rs_string.IndexOf('{') == 0) data_type_out = "json";

            if (ok)
            {
                switch (data_type_out)
                {
                    case "text":
                        string s_text = "#|" + pr.callback + "|" + pr.config + "|" + rs_string.f_StringToBase64();
                        client.Send(s_text);
                        break;
                    default:
                        var rs = new msgResult()
                        {
                            msg = rs_string.f_StringToBase64(),
                            ok = true,
                            api = pr.api,
                            callback = pr.callback,
                            data_type = pr.data_type,
                            modkey = pr.modkey,
                            config = pr.config,
                            row_total = pr.row_total,
                            row_count = pr.row_count
                        };

                        string data_out = "", data_info = "", data_heso = "";
                        if (rs_data != null && rs_data.Count > 0) data_out = JsonConvert.SerializeObject(rs_data); else data_out = rs_string;
                        if (rs_info != null && rs_info.Length > 0) data_info = JsonConvert.SerializeObject(rs_info);
                        if (rs_heso != null && rs_heso.Length > 0) data_heso = JsonConvert.SerializeObject(rs_heso);

                        if (data_out == null) data_out = "";
                        if (data_info == null) data_info = "";
                        if (data_heso == null) data_heso = "";

                        string val_main = data_out.f_StringToBase64();
                        string val_info = data_info.f_StringToBase64();
                        string val_heso = data_heso.f_StringToBase64();

                        if (!string.IsNullOrEmpty(pr.guid_id)) val_main = "#" + pr.guid_id + "|" + val_main;
                        string s_result = rs.ToString();

                        string sjson = s_result.Substring(0, s_result.Length - 1);
                        sjson += @",""data"":""" + val_main + @""",""data_heso"":""" + val_heso + @""",""data_info"":""" + val_info + @"""}";

                        client.Send(sjson);

                        break;
                }
            }
            else
            {
                string data_out = msgQuery.jsonError(pr.modkey, pr.callback, rs_string);
                client.Send(data_out);
            }
        }

    } //end class web socket


}
