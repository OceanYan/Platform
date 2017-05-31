using Platform.Controls.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
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

namespace Platform.Controls.UI
{
    /// <summary>
    /// Button.xaml 的交互逻辑
    /// </summary>
    public partial class Button
    {
        public Button()
        {
            InitializeComponent();
            this.MinWidth = 120;
            this.Height = 30;
            IsDelayClick = false;
        }

        public override string Value
        {
            get { return (string)btn_Host.Content; }
            set { btn_Host.Content = value; }
        }

        #region IconCode
        /// <summary>
        /// 前缀图标码
        /// </summary>
        public string IconCode
        {
            get { return (string)GetValue(IconCodeProperty); }
            set { SetValue(IconCodeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IconCode.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IconCodeProperty =
            DependencyProperty.Register("IconCode", typeof(string), typeof(Button), new PropertyMetadata(""));
        #endregion

        /// <summary>
        /// 是否控制点击间隔
        /// </summary>
        public bool IsDelayClick { get; set; }

        #region Click
        /// <summary>
        /// Click，点击事件
        /// </summary>
        public static readonly RoutedEvent ClickEvent = EventManager.RegisterRoutedEvent("Click",
            RoutingStrategy.Direct, typeof(RoutedEventHandler), typeof(Button));

        /// <summary>
        /// Click，点击事件
        /// </summary>
        public event RoutedEventHandler Click
        {
            add { AddHandler(ClickEvent, value); }
            remove { RemoveHandler(ClickEvent, value); }
        }

        private void btn_Host_Click(object sender, RoutedEventArgs e)
        {
            if (IsDelayClick)
            {
                //当动画执行时（按钮的透明度被更改），直接跳过
                if (btn_Host.Opacity != 1)
                    return;
                this.BeginStoryboard(Storyboard_ClickSpan);
            }
            //检查并引导Click事件
            this.RaiseEvent(new RoutedEventArgs { RoutedEvent = ClickEvent });
        }
        #endregion

        #region Override
        private Storyboard storyboard_ClickSpan;
        private Storyboard Storyboard_ClickSpan
        {
            get
            {
                if (storyboard_ClickSpan == null)
                    storyboard_ClickSpan = FindResource("Storyboard_ClickSpan") as Storyboard;
                return storyboard_ClickSpan;
            }
        }

        internal override bool OnFlowKeyDown(KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                return false;
            return base.OnFlowKeyDown(e);
        }
        #endregion
    }
}
