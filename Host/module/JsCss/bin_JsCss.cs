using Nancy;
//using Nancy.Owin.StatelessAuth;
using System.Linq;

namespace host
{
    public class binJsCssNancy : moduleAuthorization
    {
        //public binJsCssNancy(IConfigProvider configProvider, IJwtWrapper jwtWrapper) 
        public binJsCssNancy() 
            : base("/bin")
        {
            //Get["/{folder}/{module}/{theme_key}/{device_type}/css"] = parameters =>
            Get["{cache_id}.css"] = parameters =>
            {
                string data = "";
                string cache_id = "bin/" + parameters.cache_id + ".css";
                
                data = hostServer.getCache(cache_id);
                data = NancyContextKey.renderKey(data, this.Context);
                
                var o = (Response)data;
                o.StatusCode = Nancy.HttpStatusCode.OK;
                o.ContentType = "text/css";
                return o;
            };

            Get["{cache_id}.js"] = parameters =>
            {
                string data = "";
                string cache_id = "bin/" + parameters.cache_id + ".js";

                data = hostServer.getCache(cache_id);
                data = NancyContextKey.renderKey(data, this.Context);

                if (data.Contains("@pagekey"))
                {
                    string s = this.Request.Headers.Referrer, pagekey = "";
                    if (!string.IsNullOrEmpty(s))
                    {
                        s = System.Web.HttpUtility.UrlDecode(s);
                        var a = s.Split('?')[0].Split('#')[0].Split('/');
                        pagekey = a[a.Length - 1].Replace('-', '_').Replace('.', '_');
                        if(pagekey == "") pagekey = a[2].Replace('-', '_').Replace('.', '_');
                    }
                    data = data.Replace("@pagekey", pagekey);
                }

                var o = (Response)data;
                o.StatusCode = Nancy.HttpStatusCode.OK;
                o.ContentType = "text/javascript";
                return o;
            };

        }
    }

}
