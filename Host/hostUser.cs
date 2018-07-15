using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.Linq.Dynamic;
using model;


namespace host
{
    public static class hostUser
    {
        //public static string page_Admin = ConfigurationManager.AppSettings["s_domain_admin"].Replace("*ip*", hostServer.IP); //admin.com
        public static string page_Main = "*ip*:" + ConfigurationManager.AppSettings["s_port"] + "/" + hostSite.page_main + "." + hostSite.page_ext;// ConfigurationManager.AppSettings["s_domain_main"].Replace("*ip*", hostServer.IP); //amiss.com
        public static string page_Login = "/";// + // ConfigurationManager.AppSettings["s_domain_login"].Replace("*ip*", hostServer.IP);//login.ifc.com.vn
        //public static string page_Site_Member = ConfigurationManager.AppSettings["s_domain_member"].Replace("*ip*", hostServer.IP); //"chart.amiss.com|export.amiss.com|demo.com";

        public static string page_Signal = ConfigurationManager.AppSettings["s_domain_signal"].Replace("*ip*", hostServer.IP);//

        public static string device_Default = "pc";
        public static string lang_Default = "vi";
        public static string theme_Default = "light"; // light
        public static int browser_width_Default = 1366;

        private readonly static object lock_lsUser = new object();
        private readonly static object lock_token = new object();
        private readonly static object lock_R = new object();

        //private static List<m_user> lsUser = new List<m_user>() { };
        private static DictionaryListValues<string, string> dicUserToken = new DictionaryListValues<string, string>() { };
        private static Dictionary<string, string> dicToken_R = new Dictionary<string, string>() { };
        private static Dictionary<string, string> dicToken_LangUser = new Dictionary<string, string>() { };

        private static Dictionary<string, string> dicSession_refUri = new Dictionary<string, string>() { };
        private static Dictionary<string, string> dicSession_DeviceType = new Dictionary<string, string>() { };
        private static Dictionary<string, string> dicSession_ThemeKey = new Dictionary<string, string>() { };
        private static Dictionary<string, int> dicSession_BrowserWidth = new Dictionary<string, int>() { };

        public static void init()
        {
            if (!string.IsNullOrEmpty(hostServer.IP_NAT))
            {
                page_Main = page_Main.Replace("*ip*", hostServer.IP_NAT);
                page_Login = page_Login.Replace("*ip*", hostServer.IP_NAT);

                //page_Admin = ConfigurationManager.AppSettings["s_domain_admin"].Replace("*ip*", hostServer.IP_NAT); //admin.com
                //page_Main = ConfigurationManager.AppSettings["s_domain_main"].Replace("*ip*", hostServer.IP_NAT); //amiss.com
                //page_Login = ConfigurationManager.AppSettings["s_domain_login"].Replace("*ip*", hostServer.IP_NAT);//login.ifc.com.vn                
                //page_Site_Member = ConfigurationManager.AppSettings["s_domain_member"].Replace("*ip*", hostServer.IP_NAT); //"chart.amiss.com|export.amiss.com|demo.com";            
            }
            else if (string.IsNullOrEmpty(hostServer.IP_LOCAL))
            {
                page_Main = page_Main.Replace("*ip*", hostServer.IP);
                page_Login = page_Login.Replace("*ip*", hostServer.IP);

                //page_Admin = ConfigurationManager.AppSettings["s_domain_admin"].Replace("*ip*", hostServer.IP); //admin.com
                //page_Main = ConfigurationManager.AppSettings["s_domain_main"].Replace("*ip*", hostServer.IP); //amiss.com
                //page_Login = ConfigurationManager.AppSettings["s_domain_login"].Replace("*ip*", hostServer.IP);//login.ifc.com.vn                
                //page_Site_Member = ConfigurationManager.AppSettings["s_domain_member"].Replace("*ip*", hostServer.IP); //"chart.amiss.com|export.amiss.com|demo.com";                        
            }
            else
            {
                page_Main = page_Main.Replace("*ip*", hostServer.IP_LOCAL);
                page_Login = page_Login.Replace("*ip*", hostServer.IP_LOCAL);

                //page_Admin = ConfigurationManager.AppSettings["s_domain_admin"].Replace("*ip*", hostServer.IP_LOCAL); //admin.com
                //page_Main = ConfigurationManager.AppSettings["s_domain_main"].Replace("*ip*", hostServer.IP_LOCAL); //amiss.com
                //page_Login = ConfigurationManager.AppSettings["s_domain_login"].Replace("*ip*", hostServer.IP_LOCAL);//login.ifc.com.vn                
                //page_Site_Member = ConfigurationManager.AppSettings["s_domain_member"].Replace("*ip*", hostServer.IP_LOCAL); //"chart.amiss.com|export.amiss.com|demo.com";                        

                page_Signal = ConfigurationManager.AppSettings["s_domain_signal"].Replace("*ip*", hostServer.IP_LOCAL);//
            }

            //lsUser.AddRange(new m_user[] {
            //      new m_user() { user_id = Guid.NewGuid().ToString(), username = "ifc", pass  = "123", status = true }
            //    , new m_user() { user_id = Guid.NewGuid().ToString(), username = "admin", pass  = "admin", status = true }
            //    , new m_user() { user_id = Guid.NewGuid().ToString(), username = "nga", pass  = "nga", status =true }
            //    , new m_user() { user_id = Guid.NewGuid().ToString(), username = "tuan", pass  = "tuan", status = true }
            //    , new m_user() { user_id = Guid.NewGuid().ToString(), username = "thanh", pass  = "thanh", status = true }
            //    , new m_user() { user_id = Guid.NewGuid().ToString(), username = "thinh", pass  = "thinh", status = true }
            //    , new m_user() { user_id = Guid.NewGuid().ToString(), username = "duy", pass  = "duy", status = true }
            //});
        }

