using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace host
{
    public class oBackgroundWorker : BackgroundWorker
    {
        public int id { set; get; }
        public string filePath { set; get; }
    }
}
