using Platform.Common.Configure;
using Platform.Common.Helper;
using Platform.Controls.Layout;
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
using Platform.Common.Extension;
using Platform.Controls.UI.Composite;

namespace Platform.Controls.UI
{
    /// <summary>
    /// 文本框
    /// </summary>
    public class TextInput : Core.TitleComponent
    {
        #region Ctor
        #region 兼容改造，将控件改造成非UserControl，方便后续的子类继承
        private void InitializeComponent()
        {
            if (_contentLoaded)
            {
                return;
            }
            _contentLoaded = true;
            this.Title = "文本域";
            var ib = new InputBase();
            this.Content = ib;
            bd = ib.bd;
            tb_Host = ib.tb_Host;
            tb_Host.TextChanged += tb_Host_TextChanged;
            host_ClearText = ib.host_ClearText;
            host_ClearText.MouseLeftButtonDown += ClearAllText;
        }
        private bool _contentLoaded;
        internal Border bd;
        internal TextBox tb_Host;
        internal Grid host_ClearText;
        Brush disableBrush = null;
        Brush notEmptyBrush = null;
        #endregion

        public TextInput()
        {
            InitializeComponent();
            this.IsKeyboardFocusWithinChanged += (sender, e) => { CheckWaterMark(); };
            this.IsEnabledChanged += (sender, e) =>
            {
                CheckCommonSetting();
            };
            this.IsReadOnlyChanged += (sender, e) =>
            {
                CheckCommonSetting();
            };
        }

        private void CheckCommonSetting()
        {
            if (disableBrush == null)
            {
                var rd = new ResourceDictionary();
                rd.Source = new Uri("/Platform.Controls;component/Themes/CommonResource.xaml", UriKind.RelativeOrAbsolute);
                notEmptyBrush = rd["commonNotEmpty"] as Brush;
                disableBrush = rd["commonDisableGray"] as Brush;
            }
            //背景色由IsEnabled决定
            if (!IsEnabled)
            {
                bd.Background = disableBrush;
            }
            else
            {
                if (AssertNotEmpty)
                {
                    bd.Background = notEmptyBrush;
                }
                else
                {
                    bd.Background = Brushes.White;
                }
            }
            //IsReadOnly属性控制输入
            tb_Host.IsReadOnly = IsReadOnly;
            //清除按钮的控制
            host_ClearText.Width = IsEnabled && !IsReadOnly ? 20 : 0;
        }

        private void tb_Host_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (IsShowTextLength)
                Suffix = GetCurrentLength();
            CheckWaterMark();
        }

        internal override bool OnFlowKeyDown(KeyEventArgs e)
        {
            return !(e.Key == Key.Left || e.Key == Key.Right);
        }

        internal void CheckWaterMark()
        {
            WaterMarkAdorner.SetIsWaterMarkHost(tb_Host, this.IsVisible && this.IsEnabled && this.IsKeyboardFocusWithin && tb_Host.Text.Length == 0);
        }

        /// <summary>
        /// 获取当前的字节长度
        /// </summary>
        /// <returns></returns>
        private int GetCurrentLength()
        {
            var len = PlatformSettings.Encoding.GetByteCount(tb_Host.Text);
            //如果自动转全角，算法更换
            if (IsToSBC)
                len = tb_Host.Text.Length * 2;
            return len;
        }

        internal void SetTextBoxFont(double fontSize, FontWeight fontWeight)
        {
            this.tb_Host.FontSize = fontSize;
            this.tb_Host.FontWeight = fontWeight;
        }
        #endregion

        #region Prop
        /// <summary>
        /// 获取或设置元素的值
        /// </summary>
        public override string Value
        {
            get { return tb_Host.Text; }
            set { tb_Host.Text = value; }
        }

        /// <summary>
        /// 获取元素的打印输出
        /// </summary>
        public override string Text
        {
            get
            {
                return tb_Host.Text;
            }
        }

        /// <summary>
        /// 是否使用文本自动换行
        /// </summary>
        public bool IsTextWrapping
        {
            get { return tb_Host.TextWrapping == TextWrapping.Wrap; }
            set { tb_Host.TextWrapping = value ? TextWrapping.Wrap : TextWrapping.NoWrap; }
        }
        #endregion

