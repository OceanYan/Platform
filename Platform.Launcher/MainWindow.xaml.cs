using Platform.Controls.Dialog;
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
using System.Windows.Threading;

namespace Platform.Launcher
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            this.Dispatcher.UnhandledException += Dispatcher_UnhandledException;
            //计算窗体大小
            this.Left = SystemParameters.WorkArea.X;
            this.Top = SystemParameters.WorkArea.Y;
            this.Width = SystemParameters.FullPrimaryScreenWidth;
            this.Height = SystemParameters.FullPrimaryScreenHeight + SystemParameters.CaptionHeight;
            LoadLauncher();
        }

        private void LoadLauncher()
        {
            var login = PageFactory.CreatePage("Platform.Addins.Login");
            if (login == null)
                MessageBox.Show("未找到登陆界面！");
            this.Content = login;
        }

        private void Dispatcher_UnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            var page = new ConfirmDialog();
            page.Caption = "错误信息";
            page.Message = GetInnerException(e.Exception).Message;
            page.ShowDialog();
            e.Handled = true;
        }

        private Exception GetInnerException(Exception e)
        {
            if (e == null) return null;
            if (e.InnerException != null)
                return GetInnerException(e.InnerException);
            else
                return e;
        }
    }
}
