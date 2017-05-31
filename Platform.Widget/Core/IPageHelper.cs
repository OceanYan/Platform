using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Platform.Widget.Core
{
    /// <summary>
    /// 交易帮助接口
    /// </summary>
    public interface IPageHelper
    {
        /// <summary>
        /// 操作接口
        /// </summary> 
        APIsHelper APIs { get; }
    }
}