        #region IsShowTextLength - 是否显示输入文本长度
        public bool IsShowTextLength
        {
            get { return (bool)GetValue(IsShowTextLengthProperty); }
            set { SetValue(IsShowTextLengthProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsShowTextLength.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsShowTextLengthProperty =
            DependencyProperty.Register("IsShowTextLength", typeof(bool), typeof(TextInput), new PropertyMetadata(false));
        #endregion

        #region WaterMark - 输入水印
        public string WaterMark
        {
            get { return (string)GetValue(WaterMarkProperty); }
            set { SetValue(WaterMarkProperty, value); }
        }

        // Using a DependencyProperty as the backing store for WaterMark.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty WaterMarkProperty =
            DependencyProperty.Register("WaterMark", typeof(string), typeof(TextInput), new PropertyMetadata("请输入..."));
        #endregion

        #region IsToUpper - 自动转大写
        public bool IsToUpper
        {
            get { return (bool)GetValue(IsToUpperProperty); }
            set { SetValue(IsToUpperProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsToUpper.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsToUpperProperty =
            DependencyProperty.Register("IsToUpper", typeof(bool), typeof(TextInput), new PropertyMetadata(false,
                new PropertyChangedCallback((d, e) =>
                {
                    var element = d as TextInput;
                    if (element == null) return;
                    element.tb_Host.CharacterCasing = CharacterCasing.Normal;
                    if ((bool)e.NewValue)
                        element.tb_Host.CharacterCasing = CharacterCasing.Upper;
                })));
        #endregion

        #region IsToSBC - 自动半角转换全角
        public bool IsToSBC
        {
            get { return (bool)GetValue(IsToSBCProperty); }
            set { SetValue(IsToSBCProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsToSBC.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsToSBCProperty =
            DependencyProperty.Register("IsToSBC", typeof(bool), typeof(TextInput), new PropertyMetadata(false,
                new PropertyChangedCallback((d, e) =>
                {
                    var element = d as TextInput;
                    if (element == null) return;
                    element.PreviewFlowLeave -= IsToSBC_PreviewFlowLeave;
                    if ((bool)e.NewValue)
                        element.PreviewFlowLeave += IsToSBC_PreviewFlowLeave;
                })));

        private static void IsToSBC_PreviewFlowLeave(object sender, RoutedEventArgs e)
        {
            var element = sender as TextInput;
            if (element == null) return;
            var textBox = element.tb_Host;
            // 半角转全角
            char[] c = textBox.Text.ToCharArray();
            for (int i = 0; i < c.Length; i++)
            {
                if (c[i] == 32)
                {
                    c[i] = (char)12288;
                    continue;
                }
                if (c[i] < 127)
                {
                    c[i] = (char)(c[i] + 65248);
                }
            }
            textBox.Text = new string(c);
            textBox.CaretIndex = textBox.Text.Length;
        }
        #endregion

        #region IsRepeatInput - 是否启用重复输入控制
        public bool IsRepeatInput
        {
            get { return (bool)GetValue(IsRepeatInputProperty); }
            set { SetValue(IsRepeatInputProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsRepeatInput.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsRepeatInputProperty =
            DependencyProperty.Register("IsRepeatInput", typeof(bool), typeof(TextInput), new PropertyMetadata(false,
                new PropertyChangedCallback((d, e) =>
                {
                    var element = d as TextInput;
                    if (element == null) return;
                    element.PreviewFlowEnter -= IsRepeatInput_PreviewFlowEnter;
                    element.PreviewFlowEnter += IsRepeatInput_PreviewFlowEnter;
                    element.PreviewFlowLeave -= IsRepeatInput_PreviewFlowLeave;
                    if ((bool)e.NewValue)
                        element.PreviewFlowLeave += IsRepeatInput_PreviewFlowLeave;
                })));

        private static void IsRepeatInput_PreviewFlowEnter(object sender, RoutedEventArgs e)
        {
            var element = sender as TextInput;
            if (element == null) return;
            element._cacheInput = null;
        }

        private string _cacheInput;

        private static void IsRepeatInput_PreviewFlowLeave(object sender, RoutedEventArgs e)
        {
            var element = sender as TextInput;
            if (element == null) return;
            var text = element.Value;
            if (element._cacheInput == null)
            {
                element._cacheInput = text;
                element.ToastMessage("请再输入一遍...");
                element.Value = "";
                e.Handled = true;
            }
            else if (text != element._cacheInput)
            {
                element._cacheInput = null;
                element.ToastMessage("两次输入的值不一致，请重新输入...");
                element.Value = "";
                e.Handled = true;
            }
            else
                element._cacheInput = null;
        }
        #endregion

        #region InputRule - 输入值验证
        public string InputRule
        {
            get { return (string)GetValue(InputRuleProperty); }
            set { SetValue(InputRuleProperty, value); }
        }

        // Using a DependencyProperty as the backing store for InputRule.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty InputRuleProperty =
            DependencyProperty.Register("InputRule", typeof(string), typeof(TextInput), new PropertyMetadata(null,
                new PropertyChangedCallback((d, e) =>
                {
                    var element = d as TextInput;
                    if (element == null) return;
                    element.PreviewFlowLeave -= InputRule_PreviewFlowLeave;
                    if (!string.IsNullOrEmpty((string)e.NewValue))
                        element.PreviewFlowLeave += InputRule_PreviewFlowLeave;
                })));

        private static void InputRule_PreviewFlowLeave(object sender, RoutedEventArgs e)
        {
            //throw new NotImplementedException();
            var element = sender as TextInput;
            if (element == null) return;
            var rule = element.InputRule;
            var text = element.tb_Host.Text;
            string error;
            if (!string.IsNullOrEmpty(text) && !RegexRuleHelper.Instance.DoRegex(rule, text, out error))
            {
                //验证失败
                element.ToastMessage("验证失败：" + error);
                element.tb_Host.SelectAll();
                e.Handled = true;
            }
        }
        #endregion


        #region AssertNotEmpty - 标识此输入场不允许为空
        public bool AssertNotEmpty
        {
            get { return (bool)GetValue(AssertNotEmptyProperty); }
            set { SetValue(AssertNotEmptyProperty, value); }
        }

        // Using a DependencyProperty as the backing store for AssertNotEmpty.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty AssertNotEmptyProperty =
            DependencyProperty.Register("AssertNotEmpty", typeof(bool), typeof(TextInput), new PropertyMetadata(false,
                new PropertyChangedCallback((d, e) =>
                {
                    var element = d as TextInput;
                    if (element == null) return;
                    var flag = (bool)e.NewValue;
                    element.CheckCommonSetting();
                    element.PreviewFlowLeave -= AssertNotEmpty_PreviewFlowLeave;
                    if (flag)
                        element.PreviewFlowLeave += AssertNotEmpty_PreviewFlowLeave;
                })));

        private static void AssertNotEmpty_PreviewFlowLeave(object sender, RoutedEventArgs e)
        {
            var element = sender as TextInput;
            if (element == null) return;
            if (string.IsNullOrWhiteSpace(element.tb_Host.Text))
            {
                element.ToastMessage("该输入项不允许为空！");
                element.tb_Host.SelectAll();
                e.Handled = true;
            }
        }
        #endregion

        #region ClearAllText
        private void ClearAllText(object sender, MouseButtonEventArgs e)
        {
            tb_Host.Clear();
            tb_Host.Focus();
            e.Handled = true;
        }
        #endregion

        #region Prefix - 前缀
        public object Prefix
        {
            get { return (object)GetValue(PrefixProperty); }
            set { SetValue(PrefixProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Prefix.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PrefixProperty =
            DependencyProperty.Register("Prefix", typeof(object), typeof(TextInput), new PropertyMetadata(null));
        #endregion

        #region Suffix - 后缀
        public object Suffix
        {
            get { return (object)GetValue(SuffixProperty); }
            set { SetValue(SuffixProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Suffix.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SuffixProperty =
            DependencyProperty.Register("Suffix", typeof(object), typeof(TextInput), new PropertyMetadata(null));
        #endregion

        #region MinLength - 最小长度
        public int MinLength
        {
            get { return (int)GetValue(MinLengthProperty); }
            set { SetValue(MinLengthProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MinLength.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MinLengthProperty =
            DependencyProperty.Register("MinLength", typeof(int), typeof(TextInput), new PropertyMetadata(0,
                new PropertyChangedCallback((d, e) =>
                {
                    var element = d as TextInput;
                    if (element == null) return;
                    var min = (int)e.NewValue;
                    element.PreviewFlowLeave -= MinLength_PreviewFlowLeave;
                    if (min > 0)
                        element.PreviewFlowLeave += MinLength_PreviewFlowLeave;
                })));

        private static void MinLength_PreviewFlowLeave(object sender, RoutedEventArgs e)
        {
            var element = sender as TextInput;
            if (element == null) return;
            var len = element.GetCurrentLength();
            if (len < element.MinLength)
            {
                element.ToastMessage("输入必须满足最小长度：" + element.MinLength);
                e.Handled = true;
            }
        }
        #endregion

        #region MaxLength - 最大长度
        public int MaxLength
        {
            get { return (int)GetValue(MaxLengthProperty); }
            set { SetValue(MaxLengthProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MaxLength.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MaxLengthProperty =
            DependencyProperty.Register("MaxLength", typeof(int), typeof(TextInput), new PropertyMetadata(int.MaxValue,
                 new PropertyChangedCallback((d, e) =>
                 {
                     var element = d as TextInput;
                     if (element == null) return;
                     var max = (int)e.NewValue;
                     element.tb_Host.MaxLength = max;
                     element.PreviewFlowLeave -= MaxLength_PreviewFlowLeave;
                     if (max != int.MaxValue && max > 0)
                         element.PreviewFlowLeave += MaxLength_PreviewFlowLeave;
                 })));

        private static void MaxLength_PreviewFlowLeave(object sender, RoutedEventArgs e)
        {
            var element = sender as TextInput;
            if (element == null) return;
            var len = element.GetCurrentLength();
            if (len > element.MaxLength)
            {
                element.ToastMessage("输入不能超过最大长度：" + element.MaxLength);
                e.Handled = true;
            }
        }
        #endregion
    }
}
