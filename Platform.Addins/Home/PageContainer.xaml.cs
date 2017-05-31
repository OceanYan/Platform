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
using System.ComponentModel;
using Platform.Widget.Core;
using Platform.ViewModel.Menu;

namespace Platform.Addins.Home
{
    /// <summary>
    /// BusinessPage.xaml 的交互逻辑
    /// </summary>
    public partial class PageContainer
    {
        public PageContainer()
        {
            InitializeComponent();
            tab_Host.ItemsSource = pages;
            pages.CollectionChanged += (sender, e) =>
            {
                if (pages.Count == 0 && AllPagesClosed != null)
                    AllPagesClosed(this, new EventArgs());
            };
        }

        public event EventHandler AllPagesClosed;

        private ObservableCollection<PageBase> pages = new ObservableCollection<PageBase>();

        public bool OpenPage(TradeModel model)
        {
            //TODO:权限检测
            var page = PageFactory.CreatePage(model.Action);
            if (page == null)
            {
                MessageBox.Show("未能正确加载交易:" + model.Code + "[" + model.Action + "]");
                return false;
            }
            //查找同一个交易的清单，移除需要稍后进行，否则会触发意外的AllPagesClosed事件
            var removeList = pages.Where(x => (x.Tag as TradeModel) == model).ToList();
            //若打开的交易数目太多，也移除一部分
            if (pages.Count - removeList.Count > 9)
                removeList.AddRange(pages.Take(pages.Count - removeList.Count - 10).ToList());
            //载入页面交易
            page.Tag = model;
            pages.Add(page);
            foreach (var item in removeList)
                pages.Remove(item);
            tab_Host.Items.Refresh();
            tab_Host.SelectedIndex = tab_Host.Items.Count - 1;
            return true;
        }

        private void Button_Close_Click(object sender, RoutedEventArgs e)
        {
            var current = (sender as FrameworkElement).Tag as PageBase;
            if (current == null) return;
            pages.Remove(current);
            tab_Host.Items.Refresh();
            tab_Host.SelectedIndex = tab_Host.Items.Count - 1;
        }
    }
}
