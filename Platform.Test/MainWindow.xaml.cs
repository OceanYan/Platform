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

namespace Platform.Test
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            Platform.Controls.Dialog.ConfirmDialog.Show("我是测试信息我是测试信息我是测试\n信息我是测试信息我是测试信息我是测试信息我是测试信息！！！", "提示信息", MessageBoxButton.OK, MessageBoxImage.Exclamation);
        }
    }
}
