
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

//using sysSearchLib;
using System.Text;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using host;
using model;
using System.Linq.Mustache;

namespace host
{

    public class cacheExport : NancyModule
    {

        #region
        private static string html =
            @"

<!DOCTYPE html>
<html xmlns=""http://www.w3.org/1999/xhtml"">
<head>
    <title>@domain/@path</title>
</head>
<body> 

    <script>

function call(key)
{
var xhr; 
    try {  xhr = new ActiveXObject('Msxml2.XMLHTTP');   }
    catch (e) 
    {
        try {   xhr = new ActiveXObject('Microsoft.XMLHTTP');    }
        catch (e2) 
        {
          try {  xhr = new XMLHttpRequest();     }
          catch (e3) {  xhr = false;   }
        }
     }
   
xhr.onreadystatechange  = function()
{ 
if(xhr.readyState  == 4)
{
if(xhr.status  == 200) 
{
//-----------------------------------------
    var data = xhr.responseText; 
    //console.log(key + '= ' + data);
    if(key == 'node'){
        var a = data.toString().split('|');
        for(var k =0; k < a.length ; k++)
        {
            call(a[k]);
        }
    }
    else
    {
        var a = location.href.toString().split('/');
        var lik = a[0] + '//' + a[2];
        var data = 'cache_node//' + key + '//' + encodeURIComponent(data);
        parent.postMessage(data,lik);
    }

//--------------------------------------
}
else
var data = 'Error code ' + xhr.status;
}
}; 
 
   xhr.open('POST', 'cache/'+key,  true); 
   xhr.send(null); 
}

 call('node');

    </script>
</body>
</html>


";

        #endregion

        public cacheExport()
            : base("/cache")
        {
            Get["/"] = x =>
            {
                #region //....
                var o = (Response)html;
                o.StatusCode = Nancy.HttpStatusCode.OK;
                o.ContentType = "text/html";
                return o;
                #endregion
            };

            Post["/{key}"] = pr =>
            {
                string data = "";
                string key = pr.key;
                if (!string.IsNullOrWhiteSpace(key))
                {
                    ObjectCache cache = MemoryCache.Default;
                    if (key == "node")
                        data = cache[key] as string;
                    else
                    {
                        long id = key.Trim().TryParseToLong();
                        if (id > 0)
                            data = db_node.cache_node_get(id);
                    }

                    if (string.IsNullOrWhiteSpace(data)) data = "";
                }

                #region //....

                var o = (Response)data;
                o.StatusCode = Nancy.HttpStatusCode.OK;
                o.ContentType = "text/plain";
                return o;

                #endregion
            };
        }

    }//end class

}
