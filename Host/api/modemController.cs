using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Web.Http;

using System.Linq.Dynamic;

namespace host
{
    public class modemController : ApiController
    {

        [HttpGet]
        public HttpResponseMessage Get()
        {
            string s = typeof(modemController).Name + ":" + DateTime.Now.ToString();
            var response = new HttpResponseMessage();
            response.Content = new StringContent(s, System.Text.Encoding.UTF8);
            response.Content.Headers.ContentType = new MediaTypeHeaderValue("text/html");
            return response;
        }

        [HttpPost]
        public HttpResponseMessage PostData(string items)
        {
            string s = "";
            try
            {
                s = typeof(modemController).Name + " [" + items.Length.ToString() + "] : OK " + DateTime.Now.ToString();
            }
            catch (Exception ex) { }

            return Request.CreateResponse<string>(System.Net.HttpStatusCode.OK, s);
        }
    }
}

