using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Data;

namespace Platform.Controls.Layout
{
    public static class ValueConverter
    {
        #region Base
        private static Dictionary<string, object> _dict = new Dictionary<string, object>();

        /// <summary>
        /// 获取转换器，自行具备单例特性
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        private static T GetValueConverter<T>() where T : class, new()
        {
            var key = typeof(T).FullName;
            if (!_dict.ContainsKey(key))
                _dict.Add(key, new T());
            return _dict[key] as T;
        }
        #endregion

        #region BoolToVisibility
        public static BoolToVisibilityConverter BoolToVisibility
        {
            get { return GetValueConverter<BoolToVisibilityConverter>(); }
        }

        /// <summary>
        /// true=Visibility.Visible，false=Visibility.Collapsed
        /// </summary>
        public class BoolToVisibilityConverter : IValueConverter
        {
            public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
            {
                if ((bool)value)
                {
                    return Visibility.Visible;
                }
                else
                {
                    return Visibility.Collapsed;
                }
            }

            public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            {
                throw new NotImplementedException();
            }
        }
        #endregion

        #region IndexOfIListConverter
        /// <summary>
        /// 获取参数2在参数1中的索引值
        /// </summary>
        public static IndexOfIListConverter IndexOfIList
        {
            get { return GetValueConverter<IndexOfIListConverter>(); }
        }

        public sealed class IndexOfIListConverter : IMultiValueConverter
        {
            public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
            {
                if (values.Length < 2) return string.Empty;
                var list = values[0] as IList;
                if (list == null) return string.Empty;
                var index = list.IndexOf(values[1]) + 1;
                if (index > 0)
                    return index.ToString("".PadLeft(list.Count.ToString().Length, '0'));
                else
                    return string.Empty;
            }

            public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
            {
                throw new NotImplementedException("IndexOfCollectionConverter不支持反向绑定！");
            }
        }
        #endregion
    }


}