        #region >> Session >> theme_key : dark ; light ...

        public static string session_themeKey_Get(string session_id)
        {
            string theme_key = theme_Default;
            dicSession_ThemeKey.TryGetValue(session_id, out theme_key);
            if (string.IsNullOrEmpty(theme_key)) theme_key = theme_Default;
            return theme_key;
        }

        public static void session_themeKey_Set(string session_id, string theme_key)
        {
            if (string.IsNullOrWhiteSpace(session_id)) return;

            if (dicSession_ThemeKey.ContainsKey(session_id))
                dicSession_ThemeKey[session_id] = theme_key;
            else
                dicSession_ThemeKey.Add(session_id, theme_key);
        }

        #endregion

        #region >> Session >> browser width : dark ; light ...

        public static int session_BrowserWidth_Get(string session_id)
        {
            int browser_width = browser_width_Default;
            dicSession_BrowserWidth.TryGetValue(session_id, out browser_width);
            if (browser_width == 0) browser_width = browser_width_Default;
            return browser_width;
        }

        public static void session_BrowserWidth_Set(string session_id, int browser_width)
        {
            if (string.IsNullOrWhiteSpace(session_id)) return;

            if (dicSession_BrowserWidth.ContainsKey(session_id))
                dicSession_BrowserWidth[session_id] = browser_width;
            else
                dicSession_BrowserWidth.Add(session_id, browser_width);
        }

        #endregion

        #region >> Session >> device type: pc, tablet, mobi, mobimi ...

        public static string session_deviceType_Get(Nancy.NancyContext ctx, string session_id)
        {
            string device_type = device_Default;
            dicSession_DeviceType.TryGetValue(session_id, out device_type);
            if (string.IsNullOrEmpty(device_type))
            {
                if (ctx.Request.Cookies.ContainsKey("device_type") == true)
                {
                    device_type = ctx.Request.Cookies["device_type"];
                    session_deviceType_Set(session_id, device_type);
                }
            }
            return device_type;
        }

        public static void session_deviceType_Set(string session_id, string device_type)
        {
            if (string.IsNullOrWhiteSpace(session_id)) return;

            if (dicSession_DeviceType.ContainsKey(session_id))
                dicSession_DeviceType[session_id] = device_type;
            else
                dicSession_DeviceType.Add(session_id, device_type);
        }

        #endregion

        #region >> Session >> Uri closest login, logout ...

