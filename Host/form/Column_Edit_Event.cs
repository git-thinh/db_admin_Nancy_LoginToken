using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace host
{
    public class Column_Edit_EventArgs : EventArgs
    {
        public bool result { get; set; }
        public int data_type { get; set; }
    }
}
