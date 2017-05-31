using System;
using System.Collections.Generic;
using System.ComponentModel;
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

namespace Platform.Controls.Dialog
{
    /// <summary>
    /// ProgressDialog.xaml 的交互逻辑
    /// </summary>
    public partial class ProgressDialog
    {
        #region Ctor
        private ProgressDialog()
        {
            InitializeComponent();
            this.Loaded += ProgressDialog_Loaded;
        }

        private void ProgressDialog_Loaded(object sender, RoutedEventArgs e)
        {
            this.Loaded -= ProgressDialog_Loaded;
            //开启后台线程，执行业务逻辑
            _thread = new System.Threading.Thread(
                new System.Threading.ThreadStart(() =>
                {
                    var flag = false;
                    try
                    {
                        //吸收内部异常
                        Action?.Invoke();
                        flag = true;
                    }
                    catch
                    { }
                    this.Dispatcher.BeginInvoke(new Action(() =>
                    {
                        //DialogResult = flag;
                        this.State = flag ? DialogState.Close : DialogState.Exit;
                        this.Close();
                    }), null);
                }));
            _thread.IsBackground = true;
            _thread.Start();
            //开启计时器
            TimeSpan = new TimeSpan();
            var timer = new DispatcherTimer(new TimeSpan(0, 0, 1), DispatcherPriority.Normal, (sender1, e1) =>
            {
                TimeSpan = TimeSpan.Add(new TimeSpan(0, 0, 1));
                text_timeSpan.Text = "已耗时>>" + TimeSpan.ToString();
            }, this.Dispatcher);
            timer.Start();
        }
        #endregion

        #region Prop
        /// <summary>
        /// 操作耗时时间
        /// </summary>
        public TimeSpan TimeSpan { get; private set; }

        /// <summary>
        /// 操作动作
        /// </summary>
        public Action Action { get; private set; }

        /// <summary>
        /// 提示信息
        /// </summary>
        public string Message
        {
            get { return text_message.Text; }
            set { text_message.Text = value; }
        }

        /// <summary>
        /// 是否允许中断取消
        /// </summary>
        public bool CanCancel
        {
            get { return btn_Cancel.Visibility == Visibility.Visible; }
            set { btn_Cancel.Visibility = value ? Visibility.Visible : Visibility.Collapsed; }
        }
        #endregion

        #region Method
        /// <summary>
        /// 处理操作
        /// </summary>
        /// <param name="action"></param>
        /// <param name="canCancel"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public static bool ProgressWork(Action action, bool canCancel = false, string message = "后台处理中，请稍候...")
        {
            var dialog = new ProgressDialog();
            dialog.Message = message;
            dialog.CanCancel = canCancel;
            dialog.Action = action;
            return dialog.ShowDialog() ?? false;
        }
        #endregion
        private System.Threading.Thread _thread;
        private void btn_Cancel_Click(object sender, RoutedEventArgs e)
        {
            if (_thread.IsAlive)
                _thread.Abort();
            this.Close();
        }
    }
}
