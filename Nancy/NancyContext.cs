


namespace Nancy
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Nancy.Diagnostics;
    using Nancy.Responses.Negotiation;
    using Nancy.Routing;

    using Nancy.Validation;
    using System.Globalization;
    using System.Reflection;
    using System.Configuration;
    using Nancy.Security;

    public static class NancyContextKey
    {
        public const string host_main_ = "host_main";
        public const string host_login_ = "host_login";
        public const string host_connect_ = "host_connect";

        public const string port_ = "port";
        public const string domain_ = "domain";
        public const string domain_dns_ = "domain_dns";
        public const string path_ = "path";

        public const string username_ = "username";
        public const string session_id_ = "session_id";
        public const string token_id_ = "token_id";
        public const string lang_key_ = "lang_key";
        public const string theme_key_ = "theme_key";
        public const string hide_panel_ = "hide_panel";

        public const string browser_width_ = "browser_width";
        public const string browser_height_ = "browser_height";
        public const string browser_name_ = "browser_name";
        public const string browser_version_ = "browser_version";

        public const string device_name_ = "device_name";
        public const string device_type_ = "device_type";
        public const string device_version_ = "device_version";

        public const string os_name_ = "os_name";
        public const string os_version_ = "os_version";

        public const string tree_id_ = "tree_id";
        public const string tree_mod_ = "tree_mod";
        public const string tree_type_ = "tree_type";

        public static string getValue(string code, NancyContext ctx)
        {
            string val_0 = "";
            #region
            switch (code)
            {
                case host_connect_:
                    val_0 =  ConfigurationManager.AppSettings["s_domain_signal"];//ctx.host_connect;
                    break;
                case host_login_:
                    val_0 = ctx.host_login;
                    break;
                case host_main_:
                    val_0 = ctx.host_main;
                    break;

                case port_:
                    val_0 = ctx.port.ToString();
                    break;
                case domain_:
                    val_0 = ctx.domain;
                    break;
                case domain_dns_:
                    val_0 = ctx.domain_dns;
                    break;
                case path_:
                    val_0 = ctx.path;
                    break;
                case browser_height_:
                    val_0 = ctx.browser_height;
                    break;
                case browser_name_:
                    val_0 = ctx.browser_name;
                    break;
                case browser_version_:
                    val_0 = ctx.browser_version;
                    break;
                case browser_width_:
                    val_0 = ctx.browser_width.ToString();
                    break;
                case device_name_:
                    val_0 = ctx.device_name;
                    break;
                case device_type_:
                    val_0 = ctx.device_type;
                    break;
                case device_version_:
                    val_0 = ctx.device_version;
                    break;
                case lang_key_:
                    val_0 = ctx.lang_key;
                    break;
                case theme_key_:
                    val_0 = ctx.theme_key;
                    break;
                case os_name_:
                    val_0 = ctx.os_name;
                    break;
                case os_version_:
                    val_0 = ctx.os_version;
                    break;
                case session_id_:
                    val_0 = ctx.session_id;
                    break;
                case hide_panel_:
                    val_0 = ctx.hide_panel;
                    break;
                case token_id_:
                    val_0 = ctx.token_id;
                    break;
                case username_:
                    val_0 = ctx.username;
                    break;
                case tree_id_:
                    val_0 = ctx.tree_id;
                    break;
                case tree_mod_:
                    val_0 = ctx.tree_mod;
                    break;
                case tree_type_:
                    val_0 = ctx.tree_type;
                    break;
            }
            if (val_0 == null) val_0 = "";
            #endregion
            val_0 = val_0.ToLower();

            return val_0;
        }

        public static string renderKey(string template, NancyContext ctx)
        {
            var ls = typeof(NancyContextKey)
                .GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy)
                .Where(fi => fi.IsLiteral && !fi.IsInitOnly && fi.Name.EndsWith("_"))
                .Select(x => x.Name.Substring(0, x.Name.Length - 1))
                .ToArray();

            foreach (var key in ls)
            {
                string val = getValue(key, ctx);
                template = template.Replace("@" + key, val).Replace("@Context." + key, val);
            }

            return template;
        }
    }

    public sealed class NancyContext : IDisposable
    {
        public string host_main { set; get; }
        public string host_login { set; get; }
        public string host_connect { set; get; }

        public int port { set; get; }
        public string domain { set; get; }
        public string domain_dns { set; get; }

        public string path { set; get; }

        public string username { set; get; }
        public string session_id { set; get; }
        public string token_id { set; get; }
        public string lang_key { set; get; }
        public string theme_key { set; get; }

        public string hide_panel { set; get; }

        public int browser_width { set; get; }
        public string browser_height { set; get; }
        public string browser_name { set; get; }
        public string browser_version { set; get; }

        public string device_name { set; get; }
        public string device_type { set; get; }
        public string device_version { set; get; }

        public string os_name { set; get; }
        public string os_version { set; get; }

        public string tree_mod { set; get; }
        public string tree_id { set; get; }
        public string tree_type { set; get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="NancyContext"/> class.
        /// </summary>
        public NancyContext()
        {
            this.Items = new Dictionary<string, object>();
            this.Trace = new DefaultRequestTrace();
            this.ViewBag = new DynamicDictionary();

            // TODO - potentially additional logic to lock to ip etc?
            this.ControlPanelEnabled = true;

            this.Items.Add("time", DateTime.Now.ToString());
            this.ViewBag.time = DateTime.Now.ToString();
            this.ViewBag.user = new { user_name = username, time = DateTime.Now.ToString() };

            //hide_panel = "false";
            //username = "";
            //session_id = "";
            //token_id = "";
            //lang_key = "";
            //theme_key = "";

            //hide_panel = "0";

            //browser_width = "0";
            //browser_height = "0";

            //device_type = "pc";
        }

        public void setClientInfo(string sUserAgent)
        {
            if (!string.IsNullOrEmpty(sUserAgent))
            {
                //device_type = f_is_mobile(sUserAgent);
                browser_name = f_browser_name(sUserAgent);
            }
        }

        private string f_browser_name(string userAgent)
        {
            if (userAgent.Contains("firefox")) return "firefox";
            else if (userAgent.Contains("edge")) return "edge";
            else if (userAgent.Contains("chrome")) return "chrome";
            else if (userAgent.Contains("safari")) return "safari";
            else if (userAgent.Contains("opera")) return "opera";
            else if (userAgent.Contains("trident"))
            {
                if (userAgent.Contains("msie 11") || userAgent.Contains("gecko")) return "ie11";
                else if (userAgent.Contains("msie 10")) return "ie10";
                else if (userAgent.Contains("msie 9")) return "ie9";
                else if (userAgent.Contains("msie 8")) return "ie8";
                else if (userAgent.Contains("msie 7")) return "ie7";
                else if (userAgent.Contains("msie 6")) return "ie6";
                else if (userAgent.Contains("msie 5")) return "ie5";
            }
            else if (userAgent.Contains("msie 9")) return "ie9";
            else if (userAgent.Contains("msie 8")) return "ie8";
            else if (userAgent.Contains("msie 7")) return "ie7";
            else if (userAgent.Contains("msie 6")) return "ie6";
            else if (userAgent.Contains("msie 5")) return "ie5";
            else if (userAgent.Contains("netscape")) return "netscape";

            return "unknown";
        }

        private string f_is_mobile(string agent)
        {
            if (agent.Contains("iphone") ||
                agent.Contains("symbianos") ||
                agent.Contains("ipad") ||
                agent.Contains("ipod") ||
                agent.Contains("android") ||
                agent.Contains("blackberry") ||
                agent.Contains("samsung") ||
                agent.Contains("nokia") ||
                agent.Contains("windows ce") ||
                agent.Contains("sonyericsson") ||
                agent.Contains("webos") ||
                agent.Contains("wap") ||
                agent.Contains("motor") ||
                agent.Contains("symbian"))
            {
                // do your logic here for device specific requests
                return "mobi";
            }
            else
            {
                // do your logic here for PC/Mac requests
                return "pc";
            }
        }

        private Request request;

        private ModelValidationResult modelValidationResult;

        /// <summary>
        /// Gets the dictionary for storage of per-request items. Disposable items will be disposed when the context is.
        /// </summary>
        public IDictionary<string, object> Items { get; private set; }

        /// <summary>
        /// Gets or sets the resolved route
        /// </summary>
        public Route ResolvedRoute { get; set; }

        /// <summary>
        /// Gets or sets the parameters for the resolved route
        /// </summary>
        public dynamic Parameters { get; set; }

        /// <summary>
        /// Gets or sets the incoming request
        /// </summary>
        public Request Request
        {
            get
            {
                return this.request;
            }

            set
            {
                this.request = value;
                this.Trace.RequestData = value;
            }
        }


        /// <summary>
        /// Gets or sets the outgoing response
        /// </summary>
        public Response Response { get; set; }

        /// <summary>
        /// Gets or sets the current user
        /// </summary>
        public IUserIdentity CurrentUser { get; set; }

        /// <summary>
        /// Diagnostic request tracing
        /// </summary>
        public IRequestTrace Trace { get; set; }

        /// <summary>
        /// Gets a value indicating whether control panel access is enabled for this request
        /// </summary>
        public bool ControlPanelEnabled { get; private set; }

        /// <summary>
        /// Non-model specific data for rendering in the response
        /// </summary>
        public dynamic ViewBag { get; private set; }

        /// <summary>
        /// Gets or sets the model validation result.
        /// </summary>
        public ModelValidationResult ModelValidationResult
        {
            get { return this.modelValidationResult ?? (this.modelValidationResult = new ModelValidationResult()); }
            set { this.modelValidationResult = value; }
        }

        /// <summary>
        /// Gets or sets the context's culture
        /// </summary>
        public CultureInfo Culture { get; set; }

        /// <summary>
        /// Context of content negotiation (if relevant)
        /// </summary>
        public NegotiationContext NegotiationContext { get; set; }

        /// <summary>
        /// Gets or sets the dynamic object used to locate text resources.
        /// </summary>
        public dynamic Text { get; set; }

        /// <summary>
        /// Disposes any disposable items in the <see cref="Items"/> dictionary.
        /// </summary>
        public void Dispose()
        {
            foreach (var disposableItem in this.Items.Values.OfType<IDisposable>())
            {
                disposableItem.Dispose();
            }

            this.Items.Clear();

            if (this.request != null)
            {
                ((IDisposable)this.request).Dispose();
            }

            if (this.Response != null)
            {
                this.Response.Dispose();
            }
        }
    }
}
