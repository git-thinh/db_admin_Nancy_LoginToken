using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using System.Reflection;
using System.Runtime.Caching;
using System.Text;
using System.Threading.Tasks;

using System.Threading;
using System.Configuration;
using System.Net;
using Nancy.Hosting.Self;
using Microsoft.Win32;
using Microsoft.Owin.Hosting;
using Owin;
using Microsoft.AspNet.SignalR;

namespace host
{
    public static class hostServer
    {





        public static string IP_LOCAL = ConfigurationManager.AppSettings["ip_local"];
        public static string IP_NAT = ConfigurationManager.AppSettings["ip_nat"];

        public static string IP = Dns.GetHostByName(Dns.GetHostName()).AddressList[0].ToString();
        public static string pathExtSite = ".ifc";

        public static string pathRoot = AppDomain.CurrentDomain.BaseDirectory;

        public static string pathModule = ConfigurationManager.AppSettings["path_root"] + "views"; //pathRoot + "views";
        public static string pathDB = pathRoot + @"db";
        public static string pathConfig = pathRoot + @"config";

        public static string pathSysObject = pathRoot + @"system\object";
        public static string pathBind = pathRoot + @"bind";
        public static string pathJson = pathRoot + @"json";

        //public static string[] host_memner = ConfigurationManager.AppSettings["s_host_member"].Replace("*ip*", IP).Split('|').Where(x => x != "").ToArray();
        //public static string[] uri_memner = ConfigurationManager.AppSettings["s_domain_member"].Replace("*ip*", IP).Split('|').Where(x => x != "").ToArray();

        private static Dictionary<int, string> dicDNS = new Dictionary<int, string>() { };

        private static List<string> ls_UriInfo = new List<string>() { };

        public static string getDomain(int port)
        {
            string domain = "";
            dicDNS.TryGetValue(port, out domain);
            if (string.IsNullOrEmpty(domain)) return hostUser.page_Main;
            return domain;
        }

        public static void console_write_uri_info()
        {
            foreach (var s in ls_UriInfo)
                Console.WriteLine(s);
        }

