using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;

namespace Nancy
{
    public static class _ReConfig
    {
        public static string path_root = ConfigurationManager.AppSettings["path_root"];
    }
}
