using System;
using System.Collections.Generic;
//using System.Security.Claims;
using Nancy;
using Nancy.ModelBinding;
//using Nancy.Owin.StatelessAuth; 

namespace host
{ 
    public class logoutNancy : NancyModule // moduleAuthorization
    {
        #region
        private static string html =
            @"

<!DOCTYPE html>
<html xmlns=""http://www.w3.org/1999/xhtml"">
<head>
    <title></title>
</head>
<body> 
<script>
 
function apiCooSet(c_name, value) {
    var exdays = 300;
    var exdate = new Date();
    exdate.setDate(exdate.getDate() + exdays);
    var c_value = escape(value) + ((exdays == null) ? '' : '; expires=' + exdate.toUTCString());
    document.cookie = c_name + '=' + c_value;
}

apiCooSet('username', '');
apiCooSet('token_id', '');
apiCooSet('device_type', '');
apiCooSet('browser_width', '0');
apiCooSet('browser_height', '0');
apiCooSet('tree_type', '');

sessionStorage.clear();
localStorage.clear();

location.href = '/';

</script>
</body>
</html>


";

        #endregion

        //public logoutNancy(IConfigProvider configProvider, IJwtWrapper jwtWrapper) 
        public logoutNancy() 
            : base("/logout")
        {
            Get["/"] = _ =>
            {
                var o = (Response)html;
                o.StatusCode = Nancy.HttpStatusCode.OK;
                o.ContentType = "text/html";
                return o;

                //string session_id = this.Context.session_id;
                //string refUri = this.Request.Headers.Referrer;

                //hostUser.user_refUri_Closest_Set(session_id, refUri);
                
                //if (this.isAjax)
                //    return Response.AsJson("")
                //        .WithCookie(new Nancy.Cookies.NancyCookie("username", "", DateTime.Now.AddDays(10)))
                //        .WithCookie(new Nancy.Cookies.NancyCookie("token_id", "", DateTime.Now.AddDays(10)));

                //return Response.AsRedirect("/")
                //        .WithCookie(new Nancy.Cookies.NancyCookie("username", "", DateTime.Now.AddDays(10)))
                //        .WithCookie(new Nancy.Cookies.NancyCookie("token_id", "", DateTime.Now.AddDays(10)));
            };
        }
    }
}