        public static void init()
        {
            if (!string.IsNullOrEmpty(IP_LOCAL)) IP = IP_LOCAL;
            //if (string.IsNullOrEmpty(IP_NAT))
            //{
            //    host_memner = ConfigurationManager.AppSettings["s_host_member"].Replace("*ip*", IP).Split('|').Where(x => x != "").ToArray();
            //    uri_memner = ConfigurationManager.AppSettings["s_domain_member"].Replace("*ip*", IP).Split('|').Where(x => x != "").ToArray();
            //}
            //else
            //{
            //    host_memner = ConfigurationManager.AppSettings["s_host_member"].Replace("*ip*", IP_NAT).Split('|').Where(x => x != "").ToArray();
            //    uri_memner = ConfigurationManager.AppSettings["s_domain_member"].Replace("*ip*", IP_NAT).Split('|').Where(x => x != "").ToArray();
            //}

            if (Directory.Exists(pathDB)) Directory.CreateDirectory(pathDB);

            #region >>> DNS ...

            int[] portArray = new int[] { };
            string[] domainArray = new string[] { };

            portArray = ConfigurationManager
                    .AppSettings["s_port"].Split('|')
                    .Where(x => x != "")
                    .Select(x => Convert.ToInt32(x))
                    .ToArray();

            domainArray = ConfigurationManager
                .AppSettings["s_domain"].Split('|')
                .Where(x => x != "")
                .ToArray();
            for (int i = 0; i < portArray.Length; i++)
            {
                if (i < domainArray.Length)
                    dicDNS.Add(portArray[i], domainArray[i]);
            }

            #endregion

            //string[] a_page_Signal = hostUser.page_Signal.Split(':');
            //int port_Signal = 0;
            //int.TryParse(a_page_Signal[1], out port_Signal);
            //if (port_Signal > 0)
            //{
            //    string ip_Signal = a_page_Signal[0]; 
            //    if (!string.IsNullOrEmpty(hostServer.IP_LOCAL)) ip_Signal = hostServer.IP_LOCAL;

            //    //string u0 = "http://+:" + port.ToString();
            //    string url_Signal = "http://" + ip_Signal + ":" + port_Signal.ToString();
            //    WebApp.Start<StartupOwinSignalR>(url_Signal);
            //    //HubConnector.Instance.SendTagMessage();

            //    string s_info = "\n\n\t\t Signal: " + url_Signal;
            //    ls_UriInfo.Add(s_info);
            //    Console.WriteLine(s_info);
            //}

            hostModule.CacheRefresh();
            hostUser.init();

            int[] p = GetPortFree();
            if (p.Length > 4)
            {
                foreach (var port in dicDNS.Keys)
                {
                    try
                    {
                        string uri = "http://" + IP + ":" + port.ToString() + "/";
                        string s_info = "\n\n\t\t Web: " + uri + '-' + dicDNS[port];
                        ls_UriInfo.Add(s_info);

                        var config = new HostConfiguration();
                        config.RewriteLocalhost = false;

                        //CorsHandler is also defined in CorsHandler.cs.  It is what enables CORS
                        //config.MessageHandlers.Add(new CorsHandler());
                        //config.Filters.Add(new MethodAttributeExceptionHandling());

                        ////////Microsoft.AspNet.WebApi.Cors
                        //config.EnableCors();
                        //////var enableCorsAttribute = new System.Web.Http.Cors.EnableCorsAttribute("*", "*", "*")
                        //////{
                        //////    SupportsCredentials = true
                        //////};
                        //////config.EnableCors(enableCorsAttribute);

                        var nancyHost = new NancyHost(config,
                            new Uri(uri),
                            new Uri("http://127.0.0.1:" + port.ToString() + "/"),
                            new Uri("http://localhost:" + port.ToString() + "/"));

                        //var nancyHost = new NancyHost(
                        //    new Uri(uri),
                        //    new Uri("http://127.0.0.1:" + port.ToString() + "/"),
                        //    new Uri("http://localhost:" + port.ToString() + "/"));
                        nancyHost.Start();

                        Debug.WriteLine(s_info);
                    }
                    catch { }
                }

                /////openBrowser();
                string url = "http://" + hostUser.page_Main;
                main.show_notification(url);
            }
        }

        public static void openBrowser()
        {
            string url = "http://" + hostUser.page_Main;

            //bool found_chrome = false;
            //Process[] processes = Process.GetProcesses();
            //foreach (Process process in processes)
            //{
            //    //Get whatever attribute for process
            //    if (process.ProcessName.IndexOf("firefox") != -1)
            //    {
            //        // fire fox is open
            //        ProcessStartInfo startInfo = new ProcessStartInfo();
            //        startInfo.FileName = "firefox.exe";
            //        startInfo.Arguments = url;
            //        Process.Start(startInfo);
            //        return;
            //    }
            //    else if (process.ProcessName.IndexOf("chrome") != -1)
            //    {
            //        found_chrome = true;
            //    }
            //}

            //if (found_chrome)
            //{
            //    ProcessStartInfo startInfo = new ProcessStartInfo();
            //    startInfo.FileName = "chrome.exe";
            //    startInfo.Arguments = url;
            //    Process.Start(startInfo);
            //}
            //else
            //{
            //    ProcessStartInfo startInfo = new ProcessStartInfo();
            //    startInfo.FileName = "firefox.exe";
            //    startInfo.Arguments = url;
            //    Process.Start(startInfo);
            //}

            string browser = GetDefaultBrowserPath();

            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.FileName = browser;
            startInfo.Arguments = url;
            Process.Start(startInfo);
        }

