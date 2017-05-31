using Platform.Common.Configure;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Platform.Launcher
{
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            //加载系统准备
            base.OnStartup(e);
            //程序域产生异常，需要友好提示，并收集错误信息，该异常无法阻止e.IsTerminating，发生就代表程序即将奔溃
            AppDomain.CurrentDomain.UnhandledException += CurrentDomainUnhandledException;
            //系统退出模式更换，主窗体关闭即认为程序退出，尝试解决驻留进程的问题
            Application.Current.ShutdownMode = ShutdownMode.OnMainWindowClose;
            //环境变量的同步
            Environment.CurrentDirectory = AppDomain.CurrentDomain.BaseDirectory;
            //加载配置文件
            ConfigManager.GetInstance().LoadConfig(ConfigType.Client);
            ConfigManager.GetInstance().LoadConfig(ConfigType.System);
            ConfigManager.GetInstance().LoadConfig(ConfigType.User);
            //加载公共资源
            var resourceDictionary = new ResourceDictionary();
            resourceDictionary.Source = new Uri("/Platform.Controls;component/Themes/ScrollBar.xaml", UriKind.RelativeOrAbsolute);
            this.Resources.MergedDictionaries.Add(resourceDictionary);
            //欢迎屏幕
            SplashScreen splashScreen = new SplashScreen(Assembly.GetExecutingAssembly(), "Res/SplashScreen.png");
            splashScreen.Show(false);
            splashScreen.Close(new TimeSpan(0, 0, 1));
            //TODO:终端绑定验证

            //打开主界面
            new MainWindow().Show();
        }

        private void CurrentDomainUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            //throw new NotImplementedException();
            //系统奔溃的问题收集
        }

        internal void Activate()
        {
            if (this.MainWindow != null && this.MainWindow.IsLoaded)
            {
                //重新激活主窗口
                if (this.MainWindow.WindowState == WindowState.Minimized)
                    this.MainWindow.WindowState = WindowState.Normal;
                if (!this.MainWindow.IsActive)
                    this.MainWindow.Activate();
            }
        }
    }

    #region 单例模式，拷贝自微软示例
    /// <summary>
    /// 控制只启动一个程序
    /// </summary>
    public class Startup
    {
        [STAThread]
        public static void Main(string[] args)
        {
            SingleInstanceApplicationWrapper wrapper = new SingleInstanceApplicationWrapper();
            wrapper.Run(args);
        }
    }

    public class SingleInstanceApplicationWrapper : Microsoft.VisualBasic.ApplicationServices.WindowsFormsApplicationBase
    {
        /// <summary>
        /// 真正的WPF Application 
        /// </summary>
        private App app;

        public SingleInstanceApplicationWrapper()
        {
            this.IsSingleInstance = true;
        }

        /// <summary>
        /// 第一次打开调这个方法
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        protected override bool OnStartup(Microsoft.VisualBasic.ApplicationServices.StartupEventArgs e)
        {
            app = new App();
            //app.InitializeComponent();
            app.Run();
            return false;
        }

        /// <summary>
        /// 再次打开调这个方法
        /// </summary>
        /// <param name="e"></param>
        protected override void OnStartupNextInstance(Microsoft.VisualBasic.ApplicationServices.StartupNextInstanceEventArgs e)
        {
            // 当用户试图再次打开这个程序的时候，激活已有的系统
            base.OnStartupNextInstance(e);
            app.Activate();
        }
    }
    #endregion
}
