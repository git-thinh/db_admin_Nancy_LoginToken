
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

//using sysSearchLib;

namespace host
{

    public class nancyLang : moduleAuthorization
    {
        //public nancyLang(IConfigProvider configProvider, IJwtWrapper jwtWrapper)
        public nancyLang()
            : base("/lang")
        {
            Get["/{lang_key}"] = x =>
            {
                string lang_key = x.lang_key;
                var o = Response.AsRedirect("/login");

                string refUri = this.Request.Headers.Referrer;
                if (!string.IsNullOrEmpty(refUri))
                {
                    if (!string.IsNullOrEmpty(lang_key))
                    {
                        hostUser.langSet(this.Context.session_id, lang_key);
                    }

                    refUri = System.Web.HttpUtility.UrlDecode(refUri);
                    o = Response.AsRedirect(refUri)
                            .WithCookie(new Nancy.Cookies.NancyCookie("lang_key", lang_key, DateTime.Now.AddDays(10)));
                }

                return o;
            };
        }
    }

}
