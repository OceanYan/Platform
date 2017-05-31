using Platform.Widget.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace Platform.Widget.Contract
{
    public abstract class ContractBase
    {
        #region Base
        public ContractBase(ContractPage page)
        {
            PageData = page;
        }

        /// <summary>
        /// 契约界面
        /// </summary>
        public ContractPage PageData { get; private set; }
        #endregion

        #region IPageHelper
        public APIsHelper APIs
        {
            get { return PageData.APIs; }
        }
        #endregion

        #region abstract
        public abstract bool Raise();
        #endregion
    }
}
