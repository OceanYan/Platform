using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Platform.Common.Configure;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using Platform.Common.LogSystem;
using System.Collections;

namespace Platform.Common.Helper
{
    public static class JsonHelper
    {
        /// <summary>
        /// 将json串转换成Dictionary对象
        /// </summary>
        /// <param name="jsonString"></param>
        /// <returns></returns>
        public static Dictionary<string, object> ParseJson2Dictionary(string jsonString)
        {
            var json = JObject.Parse(jsonString);
            return ParseJObject2Dictionary(json);
        }

        /// <summary>
        /// 将json串转换成Dictionary对象，深度为1
        /// </summary>
        /// <param name="jsonString"></param>
        /// <returns></returns>
        public static Dictionary<string, string> ParseJson2StringDictionary(string jsonString)
        {
            var json = JObject.Parse(jsonString);
            return ParseJObject2Dictionary(json, true).ToDictionary(x => x.Key, x => x.Value.ToString());
        }

        /// <summary>
        /// 将json串转换成List对象
        /// </summary>
        /// <param name="jsonString"></param>
        /// <returns></returns>
        public static List<object> ParseJson2List(string jsonString)
        {
            var json = JArray.Parse(jsonString);
            return ParseJArray2List(json);
        }

        /// <summary>
        /// 将json串转换成List对象，深度为1
        /// </summary>
        /// <param name="jsonString"></param>
        /// <returns></returns>
        public static List<string> ParseJson2StringList(string jsonString)
        {
            var json = JArray.Parse(jsonString);
            return ParseJArray2List(json, true).Select(x => x.ToString()).ToList();
        }

        /// <summary>
        /// 将Dictionary对象转换成json串
        /// </summary>
        /// <param name="dict"></param>
        /// <returns></returns>
        public static string ParseDictionary2Json<T>(Dictionary<string, T> dict)
        {
            return JsonConvert.SerializeObject(dict);
        }

        /// <summary>
        /// 将IList对象转换成json串
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public static string ParseList2Json(IList list)
        {
            return JsonConvert.SerializeObject(list);
        }

        /// <summary>
        /// 将JObject对象转换成Dictionary对象
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        private static Dictionary<string, object> ParseJObject2Dictionary(JObject json, bool depth1 = false)
        {
            var result = new Dictionary<string, object>();
            var children = json.Children<JProperty>();
            foreach (var item in children)
            {
                object value = null;
                if (item.Value is JValue)
                    value = (item.Value as JValue).Value;
                else if (item.Value is JObject)
                    value = depth1 ? (item.Value as JObject).ToString() : (object)ParseJObject2Dictionary(item.Value as JObject);
                else if (item.Value is JArray)
                    value = depth1 ? (item.Value as JArray).ToString() : (object)ParseJArray2List(item.Value as JArray);
                else
                    LogWriter.LogSystem("parseJObject2Dictionary:无法转换的类型" + item.Value.Type, LogLevel.WARN);
                result.Add(item.Name, value);
            }
            return result;
        }

        /// <summary>
        /// 将JArray对象转换成List对象
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        private static List<object> ParseJArray2List(JArray json, bool depth1 = false)
        {
            var result = new List<object>();
            var children = json.ToArray();
            foreach (var item in children)
            {
                object value = null;
                if (item is JValue)
                    value = (item as JValue).Value;
                else if (item is JObject)
                    value = depth1 ? (item as JObject).ToString() : (object)ParseJObject2Dictionary(item as JObject);
                else if (item is JArray)
                    value = depth1 ? (item as JArray).ToString() : (object)ParseJArray2List(item as JArray);
                else
                    LogWriter.LogSystem("ParseJArray2List:无法转换的类型" + item.Type, LogLevel.WARN);
                result.Add(value);
            }
            return result;
        }
    }
}
