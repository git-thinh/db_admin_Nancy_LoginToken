using System;
using System.Collections.Generic;
using Nancy;
using System.Linq;
using System.Runtime.Caching;
using System.Dynamic;

using Nancy.ViewEngines;
using Nancy.ViewEngines.SuperSimpleViewEngine;
using System.Text.RegularExpressions;
using Microsoft.CSharp.RuntimeBinder;
using System.Runtime.CompilerServices;
using System.Reflection;

namespace host
{
    public static class renderNancy
    {
        private static readonly Regex regEx_IncludeModule = new
            Regex(@"@Module\['(?<ViewName>[^\]]+)'(?:.[ ]?@?(?<Model>(Model|Current)(?:\.(?<ParameterName>[a-zA-Z0-9-_]+))*))?\];?", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        private static readonly Regex regEx_Command_Check = new
            Regex(@"<(?<command_name>(in)|contain|null_empty|between|equal_not|equal|greater|less|greater_equal|less_equal|start_with|end_with)\b[^>]*?\b(?<context_key>[a-zA-Z0-9-_]+)\s*=\s*(?:""(?<value_check>(?:\\""|[^""])*)""|'(?<VarValue>(?:\\'|[^'])*)')\s*>",
            RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.Multiline);

        public static ObjectCache cache = MemoryCache.Default;

        private static readonly SuperSimpleViewEngine dx = new SuperSimpleViewEngine(Enumerable.Empty<ISuperSimpleViewEngineMatcher>());

        public static string Render(string pathView, dynamic model, NancyModule page)
        {
            pathView = hostServer.pathModule + @"\" + page.Request.Url.HostName + @"\" + pathView.Replace("/", @"\");
            pathView = pathView.Replace(@"\\", @"\");

            string html = cache[pathView] as string;
            if (string.IsNullOrEmpty(html)) return "";

            return RenderTemplate(html, model, page);
        }

        private static string renderSite_check(string html, NancyModule page)
        {
            foreach (Match m in regEx_Command_Check.Matches(html))
            {
                string tag = m.ToString().ToLower();
                string code = m.Groups["command_name"].Value.ToLower();
                string val_1 = m.Groups["value_check"].Value.ToLower();
                string context_key = m.Groups["context_key"].Value.ToLower();

                string val_0 = NancyContextKey.getValue(context_key, page.Context);

                bool ok_ = false;
                #region
                int int_0 = 0, int_1 = 0;
                switch (code)
                {
                    case "in":
                        val_1 = "," + val_1 + ",";
                        if (val_1.Contains("," + val_0 + ",")) ok_ = true;
                        break;
                    case "contain":
                        if (val_1.Contains(val_0)) ok_ = true;
                        break;
                    case "null_empty":
                        if (string.IsNullOrEmpty(val_0)) ok_ = true;
                        break;
                    case "equal_not":
                        if (val_0 != val_1) ok_ = true;
                        break;
                    case "equal":
                        if (val_0 == val_1) ok_ = true;
                        break;
                    case "start_with":
                        if (val_0.StartsWith(val_1)) ok_ = true;
                        break;
                    case "end_with":
                        if (val_0.EndsWith(val_1)) ok_ = true;
                        break;
                    case "greater":
                        int.TryParse(val_0.Trim(), out int_0);
                        int.TryParse(val_1.Trim(), out int_1);
                        if (int_0 > int_1) ok_ = true;
                        break;
                    case "less":
                        int.TryParse(val_0.Trim(), out int_0);
                        int.TryParse(val_1.Trim(), out int_1);
                        if (int_0 < int_1) ok_ = true;
                        break;
                    case "greater_equal":
                        int.TryParse(val_0.Trim(), out int_0);
                        int.TryParse(val_1.Trim(), out int_1);
                        if (int_0 >= int_1) ok_ = true;
                        break;
                    case "less_equal":
                        int.TryParse(val_0.Trim(), out int_0);
                        int.TryParse(val_1.Trim(), out int_1);
                        if (int_0 <= int_1) ok_ = true;
                        break;
                    case "between":
                        int.TryParse(val_0.Trim(), out int_0);
                        string[] a = val_1.Split(',').Select(x => x.Trim()).ToArray();
                        if (a.Length > 1)
                        {
                            int int_2 = 0;
                            int.TryParse(a[0].Trim(), out int_1);
                            int.TryParse(a[1].Trim(), out int_2);
                            if (int_0 <= int_2 && int_0 >= int_1) ok_ = true;
                        }
                        break;
                }
                #endregion

                if (ok_)
                {
                    #region
                    int pos = html.ToLower().IndexOf(tag);
                    while (pos != -1)
                    {
                        string s0 = html.Substring(0, pos);
                        pos = pos + tag.Length;
                        html = html.Substring(pos, html.Length - pos);
                        int end_ = html.ToLower().IndexOf("</" + code + ">");
                        if (end_ == -1)
                        {
                            pos = -1;
                            html = s0 + " " + html;
                        }
                        else
                        {
                            string content = html.Substring(0, end_);
                            end_ = end_ + code.Length + 3;
                            html = s0 + " " + content + " " + html.Substring(end_, html.Length - end_);
                            pos = html.ToLower().IndexOf(tag);
                        }
                    }
                    #endregion
                }
                else
                {
                    #region
                    int pos = html.ToLower().IndexOf(tag);
                    while (pos != -1)
                    {
                        string s0 = html.Substring(0, pos);
                        pos = pos + tag.Length;
                        html = html.Substring(pos, html.Length - pos);
                        int end_ = html.ToLower().IndexOf("</" + code + ">");
                        if (end_ == -1)
                        {
                            pos = -1;
                            html = s0 + " " + html;
                        }
                        else
                        {
                            end_ = end_ + code.Length + 3;
                            html = s0 + " " + html.Substring(end_, html.Length - end_);
                            pos = html.ToLower().IndexOf(tag);
                        }
                    }
                    #endregion
                }
            }
            return html;
        }

        public static string RenderTemplate(string template, dynamic model, NancyModule page)
        {
            if (string.IsNullOrEmpty(template)) return "";

            template = NancyContextKey.renderKey(template, page.Context);
            if (template.Contains("@Cookie."))
                template = renderCookie(template, page);
            template = renderSite_check(template, page);

            if (template.Contains("@Module") || template.Contains("@module"))
            {
                template = renderModuleView(template, model, page);
            }

            var context = page.Context;
            ViewLocationContext GetViewLocationContext = new ViewLocationContext
            {
                Context = context,
                ModuleName = context.NegotiationContext.ModuleName,
                ModulePath = context.NegotiationContext.ModulePath
            };
            IRenderContext renderContext = page.ViewFactory.getRenderContextFactory().GetRenderContext(GetViewLocationContext);
            string data = dx.Render(template, model, new NancyViewEngineHost(renderContext));

            data = data.Replace("[ERR!]", string.Empty);

            if (data.Contains("@modkey") || data.Contains("___modkey"))
                data = data.Replace("@modkey", "_modkey").Replace("___modkey", "_modkey");

            if (data.Contains("@pagekey"))
            {
                string path = page.Request.Url.Path.Split('?')[0].Split('#')[0];
                if (path.Length > 0) path = path.Substring(1);
                path = path.Replace('-', '_');
                if (path.EndsWith(hostServer.pathExtSite)) path = path.Substring(0, path.Length - 4);

                data = data.Replace("@pagekey", path);
            }

            //return data.renderHashCode(page);
            return data;
        }


        private static readonly Regex regEx_CookieReplace = new Regex(@"@(?<Encode>!)?Cookie(?:\.(?<key_name>[a-zA-Z0-9-_]+))*;?", RegexOptions.Compiled);
        private static string renderCookie(string template, NancyModule page)
        {
            string htm = template;
            foreach (Match m in regEx_CookieReplace.Matches(template))
            {
                string tag = m.ToString();
                string key_name = m.Groups["key_name"].Value;
                string val = "";
                page.Request.Cookies.TryGetValue(key_name, out val);
                if (val == "undefined") val = "";
                if (!string.IsNullOrWhiteSpace(val)) val = System.Web.HttpUtility.UrlDecode(val, System.Text.Encoding.Default);
                htm = htm.Replace(tag, val);
            }
            return htm;
        }

        private static string renderModuleView(string template, dynamic model, NancyModule page)
        {
            string htm = template;
            foreach (Match m in regEx_IncludeModule.Matches(template))
            {
                string tag = m.ToString();
                string modView = m.Groups["ViewName"].Value.ToLower();

                string modTemp = "", mod_key = modView + "/" + page.Context.lang_key;
                if (modView.Contains("api/"))
                    mod_key = modView;

                if (hostModule.dicModule.ContainsKey(mod_key))
                {
                    modTemp = hostModule.dicModule[mod_key];
                    modTemp = NancyContextKey.renderKey(modTemp, page.Context);
                    if (modTemp.Contains("@Cookie."))
                        modTemp = renderCookie(modTemp, page);

                    var partialModel = model;
                    var properties = GetCaptureGroupValues(m, "ParameterName");
                    if (m.Groups["Model"].Length > 0)
                    {
                        var modelValue = GetPropertyValueFromParameterCollection(partialModel, properties);
                        if (modelValue.Item1 != true)
                        {
                            return ""; // "[ERR!]";
                        }
                        partialModel = modelValue.Item2;
                    }
                    modTemp = renderModuleView(modTemp, partialModel, page);
                }

                htm = htm.Replace(tag, modTemp);
            }
            return htm;
        }


        private static string renderHashCode(this string template, NancyModule page)
        {
            template = template.Replace("@username", page.Context.username);
            return template;
        }

        /// <summary>
        /// A property extractor designed for ExpandoObject, but also for any
        /// type that implements IDictionary string object for accessing its
        /// properties.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <param name="propertyName">The property name.</param>
        /// <returns>Tuple - Item1 being a bool for whether the evaluation was successful, Item2 being the value.</returns>
        private static Tuple<bool, object> DynamicDictionaryPropertyEvaluator(object model, string propertyName)
        {
            var dictionaryModel = (IDictionary<string, object>)model;

            object output;
            return !dictionaryModel.TryGetValue(propertyName, out output) ? new Tuple<bool, object>(false, null) : new Tuple<bool, object>(true, output);
        }

        private static Tuple<bool, object> GetDynamicMember(object obj, string memberName)
        {
            var binder = Microsoft.CSharp.RuntimeBinder.Binder.GetMember(CSharpBinderFlags.None, memberName, obj.GetType(),
                new[] { CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null) });

            var callsite = CallSite<Func<CallSite, object, object>>.Create(binder);

            var result = callsite.Target(callsite, obj);

            return result == null ? new Tuple<bool, object>(false, null) : new Tuple<bool, object>(true, result);
        }

        /// <summary>
        /// A property extractor for standard types.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <param name="propertyName">The property name.</param>
        /// <returns>Tuple - Item1 being a bool for whether the evaluation was successful, Item2 being the value.</returns>
        private static Tuple<bool, object> StandardTypePropertyEvaluator(object model, string propertyName)
        {
            var type = model.GetType();
            var properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static);

            var property =
                properties.Where(p => string.Equals(p.Name, propertyName, StringComparison.InvariantCulture)).
                FirstOrDefault();

            if (property != null)
            {
                return new Tuple<bool, object>(true, property.GetValue(model, null));
            }

            var fields = type.GetFields(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static);

            var field =
                fields.Where(p => string.Equals(p.Name, propertyName, StringComparison.InvariantCulture)).
                FirstOrDefault();

            return field == null ? new Tuple<bool, object>(false, null) : new Tuple<bool, object>(true, field.GetValue(model));
        }

