
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
using Nancy.Routing.Constraints;
using Nancy.Session;
//using Nancy.LightningCache.Extensions;

namespace host
{
    public class nancyHome : moduleAuthorization  // NancyModule
    {
        public void checkFile(string file)
        {
            //Task.Factory.StartNew((object fi) =>
            //{
            //    string f = fi as string;
            //    if (!File.Exists(f))
            //    {
            //        using (StreamWriter sw = File.AppendText(@"C:\log.txt"))
            //        {
            //            sw.WriteLine(f);
            //        }
            //    }
            //}, file);
        }


        private Response getFileStatic(string ext, string content_type, string url, int level, dynamic parameters, NancyModule page)
        {
            string[] a = url.Split(new string[] { "//" }, StringSplitOptions.None);
            string path_file = hostServer.pathModule + "\\" + a[a.Length - 1].Replace("/", "\\");


            var o = Response.AsText(url);

            if (ext == "css" || ext == "js" || ext == "html")
            {
                ObjectCache cache = MemoryCache.Default;
                string data = cache[path_file] as string;
                o = (Response)data;
                o.StatusCode = HttpStatusCode.OK;
                o.ContentType = content_type;
            }
            else
            {
                o = Response.AsFile(path_file).WithContentType(content_type);
            }

            return o;
            //return Response.AsFile(path_file).WithContentType(content_type);
        }

        private Response getFileModule(dynamic parameters, NancyModule page)
        {
            string file_name = parameters.value;

            string[] a = file_name.Split('.');
            string ext = a[a.Length - 1];
            string content_type = "";

            content_type = hostType.GetContentType(ext);
            var o = (Response)"";

            if (ext == "css" || ext == "js")
            {
                string file = parameters.path1 + "\\" + parameters.path2 + "\\" + file_name;

                checkFile(file);

                if (hostModule.dicModule.ContainsKey(file))
                {
                    string data = hostModule.dicModule[file];
                    data = RenderTemplate(data, null);
                    o = (Response)data;
                }

                o.StatusCode = HttpStatusCode.OK;
                o.ContentType = content_type;
            }
            else
            {
                string file = hostServer.pathModule + "\\" + parameters.path1 + "\\" + parameters.path2 + "\\" + file_name;

                checkFile(file);

                o = Response.AsFile(file).WithContentType(content_type);
            }
            return o;
        }

        private Response getFileResource(string url, dynamic parameters, NancyModule page)
        {
            string file_name = parameters.value;

            string[] a = file_name.Split('.');
            string ext = a[a.Length - 1];
            string content_type = "";

            content_type = hostType.GetContentType(ext);
            var o = (Response)"";

            string file = hostServer.pathModule + "\\" + url.Replace("../", "").Replace("/", "\\");
            file = file.Replace("\\\\", "\\");

            if (ext == "css" || ext == "js")
            {
                if (hostModule.dicModule.ContainsKey(file))
                {
                    string data = hostModule.dicModule[file];
                    data = RenderTemplate(data, null);
                    o = (Response)data;
                }

                o.StatusCode = HttpStatusCode.OK;
                o.ContentType = content_type;
            }
            else
            {
                o = Response.AsFile(file).WithContentType(content_type);
            }
            return o;
        }

        private Response getFileModuleCSS(string refUri, string filePath, NancyModule page)
        {
            string[] a = filePath.Split('/');
            string file_name = a[a.Length - 1];

            a = file_name.Split('.');
            string ext = a[a.Length - 1];
            string content_type = hostType.GetContentType(ext);

            a = refUri.Split('/');
            string cssFileName = a[a.Length - 1];

            refUri = refUri.Substring(0, refUri.Length - cssFileName.Length);

            var u = new Uri(refUri + filePath.Substring(1));
            string file = hostServer.pathModule + u.LocalPath.Replace("/", "\\");

            if (File.Exists(file))
                return page.Response.AsFile(file).WithContentType(content_type);

            return 400;
        }

