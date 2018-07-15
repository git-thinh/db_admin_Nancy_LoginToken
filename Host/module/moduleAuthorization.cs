
using System;
using System.Collections.Generic;
using Nancy;
using Nancy.ModelBinding;
using System.IO;
using Nancy.TinyIoc;
using Nancy.Bootstrapper;
using Nancy.Owin;
using System.Linq;
using Nancy.Conventions;
using System.Threading;
using System.Threading.Tasks;
using System.Runtime.Caching;
using System.Net;

using System.Text;
using System.Dynamic;

using Nancy.ViewEngines;
using Nancy.ViewEngines.SuperSimpleViewEngine;
using System.Text.RegularExpressions;
using Microsoft.CSharp.RuntimeBinder;
using System.Runtime.CompilerServices;
using System.Reflection;
using Nancy.Helpers;
using System.Diagnostics;

namespace host
{
    public abstract class moduleAuthorization : NancyModule
    {

        //public static ObjectCache cache = MemoryCache.Default;
        public moduleAuthorization()
        {
            //if (string.IsNullOrEmpty(this.language_user)) this.language_user = hostUser.lang_Default;
            chechAuthorization();
        }

        public moduleAuthorization(string modulepath)
            : base(modulepath)
        {
            //if (string.IsNullOrEmpty(this.language_user)) this.language_user = hostUser.lang_Default;
            chechAuthorization();
        }


        public string Render(string pathView, dynamic model)
        {
            return renderNancy.Render(pathView, model, this);
        }

        public string RenderTemplate(string template, dynamic model)
        {
            return renderNancy.RenderTemplate(template, model, this);
        }


        public bool isAjax
        {
            get
            {
                if (this.Request.Method == "GET") return false;
                var ffs = this.Request.Form;
                if (ffs == null || ffs.Count == 0) return true;
                return false;
            }
        }

        /// <summary>
        /// Gets response with ETag. Returns empty body if ETag matches (no data changes)
        /// </summary>
        /// <param name="data">Data to match with ETag</param>
        /// <param name="model">Model to return</param>
        /// <returns>Response</returns>
        protected Response GetResponseWithEtag(object data, object model)
        {
            //string etag = Utils.ComputeHash(data);
            string etag = GetComputeHash(this.Request);

            if (CacheHelpers.ReturnNotModified(etag, null, this.Context))
            {
                return Nancy.HttpStatusCode.NotModified;
            }

            return Response.AsJson(model).WithHeader("ETag", etag);
        }

        public string GetComputeHash(Request request)
        {
            using (var md5 = new System.Security.Cryptography.MD5CryptoServiceProvider())
            {
                var hash = md5.ComputeHash(Encoding.UTF8.GetBytes(request.Url.ToString()));
                return Convert.ToBase64String(hash);
            }
        }

        private Response f_login(NancyContext ctx, string session_id, string url)
        {
            /// string uri = "http://" + hostUser.page_Login + "/login/session/" + session_id + "?" + url.Base64Encode();

            return Response.AsRedirect("/logout")
                    .WithCookie(new Nancy.Cookies.NancyCookie("session_id", session_id));
        }