        /// <summary>
        /// Get uri closest session logout or login 
        /// </summary>
        /// <param name="session_id"></param>
        /// <returns></returns>
        public static string user_refUri_Closest_Get(string session_id)
        {
            string uri = "";
            if (dicSession_refUri.TryGetValue(session_id, out uri))
                return uri;
            return "/" + hostSite.page_main + "." + hostSite.page_ext; ;
        }

        public static void user_refUri_Closest_Set(string session_id, string uri)
        {
            if (string.IsNullOrWhiteSpace(session_id)) return;

            if (dicSession_refUri.ContainsKey(session_id))
                dicSession_refUri[session_id] = uri;
            else
                dicSession_refUri.Add(session_id, uri);
        }

        #endregion

        #region >> Token - Language choose ...

        public static void langSet(string token_id, string lang_key)
        {
            if (string.IsNullOrWhiteSpace(token_id)) return;

            if (dicToken_LangUser.ContainsKey(token_id))
                dicToken_LangUser[token_id] = lang_key;
            else
                dicToken_LangUser.Add(token_id, lang_key);
        }

        public static string langGet(string token_id)
        {
            string lang = lang_Default;

            dicToken_LangUser.TryGetValue(token_id, out lang);
            if (string.IsNullOrEmpty(lang)) lang = lang_Default;

            return lang;
        }

        #endregion

