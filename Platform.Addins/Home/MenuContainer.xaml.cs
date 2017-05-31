using System;
using System.Collections.Generic;
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
using System.Collections.ObjectModel;
using Platform.ViewModel.Menu;

namespace Platform.Addins.Home
{
    /// <summary>
    /// MenuPopup.xaml 的交互逻辑
    /// </summary>
    public partial class MenuContainer : UserControl
    {
        public MenuContainer()
        {
            InitializeComponent();
            Init();
        }

        public static RoutedCommand FirstMenuComand { get; set; }

        private void Init()
        {
            if (System.ComponentModel.DesignerProperties.GetIsInDesignMode(this)) return;
            //拉取基础数据            
            var tradeVMs = TradeMenuManager.Instance.GetTradeVMs();
            //根据情况，决定是否需要进行装饰转换

            //填充数据源
            Items_First.ItemsSource = tradeVMs;
        }

        private void border_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;
            //菜单被触发
            var model = (sender as FrameworkElement).Tag as TradeModel;
            if (model == null || string.IsNullOrEmpty(model.Code)) return;
            if (OpenTradeModel != null)
                OpenTradeModel(this, new OpenTradeModelEventArgs(model));
        }

        public event EventHandler<OpenTradeModelEventArgs> OpenTradeModel;

        public class OpenTradeModelEventArgs : EventArgs
        {
            public OpenTradeModelEventArgs(TradeModel model)
            {
                Model = model;
            }

            public TradeModel Model { get; private set; }
        }
    }
}
