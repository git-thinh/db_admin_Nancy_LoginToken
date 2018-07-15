using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Caching;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace host
{
    public class hostCache_Src
    {
        private static Dictionary<string, int> dbCache = new Dictionary<string, int>() { };
        private static readonly object loc_cache = new object();
        private static ObjectCache cache = MemoryCache.Default;

        public static void clearAll()
        {
            dbCache.Clear();
        }

        public static bool hasCache(string key)
        {
            return dbCache.ContainsKey(key);
        }

        public static string getCache(string key)
        {
            return cache[key] as string;
        }

        public static void setCache(string key, string src)
        {
            cache[key] = src;
            lock (loc_cache)
            {
                if (dbCache.ContainsKey(key))
                    dbCache[key] = 1;
                else
                    dbCache.Add(key, 1);
            }
        }

        public static void setRank(string key)
        {
            lock (loc_cache)
            {
                if (dbCache.ContainsKey(key))
                    dbCache[key] = dbCache[key] + 1;
                else
                    dbCache.Add(key, 1);
            }
        }
    }
}