        /*
        #region >> SignalR ...


        public static void onQueryResult_data(string client_id, string query_id, string fun_name, string content)
        {
            if (!string.IsNullOrEmpty(client_id))
            {
                Microsoft.AspNet.SignalR.GlobalHost.ConnectionManager
                    //.GetHubContext<MyHub>().Clients.Client(client_id).onData(key, message);
                    .GetHubContext<MyHub>().Clients.Client(client_id).onQueryResult_data(query_id, fun_name, content);
            }
        }

        public static void onQueryResult_kit_v2(string client_id, string query_id, string callback, string selector, string type, string content, string config)
        {
            if (!string.IsNullOrEmpty(client_id))
            {
                Microsoft.AspNet.SignalR.GlobalHost.ConnectionManager
                    //.GetHubContext<MyHub>().Clients.Client(client_id).onData(key, message);
                    .GetHubContext<MyHub>().Clients.Client(client_id).onQueryResult_kit_v2(query_id, callback, selector, type, content, config);
            }
        }

        public static void onQueryResult_v2(string client_id, string query_id, string callback, string content, string config)
        {
            if (!string.IsNullOrEmpty(client_id))
            {
                Microsoft.AspNet.SignalR.GlobalHost.ConnectionManager
                    //.GetHubContext<MyHub>().Clients.Client(client_id).onData(key, message);
                    .GetHubContext<MyHub>().Clients.Client(client_id).onQueryResult_v2(query_id, callback, content, config);
            }
        }

        public static void onQueryResult_v2_bug(string client_id, string query_id, string content, string config)
        {
            if (!string.IsNullOrEmpty(client_id))
            {
                Microsoft.AspNet.SignalR.GlobalHost.ConnectionManager
                    //.GetHubContext<MyHub>().Clients.Client(client_id).onData(key, message);
                    .GetHubContext<MyHub>().Clients.Client(client_id).onQueryResult_v2_bug(query_id, content, config);
            }
        }

        public static void onWindow(string client_id, string module_id, string config, string para, string html, string css, string js, string message)
        {
            if (!string.IsNullOrEmpty(client_id))
            {
                Microsoft.AspNet.SignalR.GlobalHost.ConnectionManager
                    //.GetHubContext<MyHub>().Clients.Client(client_id).onData(key, message);
                    .GetHubContext<MyHub>().Clients.Client(client_id).onWindow(module_id, config, para, html, css, js, message);
            }
        }

        public static void onWindow_bug(string client_id, string module_id, string config, string para, string message)
        {
            if (!string.IsNullOrEmpty(client_id))
            {
                Microsoft.AspNet.SignalR.GlobalHost.ConnectionManager
                    //.GetHubContext<MyHub>().Clients.Client(client_id).onData(key, message);
                    .GetHubContext<MyHub>().Clients.Client(client_id).onWindow_bug(module_id, config, para, message);
            }
        }

        public static void onQueryFileResult(string client_id, string query_id, string cache_key, string config, string content)
        {
            if (!string.IsNullOrEmpty(client_id))
            {
                Microsoft.AspNet.SignalR.GlobalHost.ConnectionManager
                    //.GetHubContext<MyHub>().Clients.Client(client_id).onData(key, message);
                    .GetHubContext<MyHub>().Clients.Client(client_id).onQueryFileResult(query_id, cache_key, config, content);
            }
        }

        public static void onBluetoothUpdate(string client_id, string meter_id, string data)
        {
            if (!string.IsNullOrEmpty(client_id))
            {
                Microsoft.AspNet.SignalR.GlobalHost.ConnectionManager
                    //.GetHubContext<MyHub>().Clients.Client(client_id).onData(key, message);
                    .GetHubContext<MyHub>().Clients.Client(client_id).onBluetoothUpdate(meter_id, data);
            }
        }

        public static void onLogin(string client_id, string token, string data)
        {
            if (!string.IsNullOrEmpty(client_id))
            {
                Microsoft.AspNet.SignalR.GlobalHost.ConnectionManager
                    //.GetHubContext<MyHub>().Clients.Client(client_id).onData(key, message);
                    .GetHubContext<MyHub>().Clients.Client(client_id).onLogin(token, data);
            }
        }

        public static void onLoadJson(string client_id, string path_key, string config, string content)
        {
            if (!string.IsNullOrEmpty(client_id))
            {
                Microsoft.AspNet.SignalR.GlobalHost.ConnectionManager
                    //.GetHubContext<MyHub>().Clients.Client(client_id).onData(key, message);
                    .GetHubContext<MyHub>().Clients.Client(client_id).onLoadJson(path_key, config, content);
            }
        }

        public static void onQueryResult(string client_id, string query_id, string content)
        {
            if (!string.IsNullOrEmpty(client_id))
            {
                Microsoft.AspNet.SignalR.GlobalHost.ConnectionManager
                    //.GetHubContext<MyHub>().Clients.Client(client_id).onData(key, message);
                    .GetHubContext<MyHub>().Clients.Client(client_id).onQueryResult(query_id, content);
            }
        }

        public static void onAlert(string client_id, string title, string content)
        {
            if (!string.IsNullOrEmpty(client_id))
            {
                Microsoft.AspNet.SignalR.GlobalHost.ConnectionManager
                    //.GetHubContext<MyHub>().Clients.Client(client_id).onData(key, message);
                    .GetHubContext<MyHub>().Clients.Client(client_id).onAlert(title, content);
            }
        }

        public static void onNoti(string client_id, string title, string content, int time_out)
        {
            if (!string.IsNullOrEmpty(client_id))
            {
                Microsoft.AspNet.SignalR.GlobalHost.ConnectionManager
                    //.GetHubContext<MyHub>().Clients.Client(client_id).onData(key, message);
                    .GetHubContext<MyHub>().Clients.Client(client_id).onNoti(title, content, time_out);
            }
        }

        public static void onData(string client_id, string key, string data)
        {
            if (!string.IsNullOrEmpty(client_id))
            {
                Microsoft.AspNet.SignalR.GlobalHost.ConnectionManager
                    //.GetHubContext<MyHub>().Clients.Client(client_id).onData(key, message);
                    .GetHubContext<MyHub>().Clients.Client(client_id).onData(key, data);
            }
        }

        public static void onPop(string client_id, string content)
        {
            if (!string.IsNullOrEmpty(client_id))
            {
                Microsoft.AspNet.SignalR.GlobalHost.ConnectionManager
                    //.GetHubContext<MyHub>().Clients.Client(client_id).onData(key, message);
                    .GetHubContext<MyHub>().Clients.Client(client_id).onPop(content);
            }
        }


        public static string signalGetFirstTokenID()
        {
            string key = "";
            if (dicToken_R.Count > 0)
                key = dicToken_R.Keys.ToList()[0];
            return key;
        }

        public static void signalSendDataToAllClient(string token_id, object data)
        {
            string key = "";
            Microsoft.AspNet.SignalR.GlobalHost.ConnectionManager
                .GetHubContext<MyHub>().Clients.All.broadcastMessage(key, data);
        }

        public static void signalSendMessageToAllClient(string token_id, string message)
        {
            string key = "";
            Microsoft.AspNet.SignalR.GlobalHost.ConnectionManager
                .GetHubContext<MyHub>().Clients.All.broadcastMessage(key, message);
        }

        public static void sendNotificationToAllClient(string title, string message)
        {
            Microsoft.AspNet.SignalR.GlobalHost.ConnectionManager
                .GetHubContext<MyHub>().Clients.All.onNotiAllUser(title, message);
        }

        public static void signalSendDataToClient(string token_id, object data)
        {
            string client_id = "";
            if (dicToken_R.ContainsKey(token_id)) client_id = dicToken_R[token_id];

            if (client_id != "")
            {
                string key = "";
                Microsoft.AspNet.SignalR.GlobalHost.ConnectionManager
                    //.GetHubContext<MyHub>().Clients.Client(client_id).onData(key, data);
                    .GetHubContext<MyHub>().Clients.Client(client_id).broadcastMessage(key, data);
            }
        }

        public static void signalSendMessageToClient(string token_id, string message)
        {
            string client_id = "";
            if (dicToken_R.ContainsKey(token_id)) client_id = dicToken_R[token_id];

            if (client_id != "")
            {
                string key = "";
                Microsoft.AspNet.SignalR.GlobalHost.ConnectionManager
                    //.GetHubContext<MyHub>().Clients.Client(client_id).onData(key, message);
                    .GetHubContext<MyHub>().Clients.Client(client_id).broadcastMessage(key, message);
            }
        }

        public static void signalSendAlertToClientID(string client_id, string title, string content)
        {
            if (client_id != "")
            {
                Microsoft.AspNet.SignalR.GlobalHost.ConnectionManager
                    //.GetHubContext<MyHub>().Clients.Client(client_id).onData(key, message);
                    .GetHubContext<MyHub>().Clients.Client(client_id).onAlert(title, content);
            }
        }

        public static void signalSendMessageToClientID(string client_id, string key, string message)
        {
            if (client_id != "")
            {
                Microsoft.AspNet.SignalR.GlobalHost.ConnectionManager
                    //.GetHubContext<MyHub>().Clients.Client(client_id).onData(key, message);
                    .GetHubContext<MyHub>().Clients.Client(client_id).broadcastMessage(key, message);
            }
        }

        public static void signalJoinClient(string token_id, string signalR_id)
        {
            if (dicToken_R.ContainsKey(token_id))
                dicToken_R[token_id] = signalR_id;
            else
                lock (lock_R)
                {
                    dicToken_R.Add(token_id, signalR_id);
                }
        }

        #endregion

         */