        public static string GetDefaultBrowserPath()
        {
            string urlAssociation = @"Software\Microsoft\Windows\Shell\Associations\UrlAssociations\http";
            string browserPathKey = @"$BROWSER$\shell\open\command";

            RegistryKey userChoiceKey = null;

            try
            {
                //Read default browser path from userChoiceLKey
                userChoiceKey = Registry.CurrentUser.OpenSubKey(urlAssociation + @"\UserChoice", false);

                //If user choice was not found, try machine default
                if (userChoiceKey == null)
                {
                    //Read default browser path from Win XP registry key
                    var browserKey = Registry.ClassesRoot.OpenSubKey(@"HTTP\shell\open\command", false);

                    //If browser path wasn’t found, try Win Vista (and newer) registry key
                    if (browserKey == null)
                    {
                        browserKey =
                        Registry.CurrentUser.OpenSubKey(
                        urlAssociation, false);
                    }
                    //string s_bro = browserKey.GetValue(null) as string;
                    //var path = CleanifyBrowserPath(s_bro);

                    //Remove quotation marks
                    string browserPath1 = (browserKey.GetValue(null) as string).ToLower().Replace("\"", "");

                    //Cut off optional parameters
                    if (!browserPath1.EndsWith("exe"))
                    {
                        browserPath1 = browserPath1.Substring(0, browserPath1.LastIndexOf(".exe") + 4);
                    }

                    browserKey.Close();
                    return browserPath1;
                }
                else
                {
                    // user defined browser choice was found
                    string progId = (userChoiceKey.GetValue("ProgId").ToString());
                    userChoiceKey.Close();

                    // now look up the path of the executable
                    string concreteBrowserKey = browserPathKey.Replace("$BROWSER$", progId);
                    var kp = Registry.ClassesRoot.OpenSubKey(concreteBrowserKey, false);


                    //Remove quotation marks
                    string browserPath2 = (kp.GetValue(null) as string).ToLower().Replace("\"", "");

                    //Cut off optional parameters
                    if (!browserPath2.EndsWith("exe"))
                    {
                        browserPath2 = browserPath2.Substring(0, browserPath2.LastIndexOf(".exe") + 4);
                    }

                    kp.Close();
                    return browserPath2;
                }
            }
            catch (Exception ex)
            {
                return "";
            }
        }

        public static string path_root
        {
            get
            {
                string codeBase = Assembly.GetExecutingAssembly().CodeBase;
                UriBuilder uri = new UriBuilder(codeBase);
                string path = Uri.UnescapeDataString(uri.Path);
                return Path.GetDirectoryName(path);
            }
        }

        public static string getCache(string key)
        {
            ObjectCache cache = MemoryCache.Default;

            if (string.IsNullOrEmpty(key)) return "";
            else key = key.ToLower();

            string s = cache[key] as string;
            if (s == null) s = "";

            return s;
        }

        private static int[] GetPortFree()
        {
            //int port = 0;

            List<int> ls_port_ok = new List<int>() { };
            for (int k = 3000; k < 65000; k++) ls_port_ok.Add(k);

            List<int> ls_port = new List<int>() { };

            IPGlobalProperties ipGlobalProperties = IPGlobalProperties.GetIPGlobalProperties();
            TcpConnectionInformation[] tcpConnInfoArray = ipGlobalProperties.GetActiveTcpConnections();
            IEnumerator myEnum = tcpConnInfoArray.GetEnumerator();

            while (myEnum.MoveNext())
            {
                TcpConnectionInformation TCPInfo = (TcpConnectionInformation)myEnum.Current;
                //Console.WriteLine("Port {0} {1} {2} ", TCPInfo.LocalEndPoint, TCPInfo.RemoteEndPoint, TCPInfo.State);
                ls_port.Add(TCPInfo.LocalEndPoint.Port);
            }

            ls_port = ls_port.OrderBy(x => x).ToList();
            ls_port_ok = ls_port_ok.Where(p => !ls_port.Any(x => x == p)).ToList();

            //if (ls_port_ok.Count > 0) port = ls_port_ok[0];

            return ls_port_ok.ToArray();
        }

    }
}
