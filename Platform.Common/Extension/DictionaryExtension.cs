using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace Platform.Common.Extension
{
    /// <summary>
    /// 基于Dictionary的扩展方法集合
    /// </summary>
    public static class DictionaryExtension
    {
        /// <summary>
        /// 获取字典数据元素
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public static T GetDictionaryItem<T>(this IDictionary<string, T> dict, string key)
        {
            T result = default(T);
            if (dict != null && dict.ContainsKey(key))
            {
                result = (T)dict[key];
            }
            return result;
        }

        /// <summary>
        /// 设置字典数据元素
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public static void SetDictionaryItem<T>(this IDictionary<string, T> dict, string key, T value)
        {
            if (dict == null)
                dict = new Dictionary<string, T>();
            if (!dict.ContainsKey(key))
                dict.Add(key, value);
            else
                dict[key] = value;
        }
    }
}
