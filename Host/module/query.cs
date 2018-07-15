
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

namespace host
{ 
    public class nancyQuery : NancyModule
    {         
        public nancyQuery()
            : base("/query")
        {
            Post["/"] = x =>
            {
                string data = "";

                var itemp = this.Request.Query;
                var parr = new Dictionary<string, string>();
                foreach (var key in itemp.Keys)
                    parr.Add(key, itemp[key]);

                var o = (Response)data;
                o.StatusCode = Nancy.HttpStatusCode.OK;
                o.ContentType = "text/html";

                return o;
            };

             

            //Get["/page"] = x =>
            //{
            //    string path = hostServer.pathRoot + @"views\";
            //    string data = File.ReadAllText(path);
            //    var o = (Response)data;
            //    o.StatusCode = Nancy.HttpStatusCode.OK;
            //    o.ContentType = "text/html";

            //    return o;
            //};
        }



    }//end class

}
