using Microsoft.CSharp;
using Newtonsoft.Json;
using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Dynamic;
using System.Linq;
using System.Reflection;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Web.Script.Serialization;
//using System.Linq.Dynamic.AutoMapper;

namespace System.Linq.Dynamic
{
    public static class dynamicArray
    {
        private static readonly object lockType = new object();
        private static Dictionary<string, Type> dbType = new Dictionary<string, Type>() { };
        public static bool TryFindType(string typeName, out Type t)
        {
            lock (dbType)
            {
                if (!dbType.TryGetValue(typeName, out t))
                {
                    foreach (Assembly a in AppDomain.CurrentDomain.GetAssemblies())
                    {
                        t = a.GetType(typeName);
                        if (t != null)
                            break;
                    }
                    dbType[typeName] = t; // perhaps null
                }
            }
            return t != null;
        }

        public static IList ToListDynamic(this byte[] data, bool isUTF8 = false)
        {
            string json = "";
            if (isUTF8)
            {
                UTF8Encoding encoding = new UTF8Encoding();
                json = encoding.GetString(data);
            }
            else
            {
                ASCIIEncoding encoding = new ASCIIEncoding();
                json = encoding.GetString(data);
            }

            if (!string.IsNullOrWhiteSpace(json))
            {
                //var serializer = new JavaScriptSerializer();
                //serializer.RegisterConverters(new[] { new DynamicJsonConverter() });
                //dynamic arrayObject = serializer.Deserialize(json, typeof(object));

                //return ConvertToListDynamic(arrayObject); 
            }

            return null;
        }

        private static IList ConvertToListDynamic(object[] arrayObject)
        {
            var item = arrayObject[0] as DynamicJsonConverter.DynamicJsonObject;
            var dicFields = item.getData();

            string cols = " new ( " + string.Join(", ", dicFields.Keys.ToArray()).ToUpper() + " ) ";
            string class_key = string.Join("|", dicFields.Keys.ToArray()).ToLower();

            Type myType = null;
            if (dbType.ContainsKey(class_key))
                myType = dbType[class_key];
            else
            {
                string className = "cls_" + Guid.NewGuid().ToString().Replace("-", "");
                myType = dynamicTypeBuilder.CompileResultType(className, dicFields);
                lock (lockType) {
                    dbType.Add(class_key, myType);
                }
            }

            //var myObject = Activator.CreateInstance(myType);
            //Type myClassType = Type.GetType(className);
            //Type to;
            //var ti = TryFindType(className, out to);

            var aItem = arrayObject.Cast<DynamicJsonConverter.DynamicJsonObject>().Select(oi => oi.getData()).ToArray();

            var ls = Map(aItem, myType).ToListDynamic()
                        .Select(cols).ToListDynamic();
            return ls;
        }

        private static IEnumerable Map(IEnumerable<IDictionary<string, object>> listOfProperties, Type typeObject)
        {
            var instanceCache = new Dictionary<object, object>();

            foreach (var properties in listOfProperties)
            {
                var getInstanceResult = System.Linq.Dynamic.AutoMapper.InternalHelpers.GetInstance(typeObject, properties);

                object instance = getInstanceResult.Item2;

                int instanceIdentifierHash = getInstanceResult.Item3;

                if (instanceCache.ContainsKey(instanceIdentifierHash) == false)
                {
                    instanceCache.Add(instanceIdentifierHash, instance);
                }

                var caseInsensitiveDictionary = new Dictionary<string, object>(properties, StringComparer.OrdinalIgnoreCase);

                System.Linq.Dynamic.AutoMapper.InternalHelpers.Map(caseInsensitiveDictionary, instance);
            }

            foreach (var pair in instanceCache)
            {
                yield return (dynamic)pair.Value;
            }
        }


    }//end class
}//end namespace
