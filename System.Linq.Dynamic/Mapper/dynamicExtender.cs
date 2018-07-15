using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System.Linq.Dynamic
{
    public static class dynamicExtender
    {
        public static byte[] toByteJson(this IEnumerable data, bool isUTF8 = false)
        {
            string json = JsonConvert.SerializeObject(data);
            if (isUTF8)
            {
                ASCIIEncoding encoding = new ASCIIEncoding();
                return encoding.GetBytes(json);
            }
            else
            {
                ASCIIEncoding encoding = new ASCIIEncoding();
                return encoding.GetBytes(json);
            }
        }
    }
}
