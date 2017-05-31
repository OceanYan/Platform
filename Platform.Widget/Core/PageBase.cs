using Platform.Common.LogSystem;
using System;
using System.ComponentModel;
using System.Configuration;
using System.Windows;
using System.Windows.Controls;

namespace Platform.Widget.Core
{
    /// <summary>
    /// 页面基类，填充通用的业务接口API
    /// 注意:该对象需要支持序列化/反序列化
    /// </summary>
    [DesignTimeVisible(false)]
    public class PageBase : ContentControl, IPageHelper
    {
        #region InitPage
        public PageBase()
        {
            ////所有组件的界面都采用宋体16进行显示
            this.FontFamily = new System.Windows.Media.FontFamily("SimSun");
            this.FontSize = 16;
            this.Loaded += PageBase_Loaded;
            this.FocusVisualStyle = null;
        }

        private void PageBase_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            this.Loaded -= PageBase_Loaded;
            //执行InitPage方法，初始化界面
            var flag = false;
            try
            {
                flag = InitPage();
            }
            catch (Exception ex)
            {
                LogWriter.LogSystem("InitPage发生异常", ex);
            }
            if (!flag)
            {
                APIs.ShowMessage("界面初始化失败！");
                ClosePage();
            }
        }

        /// <summary>
        /// 初始化页面
        /// </summary>
        /// <returns></returns>
        public virtual bool InitPage()
        {
            return true;
        }
        #endregion

        #region ClosePage
        public event EventHandler PageClosed;
        public void ClosePage()
        {
            PageClosed?.Invoke(this, new EventArgs());
        }
        #endregion

        #region 访问接口
        #region 操作接口 APIs
        private APIsHelper _tradeAPIs;
        /// <summary>
        /// 操作接口
        /// </summary>
        public APIsHelper APIs
        {
            get
            {
                if (_tradeAPIs == null)
                    _tradeAPIs = new APIsHelper(this);
                return _tradeAPIs;
            }
        }
        #endregion
        #endregion

        #region DisplayWidth - 显示宽度设置
        public double DisplayWidth
        {
            get { return (double)GetValue(DisplayWidthProperty); }
            set { SetValue(DisplayWidthProperty, value); }
        }

        // Using a DependencyProperty as the backing store for DisplayWidth.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DisplayWidthProperty =
            DependencyProperty.Register("DisplayWidth", typeof(double), typeof(PageBase), new PropertyMetadata(double.NaN));

        protected override void OnContentChanged(object oldContent, object newContent)
        {
            base.OnContentChanged(oldContent, newContent);
            var element = newContent as FrameworkElement;
            if (element == null) return;
            element.Dispatcher.BeginInvoke(new Action(() =>
            {
                element.Width = DisplayWidth;
                element.HorizontalAlignment = double.IsNaN(DisplayWidth) ? HorizontalAlignment.Stretch : HorizontalAlignment.Left;
            }), System.Windows.Threading.DispatcherPriority.Background);
        }
        #endregion
    }
}
