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

namespace Platform.Common.Base
{
    /// <summary>
    /// IconBox.xaml 的交互逻辑
    /// </summary>
    public partial class IconBox : UserControl
    {
        #region Ctor
        public IconBox()
        {
            InitializeComponent();
        }
        #endregion

        #region Code
        /// <summary>
        /// 图标字体的值
        /// </summary>
        public string Code
        {
            get { return (string)GetValue(CodeProperty); }
            set { SetValue(CodeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Code.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CodeProperty =
            DependencyProperty.Register("Code", typeof(string), typeof(IconBox), new PropertyMetadata(""));
        #endregion

        #region Static
        /// <summary>
        /// 获取参数指定的图标画刷
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public static Brush GetIconBrush(string code, Brush foreground)
        {
            var icon = new IconBox();
            icon.Code = code;
            icon.Foreground = foreground;
            return new VisualBrush(icon);
        }

        /// <summary>
        /// 获取参数指定的图像源
        /// </summary>
        /// <param name="code"></param>
        /// <param name="foreground"></param>
        /// <returns></returns>
        public static ImageSource GetIconSource(string code, Brush foreground)
        {
            var icon = new IconBox();
            icon.Code = code;
            icon.Foreground = foreground;
            var window = new Window
            {
                Content = new ScrollViewer
                {
                    Content = icon,
                    VerticalScrollBarVisibility = ScrollBarVisibility.Visible,
                    HorizontalScrollBarVisibility = ScrollBarVisibility.Visible
                },
                SizeToContent = SizeToContent.WidthAndHeight
            };
            window.ShowInTaskbar = false;
            window.WindowState = WindowState.Normal;
            window.Top = 0 - 768;
            window.Left = 0 - 1024;
            window.Show();
            window.Close();
            //开始创建对象
            var rtb = new RenderTargetBitmap((int)icon.ActualWidth, (int)icon.ActualHeight, 96, 96, PixelFormats.Pbgra32);
            rtb.Render(icon);
            return rtb;
        }
        #endregion

        public enum IconType
        {

        }
    }
}
