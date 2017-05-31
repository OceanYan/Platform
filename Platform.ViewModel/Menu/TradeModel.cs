using Platform.Common.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Platform.ViewModel.Menu
{
    /// <summary>
    /// 交易的模型
    /// </summary>
    public class TradeModel
    {
        #region Ctor
        public TradeModel()
        {
            Metadata = new Dictionary<string, string>();
        }
        #endregion

        #region Prop
        /// <summary>
        /// 主键id，GUID
        /// </summary>
        public string ID { get; set; }

        /// <summary>
        /// 交易或目录的名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 交易代码
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// 交易关联
        /// </summary>
        public string Action { get; set; }

        /// <summary>
        /// 可用标志
        /// </summary>
        public bool IsVisible { get; set; }

        /// <summary>
        /// 描述
        /// </summary>
        public string Desc { get; set; }

        /// <summary>
        /// 交易的扩展元数据组集合
        /// </summary>
        public IDictionary<string, string> Metadata { get; private set; }

        /// <summary>
        /// 父元素
        /// </summary>
        public TradeModel Parent { get; set; }
        #endregion
    }
}
