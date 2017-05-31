using Platform.Common.Base;
using Platform.Controls.Core;
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
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Platform.Common.Extension;

namespace Platform.Controls.Dialog
{
    /// <summary>
    /// 按照步骤 1a 或 1b 操作，然后执行步骤 2 以在 XAML 文件中使用此自定义控件。
    ///
    /// 步骤 1a) 在当前项目中存在的 XAML 文件中使用该自定义控件。
    /// 将此 XmlNamespace 特性添加到要使用该特性的标记文件的根 
    /// 元素中: 
    ///
    ///     xmlns:MyNamespace="clr-namespace:Platform.Controls.Dialog"
    ///
    ///
    /// 步骤 1b) 在其他项目中存在的 XAML 文件中使用该自定义控件。
    /// 将此 XmlNamespace 特性添加到要使用该特性的标记文件的根 
    /// 元素中: 
    ///
    ///     xmlns:MyNamespace="clr-namespace:Platform.Controls.Dialog;assembly=Platform.Controls.Dialog"
    ///
    /// 您还需要添加一个从 XAML 文件所在的项目到此项目的项目引用，
    /// 并重新生成以避免编译错误: 
    ///
    ///     在解决方案资源管理器中右击目标项目，然后依次单击
    ///     “添加引用”->“项目”->[浏览查找并选择此项目]
    ///
    ///
    /// 步骤 2)
    /// 继续操作并在 XAML 文件中使用控件。
    ///
    ///     <MyNamespace:PageDialog/>
    ///
    /// </summary>
    public class PageDialog : Window
    {
        #region Ctor
        static PageDialog()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(PageDialog), new FrameworkPropertyMetadata(typeof(PageDialog)));
        }

        public PageDialog()
        {
            this.DataContext = this;
            //this.IsReady = true;
            this.Closed += (sender, e) =>
            {
                //窗口关闭时，管理其状态
                if (this.State == DialogState.Ready)
                    this.State = DialogState.Close;
            };
            this.Dispatcher.UnhandledException += Dispatcher_UnhandledException;
            //最大高度限定
            this.MaxHeight = SystemParameters.FullPrimaryScreenHeight - 100;
        }

        private void Dispatcher_UnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            //发生异常，登记即可
            e.Handled = true;
        }

        private BaseCommand _closeCommand;
        public BaseCommand CloseCommand
        {
            get
            {
                return _closeCommand ??
                    (_closeCommand = new BaseCommand((x) =>
                {
                    //提示信息
                    var ret = ConfirmDialog.Show("确认要关闭窗口么？", "询问", MessageBoxButton.OKCancel, MessageBoxImage.Question, MessageBoxResult.Cancel);
                    if (ret == MessageBoxResult.OK)
                    {
                        //this.DialogResult = false;
                        this.State = DialogState.Exit;
                        this.Close();
                    }
                }));
            }
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            //支持Header拖拽
            var border = this.GetTemplateChild("bd_Header") as Border;
            if (border != null)
            {
                border.MouseLeftButtonDown -= Border_MouseLeftButtonDown;
                border.MouseLeftButtonDown += Border_MouseLeftButtonDown;
            }
        }

        private void Border_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }
        #endregion

        #region DialogMode
        /// <summary>
        /// 显示的模式
        /// </summary>
        public DialogMode Mode { get; set; }

        /// <summary>
        /// 窗口的状态
        /// </summary>
        public DialogState State { get; protected set; }

        public enum DialogState
        {
            /// <summary>
            /// 无
            /// </summary>
            None,
            /// <summary>
            /// 准备完毕
            /// </summary>
            Ready,
            /// <summary>
            /// 中断退出
            /// </summary>
            Exit,
            /// <summary>
            /// 正常关闭
            /// </summary>
            Close
        }

        public new void Show()
        {
            ShowDialog();
        }

        public new bool? ShowDialog()
        {
            //检查Owner设置
            CheckOwner();
            if (this.Owner != null)
                InitByMode();
            base.ShowDialog();
            switch (State)
            {
                case DialogState.Exit:
                    return false;
                case DialogState.Close:
                    return true;
                default:
                    return null;
            }
        }

        private void CheckOwner()
        {
            if (this.Owner == null)
            {
                //找到当前的激活窗体
                foreach (Window win in Application.Current.Windows)
                {
                    //找到未有子界面，且加载完毕的窗口
                    if (win.IsLoaded && win.OwnedWindows.Count == 0)
                    {
                        this.Owner = win;
                        //如果窗口也已经被激活，则采用
                        if (win.IsActive)
                            break;
                    }
                }
                //若仍未找到，则取用主界面窗口
                if (this.Owner == null && Application.Current.MainWindow.IsLoaded)
                    this.Owner = Application.Current.MainWindow;
            }
        }

        public new void Close()
        {
            switch (Mode)
            {
                case DialogMode.Drawer:
                    this.IsEnabled = false;
                    var storyboard = new Storyboard();
                    var daukf = new DoubleAnimationUsingKeyFrames() { Duration = new Duration(TimeSpan.FromSeconds(0.5)) };
                    daukf.KeyFrames.Add(new EasingDoubleKeyFrame(this.Top, KeyTime.FromPercent(0)));
                    daukf.KeyFrames.Add(new EasingDoubleKeyFrame(this.Top + this.ActualHeight, KeyTime.FromPercent(1)));
                    Storyboard.SetTarget(daukf, this);
                    Storyboard.SetTargetProperty(daukf, new PropertyPath("Top"));
                    storyboard.Children.Add(daukf);
                    storyboard.Completed += (sender, e) => { base.Close(); };
                    storyboard.Begin();
                    break;
                default:
                    base.Close();
                    break;
            }
        }

        private void InitByMode()
        {
            this.Owner.Opacity = 0.8;
            this.Closed += (sender, e) => { this.Owner.Opacity = 1; };
            if (IsLoaded) return;
            switch (Mode)
            {
                case DialogMode.Page:
                    this.WindowStartupLocation = WindowStartupLocation.CenterOwner;
                    this.SizeToContent = SizeToContent.WidthAndHeight;
                    this.State = DialogState.Ready;
                    break;
                case DialogMode.Drawer:
                    //采用Owner视觉树的第一个节点做坐标换算，Owner本身存在误差
                    var root = Application.Current.MainWindow.FirstVisualTreeChildren(x => x is FrameworkElement) as FrameworkElement;
                    if (root == null) break;
                    this.Title = "";
                    this.SizeToContent = SizeToContent.Height;
                    this.Width = root.ActualWidth;
                    this.WindowStartupLocation = WindowStartupLocation.Manual;
                    this.Top = -1000;
                    this.Dispatcher.BeginInvoke(new Action(() =>
                    {
                        var point = root.PointToScreen(new Point(0, 0));
                        this.Left = point.X;
                        //采用动画改变Top的值
                        //this.Top = point.Y + border.ActualHeight - this.ActualHeight;
                        //考虑动画执行过程中，禁止用户操作
                        var storyboard = new Storyboard();
                        var daukf = new DoubleAnimationUsingKeyFrames() { Duration = new Duration(TimeSpan.FromSeconds(0.5)) };
                        daukf.KeyFrames.Add(new EasingDoubleKeyFrame(point.Y + root.ActualHeight, KeyTime.FromPercent(0)));
                        daukf.KeyFrames.Add(new EasingDoubleKeyFrame(point.Y + root.ActualHeight - this.ActualHeight, KeyTime.FromPercent(1)));
                        Storyboard.SetTarget(daukf, this);
                        Storyboard.SetTargetProperty(daukf, new PropertyPath("Top"));
                        storyboard.Children.Add(daukf);
                        storyboard.Completed += (sender, e) => { this.State = DialogState.Ready; };
                        storyboard.Begin();
                    }), System.Windows.Threading.DispatcherPriority.DataBind);
                    break;
                default:
                    break;
            }
        }
        #endregion
    }

    public enum DialogMode
    {
        /// <summary>
        /// 页面
        /// </summary>
        Page,

        /// <summary>
        /// 抽屉式
        /// </summary>
        Drawer
    }
}
