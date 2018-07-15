using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Dynamic;
using System.Runtime.Caching;

namespace host
{
    public class dbCache
    {
        public static DictionaryList<string, string> dicKeys = new DictionaryList<string, string>() { };

        public static void clear(string key_group)
        {
            if (dicKeys.ContainsKey(key_group))
            {
                List<string> ls = new List<string>() { };
                dicKeys.TryGetValue(key_group, out ls);

                ObjectCache ca = MemoryCache.Default;

                if (ls != null && ls.Count > 0)
                {
                    foreach (string li in ls)
                        ca.Remove(li);
                }
                dicKeys.Remove(key_group);
            }
        }

        public static void set(string key, object data) 
        {
            ObjectCache cache = MemoryCache.Default;
            CacheItemPolicy policy = new CacheItemPolicy();
            policy.Priority = System.Runtime.Caching.CacheItemPriority.Default;
            policy.AbsoluteExpiration = DateTimeOffset.Now.AddMinutes(30);
            
            //Muốn tạo sự kiện thì tạo ở đây còn k thì hoy
            //(MyCacheItemPriority == MyCachePriority.Default) ? CacheItemPriority.Default : CacheItemPriority.NotRemovable;
            ///Globals.policy.RemovedCallback = callback; 

            cache.Set(key, data, policy, null);        
        }


    }
}
