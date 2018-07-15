using Nancy.Hosting.Self;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Caching;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using Newtonsoft.Json;

namespace host
{
    public class hostModule
    {
        public static List<string> listFileCache = new List<string>() { };
        public static List<string> listFileStatic = new List<string>() { };

        public static Dictionary<string, string> dicModule = new Dictionary<string, string>() { };
        public static Dictionary<string, string> dicModuleJson = new Dictionary<string, string>() { }; // <[ mod_key/file_name ; data_json ]>
        
        private static void cache_files(string path)
        {
            ObjectCache cache = MemoryCache.Default;

            var di = Directory.GetFiles(path)
            .Select(x => x.ToLower())
            .Where(x => x.EndsWith(".htm") || x.EndsWith(".html") || x.EndsWith(".js") || x.EndsWith(".css"))
            .ToArray();

            if (di.Length > 0)
            {
                lock (listFileCache)
                    listFileCache.AddRange(di);

                for (int i = 0; i < di.Length; i++)
                {
                    string file = di[i];
                    //Console.WriteLine(file);
                    Task.Factory.StartNew((object fi) =>
                    {
                        string f = fi as string;
                        string htm = File.ReadAllText(f);

                        htm = hostBind.render_Bind_Tag_LinkSrc(htm);
                        htm = url_fix_rewrite_site(htm);

                        if (f.EndsWith("index_.html"))
                        {
                            //htm = Regex.Replace(htm,
                            //    "<form", @"<form method=""post"" onsubmit=""apiForm(this);return false;"" ",
                            //    RegexOptions.IgnoreCase | RegexOptions.Compiled);

                            //htm = Regex.Replace(htm,
                            //    "<body", @"<body class=""mod"" ",
                            //    RegexOptions.IgnoreCase | RegexOptions.Compiled);

                            #region
                            string pi = f.Substring(0, f.Length - 11);
                            string pi_lang = pi + "lang";

                            if (Directory.Exists(pi_lang))
                            {
                                var dsFileLang = Directory.GetFiles(pi_lang)
                                    .Where(x => x.EndsWith(".json")).Select(x => x.ToLower()).ToArray();
                                if (dsFileLang.Length > 0)
                                {
                                    foreach (var fi_lang in dsFileLang)
                                    {
                                        string[] a009 = fi_lang.Split('\\');
                                        string lang_name = a009[a009.Length - 1].Replace(".json", string.Empty).Trim();

                                        string lang_json = File.ReadAllText(fi_lang).Trim();

                                        string htm_lang = htm;

                                        // var la = JsonConvert.DeserializeObject<Dictionary<string, string>>(json_lang);
                                        // foreach (var li in la) htm_lang = htm_lang.Replace("[lang_" + li.Key + "]", li.Value);

                                        var la = lang_json.Split(new string[] { Environment.NewLine }, StringSplitOptions.None)
                                            .Select(x => x.Trim())
                                            .Where(x => x.Contains(":"))
                                            .Select(x => new Tuple<string, string>(
                                                "[lang_" + x.Split(':')[0].Trim() + "]",
                                                x.Split(':')[1].Replace(@"""", string.Empty).Replace(",", string.Empty).Trim()))
                                                .ToArray();

                                        foreach (var li in la)
                                            htm_lang = htm_lang.Replace(li.Item1, li.Item2);

                                        string key = f + "/" + lang_name;
                                        //cache[key] = htm_lang;
                                        dicModule.Add(key, htm_lang);
                                    }
                                }
                            }
                            #endregion
                        }
                        else
                            cache[f] = htm;

                    }, file);
                }
            }
        }

        private static void static_files(string path)
        {
            var di = Directory.GetFiles(path)
            .Select(x => x.ToLower())
            .Where(x => !x.EndsWith(".html") && !x.EndsWith(".htm") && !x.EndsWith(".js") && !x.EndsWith(".css"))
            .ToArray();

            if (di.Length > 0)
                lock (listFileStatic)
                    listFileStatic.AddRange(di);
        }

        private static void cache_dir(string path)
        {
            ObjectCache cache = MemoryCache.Default;

            var ds = Directory
                        .GetDirectories(path)
                        .Select(x => x.ToLower())
                        .ToArray();

            for (int i = 0; i < ds.Length; i++)
            {
                string dir = ds[i];

                cache_files(dir);
                static_files(dir);

                cache_dir(dir);
            }
        }

        private static string url_fix_rewrite_site(string html)
        {
            string htm = html;

            foreach (Match m in regEx_ModuleResourceURL.Matches(htm))
            {
                string tag = m.ToString();
                string uri = m.Groups["URL"].Value.Replace(@"\", "/"),
                    uri_ = uri.ToLower().Replace("../", string.Empty);

                if (uri_ != "#")
                {
                    if (uri_.StartsWith("./"))
                    {
                        uri_ = uri_.Substring(1);
                        string tag1 = tag.Replace(uri, uri_);
                        htm = htm.Replace(tag, tag1);
                    }
                }
            }//end for

            foreach (Match m in regEx_Form.Matches(htm))
            {
                string tag = m.ToString();
                //string uri = m.Groups["URL"].Value.Replace(@"\", "/");

                string fom = @"<form method=""post"" onsubmit=""apiForm(this);return false;"" " + tag.Substring(5, tag.Length - 5);

                htm = htm.Replace(tag, fom);

                //htm = Regex.Replace(htm,
                //    "<form", @"<form method=""post"" onsubmit=""apiForm(this);return false;"" ",
                //    RegexOptions.IgnoreCase | RegexOptions.Compiled);

                //if (uri_ != "#")
                //{
                //    if (uri_.StartsWith("./"))
                //    {
                //        uri_ = uri_.Substring(1);
                //        string tag1 = tag.Replace(uri, uri_);
                //        htm = htm.Replace(tag, tag1);
                //    }
                //}
            }//end for

            return htm;
        }

        private static string url_fix_rewrite_module(string dir, string item, string html)
        {
            string htm = html;

            foreach (Match m in regEx_ModuleResourceURL.Matches(htm))
            {
                string tag = m.ToString();
                string uri = m.Groups["URL"].Value.Replace(@"\", "/"),
                    uri_ = uri.ToLower().Replace("../", string.Empty);
                if (uri_ != "#")
                {
                    if (uri_.StartsWith("./"))
                    {
                        uri_ = uri_.Substring(1);
                        string tag1 = tag.Replace(uri, uri_);
                        htm = htm.Replace(tag, tag1);
                    }
                    else
                    {
                        if (!uri_.Contains("." + dir + "/"))
                        {
                            if (!uri_.StartsWith("http://")
                            && !uri_.StartsWith("https://")
                            && !uri_.StartsWith("@"))
                            {
                                if (!uri.StartsWith("/"))
                                    uri_ = string.Concat("/", uri_);

                                if (uri_.StartsWith("/" + item + "/"))
                                    uri_ = "." + dir + uri_;
                                else
                                    uri_ = "." + dir + "/" + item + uri_;

                                string tag1 = tag.Replace(uri, uri_);

                                htm = htm.Replace(tag, tag1);
                            }
                        }
                    }
                }
            }//end for

            return htm;
        }

        public static string get_json_module(string path_key)
        {
            string json = "";
            dicModuleJson.TryGetValue(path_key, out json);
            return json;
        }

        public static void ClearCacheAll()
        {
            hostCacheHTMLJsCss.ClearAll();

            dicModuleJson.Clear();
            dicModule.Clear();

            listFileStatic.Clear();
            listFileCache.Clear();

            hostCache_Src.clearAll();
        }

        public static void CacheRefresh()
        {
            ClearCacheAll();


            #region //> Module ...

            char charMod = '.';

            var dsMods = Directory.GetDirectories(hostServer.pathModule)
                .Select(x => x.ToLower())
                .Where(x => x.Contains("\\" + charMod))
                .ToArray();

            #region //> Module/json ...

            if (dsMods.Length > 0)
            {
                foreach (var ms in dsMods)
                {
                    string[] a = ms.Split(charMod);
                    string dir = a[a.Length - 1].ToLower();

                    var mods = Directory.GetDirectories(ms);
                    foreach (var mod in mods)
                    {
                        a = mod.Split(charMod);
                        string mod_key = a[a.Length - 1].ToLower();

                        string path_json = mod + @"\json";

                        if (Directory.Exists(path_json))
                        {
                            var fjs = Directory.GetFiles(path_json).Select(x => x.ToLower()).Where(x => x.EndsWith(".json")).ToArray();
                            if (fjs.Length > 0)
                            {
                                foreach (var fi in fjs)
                                {
                                    a = fi.Split('\\');
                                    string file_name = a[a.Length - 1];
                                    string key = a[a.Length - 4].Substring(1) + "/" + a[a.Length - 3] + "/" + file_name;

                                    string json = File.ReadAllText(fi);

                                    dicModuleJson.Add(key, json);
                                }
                            }
                        }
                    } // end mods
                }
            }

            #endregion

            foreach (var ms in dsMods)
            {
                string[] a = ms.Split(charMod);
                string dir = a[a.Length - 1].ToLower();
                var dsItem = Directory.GetDirectories(ms);
                if (dsItem.Length > 0)
                {
                    foreach (var mi in dsItem)
                    {
                        string[] ai = mi.Split('\\');
                        string item = ai[ai.Length - 1].ToLower();

                        #region >>> List module item ...

                        string fi_htm = "";
                        if (dir == "api")
                            fi_htm = mi + "\\index.html";
                        else
                            fi_htm = mi + "\\index_.html";

                        string fi_bin_css = mi + "\\bin.css";
                        string fi_bin_js = mi + "\\bin.js";

                        if (File.Exists(fi_htm))
                        {
                            string htm = File.ReadAllText(fi_htm);
                            htm = Regex.Replace(htm,
                                "<form", @"<form method=""post"" onsubmit=""apiForm(this);return false;"" ",
                                RegexOptions.IgnoreCase | RegexOptions.Compiled);

                            #region >> Fix path dynamic .dir/module/...

                            htm = url_fix_rewrite_module(dir, item, htm);

                            //foreach (Match m in regEx_ModuleResourceURL.Matches(htm))
                            //{
                            //    string tag = m.ToString();
                            //    string uri = m.Groups["URL"].Value.Replace(@"\", "/"),
                            //        uri_ = uri.ToLower().Replace("../", string.Empty);
                            //    if (uri_ != "#")
                            //    {
                            //        if (uri_.StartsWith("./"))
                            //        {
                            //            uri_ = uri_.Substring(1);
                            //            string tag1 = tag.Replace(uri, uri_);
                            //            htm = htm.Replace(tag, tag1);
                            //        }
                            //        else
                            //        {
                            //            if (!uri_.Contains("." + dir + "/"))
                            //            {
                            //                if (!uri_.StartsWith("http://")
                            //                && !uri_.StartsWith("https://"))
                            //                {
                            //                    if (!uri.StartsWith("/"))
                            //                        uri_ = string.Concat("/", uri_);

                            //                    if (uri_.StartsWith("/" + item + "/"))
                            //                        uri_ = "." + dir + uri_;
                            //                    else
                            //                        uri_ = "." + dir + "/" + item + uri_;

                            //                    string tag1 = tag.Replace(uri, uri_);

                            //                    htm = htm.Replace(tag, tag1);
                            //                }
                            //            }
                            //        }
                            //    }
                            //}//end for 

                            #endregion

                            #region >> File: bin.css; Bin.js ...

                            #region >>> js ...

                            var fs_bin_js = Directory.GetFiles(mi)
                                .Select(o => o.ToLower())
                                .Where(o => o.EndsWith(".js") && o.Contains(@"\bin_")).ToArray();

                            if (fs_bin_js.Length > 0)
                            {
                                foreach (var fo in fs_bin_js)
                                {
                                    Task.Factory.StartNew((object fi) =>
                                    {
                                        var o = fi as Tuple<string, string, string>;
                                        string[] acss = o.Item1.Split('\\');

                                        string f_css_name = acss[acss.Length - 1];
                                        f_css_name = f_css_name.Substring(0, f_css_name.Length - 3)
                                                        .ToLower().Replace('-', '_').Substring(4);

                                        string mod_key = (o.Item2 + "_" + o.Item3).Replace('-', '_');
                                        string data = File.ReadAllText(o.Item1),
                                            id_cache = "bin/" + mod_key + "_" + f_css_name + ".js";

                                        data = data.Replace("@modkey", mod_key).Replace("___modkey", mod_key);

                                        ObjectCache cache = MemoryCache.Default;
                                        cache[id_cache] = data;
                                    }, new Tuple<string, string, string>(fo, dir, item));
                                }

                                htm = htm + Environment.NewLine +
                                    @"<script src=""/" + "bin/" + dir + "_" + item.Replace('-', '_') + "_@theme_key_@device_type.js" + @"""></script>";
                            }

                            #endregion



                            var fs_bin_css = Directory.GetFiles(mi)
                                .Select(o => o.ToLower())
                                .Where(o => o.EndsWith(".css") && o.Contains(@"\bin_")).ToArray();

                            if (fs_bin_css.Length > 0)
                            {
                                foreach (var fo in fs_bin_css)
                                {
                                    Task.Factory.StartNew((object fi) =>
                                    {
                                        var o = fi as Tuple<string, string, string>;
                                        string[] acss = o.Item1.Split('\\');

                                        string f_css_name = acss[acss.Length - 1];
                                        f_css_name = f_css_name.Substring(0, f_css_name.Length - 4)
                                                        .ToLower().Replace('-', '_').Substring(4);

                                        string mod_key = (o.Item2 + "_" + o.Item3).Replace('-', '_');
                                        string data = File.ReadAllText(o.Item1),
                                            id_cache = "bin/" + mod_key + "_" + f_css_name + ".css";

                                        data = data.Replace("@modkey", mod_key).Replace("___modkey", mod_key);

                                        ObjectCache cache = MemoryCache.Default;
                                        cache[id_cache] = data;
                                    }, new Tuple<string, string, string>(fo, dir, item));
                                }

                                htm = @"<link href=""/" + "bin/" + dir + "_" + item.Replace('-', '_') +
                                    "_@theme_key_@device_type.css" + @""" rel=""stylesheet"" /> " +
                                    Environment.NewLine + htm;
                            }

                            #endregion

                            if (dir != "api")
                                htm = "<div id='___modkey'><div class='mod ___modkey module__' mdir='" + dir + "' mkey='" + item + "'>" +
                                    @"<em class='btnConfig fa fa-wrench @username' onclick=""apiConfig('" + dir + "/" + item + @"')""></em>" + htm + "</div></div>";

                            string modkey = (dir + "_" + item).Replace('-', '_');
                            htm = htm.Replace("@modkey", modkey).Replace("___modkey", modkey);


                            #region >>> Multi language ...

                            if (dir == "api")
                            {
                                dicModule.Add(dir + "/" + item, htm);
                            }
                            else
                            {
                                string path_mod_lang = mi + "\\lang";
                                if (Directory.Exists(path_mod_lang))
                                {
                                    var dsFileLang = Directory.GetFiles(path_mod_lang)
                                        .Where(x => x.EndsWith(".json")).Select(x => x.ToLower()).ToArray();
                                    if (dsFileLang.Length > 0)
                                    {
                                        foreach (var fi_lang in dsFileLang)
                                        {
                                            string[] a009 = fi_lang.Split('\\');
                                            string lang_name = a009[a009.Length - 1].Replace(".json", string.Empty).Trim();

                                            string lang_json = File.ReadAllText(fi_lang).Trim();

                                            string htm_lang = htm;

                                            // var la = JsonConvert.DeserializeObject<Dictionary<string, string>>(json_lang);
                                            // foreach (var li in la) htm_lang = htm_lang.Replace("[lang_" + li.Key + "]", li.Value);

                                            var la = lang_json.Split(new string[] { Environment.NewLine }, StringSplitOptions.None)
                                                .Select(x => x.Trim())
                                                .Where(x => x.Contains(":"))
                                                .Select(x => new Tuple<string, string>(
                                                    "[lang_" + x.Split(':')[0].Trim() + "]",
                                                    x.Split(':')[1].Replace(@"""", string.Empty).Replace(",", string.Empty).Trim()))
                                                    .ToArray();

                                            foreach (var li in la) htm_lang = htm_lang.Replace(li.Item1, li.Item2);

                                            dicModule.Add(dir + "/" + item + "/" + lang_name, htm_lang);
                                        }
                                    }
                                }
                            }
                            #endregion


                            //var fis = Directory.GetFiles(mi).Where(x => !x.EndsWith("index.htm")).ToArray();
                            //if (fis.Length > 0)
                            //{
                            //    foreach (var fi in fis)
                            //    {
                            //        string[] aii = fi.Split('\\');
                            //        string fi_name = aii[aii.Length - 1].ToLower();
                            //        string key = "." + dir + "\\" + item + "\\" + fi_name;
                            //        string val = fi;

                            //        if (fi_name.EndsWith(".js") || fi_name.EndsWith(".css"))
                            //        {
                            //            val = File.ReadAllText(fi);
                            //            listModule.Add(key, val);
                            //        }                                    
                            //    }
                            //}//end if has contain css, js

                            //listModule.Add(dir + "/" + item, htm);
                        }

                        #endregion
                    }// list module item
                }
            }

            #endregion

            #region //> Site ....

            var dsSite = Directory.GetDirectories(hostServer.pathModule)
                .Select(x => x.ToLower())
                .Where(x => !x.Contains("\\~") && !x.Contains("\\."))
                .ToArray();

            for (int j = 0; j < dsSite.Length; j++)
            {
                string s_site_path = dsSite[j];
                var s_a_ = s_site_path.Split('\\');
                string s_site = s_a_[s_a_.Length - 1];

                cache_files(s_site_path);
                static_files(s_site_path);

                var dsPage = Directory.GetDirectories(s_site_path)
                    .Where(x => !x.StartsWith("."))
                    .Select(x => x.ToLower())
                    .ToArray();
                for (int k = 0; k < dsPage.Length; k++)
                {
                    string page = dsPage[k];
                    if (page.Contains("\\_"))
                    {
                        var dsLayout = Directory.GetFiles(page)
                            .Select(x => x.ToLower())
                            .Where(x => x.EndsWith(".css") || x.EndsWith(".js") || x.EndsWith(".txt") || x.EndsWith(".json"))
                            .ToArray();
                        for (int ki = 0; ki < dsLayout.Length; ki++)
                        {
                            string[] a_la = dsLayout[ki].Split('\\');
                            string layout_key = a_la[a_la.Length - 3] + "/" + a_la[a_la.Length - 2] + "/" + a_la[a_la.Length - 1];
                            string data = File.ReadAllText(dsLayout[ki]);
                            dicModule.Add(layout_key, data);
                        }
                    }
                    else
                    {
                        cache_files(page);
                        static_files(page);
                        cache_dir(page);
                    }
                }
            }

            #endregion

        }

        private static readonly Regex regEx_Form = new
            Regex(@"<(?<Tag_Name>(form))\b[^>]*?\b(?<PostUrl>post)\s*=\s*(?:""(?<URL>(?:\\""|[^""])*)""|'(?<URL>(?:\\'|[^'])*)')", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        private static readonly Regex regEx_ModuleResourceURL = new
            //Regex(@"<(?<Tag_Name>(a)|img|link|script)\b[^>]*?\b(?<URL_Type>(?(1)href|src))\s*=\s*(?:""(?<URL>(?:\\""|[^""])*)""|'(?<URL>(?:\\'|[^'])*)')", RegexOptions.Compiled | RegexOptions.IgnoreCase);
            Regex(@"<(?<Tag_Name>(a)|iframe|img|link|script)\b[^>]*?\b(?<URL_Type>src|href)\s*=\s*(?:""(?<URL>(?:\\""|[^""])*)""|'(?<URL>(?:\\'|[^'])*)')", RegexOptions.Compiled | RegexOptions.IgnoreCase);


    }//end class

}
