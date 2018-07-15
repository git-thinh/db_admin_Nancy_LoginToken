
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;

namespace host
{
    public static class systemFunction
    {

        static public string dataType_ToText(this int data_type)
        {
            string s = "";
            char[] c = data_type.ToString().ToCharArray();
            if (c.Length == 7)
            {
                string pha = ((d1_pha)(c[0].TryToInt() * 1000000)).ToString();
                string data = ((d2_data)(c[1].TryToInt() * 100000)).ToString();
                string tech = ((d4_tech)(c[3].TryToInt() * 1000)).ToString();
                string nsx = ((d5_nsx)((c[c.Length - 2].ToString() + c[c.Length - 1].ToString()).TryParseToInt())).ToString();

                s = pha + ';' + data + ";" + tech + ";" + nsx;
            }

            return s;
        }

        /// <summary>
        /// Chuyển chuỗi unicode sang ascii (lọc bỏ dấu tiếng việt) 
        /// </summary>
        /// <param name="unicode"></param>
        /// <returns></returns>
        static public String ToAscii(this String unicode)
        {
            unicode = Regex.Replace(unicode, "[áàảãạăắằẳẵặâấầẩẫậ]", "a");
            unicode = Regex.Replace(unicode, "[óòỏõọôồốổỗộơớờởỡợ]", "o");
            unicode = Regex.Replace(unicode, "[éèẻẽẹêếềểễệ]", "e");
            unicode = Regex.Replace(unicode, "[íìỉĩị]", "i");
            unicode = Regex.Replace(unicode, "[úùủũụưứừửữự]", "u");
            unicode = Regex.Replace(unicode, "[ýỳỷỹỵ]", "y");
            unicode = Regex.Replace(unicode, "[đ]", "d");
            unicode = Regex.Replace(unicode, "[-\\s+/]+", "_");
            unicode = Regex.Replace(unicode, "\\W+", "_"); //Nếu bạn muốn thay dấu khoảng trắng thành dấu "_" hoặc dấu cách " " thì thay kí tự bạn muốn vào đấu "-"
            return unicode;
        }

        public static int TryToInt(this char c)
        {
            int k = 0;
            int.TryParse(c.ToString(), out k);
            return k;
        }

        public static int IntDateToDayOfYear(this int date_yyMMdd)
        {
            int k = 0;
            string s = date_yyMMdd.ToString();
            if (s.Length == 6)
            {
                int yy = ("20" + s.Substring(0, 2)).TryParseToInt();
                int mm = s.Substring(2, 2).TryParseToInt();
                int dd = s.Substring(4, 2).TryParseToInt();
                k = new DateTime(yy, mm, dd).DayOfYear - 1;
            }
            return k;
        }

        public static decimal TryParseToDecimal(this string decimal_string)
        {
            decimal val = 0;
            decimal.TryParse(decimal_string, out val);
            return val;
        }


        public static byte TryParseToByte(this string decimal_string)
        {
            byte val = 0;
            byte.TryParse(decimal_string, out val);
            return val;
        }

        public static int TryParseToInt(this string decimal_string)
        {
            int val = 0;
            int.TryParse(decimal_string, out val);
            return val;
        }

        public static UInt32 TryParseToUInt32(this string decimal_string)
        {
            UInt32 val = 0;
            UInt32.TryParse(decimal_string, out val);
            return val;
        }

        public static float TryParseToFloat(this string decimal_string)
        {
            float val = 0;
            float.TryParse(decimal_string, out val);
            return val;
        }

        public static double TryParseToDouble(this string decimal_string)
        {
            double val = 0;
            double.TryParse(decimal_string, out val);
            return val;
        }

        public static long TryParseToLong(this string decimal_string)
        {
            long val = 0;
            long.TryParse(decimal_string, out val);
            return val;
        }

        public static string f_Base64ToString(this string text)
        {
            //return HttpUtility.UrlDecode(System.Text.Encoding.UTF8.GetString(System.Convert.FromBase64String(text))); 
            string s = "";
            try
            {
                byte[] b = Convert.FromBase64String(text);
                s = ASCIIEncoding.UTF8.GetString(b);
            }
            catch
            {
                return text;
            }
            return s;
        }

        public static string f_StringToBase64(this string text)
        {
            char[] chars = HttpUtility.HtmlEncode(text).ToCharArray();
            StringBuilder result = new StringBuilder(text.Length + (int)(text.Length * 0.1));

            foreach (char c in chars)
            {
                int value = Convert.ToInt32(c);
                if (value > 127)
                    result.AppendFormat("&#{0};", value);
                else
                    result.Append(c);
            }

            //return result.ToString();
            return System.Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(result.ToString()));
        }

        public static byte[] GetByteArrayFromIntArray(this int[] intArray)
        {
            byte[] data = new byte[intArray.Length * 4];
            for (int i = 0; i < intArray.Length; i++)
                Array.Copy(BitConverter.GetBytes(intArray[i]), 0, data, i * 4, 4);
            return data;
        }

        public static int[] GetIntArrayFromByteArray(this byte[] byteArray)
        {
            int[] intArray = new int[byteArray.Length / 4];
            for (int i = 0; i < byteArray.Length; i += 4)
                intArray[i / 4] = BitConverter.ToInt32(byteArray, i);
            return intArray;
        }



        public static string ToString_CacheID(this string text)
        {
            return System.Text.RegularExpressions.Regex.Replace(text, "[^0-9a-zA-Z]+", "-", System.Text.RegularExpressions.RegexOptions.IgnoreCase | System.Text.RegularExpressions.RegexOptions.Multiline);
        }

        /// <summary>
        /// Getpath type file: .htm, .css, .js ...
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static string ToString_pathExt(this string path)
        {
            if (path.Contains("."))
                return path.Split('.')[1].Split('?')[0];

            return "";
        }
    }

    public static class Extensions_Function
    {
        public static string Base64Encode(this string plainText)
        {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
            return System.Convert.ToBase64String(plainTextBytes);
        }

        public static string Base64Decode(this string base64EncodedData)
        {
            var base64EncodedBytes = System.Convert.FromBase64String(base64EncodedData);
            return System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
        }

        public static string ToJson(this Dictionary<string, string> dictionary)
        {
            var kvs = dictionary.Select(kvp => string.Format("\"{0}\":\"{1}\"", kvp.Key, string.Join(",", kvp.Value)));
            return string.Concat("{", string.Join(",", kvs), "}");
        }

        public static Dictionary<string, string> FromJsonToDictionary(this string json)
        {
            string[] keyValueArray = json.Replace("{", string.Empty).Replace("}", string.Empty).Replace("\"", string.Empty).Split(',');
            return keyValueArray.ToDictionary(item => item.Split(':')[0], item => item.Split(':')[1]);
        }
    }
}
