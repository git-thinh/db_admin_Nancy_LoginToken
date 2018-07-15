
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

//using Thinktecture.IdentityModel.Hawk.Core;
//using Thinktecture.IdentityModel.Hawk.Core.Helpers;

using System.Net;

namespace host
{

    public class nancyTheme : moduleAuthorization
    {
        //public nancyTheme(IConfigProvider configProvider, IJwtWrapper jwtWrapper) :
        public nancyTheme() :
            base("/theme")
        {
            Get["/{theme_key}"] = x =>
            {
                string theme_key = x.theme_key;
                 
                string refUri = this.Request.Headers.Referrer;
                refUri = System.Web.HttpUtility.UrlDecode(refUri);

                if (string.IsNullOrWhiteSpace(refUri) || refUri.Contains("/login"))
                    refUri = hostUser.user_refUri_Closest_Get(this.Context.session_id);

                if (string.IsNullOrWhiteSpace(refUri))
                    refUri = hostUser.page_Main;

                if (!string.IsNullOrEmpty(theme_key))
                {
                    hostUser.session_themeKey_Set(this.Context.session_id, theme_key);
                }

                var o = Response.AsRedirect(refUri)
                        .WithCookie(new Nancy.Cookies.NancyCookie("theme_key", theme_key, DateTime.Now.AddDays(10)));
                return o;
            };
        }
    }

}
