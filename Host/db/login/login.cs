using System;
using System.Collections.Generic;
//using System.Security.Claims;
using Nancy;
using Nancy.ModelBinding;
//using Nancy.Owin.StatelessAuth;
//using Owin;
using System.IO;
using Nancy.TinyIoc;
using Nancy.Bootstrapper;
using Nancy.Owin;
using System.Linq;
using Nancy.Conventions;
using System.Threading;
using System.Threading.Tasks;
using System.Runtime.Caching;
//using Thinktecture.IdentityModel.Hawk.Client;
//using System.Net.Http;
//using Thinktecture.IdentityModel.Hawk.WebApi;
using System.Net;
//using Thinktecture.IdentityModel.Hawk.Core;
//using Thinktecture.IdentityModel.Hawk.Core.Helpers;

using System.Text;
using System.Dynamic;
using Nancy.ViewEngines;
using Nancy.ViewEngines.SuperSimpleViewEngine;
using Nancy.Responses;
using model;

namespace host
{
    public class loginNancy : NancyModule // moduleAuthorization
    {
        //public loginNancy(IConfigProvider configProvider, IJwtWrapper jwtWrapper) :
        public loginNancy() :
            base("/login")
        {
            #region >>> Post Login v1...

            //Post["/"] = _ =>
            //{
            //    var user = this.Bind<oUser>();

            //    oUser u = hostUser.login(user);
            //    if (u == null)
            //    {
            //        //return 401;
            //        //return Response.AsRedirect("/login?msg=Please login again");

            //        ViewBag.msg = "Please login again...";

            //        string fLogin = "/login/index.htm";
            //        string html = this.Render(fLogin, null);
            //        return Response.AsHTML(html);
            //    }

            //    string host = this.Request.Url.HostName;

            //    var time = DateTime.UtcNow.AddDays(7);
            //    var jwttoken = new JwtToken()
            //    {
            //        Issuer = "http://" + host,
            //        Audience = "http://" + host,
            //        Claims =
            //            new List<Claim>(new[]
            //            {
            //                //new Claim("http://schemas.microsoft.com/ws/2008/06/identity/claims/role", "Administrator"),
            //                //new Claim(ClaimTypes.Name, "admin")
            //                new Claim(ClaimTypes.Name, u.username)
            //            }),
            //        Expiry = time
            //    };

            //    var token = jwtWrapper.Encode(jwttoken, MySecureTokenValidator.securekey, JwtHashAlgorithm.HS256);

            //    hostUser.tokenAdd(u.username, token, time);

            //    //return Negotiate.WithModel(token);

            //    if (this.isAjax)
            //        return Response.AsJson(token)
            //            .WithCookie(new Nancy.Cookies.NancyCookie("token_id", token, DateTime.Now.AddDays(10)))
            //            .WithCookie(new Nancy.Cookies.NancyCookie("lang_key", hostUser.langGet(u.username), DateTime.Now.AddDays(10)));

            //    return Response.AsRedirect("/")
            //        .WithCookie(new Nancy.Cookies.NancyCookie("token_id", token, DateTime.Now.AddDays(10)))
            //        .WithCookie(new Nancy.Cookies.NancyCookie("lang_key", hostUser.langGet(u.username), DateTime.Now.AddDays(10)));
            //};

            #endregion

            #region >>> Post Login v2 ...

            //Post["/"] = _ =>
            //{
            //    var user = this.Bind<m_user>();

            //    m_user u = db_user.login(user);
            //    if (u.status == false)
            //    {
            //        //return 401;
            //        //return Response.AsRedirect("/login?msg=Please login again");

            //        //ViewBag.msg = "Please login again...";

            //        //string fLogin = "/login/index.htm";
            //        //string html = this.Render(fLogin, null);
            //        //return Response.AsHTML(html);

            //        string uri_login_again = "http://" + hostUser.page_Login + "?msg=Vui+l%C3%B2ng+%C4%91%C4%83ng+nh%E1%BA%ADp+t%C3%A0i+kho%E1%BA%A3n+ch%C3%ADnh+x%C3%A1c";
            //        return Response.AsRedirect(uri_login_again);
            //    }

            //    string host = this.Context.domain_dns;// this.Request.Url.HostName;

            //    var time = DateTime.UtcNow.AddDays(7);

            //    string token = "";

            //    ////////var jwttoken = new JwtToken()
            //    ////////{
            //    ////////    Issuer = "http://" + host,
            //    ////////    Audience = "http://" + host,
            //    ////////    Claims =
            //    ////////        new List<Claim>(new[]
            //    ////////        {
            //    ////////            //new Claim("http://schemas.microsoft.com/ws/2008/06/identity/claims/role", "Administrator"),
            //    ////////            //new Claim(ClaimTypes.Name, "admin")
            //    ////////            new Claim(ClaimTypes.Name, u.username)
            //    ////////        }),
            //    ////////    Expiry = time
            //    ////////};

            //    ////////var token = jwtWrapper.Encode(jwttoken, MySecureTokenValidator.securekey, JwtHashAlgorithm.HS256);

            //    var dui = new Dictionary<string, object>();
            //    dui.Add(ClaimTypes.Name, u.username);

            //    var jwttoken = new JwtToken()
            //    {
            //        Issuer = "http://" + host,
            //        Audience = "http://" + host,
            //        Claims = new Dictionary<string, object>[] { dui },
            //        Expiry = time
            //    };

            //    token = JsonWebToken.Encode(jwttoken, tokenLogin.securekey, JwtHashAlgorithm.HS256);

            //    hostUser.tokenAdd(u.username, token, time);

            //    //return Negotiate.WithModel(token);

            //    //if (this.isAjax)
            //    //    return Response.AsJson(token)
            //    //        .WithCookie(new Nancy.Cookies.NancyCookie("token_id", token, DateTime.Now.AddDays(10)))
            //    //        .WithCookie(new Nancy.Cookies.NancyCookie("lang_key", hostUser.langGet(u.username), DateTime.Now.AddDays(10)));

            //    //return Response.AsRedirect("/")
            //    //    .WithCookie(new Nancy.Cookies.NancyCookie("token_id", token, DateTime.Now.AddDays(10)))
            //    //    .WithCookie(new Nancy.Cookies.NancyCookie("lang_key", hostUser.langGet(u.username), DateTime.Now.AddDays(10)));

            //    string uri = "/";
            //    string session_id = this.Context.session_id;

            //    int browser_width = 0;
            //    string s_browser_width = "0";
            //    this.Request.Cookies.TryGetValue("browser_width", out s_browser_width);
            //    int.TryParse(s_browser_width.Trim(), out browser_width);
            //    if (browser_width == 0) browser_width = hostUser.browser_width_Default;
            //    this.Context.browser_width = browser_width;
            //    hostUser.session_BrowserWidth_Set(session_id, browser_width);

            //    string device_type = "";
            //    this.Request.Cookies.TryGetValue("device_type", out device_type);
            //    if (string.IsNullOrEmpty(device_type)) device_type = "pc";
            //    this.Context.device_type = device_type;
            //    hostUser.session_deviceType_Set(session_id, device_type);

            //    string refUri = hostUser.user_refUri_Closest_Get(session_id);
            //    if (string.IsNullOrWhiteSpace(refUri))
            //    {
            //        refUri = System.Web.HttpUtility.UrlDecode(this.Request.Headers.Referrer.ToString());
            //        if (refUri.Contains("?"))
            //            refUri = refUri.Split('?')[1].Trim().Base64Decode();
            //        else
            //            refUri = "";
            //    }

            //    if (string.IsNullOrEmpty(refUri)) refUri = "http://" + hostUser.page_Main;

            //    if (string.IsNullOrWhiteSpace(refUri))
            //    {
            //        ViewBag.msg = "You login succesful. Redirect link is NULL.";
            //    }
            //    else
            //    {
            //        string theme_key = hostUser.session_themeKey_Get(session_id);

            //        string[] a = refUri.Split('/');
            //        if (a.Length > 1)
            //            uri = "http://" + a[2] + "/login/go" + "?" +
            //                hostUser.langGet(u.username) + "/" + theme_key + "/" + session_id + "/" + browser_width.ToString() + "/" +
            //                hostUser.page_Site_Member.Base64Encode() + "/" + refUri.Base64Encode() + "/" + token;
            //    }

            //    return Response.AsRedirect(uri);
            //};

            #endregion
            
            #region >>> Post Login base on this one site ...

            Post["/"] = _ =>
            {
                var user = this.Bind<m_user>();
                m_user u = db_user.login(user);
                if (u.status == false)
                {
                    //return 401;
                    return Response.AsRedirect("/?msg=Vui+l%C3%B2ng+%C4%91%C4%83ng+nh%E1%BA%ADp+t%C3%A0i+kho%E1%BA%A3n+ch%C3%ADnh+x%C3%A1c");
                }

                string host = this.Context.domain_dns;// this.Request.Url.HostName;

                var time = DateTime.UtcNow.AddDays(7);

                string token = "";
                var dui = new Dictionary<string, object>();
                dui.Add(ClaimTypes.Name, u.username);

                var jwttoken = new JwtToken()
                {
                    Issuer = "http://" + host,
                    Audience = "http://" + host,
                    Claims = new Dictionary<string, object>[] { dui },
                    Expiry = time
                };

                token = JsonWebToken.Encode(jwttoken, tokenLogin.securekey, JwtHashAlgorithm.HS256);

                hostUser.tokenAdd(u.username, token, time);

                string session_id = this.Context.session_id;

                int browser_width = 0;
                string s_browser_width = "0";
                this.Request.Cookies.TryGetValue("browser_width", out s_browser_width);
                int.TryParse(s_browser_width.Trim(), out browser_width);
                if (browser_width == 0) browser_width = hostUser.browser_width_Default;
                this.Context.browser_width = browser_width;
                hostUser.session_BrowserWidth_Set(session_id, browser_width);

                string device_type = "";
                this.Request.Cookies.TryGetValue("device_type", out device_type);
                if (string.IsNullOrEmpty(device_type)) device_type = "pc";
                this.Context.device_type = device_type;
                hostUser.session_deviceType_Set(session_id, device_type);

                string uri = "/" + hostSite.page_main + "." + hostSite.page_ext;
                return Response.AsRedirect(uri)
                    .WithCookie(new Nancy.Cookies.NancyCookie("username", u.username, DateTime.Now.AddDays(10)))
                    .WithCookie(new Nancy.Cookies.NancyCookie("token_id", token, DateTime.Now.AddDays(10)));
            };

            #endregion

            //Get["/"] = parameters =>
            //{
            //    #region //----------------------

            //    string u = this.Request.Url.ToString();

            //    string url = u.Split('?')[0].ToLower();
            //    string root = hostServer.pathModule + "\\" + this.Context.domain;

            //    string htm = "";
            //    string folder = "login";
            //    string file = root + "\\" + folder + "\\index_.html";

            //    if (File.Exists(file))
            //    {
            //        string lang_key = "";
            //        this.Request.Cookies.TryGetValue("lang_key", out lang_key);

            //        if (string.IsNullOrEmpty(lang_key))
            //            lang_key = hostUser.lang_Default;

            //        this.Context.lang_key = lang_key;

            //        string key = file.ToLower() + "/" + lang_key;

            //        //htm = hostServer.getCache(key);
            //        hostModule.dicModule.TryGetValue(key, out htm);

            //        htm = RenderTemplate(htm, null);
            //    }
            //    else
            //        htm = "Can not find page: " + file;

            //    var o = (Response)htm;
            //    o.StatusCode = Nancy.HttpStatusCode.OK;
            //    o.ContentType = "text/html";
            //    o.WithCookie(new Nancy.Cookies.NancyCookie("page_url", url));

            //    return o;

            //    #endregion
            //};


            Get["go"] = x =>
            {
                #region // ...

                string url = this.Request.Url.ToString();
                if (!url.Contains("?"))
                    return Response.AsText("Go link is NULL: /go?{link_base64}");

                string para = url.Split('?')[1].Trim();
                string[] a = para.Split('/').Select(o => o.Trim()).ToArray();
                if (a.Length > 6)
                {
                    string lang_key = a[0], theme_key = a[1], session_id = a[2], browser_width = a[3],
                            site_member = a[4].Base64Decode(), refUri = a[5], token_id = a[6];

                    string[] a_uri = site_member.Split('|').Where(o => o.Length > 3).ToArray();

                    string uri = "";
                    if (a_uri.Length == 0)
                        uri = refUri.Base64Decode();
                    else
                    {
                        site_member = "";
                        for (int k = 1; k < a_uri.Length; k++)
                            if (site_member == "")
                                site_member = a_uri[k];
                            else
                                site_member = site_member + "|" + a_uri[k];

                        uri = "http://" + a_uri[0] + "/login/go?" +
                            lang_key + "/" + theme_key + "/" + session_id + "/" + browser_width + "/" +
                            site_member.Base64Encode() + "/" + refUri + "/" + token_id;
                    }

                    return Response.AsRedirect(uri)
                        .WithCookie(new Nancy.Cookies.NancyCookie("token_id", token_id, DateTime.Now.AddDays(10)))
                        .WithCookie(new Nancy.Cookies.NancyCookie("session_id", session_id, DateTime.Now.AddDays(10)))
                        .WithCookie(new Nancy.Cookies.NancyCookie("browser_width", browser_width, DateTime.Now.AddDays(10)))
                        .WithCookie(new Nancy.Cookies.NancyCookie("theme_key", theme_key, DateTime.Now.AddDays(10)))
                        .WithCookie(new Nancy.Cookies.NancyCookie("lang_key", lang_key, DateTime.Now.AddDays(10)));
                }

                return Response.AsText("Go link is NULL: /go?{link_base64}");

                #endregion
            };

            Get["session/{session_id}"] = x =>
            {
                #region // ...

                string session_id = x.session_id;
                if (string.IsNullOrWhiteSpace(session_id))
                    session_id = "";

                string url = this.Request.Url.ToString();
                if (!url.Contains("?"))
                    return Response.AsText("Go link is NULL: /go/{session_id}?{link_base64}");

                string uri = "http://" + this.Context.domain_dns + "/?" + url.Split('?')[1].Trim();

                return Response.AsRedirect(uri)
                    .WithCookie(new Nancy.Cookies.NancyCookie("session_id", session_id, DateTime.Now.AddDays(10)));
                
                #endregion
            };

            Get["/theme/{theme_key}"] = x =>
            {
                #region // ...

                string theme_key = x.theme_key;
                var o = Response.AsRedirect("/login");

                bool new_user = false;
                string session_id = "";

                if (string.IsNullOrEmpty(theme_key)) theme_key = hostUser.theme_Default;

                this.Request.Cookies.TryGetValue("session_id", out session_id);
                if (string.IsNullOrEmpty(session_id))
                {
                    session_id = Guid.NewGuid().ToString();
                    new_user = true;
                }

                this.Context.session_id = session_id;
                hostUser.session_themeKey_Set(this.Context.session_id, theme_key);

                if (new_user)
                    o = Response.AsRedirect("/login")
                                    .WithCookie(new Nancy.Cookies.NancyCookie("session_id", session_id, DateTime.Now.AddDays(10)))
                                    .WithCookie(new Nancy.Cookies.NancyCookie("theme_key", theme_key, DateTime.Now.AddDays(10)));
                else
                    o = Response.AsRedirect("/login")
                                    .WithCookie(new Nancy.Cookies.NancyCookie("theme_key", theme_key, DateTime.Now.AddDays(10)));

                return o;

                #endregion
            };

            Get["/lang/{lang_key}"] = x =>
            {
                #region // ...

                string lang_key = x.lang_key;
                var o = Response.AsRedirect("/login");

                bool new_user = false;
                string session_id = "";

                if (string.IsNullOrEmpty(lang_key)) lang_key = hostUser.lang_Default;

                this.Request.Cookies.TryGetValue("session_id", out session_id);
                if (string.IsNullOrEmpty(session_id))
                {
                    session_id = Guid.NewGuid().ToString();
                    new_user = true;
                }

                this.Context.session_id = session_id;
                hostUser.langSet(this.Context.session_id, lang_key);

                if (new_user)
                    o = Response.AsRedirect("/login")
                                    .WithCookie(new Nancy.Cookies.NancyCookie("session_id", session_id, DateTime.Now.AddDays(10)))
                                    .WithCookie(new Nancy.Cookies.NancyCookie("lang_key", lang_key, DateTime.Now.AddDays(10)));
                else
                    o = Response.AsRedirect("/login")
                                    .WithCookie(new Nancy.Cookies.NancyCookie("lang_key", lang_key, DateTime.Now.AddDays(10)));

                return o;

                #endregion
            };


        }//end class
    }
}
