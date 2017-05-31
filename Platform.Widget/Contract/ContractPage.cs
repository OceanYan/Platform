using Platform.Widget.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Platform.Widget.Contract
{
    [DesignTimeVisible(false)]
    public class ContractPage : PageBase
    {
        /// <summary>
        /// 绑定的契约
        /// </summary>
        public ContractBase Contract { get; protected set; }

        /// <summary>
        /// 用于缓存的数据集合对象
        /// </summary>
        public ApplicationSettingsBase DataBus { get; protected set; }

        /// <summary>
        /// 设置数据总线对象
        /// </summary>
        /// <param name="dataBus"></param>
        internal void SetDataBus(ApplicationSettingsBase dataBus)
        {
            DataBus = dataBus;
        }
    }
}
