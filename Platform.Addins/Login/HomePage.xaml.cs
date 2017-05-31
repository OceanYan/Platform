using Platform.Widget.Core;
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

namespace Platform.Addins.Login
{
    /// <summary>
    /// MainPage.xaml 的交互逻辑
    /// </summary>
    public partial class MainPage
    {
        public MainPage()
        {
            InitializeComponent();
        }

        private void Cancel_FlowLeave(object sender, RoutedEventArgs e)
        {
            Application.Current.MainWindow.Close();
        }

        private void Login_FlowLeave(object sender, RoutedEventArgs e)
        {
            var home = PageFactory.CreatePage("Platform.Addins.Home");
            if (home == null)
            {
                MessageBox.Show("未找到主界面！");
                return;
            }
            var parent = this.Parent as ContentControl;
            if (parent != null)
                parent.Content = home;
        }
    }
}