        private Response getFile(int level, dynamic parameters, NancyModule page)
        {
            var ri = this.Request.Url;
            //string root = hostServer.pathModule + "\\" + ri.HostName;
            string root = hostServer.pathModule + "\\" + this.Context.domain;

            string url = ri.ToString().ToLower().Split('?')[0];

            string[] a = url.Split('.');
            string ext = a[a.Length - 1];
            string content_type = "";
            a = url.Split('/');
            string file_name = a[a.Length - 1];
            content_type = hostType.GetContentType(ext);

            var o = (Response)"";
            if (ri.Path.StartsWith("/_") && ri.Path.Contains("."))
            {
                //string key_ri = url.Replace("http://", string.Empty);
                string key_ri = page.Context.domain + ri.Path;
                string data = "";

                hostModule.dicModule.TryGetValue(key_ri, out data);

                o = (Response)data;
                o.StatusCode = HttpStatusCode.OK;
                o.ContentType = content_type;
            }
            else
            {
                string path1 = parameters.path1;
                //if (path1 != null)
                //{
                //    switch (path1.ToArray()[0])
                //    {
                //        case '~':
                //            return getFileResource(url, parameters, page);
                //        case '.':
                //            return getFileModule(parameters, page);
                //    }
                //}


                //if (content_type == "") content_type = "text/plain";

                string refUri = page.Request.Headers.Referrer;
                string host = page.Request.Url.HostName.ToLower();
                string mod_folder = "";
                //if (refUri == "") refUri = this.Request.Url.SiteBase;

                #region >> ref uri ....

                if (!string.IsNullOrEmpty(refUri))
                {
                    if (refUri.EndsWith(".html")) return getFileStatic(ext, content_type, url, level, parameters, page);

                    refUri = System.Web.HttpUtility.UrlDecode(refUri.ToLower().Split('?')[0]).Replace("\\", "/");
                    var uri = new Uri(refUri);
                    string refHost = uri.getDomain(),
                    refPath = uri.LocalPath;

                    if (host == refHost)
                    {
                        if (refUri.EndsWith(".css"))
                        {
                            if (refUri.Contains("/~"))
                                return getFileModuleCSS(refUri, this.Request.Url.Path, this);

                            string page_url = this.Request.Cookies["page_url"];
                            if (!string.IsNullOrEmpty(page_url))
                            {
                                if (page_url.EndsWith(".html")) return getFileStatic(ext, content_type, url, level, parameters, page);

                                page_url = System.Web.HttpUtility.UrlDecode(page_url);

                                var uri0 = new Uri(page_url);
                                string page_path = uri0.LocalPath;

                                if (page_path == "" || page_path == "/")
                                    page_path = "index";
                                else
                                {
                                    if (page_path.StartsWith("/")) page_path = page_path.Substring(1);
                                    page_path = page_path.Split('/')[0];

                                }

                                if (page_path.EndsWith(hostServer.pathExtSite)) page_path = page_path.Substring(0, page_path.Length - hostServer.pathExtSite.Length);
                                mod_folder = page_path;
                            }
                        }
                        else
                        {
                            if (refPath == "" || refPath == "/")
                                mod_folder = "index";
                            else
                            {
                                if (refPath.StartsWith("/")) refPath = refPath.Substring(1);
                                refPath = refPath.Split('/')[0];
                                if (refPath.EndsWith(hostServer.pathExtSite)) refPath = refPath.Substring(0, refPath.Length - hostServer.pathExtSite.Length);
                                mod_folder = refPath;
                            }
                        }
                    }
                }

                #endregion

                o = Response.AsText(url);

                if (mod_folder != "" || ext == "html")
                {
                    string p = "", p1 = path1, p2 = "", p3 = "", p4 = "", p5 = "";
                    switch (level)
                    {
                        case 0:
                            p = "\\";
                            break;
                        case 1:
                            p = "\\" + p1 + "\\";
                            break;
                        case 2:
                            p2 = parameters.path2;
                            p = "\\" + p1 + "\\" + p2 + "\\";
                            break;
                        case 3:
                            p2 = parameters.path2;
                            p3 = parameters.path3;
                            p = "\\" + p1 + "\\" + p2 + "\\" + p3 + "\\";
                            break;
                        case 4:
                            p2 = parameters.path2;
                            p3 = parameters.path3;
                            p4 = parameters.path4;
                            p = "\\" + p1 + "\\" + p2 + "\\" + p3 + "\\" + p4 + "\\";
                            break;
                        case 5:
                            p2 = parameters.path2;
                            p3 = parameters.path3;
                            p4 = parameters.path4;
                            p5 = parameters.path5;
                            p = "\\" + p1 + "\\" + p2 + "\\" + p3 + "\\" + p4 + "\\" + p5 + "\\";
                            break;
                        default:
                            break;
                    }

                    string path_file = (root + "\\" + mod_folder + p + file_name).ToLower();
                    if (p.StartsWith("\\."))
                        path_file = (root + p + file_name).ToLower();

                    if (ext == "css" || ext == "js" || ext == "html")
                    {
                        ObjectCache cache = MemoryCache.Default;

                        if (ext == "html")
                        {
                            string data = cache[path_file] as string;
                            data = RenderTemplate(data, null);
                            o = (Response)data;
                            o.WithCookie(new Nancy.Cookies.NancyCookie("page_url", url));
                        }
                        else
                        {
                            string data = cache[path_file] as string;
                            o = (Response)data;
                        }

                        o.StatusCode = HttpStatusCode.OK;
                        o.ContentType = content_type;
                    }
                    else
                    {
                        checkFile(path_file);
                        o = Response.AsFile(path_file).WithContentType(content_type);
                    }
                }
            }
            return o;
        }