        private void chechAuthorization()
        {
            Before.AddItemToEndOfPipeline(
                new PipelineItem<Func<NancyContext, Response>>(
                    "RequiresWindowsAuthentication",
                    ctx =>
                    {
                        ctx.host_main = hostUser.page_Main;
                        ctx.host_login = hostUser.page_Login;
                        ctx.host_connect = hostUser.page_Signal;

                        //for (int i = 0; i < hostServer.host_memner.Length; i++)
                        //{
                        //    if (i < hostServer.uri_memner.Length)
                        //        ctx.Items.Add(hostServer.host_memner[i], hostServer.uri_memner[i]);
                        //}

                        var uri = ctx.Request.Url;

                        ctx.port = (int)uri.Port;
                        ctx.domain = hostServer.getDomain(ctx.port);
                        ctx.domain_dns = uri.SiteBase.Split('/')[2];

                        string url = uri.ToString(), host_name = uri.HostName, path = uri.Path;
                        if (string.IsNullOrEmpty(path) || path == "/")
                            path = "home";
                        else path = path.Substring(1);
                        ctx.path = path;

                        string refUri = this.Request.Headers.Referrer; if (refUri == null) refUri = "";

                        string session_id = "";
                        ctx.Request.Cookies.TryGetValue("session_id", out session_id);
                        if (string.IsNullOrEmpty(session_id))
                            session_id = Guid.NewGuid().ToString();
                        ctx.session_id = session_id;
                        //this.Context.session_id = session_id;

                        string tree_type = "";
                        string hide_panel = "";
                        if (url.Contains("." + hostSite.page_ext))
                        {
                            ctx.device_type = hostUser.session_deviceType_Get(ctx, session_id);
                            ctx.lang_key = hostUser.langGet(session_id);
                            ctx.theme_key = hostUser.session_themeKey_Get(session_id);

                            ctx.Request.Cookies.TryGetValue("tree_type", out tree_type);
                            if (string.IsNullOrEmpty(tree_type)) tree_type = "";
                            ctx.tree_type = tree_type;
                            
                            ctx.Request.Cookies.TryGetValue("hide_panel", out hide_panel);
                            if (string.IsNullOrEmpty(hide_panel)) hide_panel = "false";
                            ctx.hide_panel = hide_panel;
                        }

                        string cache_key = tree_type + "~" + hide_panel + "~" +
                                            ctx.theme_key + "~" + ctx.device_type + "~" + ctx.lang_key + "~" +
                                            refUri + "~" + url;
                        if (hostCache_Src.hasCache(cache_key))
                        {
                            string page_type = path.ToString_pathExt();
                            //if (path == "/" || path == "" || page_type == "ifc" || page_type == "css" || page_type == "js")
                            if (path == "/" || path == "" || page_type == "css" || page_type == "js")
                            {
                                string content_type = hostType.GetContentType(page_type);
                                string cache_data = hostCache_Src.getCache(cache_key);

                                //Debug.WriteLine("get cache >> " + cache_key);

                                var o = (Response)cache_data;
                                o.ContentType = content_type;
                                return o;
                            }
                        }

                        string uaString = this.Request.Headers.UserAgent.ToLower();
                        this.Context.setClientInfo(uaString);

                        ctx.browser_width = hostUser.session_BrowserWidth_Get(session_id);

                        if (
                            //host_name.StartsWith("admin") &&
                            //uri.Path != "/login"
                                !uri.Path.Contains("/login")
                                && !uri.ToString().Contains(hostUser.page_Login)
                                && uri.Path != "/logout"
                                && (
                            // uri.Path.Contains("/lang/") == false ||
                                    uri.Path.Contains(".") == false ||
                                    uri.Path.EndsWith(hostServer.pathExtSite) ||
                                    uri.Path.EndsWith(".html") ||
                                    uri.Path.EndsWith(".htm")
                                 )
                            )
                        {
                            if (ctx.Request.Cookies.ContainsKey("token_id") == false)
                            {
                                return f_login(ctx, session_id, url);
                            }
                            else
                            {
                                string token_id_ = ctx.Request.Cookies["token_id"];
                                string username = tokenLogin.decode_get_username(token_id_);

                                if (string.IsNullOrEmpty(username) && path != "home")
                                {
                                    return f_login(ctx, session_id, url);
                                }
                                else
                                {
                                    this.Context.username = username;
                                    this.Context.token_id = token_id_;
                                }
                            }
                        }
                        else
                        {
                            string u_login = url;
                            if (u_login.EndsWith("/")) u_login = u_login.Substring(0, u_login.Length - 1);

                            if (u_login == hostUser.page_Login)
                                return f_login(ctx, session_id, hostUser.page_Main);
                        }

                        return null;
                    }));

            After.AddItemToEndOfPipeline(ctx =>
            {
                //////string refUri = this.Request.Headers.Referrer;
                //////if (refUri == null) refUri = "";

                //////string tree_type = "";
                //////ctx.Request.Cookies.TryGetValue("tree_type", out tree_type);
                //////if (string.IsNullOrEmpty(tree_type)) tree_type = "";

                //////string cache_key = tree_type + "~" + ctx.hide_panel + "~" +
                //////                    ctx.theme_key + "~" + ctx.device_type + "~" + ctx.lang_key + "~" +
                //////                    refUri + "~" + ctx.Request.Url.ToString();
                //////if (hostCache_Src.hasCache(cache_key) == false)
                //////{
                //////    Task.Factory.StartNew((object obj) =>
                //////    {
                //////        resCache res = (resCache)obj;
                //////        using (var stream = new MemoryStream())
                //////        {
                //////            res.data.Invoke(stream);
                //////            stream.Position = 0;
                //////            using (var reader = new StreamReader(stream))
                //////            {
                //////                // get the origin data
                //////                var cache_data = reader.ReadToEnd();
                //////                hostCache_Src.setCache(res.cache_key, cache_data);
                //////                //Debug.WriteLine("set cache >> " + res.cache_key);
                //////            }
                //////        }
                //////    }, new resCache(ctx.Response.Contents, cache_key));
                //////}

            });

















            //////////Before.AddItemToEndOfPipeline(ctx =>
            //////////{
            //////////    ctx.host_main = hostUser.page_Main;
            //////////    ctx.host_login = hostUser.page_Login;
            //////////    ctx.host_connect = hostUser.page_Signal;

            //////////    for (int i = 0; i < hostServer.host_memner.Length; i++) {
            //////////        if (i < hostServer.uri_memner.Length)
            //////////            ctx.Items.Add(hostServer.host_memner[i], hostServer.uri_memner[i]);
            //////////    }                 

            //////////    var uri = ctx.Request.Url;

            //////////    ctx.port = (int)uri.Port;
            //////////    ctx.domain = hostServer.getDomain(ctx.port);
            //////////    ctx.domain_dns = uri.SiteBase.Split('/')[2];

            //////////    string url = uri.ToString(), host_name = uri.HostName, path = uri.Path;
            //////////    if (string.IsNullOrEmpty(path) || path == "/") path = "home";
            //////////    else path = path.Substring(1);
            //////////    ctx.path = path;

            //////////    string refUri = this.Request.Headers.Referrer; if (refUri == null) refUri = "";

            //////////    string session_id = "";
            //////////    ctx.Request.Cookies.TryGetValue("session_id", out session_id);
            //////////    if (string.IsNullOrEmpty(session_id))
            //////////        session_id = Guid.NewGuid().ToString();
            //////////    ctx.session_id = session_id;
            //////////    //this.Context.session_id = session_id;

            //////////    ctx.device_type = hostUser.session_deviceType_Get(session_id);
            //////////    ctx.lang_key = hostUser.langGet(session_id);
            //////////    ctx.theme_key = hostUser.session_themeKey_Get(session_id);

            //////////    string tree_type = "";
            //////////    ctx.Request.Cookies.TryGetValue("tree_type", out tree_type);
            //////////    if (string.IsNullOrEmpty(tree_type)) tree_type = "";
            //////////    ctx.tree_type = tree_type;



            //////////    string hide_panel = "";
            //////////    ctx.Request.Cookies.TryGetValue("hide_panel", out hide_panel);
            //////////    if (string.IsNullOrEmpty(hide_panel)) hide_panel = "false";
            //////////    ctx.hide_panel = hide_panel;

            //////////    string cache_key = tree_type + "~" + hide_panel + "~" +
            //////////                        ctx.theme_key + "~" + ctx.device_type + "~" + ctx.lang_key + "~" +
            //////////                        refUri + "~" + url;
            //////////    if (hostCache_Src.hasCache(cache_key))
            //////////    {
            //////////        string page_type = path.ToString_pathExt();
            //////////        //if (path == "/" || path == "" || page_type == "ifc" || page_type == "css" || page_type == "js")
            //////////        if (path == "/" || path == "" || page_type == "css" || page_type == "js")
            //////////        {
            //////////            string content_type = hostType.GetContentType(page_type);
            //////////            string cache_data = hostCache_Src.getCache(cache_key);

            //////////            //Debug.WriteLine("get cache >> " + cache_key);

            //////////            var o = (Response)cache_data;
            //////////            o.ContentType = content_type;
            //////////            return o;
            //////////        }
            //////////    }

            //////////    string uaString = this.Request.Headers.UserAgent.ToLower();
            //////////    this.Context.setClientInfo(uaString);

            //////////    ctx.browser_width = hostUser.session_BrowserWidth_Get(session_id);

            //////////    if (
            //////////            //host_name.StartsWith("admin") &&
            //////////            //uri.Path != "/login"
            //////////            !uri.Path.Contains("/login")
            //////////            && !uri.ToString().Contains(hostUser.page_Login)
            //////////            && uri.Path != "/logout"
            //////////            && (
            //////////                // uri.Path.Contains("/lang/") == false ||
            //////////                uri.Path.Contains(".") == false ||
            //////////                uri.Path.EndsWith(".html") ||
            //////////                uri.Path.EndsWith(".htm")
            //////////             )
            //////////        )
            //////////    {
            //////////        if (ctx.Request.Cookies.ContainsKey("token_id") == false)
            //////////        {
            //////////            return f_login(ctx, session_id, url);
            //////////        }
            //////////        else
            //////////        {
            //////////            string token_id_ = ctx.Request.Cookies["token_id"];
            //////////            string username = MySecureTokenValidator.Login2(token_id_);

            //////////            if (string.IsNullOrEmpty(username))
            //////////            {
            //////////                return f_login(ctx, session_id, url);
            //////////            }
            //////////            else
            //////////            {
            //////////                this.Context.username = username;
            //////////                this.Context.token_id = token_id_;
            //////////            }
            //////////        }
            //////////    }
            //////////    else
            //////////    {
            //////////        string u_login = url;
            //////////        if (u_login.EndsWith("/")) u_login = u_login.Substring(0, u_login.Length - 1);

            //////////        if (u_login == hostUser.page_Login)
            //////////            return f_login(ctx, session_id, hostUser.page_Main);
            //////////    }

            //////////    return null;
            //////////});

            //////////After.AddItemToEndOfPipeline(ctx =>
            //////////{
            //////////    //////string refUri = this.Request.Headers.Referrer;
            //////////    //////if (refUri == null) refUri = "";

            //////////    //////string tree_type = "";
            //////////    //////ctx.Request.Cookies.TryGetValue("tree_type", out tree_type);
            //////////    //////if (string.IsNullOrEmpty(tree_type)) tree_type = "";

            //////////    //////string cache_key = tree_type + "~" + ctx.hide_panel + "~" +
            //////////    //////                    ctx.theme_key + "~" + ctx.device_type + "~" + ctx.lang_key + "~" +
            //////////    //////                    refUri + "~" + ctx.Request.Url.ToString();
            //////////    //////if (hostCache_Src.hasCache(cache_key) == false)
            //////////    //////{
            //////////    //////    Task.Factory.StartNew((object obj) =>
            //////////    //////    {
            //////////    //////        resCache res = (resCache)obj;
            //////////    //////        using (var stream = new MemoryStream())
            //////////    //////        {
            //////////    //////            res.data.Invoke(stream);
            //////////    //////            stream.Position = 0;
            //////////    //////            using (var reader = new StreamReader(stream))
            //////////    //////            {
            //////////    //////                // get the origin data
            //////////    //////                var cache_data = reader.ReadToEnd();
            //////////    //////                hostCache_Src.setCache(res.cache_key, cache_data);
            //////////    //////                //Debug.WriteLine("set cache >> " + res.cache_key);
            //////////    //////            }
            //////////    //////        }
            //////////    //////    }, new resCache(ctx.Response.Contents, cache_key));
            //////////    //////}

            //////////});
        }
    }//end class

}
