using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nancy;
using System.IO;
using System.Data;
using System.Xml;
using Newtonsoft.Json;
using System.Collections;
using System.Xml.Linq;
using System.Reflection;
using System.Web;

namespace host
{
    public class uploadfile_cmiss : NancyModule
    {
        public uploadfile_cmiss()
            : base("uploadfile_cmiss")
        {
            Get[""] = para =>
            {
                string data = @"[{""time_server"":""" + DateTime.Now.ToString() + @"""}]";
                var o = (Response)data;
                o.StatusCode = HttpStatusCode.OK;
                o.ContentType = "application/json";

                return o;
            };

            Get["/list_file"] = para =>
            {

                string uploadDirectory = AppDomain.CurrentDomain.BaseDirectory + @"\upload_cmiss_xml";

                uploadDirectory = uploadDirectory.Replace(@"\\", @"\");

                var file_list = Directory.GetFiles(uploadDirectory).Select(path => Path.GetFileName(path)).ToArray(); ;
                string data = string.Join("|", file_list);


                var o = (Response)data;
                o.StatusCode = HttpStatusCode.OK;
                o.ContentType = " text/plain";

                return o;
            };


            Post["/del_file/{file_name}"] = para =>
            {

                string file_name = para.file_name;
                file_name = file_name.Replace(' ', '_') + ".xml";

                string uploadDirectory = AppDomain.CurrentDomain.BaseDirectory + @"\upload_cmiss_xml";
                string path_file = uploadDirectory + @"\" + file_name;
                path_file = path_file.Replace(@"\\", @"\");
                string data = "";

                if (System.IO.File.Exists(path_file))
                {
                    // Use a try block to catch IOExceptions, to
                    // handle the case of the file already being
                    // opened by another process.
                    try
                    {
                        System.IO.File.Delete(path_file);
                        data = "ok";
                    }
                    catch (System.IO.IOException e)
                    {
                        Console.WriteLine(e.Message);
                        data = "not ok";
                    }
                }



                var o = (Response)data;
                o.StatusCode = HttpStatusCode.OK;
                o.ContentType = " text/plain";

                return o;
            };

            Post["/save_new/{ten_file}"] = para =>
            {
                string data = "";
                string file_name = para.ten_file;

                file_name = file_name.Replace(' ', '_') + ".xml";

                string uploadDirectory = AppDomain.CurrentDomain.BaseDirectory + @"\upload_cmiss_xml";
                //var uploadDirectory = Path.Combine("D:\\", "Upload", "xml");
                if (!Directory.Exists(uploadDirectory)) Directory.CreateDirectory(uploadDirectory);

                if (!Directory.Exists(uploadDirectory))
                {
                    Directory.CreateDirectory(uploadDirectory);
                }

                //string xml = "";

                string s = Request.Form.xml;  // is null
                string xml = System.Uri.UnescapeDataString(s);
                //var xml = Request.Form["xml"];  // is also null

                string path_file = uploadDirectory + @"\" + file_name;
                File.WriteAllText(path_file, xml);

                var file_list = Directory.GetFiles(uploadDirectory);
                data = string.Join("|", file_list);

                var o = (Response)data;
                o.StatusCode = HttpStatusCode.OK;
                o.ContentType = " text/plain";

                return o;
            };

            Post["/select_combo/{file_name}"] = para =>
            {
                var file_name = para.file_name; ;
                file_name = file_name + ".xml";
                string uploadDirectory = AppDomain.CurrentDomain.BaseDirectory + @"\upload_cmiss_xml";
                string path_file = uploadDirectory + @"\" + file_name;
       

                string xmlText = LoadFileXml(path_file);
                var o = (Response)xmlText;
                o.StatusCode = HttpStatusCode.OK;
                o.ContentType = "application/xml";

                return o;
            };

            Post[""] = para =>
            {
                var itemp = this.Request.Query;
                var parr = new Dictionary<string, string>();
                foreach (var key in itemp.Keys)
                {
                    parr.Add(key, itemp[key]);
                }

                string uploadDirectory = AppDomain.CurrentDomain.BaseDirectory + @"upload_cmiss_xml";
                //var uploadDirectory = Path.Combine("D:\\", "Upload", "xml");
                if (!Directory.Exists(uploadDirectory)) Directory.CreateDirectory(uploadDirectory);

                if (!Directory.Exists(uploadDirectory))
                {
                    Directory.CreateDirectory(uploadDirectory);
                }
                var filename = "";
                foreach (var file in Request.Files)
                {
                    filename = Path.Combine(uploadDirectory, file.Name);
                    using (FileStream fileStream = new FileStream(filename, FileMode.Create))
                    {
                        file.Value.CopyTo(fileStream);
                    }
                }
                string xmlText = LoadFileXml(filename);
                var o = (Response)xmlText;
                o.StatusCode = HttpStatusCode.OK;
                o.ContentType = "application/xml";

                return o;
            };
        }

        private string HtmlDecode(string xml)
        {
            throw new NotImplementedException();
        }

        public string LoadFileXml(string fileName)
        {
            try
            {
                var dsAdapter = new DataSet();
                dsAdapter.ReadXml(fileName);
                var dtXml = dsAdapter.Tables[0];
                if (dtXml == null) return "";
                dtXml.Columns.Add("Key", typeof(string));
                foreach (DataRow row in dtXml.Rows)
                {
                    row["Key"] = row["MA_DDO"] + "-" + row["LOAI_BCS"];
                }

                // Xóa file XML
                //File.Delete(fileName);

                string result;
                using (var sw = new StringWriter())
                {
                    dtXml.WriteXml(sw);
                    result = sw.ToString();
                }
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private static string XmlToJSON(XmlDocument xmlDoc)
        {
            StringBuilder sbJSON = new StringBuilder();
            sbJSON.Append("{ ");
            XmlToJSONnode(sbJSON, xmlDoc.DocumentElement, true);
            sbJSON.Append("}");
            return sbJSON.ToString();
        }

        //  XmlToJSONnode:  Output an XmlElement, possibly as part of a higher array
        private static void XmlToJSONnode(StringBuilder sbJSON, XmlElement node, bool showNodeName)
        {
            if (showNodeName)
                sbJSON.Append("\"" + SafeJSON(node.Name) + "\": ");
            sbJSON.Append("{");
            // Build a sorted list of key-value pairs
            //  where   key is case-sensitive nodeName
            //          value is an ArrayList of string or XmlElement
            //  so that we know whether the nodeName is an array or not.
            SortedList childNodeNames = new SortedList();

            //  Add in all node attributes
            if (node.Attributes != null)
                foreach (XmlAttribute attr in node.Attributes)
                    StoreChildNode(childNodeNames, attr.Name, attr.InnerText);

            //  Add in all nodes
            foreach (XmlNode cnode in node.ChildNodes)
            {
                if (cnode is XmlText)
                    StoreChildNode(childNodeNames, "value", cnode.InnerText);
                else if (cnode is XmlElement)
                    StoreChildNode(childNodeNames, cnode.Name, cnode);
            }

            // Now output all stored info
            foreach (string childname in childNodeNames.Keys)
            {
                ArrayList alChild = (ArrayList)childNodeNames[childname];
                if (alChild.Count == 1)
                    OutputNode(childname, alChild[0], sbJSON, true);
                else
                {
                    sbJSON.Append(" \"" + SafeJSON(childname) + "\": [ ");
                    foreach (object Child in alChild)
                        OutputNode(childname, Child, sbJSON, false);
                    sbJSON.Remove(sbJSON.Length - 2, 2);
                    sbJSON.Append(" ], ");
                }
            }
            sbJSON.Remove(sbJSON.Length - 2, 2);
            sbJSON.Append(" }");
        }

        //  StoreChildNode: Store data associated with each nodeName
        //                  so that we know whether the nodeName is an array or not.
        private static void StoreChildNode(SortedList childNodeNames, string nodeName, object nodeValue)
        {
            // Pre-process contraction of XmlElement-s
            if (nodeValue is XmlElement)
            {
                // Convert  <aa></aa> into "aa":null
                //          <aa>xx</aa> into "aa":"xx"
                XmlNode cnode = (XmlNode)nodeValue;
                if (cnode.Attributes.Count == 0)
                {
                    XmlNodeList children = cnode.ChildNodes;
                    if (children.Count == 0)
                        nodeValue = null;
                    else if (children.Count == 1 && (children[0] is XmlText))
                        nodeValue = ((XmlText)(children[0])).InnerText;
                }
            }
            // Add nodeValue to ArrayList associated with each nodeName
            // If nodeName doesn't exist then add it
            object oValuesAL = childNodeNames[nodeName];
            ArrayList ValuesAL;
            if (oValuesAL == null)
            {
                ValuesAL = new ArrayList();
                childNodeNames[nodeName] = ValuesAL;
            }
            else
                ValuesAL = (ArrayList)oValuesAL;
            ValuesAL.Add(nodeValue);
        }

        private static void OutputNode(string childname, object alChild, StringBuilder sbJSON, bool showNodeName)
        {
            if (alChild == null)
            {
                if (showNodeName)
                    sbJSON.Append("\"" + SafeJSON(childname) + "\": ");
                sbJSON.Append("null");
            }
            else if (alChild is string)
            {
                if (showNodeName)
                    sbJSON.Append("\"" + SafeJSON(childname) + "\": ");
                string sChild = (string)alChild;
                sChild = sChild.Trim();
                sbJSON.Append("\"" + SafeJSON(sChild) + "\"");
            }
            else
                XmlToJSONnode(sbJSON, (XmlElement)alChild, showNodeName);
            sbJSON.Append(", ");
        }

        // Make a string safe for JSON
        private static string SafeJSON(string sIn)
        {
            StringBuilder sbOut = new StringBuilder(sIn.Length);
            foreach (char ch in sIn)
            {
                if (Char.IsControl(ch) || ch == '\'')
                {
                    int ich = (int)ch;
                    sbOut.Append(@"\u" + ich.ToString("x4"));
                    continue;
                }
                else if (ch == '\"' || ch == '\\' || ch == '/')
                {
                    sbOut.Append('\\');
                }
                sbOut.Append(ch);
            }
            return sbOut.ToString();
        }



    }
}