        //public nancyHome(IConfigProvider configProvider, IJwtWrapper jwtWrapper)
        public nancyHome()
        {
            Get["/"] = parameters =>
            {
                //string root = hostServer.pathModule + "\\" + this.Request.Url.HostName;
                string root = hostServer.pathModule + "\\" + this.Context.domain;

                string htm = "";
                string folder = parameters.path;
                string file = root + "\\index\\index_.html";

                var o = (Response)htm;

                if (File.Exists(file))
                {
                    file = file.ToLower();

                    string key = file.ToLower() + "/" +
                    (string.IsNullOrEmpty(this.Context.lang_key) ?
                    hostUser.lang_Default : this.Context.lang_key);

                    //htm = hostServer.getCache(key);  
                    //htm = cache[key] as string;                  
                    hostModule.dicModule.TryGetValue(key, out htm);


                    htm = RenderTemplate(htm, null).Trim();
                    int pos = htm.ToLower().IndexOf("<head>");
                    if (pos > 0)
                    {
                        pos = pos + 6;
                        htm = htm.Substring(0, pos) + @"<script src="".api/init/init.js""></script>" + htm.Substring(pos, htm.Length - pos);
                    }

                    o = (Response)htm;
                    o.StatusCode = HttpStatusCode.OK;
                    o.ContentType = "text/html";

                    string url = this.Request.Url.ToString().Split('?')[0].ToLower();
                    o.WithCookie(new Nancy.Cookies.NancyCookie("page_url", url));
                }
                else
                {
                    file = root + "\\index.html";
                    if (File.Exists(file))
                        return Response.AsRedirect("index.html");
                }

                return o;
            };

            Get["/{value:file}"] = x =>
            {
                string path = x.value;
                //string url = this.Request.Url.ToString().Split('?')[0].ToLower();
                //if (url.EndsWith(".ifc"))
                if (path != null && path.EndsWith(".ifc"))
                {
                    string root = hostServer.pathModule + "\\" + this.Context.domain;

                    string htm = "";
                    string folder = path.Substring(0, path.Length - 4);
                    string file = root + "\\" + folder + "\\index_.html";

                    if (File.Exists(file))
                    {
                        string key = file.ToLower() + "/" +
                            (string.IsNullOrEmpty(this.Context.lang_key) ?
                            hostUser.lang_Default : this.Context.lang_key);

                        //htm = hostServer.getCache(key);
                        hostModule.dicModule.TryGetValue(key, out htm);

                        htm = RenderTemplate(htm, null).Trim();
                        int pos = htm.ToLower().IndexOf("<head>");
                        if (pos > 0)
                        {
                            pos = pos + 6;
                            htm = htm.Substring(0, pos) + @"<script src="".api/init/init.js""></script>" + htm.Substring(pos, htm.Length - pos);
                        }
                    }
                    else
                        htm = "Can not find page: " + file;

                    var o = (Response)htm;
                    o.StatusCode = HttpStatusCode.OK;
                    o.ContentType = "text/html";
                    //o.WithCookie(new Nancy.Cookies.NancyCookie("page_url", url));
                    //o.WithHeader("Cache-Control", "max-age=45"); 
                    //o.AsCacheable(DateTime.Now.AddSeconds(120)); 
                    return o;
                }
                else
                    return getFile(0, x, this);
            };

            Get["/{path1}/{value:file}"] = x => getFile(1, x, this);
            Get["/{path1}/{path2}/{value:file}"] = x => getFile(2, x, this);
            Get["/{path1}/{path2}/{path3}/{value:file}"] = x => getFile(3, x, this);
            Get["/{path1}/{path2}/{path3}/{path4}/{value:file}"] = x => getFile(4, x, this);
            Get["/{path1}/{path2}/{path3}/{path4}/{path5}/{value:file}"] = x => getFile(5, x, this);

            //Get["/{path}"] = parameters =>
            //{
            //    string url = this.Request.Url.ToString().Split('?')[0].ToLower();
            //    string root = hostServer.pathModule + "\\" + this.Request.Url.HostName;

            //    string htm = "";
            //    string folder = parameters.path;
            //    string file = root + "\\" + folder + "\\index_.html";

            //    if (File.Exists(file))
            //    {
            //        string key = file.ToLower() + "/" +
            //            (string.IsNullOrEmpty(this.Context.lang_key) ?
            //            hostUser.lang_Default : this.Context.lang_key);

            //        //htm = hostServer.getCache(key);
            //        hostModule.listModule.TryGetValue(key, out htm);

            //        htm = RenderTemplate(htm, null).Trim();
            //        int pos = htm.ToLower().IndexOf("<head>");
            //        if (pos > 0)
            //        {
            //            pos = pos + 6;
            //            htm = htm.Substring(0, pos) + @"<script src="".api/init/init.js""></script>" + htm.Substring(pos, htm.Length - pos);
            //        }
            //    }
            //    else
            //        htm = "Can not find page: " + file;

            //    var o = (Response)htm;
            //    o.StatusCode = HttpStatusCode.OK;
            //    o.ContentType = "text/html";
            //    o.WithCookie(new Nancy.Cookies.NancyCookie("page_url", url));
            //    //o.WithHeader("Cache-Control", "max-age=45"); 
            //    //o.AsCacheable(DateTime.Now.AddSeconds(120)); 

            //    return o;
            //};

        }//end function
    }
}
