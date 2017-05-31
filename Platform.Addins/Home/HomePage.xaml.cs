using Platform.Common.Configure;
using Platform.ViewModel.Menu;
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

namespace Platform.Addins.Home
{
    /// <summary>
    /// HomePage.xaml 的交互逻辑
    /// </summary>
    public partial class HomePage
    {
        public HomePage()
        {
            InitializeComponent();
            LoadMenu();
        }

        #region Menu
        private void LoadMenu()
        {
            var tradeVMs = TradeMenuManager.Instance.GetTradeVMs();

        }
        #endregion

        #region TopLinker
        private void TopLinker_Raise(object sender, MouseButtonEventArgs e)
        {

        }
        #endregion

        #region TradeFilter
        private void tb_TradeFilter_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            var code = tb_TradeFilter.Text.Trim();
            if (e.Key == Key.Enter && code.Length > 0)
            {
                e.Handled = true;
                tb_TradeFilter.Clear();
                popup_TradeFilter.IsOpen = false;
                var model = TradeMenuManager.Instance.TradeModelList.FirstOrDefault(x => x.Code == code);
                PC_Host.OpenPage(model);
                return;
            }
            if ((e.Key == Key.Up || e.Key == Key.Down) && lb_TradeFilter.Items.Count > 0)
            {
                e.Handled = true;
                lb_TradeFilter.SelectedIndex = 0;
                var first = lb_TradeFilter.ItemContainerGenerator.ContainerFromIndex(0) as UIElement;
                if (first != null)
                    first.Focus();
                return;
            }
        }

        private void tb_TradeFilter_TextChanged(object sender, TextChangedEventArgs e)
        {
            var code = tb_TradeFilter.Text.Trim();
            //判断是否展示数据
            var len = PlatformSettings.Encoding.GetByteCount(code);
            if (len < 2)
            {
                popup_TradeFilter.IsOpen = false;
                return;
            }
            //筛选器
            var list = TradeMenuManager.Instance.TradeModelList.Where(x => !String.IsNullOrEmpty(x.Code) && (x.Code.Contains(code) || x.Name.Contains(code)));
            lb_TradeFilter.ItemsSource = list;
            lb_TradeFilter.Items.Refresh();
            popup_TradeFilter.IsOpen = true;
        }

        private void lb_TradeFilter_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            tb_TradeFilter.Clear();
            popup_TradeFilter.IsOpen = false;
            var model = (sender as Button).Tag as TradeModel;
            PC_Host.OpenPage(model);
        }
        #endregion

        private void MenuContainer_OpenTradeModel(object sender, MenuContainer.OpenTradeModelEventArgs e)
        {
            PC_Host.OpenPage(e.Model);
            toggle_Menu.IsChecked = false;
        }
    }
}
