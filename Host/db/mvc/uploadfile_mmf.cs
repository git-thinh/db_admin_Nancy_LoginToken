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
using model;

namespace host
{
    public class uploadfile_mmf : NancyModule
    {
        private static List<m_meter_plc> list = new List<m_meter_plc>() { };

        public uploadfile_mmf()
            : base("uploadfile_mmf")
        {
            Post[""] = para =>
            {
                var itemp = this.Request.Query;
                var parr = new Dictionary<string, string>();
                foreach (var key in itemp.Keys)
                {
                    parr.Add(key, itemp[key]);
                }

                string uploadDirectory = AppDomain.CurrentDomain.BaseDirectory + @"\upload_mmf";

                if (!Directory.Exists(uploadDirectory)) Directory.CreateDirectory(uploadDirectory);

                if (!Directory.Exists(uploadDirectory))
                {
                    Directory.CreateDirectory(uploadDirectory);
                }
                var filename = "";
                var path = "";
                foreach (var file in Request.Files)
                {
                    filename = Path.Combine(uploadDirectory, file.Name);
                    using (FileStream fileStream = new FileStream(filename, FileMode.Create))
                    {
                        file.Value.CopyTo(fileStream);
                    }
                    path = uploadDirectory + @"\"+ file.Name;
                }

                path = path.Replace(@"\\", @"\");

                LoadFileMmf(path);

                string ok = "ok";
                var o = (Response)ok;
                o.StatusCode = HttpStatusCode.OK;
                o.ContentType = "application/xml";

                return o;
            };
        }

        public void LoadFileMmf(string fileName)
        {
            list = hostFile.read_file_MMF<m_meter_plc>(fileName).ToList();
        }
    }
}
