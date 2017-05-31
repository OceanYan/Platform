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
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Platform.Controls.UI
{
    /// <summary>
    /// DateTimeRange.xaml 的交互逻辑
    /// </summary>
    public partial class RangeBox
    {
        #region Ctor
        public RangeBox()
        {
            InitializeComponent();
            this.Loaded += (sender, e) =>
            {
                if (RangeStart == null || RangeEnd == null || RangeStart.GetType() != RangeEnd.GetType())
                    throw new ArgumentException("设定的RangeStart/RangeEnd参数不正确！");
                SetStyle(RangeStart);
                SetStyle(RangeEnd);
            };
            this.FlowLeave += RangeBox_FlowLeave;
        }

        private void RangeBox_FlowLeave(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(RangeStart.Value) && string.IsNullOrEmpty(RangeEnd.Value))
            {
                if (AssertNotEmpty)
                {
                    RangeEnd.ToastMessage("范围值必须有值！");
                    e.Handled = true;
                }
                return;
            }
            if (string.IsNullOrEmpty(RangeStart.Value) || string.IsNullOrEmpty(RangeEnd.Value))
            {
                RangeEnd.ToastMessage("范围值必须成对出现！");
                e.Handled = true;
                return;
            }
            if (RangeEnd.CompareTo(RangeStart) < 0)
            {
                RangeEnd.ToastMessage("必须为有效范围！");
                e.Handled = true;
                return;
            }
        }

        private void SetStyle(ComponentBase c)
        {
            if (c == null) return;
            c.SetValue(TitleComponent.TitleProperty, null);
            c.SetValue(Platform.Controls.UI.TextInput.AssertNotEmptyProperty, AssertNotEmpty);
        }
        #endregion

        #region AssertNotEmpty
        /// <summary>
        /// 是否判断非空
        /// </summary>
        public bool AssertNotEmpty { get; set; }
        #endregion

        #region RangeStart
        public ComponentBase RangeStart
        {
            get { return (ComponentBase)GetValue(RangeStartProperty); }
            set { SetValue(RangeStartProperty, value); }
        }

        // Using a DependencyProperty as the backing store for RangeStart.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty RangeStartProperty =
            DependencyProperty.Register("RangeStart", typeof(ComponentBase), typeof(RangeBox), new PropertyMetadata(null,
                new PropertyChangedCallback((d, e) =>
                {
                    var element = d as RangeBox;
                    if (element == null) return;
                    element.FlowEnterComponent = e.NewValue as ComponentBase;
                })));
        #endregion

        #region RangeEnd
        public ComponentBase RangeEnd
        {
            get { return (ComponentBase)GetValue(RangeEndProperty); }
            set { SetValue(RangeEndProperty, value); }
        }

        // Using a DependencyProperty as the backing store for RangeEnd.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty RangeEndProperty =
            DependencyProperty.Register("RangeEnd", typeof(ComponentBase), typeof(RangeBox), new PropertyMetadata(null,
                  new PropertyChangedCallback((d, e) =>
                  {
                      var element = d as RangeBox;
                      if (element == null) return;
                      element.FlowLeaveComponent = e.NewValue as ComponentBase;
                  })));
        #endregion
    }
}
