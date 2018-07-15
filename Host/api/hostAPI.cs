using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading;
using System.Web.Http;
using System.Web.Http.SelfHost;

namespace host
{
    public class hostAPI
    {
        private const int DefaultMaxConcurrentRequests = 1000;
        public static string init()
        {
            ThreadPool.SetMaxThreads(1000, 100);
            string uri = "http://" + ConfigurationManager.AppSettings["db_api_uri"] + ":" + ConfigurationManager.AppSettings["db_api_uri_port"];

            HttpSelfHostConfiguration config = new HttpSelfHostConfiguration(uri);

            config.MaxReceivedMessageSize = 2147483647; //use config for this value            
            config.TransferMode = TransferMode.StreamedRequest; //requests only            
            config.TransferMode = TransferMode.StreamedResponse; //responses only            
            config.TransferMode = TransferMode.Streamed; // both

            config.ReceiveTimeout = new TimeSpan(0, 9, 0);
            config.SendTimeout = new TimeSpan(0, 9, 0);
            //config.IncludeErrorDetailPolicy = IncludeErrorDetailPolicy.Never;
            config.MaxBufferSize = 2147483647;
            config.MaxReceivedMessageSize = 2147483647;
            config.MaxConcurrentRequests = Math.Max(Environment.ProcessorCount * DefaultMaxConcurrentRequests, DefaultMaxConcurrentRequests);


            config.Routes.MapHttpRoute(
                name: "default",
                routeTemplate: "{controller}/{id}",
                defaults: new { id = RouteParameter.Optional });

            ////////////config.MapHttpAttributeRoutes();
            //////////////config.Routes.MapHttpRoute(
            //////////////    name: "default",
            //////////////    routeTemplate: "{controller}/{id}",
            //////////////    defaults: new { id = RouteParameter.Optional });
            ////////////config.Routes.IgnoreRoute("{resource}.axd/{*pathInfo}", "");
            ////////////config.Routes.IgnoreRoute("{resource}.ico", "");

            //Only JSON OUPUT
            var appXmlType = config.Formatters.XmlFormatter.SupportedMediaTypes.FirstOrDefault(t => t.MediaType == "application/xml");
            config.Formatters.XmlFormatter.SupportedMediaTypes.Remove(appXmlType);
            config.Formatters.JsonFormatter.SupportedEncodings.Add(Encoding.UTF8);

            ////////CorsHandler is also defined in CorsHandler.cs.  It is what enables CORS
            ////////config.MessageHandlers.Add(new CorsHandler());
            //////config.Filters.Add(new MethodAttributeExceptionHandling());

            ////////Microsoft.AspNet.WebApi.Cors
            //////config.EnableCors();
            //////var enableCorsAttribute = new System.Web.Http.Cors.EnableCorsAttribute("*", "*", "*")
            //////{
            //////    SupportsCredentials = true
            //////};
            //////config.EnableCors(enableCorsAttribute);

            //config.Services.Replace(typeof(IAssembliesResolver), new SelfHostAssemblyResolver());
            //config.Services.Replace(typeof(IHttpControllerSelector), new CustomControllerSelector((config)));

            //var credentialStorage = new List<Credential>()
            //{
            //    new Credential()
            //    {
            //        Id = "dh37fgj492je",
            //        Algorithm = SupportedAlgorithms.SHA256,
            //        User = "Steve",
            //        Key = Convert.FromBase64String("wBgvhp1lZTr4Tb6K6+5OQa1bL9fxK7j8wBsepjqVNiQ=")
            //    }
            //};

            //var options = new Options()
            //{
            //    ClockSkewSeconds = 60,
            //    LocalTimeOffsetMillis = 0,
            //    CredentialsCallback = (id) => credentialStorage.FirstOrDefault(c => c.Id == id),
            //    ResponsePayloadHashabilityCallback = (r) => true,
            //    VerificationCallback = (request, ext) =>
            //    {
            //        if (String.IsNullOrEmpty(ext))
            //            return true;

            //        string name = "X-Request-Header-To-Protect";
            //        return ext.Equals(name + ":" + request.Headers[name].First());
            //    }
            //};
            //config.MessageHandlers.Add(new apiHandler(options));
            //////////////config.MessageHandlers.Add(new apiHandler());

            //var cacheCowCacheHandler = new CachingHandler(config)
            //{
            //    AddLastModifiedHeader = false
            //};
            //config.MessageHandlers.Add(cacheCowCacheHandler);


            HttpSelfHostServer server = new HttpSelfHostServer(config);
            server.OpenAsync().Wait();

            //makeCall4();

            main.show_notification(uri);

            return uri;
        }
    }
}
