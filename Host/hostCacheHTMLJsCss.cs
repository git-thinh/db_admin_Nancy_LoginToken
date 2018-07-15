using Nancy;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace host
{ 
     
    public class hostCacheHTMLJsCss
    {
        public static void ClearAll() {
            db.Clear();
        }

        private static Dictionary<string, string> db = new Dictionary<string, string>() { };
        
        public static bool Exist(string key)
        {
            return db.ContainsKey(key);
        }

        public static string Get(string key)
        {
            string data = "";
            db.TryGetValue(key, out data);
            return data;
        }

        public static void Set(string key, string data)
        {
            if (db.ContainsKey(key))
                db[key] = data;
            else
            {
                lock (db)
                {
                    db.Add(key, data);
                }
            }
        }

        public static void Remove(string key)
        {
            if (db.ContainsKey(key))
            {
                lock (db)
                {
                    db.Remove(key);
                }
            }
        }

    } // end class
}