        #region >> Token ...

        public static void tokenRemove(string user, string token)
        {
            if (dicUserToken.ContainsKey(user))
                dicUserToken.Remove(user, token);
        }

        public static void tokenAdd(string user, string token, DateTime dateExpiry)
        {
            if (dicUserToken.ContainsKey(user))
            {
                var ls = dicUserToken[user];

                if (ls.IndexOf(token) == -1)
                {
                    lock (lock_token)
                    {
                        dicUserToken.Add(user, token);
                    }
                }
            }
            else
            {
                lock (lock_token)
                {
                    dicUserToken.Add(user, token);
                }
            }
        }

        #endregion

        #region >> User ...

        //public static string Add(m_user u)
        //{
        //    string id = "";
        //    var ls = lsUser.Where(x => x.username == u.username).ToList();
        //    if (ls.Count == 0)
        //    {
        //        id = Guid.NewGuid().ToString();
        //        u.id = id;
        //        u.status = false;
        //        lock (lock_lsUser)
        //        {
        //            lsUser.Add(u);
        //        }
        //    }
        //    return id;
        //}

        //public static IEnumerable<m_user> getUser()
        //{
        //    return lsUser;
        //}

        //public static m_user login(string username, string password)
        //{
        //    m_user u = new m_user();
        //    var dt = lsUser.Where(x => x.username == username && x.pass == password && x.status == true).ToList();

        //    if (dt.Count > 0)
        //    {
        //        u = dt[0];
        //    }

        //    return u;
        //}

        //public static m_user login(m_user o)
        //{
        //    m_user u = new m_user();
        //    if (o.username == null || o.pass == null) return u;
        //    u = login(o.username, o.pass);
        //    return u;
        //}

        #endregion
    }
}
