using Platform.Controls.Core;
using System;
using System.Collections;
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

namespace Platform.Controls.UI
{
    /// <summary>
    /// ComboBox.xaml 的交互逻辑
    /// </summary>
    public partial class ComboBox
    {
        public ComboBox()
        {
            InitializeComponent();
            this.PreviewFlowLeave += (sender, e) =>
            {
                if (AssertNotEmpty && cbb_Host.SelectedIndex == -1)
                {
                    this.ToastMessage("必须选择一项！");
                    e.Handled = true;
                }
            };
            this.cbb_Host.SelectionChanged += Cbb_Host_SelectionChanged;
            this.IsEnabledChanged += (sender, e) => { CheckCommonSetting(); };
            this.IsReadOnlyChanged += (sender, e) => { CheckCommonSetting(); };
        }

        private void CheckCommonSetting()
        {
            mask_ReadOnly.Visibility = IsEnabled && IsReadOnly ? Visibility.Visible : Visibility.Collapsed;
            cbb_Host.IsEnabled = !IsReadOnly && IsEnabled;
        }

        private void Cbb_Host_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var fm = FlowManager.GetManagerInstance(this);
            if (fm == null || fm.CurrentElement != this || fm.IsContinue) return;
            //延后执行，等效果呈现正确在进行移动
            this.Dispatcher.BeginInvoke(new Action(() =>
            {
                fm.MoveFocusHost(FlowNavigation.Forward);
            }), DispatcherPriority.Input);
        }

        internal override bool OnFlowKeyDown(KeyEventArgs e)
        {
            //只读情况下，直接交由后续处理
            if (IsReadOnly) return true;
            if (e.Key == Key.F2)
            {
                cbb_Host.IsDropDownOpen = !cbb_Host.IsDropDownOpen;
                e.Handled = true;
                return false;
            }
            else if ((e.Key == Key.Delete || e.Key == Key.Back) && !cbb_Host.IsDropDownOpen)
            {
                //处理删除选项
                cbb_Host.SelectedIndex = -1;
                e.Handled = true;
                return false;
            }
            if (cbb_Host.IsDropDownOpen && (e.Key == Key.Up || e.Key == Key.Down || e.Key == Key.Enter))
                return false;
            return base.OnFlowKeyDown(e);
        }

        #region prop  
        public void SetItems(Dictionary<string, string> dict, int index = 0)
        {
            cbb_Host.ItemsSource = dict;
            cbb_Host.SelectedValuePath = "Value";
            cbb_Host.SelectedIndex = index;
            cbb_Host.Items.Refresh();
        }

        public void SetItems(Dictionary<string, string> dict, string value)
        {
            if (dict == null) return;
            var index = -1;
            if (dict.ContainsValue(value))
                index = dict.Values.ToList().IndexOf(value);
            SetItems(dict, index);
        }

        /// <summary>
        /// 是否断言非空
        /// </summary>
        public bool AssertNotEmpty
        {
            get { return (bool)GetValue(AssertNotEmptyProperty); }
            set { SetValue(AssertNotEmptyProperty, value); }
        }

        // Using a DependencyProperty as the backing store for AssertNotEmpty.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty AssertNotEmptyProperty =
            DependencyProperty.Register("AssertNotEmpty", typeof(bool), typeof(ComboBox), new PropertyMetadata(true));

        /// <summary>
        /// 获取或设置元素的值
        /// </summary>
        public override string Value
        {
            get
            {
                if (cbb_Host.SelectedIndex < 0)
                    return string.Empty;
                return cbb_Host.SelectedValue.ToString();
            }
            set
            {
                if (!string.IsNullOrEmpty(value))
                {
                    //检查设定项的合法性
                    var dict = cbb_Host.ItemsSource as Dictionary<string, string>;
                    if (dict == null || !dict.ContainsValue(value))
                        throw new ArgumentOutOfRangeException("ComboBox:设定值不是选择范围中的有效值！[" + this.Title + "=" + value + "]");
                }
                cbb_Host.SelectedValue = value;
            }
        }

        /// <summary>
        /// 获取元素的打印输出
        /// </summary>
        public override string Text
        {
            get
            {
                return cbb_Host.SelectedItem == null ? string.Empty : ((KeyValuePair<string, string>)cbb_Host.SelectedItem).Key;
            }
        }
        #endregion 
    }
}
