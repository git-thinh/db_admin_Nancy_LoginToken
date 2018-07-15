using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;

namespace host
{
    public class hostSite
    {
        public static string page_main = ConfigurationManager.AppSettings["page_main"];
        public static string page_ext = ConfigurationManager.AppSettings["page_ext"];
    }
}