        /// <summary>
        /// <para>
        /// Gets a property value from the given model.
        /// </para>
        /// <para>
        /// Anonymous types, standard types and ExpandoObject are supported.
        /// Arbitrary dynamics (implementing IDynamicMetaObjectProvicer) are not, unless
        /// they also implement IDictionary string, object for accessing properties.
        /// </para>
        /// </summary>
        /// <param name="model">The model.</param>
        /// <param name="propertyName">The property name to evaluate.</param>
        /// <returns>Tuple - Item1 being a bool for whether the evaluation was successful, Item2 being the value.</returns>
        /// <exception cref="ArgumentException">Model type is not supported.</exception>
        private static Tuple<bool, object> GetPropertyValue(object model, string propertyName)
        {
            if (model == null || string.IsNullOrEmpty(propertyName))
            {
                return new Tuple<bool, object>(false, null);
            }

            if (model is IDictionary<string, object>)
            {
                return DynamicDictionaryPropertyEvaluator(model, propertyName);
            }

            if (!(model is IDynamicMetaObjectProvider))
            {
                return StandardTypePropertyEvaluator(model, propertyName);
            }

            if (model is DynamicDictionaryValue)
            {
                var dynamicModel = model as DynamicDictionaryValue;

                return GetPropertyValue(dynamicModel.Value, propertyName);
            }

            if (model is DynamicObject)
            {
                return GetDynamicMember(model, propertyName);
            }

            throw new ArgumentException("model must be a standard type or implement IDictionary<string, object>", "model");
        }

