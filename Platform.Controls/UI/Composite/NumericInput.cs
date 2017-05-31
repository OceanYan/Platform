using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows.Input;
using Platform.Controls.Core;

namespace Platform.Controls.UI.Composite
{
    public class NumericInput : TextInput
    {
        #region Ctor
        public NumericInput()
        {
            Title = "数值域";
            //禁用输入法
            InputMethod.SetIsInputMethodEnabled(tb_Host, false);
            Accuracy = 2;
            MinValue = 0;
            MaxValue = decimal.MaxValue;
            //获得焦点时，全选
            GotFocus += (sender, e) =>
            {
                if (Value == "")
                    Value = GetTextByAccuracy(MinValue);
                tb_Host.SelectAll();
            };
            PreviewKeyDown += NumericInput_PreviewKeyDown;
            tb_Host.TextChanged += Tb_Host_TextChanged;
            this.PreviewFlowLeave += NumericInput_PreviewFlowLeave;
        }

        private void NumericInput_PreviewFlowLeave(object sender, System.Windows.RoutedEventArgs e)
        {
            //throw new NotImplementedException();
            if (AssertNotEmpty && CurrentNumeric == 0)
            {
                ToastMessage("必须输入有效的项！");
                e.Handled = true;
            }
        }

        /// <summary>
        /// 缓存上次录入的正确数据
        /// </summary>
        private Tuple<decimal, int> _cache = new Tuple<decimal, int>(0, 0);

        private void Tb_Host_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            tb_Host.TextChanged -= Tb_Host_TextChanged;
            //处理输入数据
            var text = tb_Host.Text.TrimStart('0');
            if (text == "")
                text = GetTextByAccuracy(MinValue);
            //尝试进行转换
            decimal d = 0;
            var careIndex = tb_Host.CaretIndex;
            //当首位0需要被清理掉时，注意光标位置-1
            if (tb_Host.Text.StartsWith("0") && careIndex > 0)
                careIndex--;
            if (decimal.TryParse(text, out d))
            {
                //阈值检查 
                ToastMessage(string.Empty);
                if (d >= MinValue && d <= MaxValue)
                    _cache = new Tuple<decimal, int>(d, careIndex);
                else
                    ToastMessage(string.Format("输入超出限制，要求的范围是[{0}]-[{1}]", MinValue, MaxValue));
            }
            careIndex = _cache.Item2;
            //计算小数点
            var newText = GetTextByAccuracy(_cache.Item1);
            if (tb_Host.Text != newText)
            {
                tb_Host.Text = newText;
                tb_Host.CaretIndex = careIndex;
                //若光标在首位，且首位数值为0，则光标修正为1
                if (tb_Host.Text.StartsWith("0.") && tb_Host.CaretIndex == 0)
                    tb_Host.CaretIndex = 1;
            }
            tb_Host.TextChanged += Tb_Host_TextChanged;
        }

        private void NumericInput_PreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.KeyboardDevice.Modifiers != ModifierKeys.None)
            {
                e.Handled = true;
                return;
            }
            if (e.Key == Key.Back)
            {
                var index = tb_Host.Text.IndexOf('.');
                if (tb_Host.CaretIndex == index + 1)
                    tb_Host.CaretIndex = index;
            }
            else if (e.Key == Key.Delete)
            {
                var index = tb_Host.Text.IndexOf('.');
                if (tb_Host.CaretIndex == index)
                    tb_Host.CaretIndex = index + 1;
            }
            else if (e.Key == Key.OemPeriod || e.Key == Key.Decimal)
            {
                var index = tb_Host.Text.IndexOf('.');
                if (tb_Host.CaretIndex == index)
                    tb_Host.CaretIndex = index + 1;
                e.Handled = true;
            }
        }

        private string GetTextByAccuracy(decimal d)
        {
            //考虑到四舍五入的问题，根据精度先处理d
            var pow = new decimal(Math.Pow(10, Accuracy));
            return (decimal.Truncate(d * pow) / pow).ToString("0.".PadRight(Accuracy + 2, '0'));
        }
        #endregion

        #region Prop
        /// <summary>
        /// 精确度
        /// </summary>
        public int Accuracy { get; set; }

        /// <summary>
        /// 最小值
        /// </summary>
        public decimal MinValue { get; set; }

        /// <summary>
        /// 最大值
        /// </summary>
        public decimal MaxValue { get; set; }
        #endregion

        #region Override
        public override string Value
        {
            get
            {
                return base.Value;
            }
            set
            {
                //检查范围
                decimal d;
                if (!decimal.TryParse(value, out d) || (d > MaxValue || d < MinValue))
                    throw new ArgumentOutOfRangeException("NumericInput.SetValue发生异常：设定值无效或不在范围内！[" + this.Title + "=" + value + "]");
                base.Value = value;
            }
        }

        public override int CompareTo(ComponentBase c)
        {
            var compare = c as NumericInput;
            if (compare == null)
                throw new ArgumentOutOfRangeException("参数必须为NumericInput类型！");
            var d = decimal.Parse(Value);
            var v = decimal.Parse(compare.Value);
            return d - v == 0 ? 0 : (d > v ? 1 : -1);
        }

        public decimal CurrentNumeric
        {
            get
            {
                return decimal.Parse(Value);
            }
            set
            {
                Value = value.ToString();
            }
        }
        #endregion
    }
}
