using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Platform.Devices.Core
{
    public class DeviceSet<T>
    {
        #region Ctor
        public DeviceSet(bool hasError, string errorMsg = "")
        {
            HasError = hasError;
            ErrorMsg = errorMsg;
        }
        #endregion

        #region Prop
        /// <summary>
        /// 数据部分
        /// </summary>
        public T Data { get; set; }

        /// <summary>
        /// 是否发生错误
        /// </summary>
        public bool HasError { get; set; }

        /// <summary>
        /// 错误信息
        /// </summary>
        public string ErrorMsg { get; set; }
        #endregion
    }
}
