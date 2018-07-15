using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Web.Http;
using System.Net.Http.Headers;
using System.Reflection;

namespace host
{
    public class syncController : ApiController
    {

        public syncController()
        {
        }


        [HttpGet]
        public HttpResponseMessage Get()
        {

            string s = "";

            //using (var db = dbConnect<dbAPI>.Open("http://localhost:51387/"))
            //{
            //    s = db.API.GetMessage();
            //}

            s = "Upload dynamic API to call at: " + DateTime.Now.ToString();

            var response = new HttpResponseMessage();
            response.Content = new StringContent(s, System.Text.Encoding.UTF8);
            response.Content.Headers.ContentType = new MediaTypeHeaderValue("text/html");
            return response;
        }

        public static List<dynamic> list = new List<dynamic>() { };
        [HttpPost]
        public HttpResponseMessage PostData(dynamic[] items)
        {
            list.AddRange(items);
            string message = String.Format("Hello, {0}. Thanks for flying Hawk", items.Length);
            return Request.CreateResponse<string>(System.Net.HttpStatusCode.OK, message);
        }


        //[HttpPost]
        //public HttpResponseMessage Post([FromBody]string name)
        //{
        //    string message = String.Format("Hello, {0}. Thanks for flying Hawk", name);
        //    return Request.CreateResponse<string>(System.Net.HttpStatusCode.OK, message);
        //}


        ////[Authorize]
        //[Route("list")]
        //[HttpGet]
        //public IEnumerable<dynamic> GetList()
        //{
        //    return list;
        //}

        //public static List<dynamic> list = new List<dynamic>() { };
        //[HttpPost]
        //[Route("")]
        //public IEnumerable<dynamic> PostData(dynamic[] items)
        //{
        //    list.AddRange(items);
        //    return products;
        //}

        ////public static IEnumerable<oELSTER_TSVH_MapAPI> db;// = new List<oELSTER_TSVH_MapAPI>() { };

        //// POST api/Orders 
        ////public HttpResponseMessage PostData([FromBody]IEnumerable<dynamic> items)
        ////public HttpResponseMessage PostData(oELSTER_TSVH_MapAPI[] items)
        ////public HttpResponseMessage PostData([FromBody]JObject[] items)
        //public HttpResponseMessage PostData(dynamic[] items)
        //{
        //    // Extract your concrete objects from the json object.
        //    //var content = stuff["content"].ToObject<Content>();
        //    //var config = items.AsParallel().Select(i => i.ToObject<oELSTER_TSVH_MapAPI>()).ToList();

        //    try
        //    {
        //        //var ls = items.Cast<oELSTER_TSVH_MapAPI>().ToList();

        //        //Mapper.CreateMap<dynamic, oELSTER_TSVH_MapAPI>();
        //        //var ls = Mapper.Map<oELSTER_TSVH_MapAPI[]>(items);

        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine(ex.Message);
        //    }

        //    //db.Concat(ls);


        //    Product o = new Product() { Id = new Random().Next(111111, 999999999), Name = Guid.NewGuid().ToString() };

        //    //HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.Created, o);
        //    //response.Headers.Location = new Uri(Url.Link("DefaultApi", new { id = o.Id }));
        //    //return response;
        //    //}
        //    //else
        //    //{
        //    //    return Request.CreateResponse(HttpStatusCode.BadRequest);
        //    //}

        //    var response = new HttpResponseMessage();
        //    response.Content = new StringContent(DateTime.Now.ToString(), System.Text.Encoding.UTF8);
        //    response.Content.Headers.ContentType = new MediaTypeHeaderValue("text/html");
        //    return response;


        //}



    }
}

