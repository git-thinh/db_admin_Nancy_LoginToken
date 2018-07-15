using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Text.RegularExpressions;

namespace host
{
    public static class hostBind
    {
        private static Dictionary<string, string> dicBind = new Dictionary<string, string>() { };

        public static void init()
        {
            Cache();
        }

        public static string GetTemplate(string path)
        {
            string s = "";

            if (dicBind.ContainsKey(path)) s = dicBind[path];

            return s;
        }

        public static void Cache()
        {
            if (Directory.Exists(hostServer.pathBind))
            {
                var dr = Directory.GetDirectories(hostServer.pathBind).Select(x => x.ToLower()).ToArray();
                foreach (var di in dr)
                {
                    string[] da = di.Split('\\');
                    string dir = da[da.Length - 1];

                    var fs = Directory.GetFiles(di).Select(x => x.ToLower()).ToArray();
                    foreach (var fi in fs)
                    {
                        string[] sa = fi.Split('\\');
                        string file = sa[sa.Length - 1];
                        string key = dir + "/" + file;
                        string data = File.ReadAllText(fi);
                        dicBind.Add(key, data);
                    }
                }
            }
        }

        public static string render_Bind_Tag_LinkSrc(string template)
        {
            string htm = template;
            if (template.Contains("@Bind") || template.Contains("@bind"))
            {
                foreach (Match m in regEx_IncludeBind.Matches(htm))
                {
                    string tag = m.ToString();
                    string uri = "bind/" + m.Groups["URL"].Value.Replace(@"\", "/")
                        .Replace("[", string.Empty).Replace("]", string.Empty).Replace("'", string.Empty).Trim().ToLower();

                    string modTemp = tag.Substring(0, tag.IndexOf('@')) + uri + @"""";
                    htm = htm.Replace(tag, modTemp);
                }
            }

            return htm;
        }

        private static readonly Regex regEx_IncludeBind = new
            //Regex(@"@Bind\['(?<ViewName>[^\]]+)'(?:.[ ]?@?(?<Model>(Model|Current)(?:\.(?<ParameterName>[a-zA-Z0-9-_]+))*))?\];?", RegexOptions.Compiled | RegexOptions.IgnoreCase);
            Regex(@"<(?<Tag_Name>(a)|link|script)\b[^>]*?\b(?<URL_Type>src|href)\s*=\s*(?:""@Bind(?<URL>(?:\\""|[^""])*)""|'(?<URL>(?:\\'|[^'])*)')", RegexOptions.Compiled | RegexOptions.IgnoreCase);
            //Regex(@"<(?<Tag_Name>(a)|iframe|img|link|script)\b[^>]*?\b(?<URL_Type>src|href)\s*=\s*(?:""(?<URL>(?:\\""|[^""])*)""|'(?<URL>(?:\\'|[^'])*)')", RegexOptions.Compiled | RegexOptions.IgnoreCase);
    } // end class
}