        /// <summary>
        /// Gets a property value from a collection of nested parameter names
        /// </summary>
        /// <param name="model">The model containing properties.</param>
        /// <param name="parameters">A collection of nested parameters (e.g. User, Name</param>
        /// <returns>Tuple - Item1 being a bool for whether the evaluation was successful, Item2 being the value.</returns>
        private static Tuple<bool, object> GetPropertyValueFromParameterCollection(object model, IEnumerable<string> parameters)
        {
            if (parameters == null)
            {
                return new Tuple<bool, object>(true, model);
            }

            var currentObject = model;

            foreach (var parameter in parameters)
            {
                var currentResult = GetPropertyValue(currentObject, parameter);

                if (currentResult.Item1 == false)
                {
                    return new Tuple<bool, object>(false, null);
                }

                currentObject = currentResult.Item2;
            }

            return new Tuple<bool, object>(true, currentObject);
        }

        /// <summary>
        /// Gets an IEnumerable of capture group values
        /// </summary>
        /// <param name="m">The match to use.</param>
        /// <param name="groupName">Group name containing the capture group.</param>
        /// <returns>IEnumerable of capture group values as strings.</returns>
        private static IEnumerable<string> GetCaptureGroupValues(Match m, string groupName)
        {
            return m.Groups[groupName].Captures.Cast<Capture>().Select(c => c.Value);
        }

    }
}
