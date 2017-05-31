using Platform.Controls.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;

namespace Platform.Controls.UI.Composite
{
    public class DateTimeInput : TextInput
    {
        #region Ctor
        public DateTimeInput()
        {
            Title = "日期时间域";
            //禁用输入法
            InputMethod.SetIsInputMethodEnabled(tb_Host, false);
            DateTimeFormatString = "yyyy年MM月dd日";
            MinValue = DateTime.MinValue;
            MaxValue = DateTime.MaxValue;
            //获得焦点时，全选
            GotFocus += (sender, e) => { tb_Host.SelectAll(); };
            PreviewKeyDown += DateTimeInput_PreviewKeyDown;
            this.PreviewFlowLeave += (sender, e) =>
            {
                var text = tb_Host.Text;
                if (string.IsNullOrEmpty(text)) return;
                var dt = GetDateTimeByString(tb_Host.Text, DateTimeFormatString);
                if (dt == null)
                {
                    this.ToastMessage("该输入项不是有效的日期时间！");
                    e.Handled = true;
                }
                else if (dt.Value.CompareTo(MinValue) < 0 || dt.Value.CompareTo(MaxValue) > 0)
                {
                    this.ToastMessage(string.Format("该输入项不在有效的范围内！要求的范围是[{0}]-[{1}]",
                        MinValue.ToString(DateTimeFormatString), MaxValue.ToString(DateTimeFormatString)));
                    e.Handled = true;
                }
            };
        }

        private Tuple<DateTime, int> _cache = new Tuple<DateTime, int>(DateTime.Now, 0);

        private void DateTimeInput_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            e.Handled = true;
            if (IsReadOnly || e.KeyboardDevice.Modifiers != ModifierKeys.None)
                return;
            //不允许删除
            if (e.Key == Key.Delete)
                return;
            if (e.Key == Key.Back)
            {
                var careIndex = tb_Host.CaretIndex;
                if (careIndex > 0)
                    tb_Host.CaretIndex = careIndex - 1;
                if (tb_Host.SelectionLength == tb_Host.Text.Length)
                    tb_Host.Text = "";
                return;
            }
            //允许调整输入位置
            if (e.Key == Key.Left || e.Key == Key.Right)
            {
                e.Handled = false;
                return;
            }
            var value = -1;
            if (e.Key >= Key.D0 && e.Key <= Key.D9)
                value = e.Key - Key.D0;
            else if (e.Key >= Key.NumPad0 && e.Key <= Key.NumPad9)
                value = e.Key - Key.NumPad0;
            //键入的值为数字
            if (value >= 0)
            {
                var careIndex = tb_Host.CaretIndex;
                var array = tb_Host.Text.ToArray();
                //若为空值，则采用最小值
                if (array.Length == 0) array = GetTextByFormat(MinValue).ToArray();
                if (careIndex < array.Length)
                {
                    //过滤分隔符的处理
                    while (array.Length > careIndex && IsSplitChar(array[careIndex]))
                        careIndex++;
                    if (careIndex == array.Length) return;
                    array[careIndex] = char.Parse(value.ToString());
                    var newText = new string(array);
                    //检测验证
                    //1.如果是最后一位
                    if (array.Length == careIndex + 1)
                    {
                        //验证结果不通过，视为无效输入
                        if (GetDateTimeByString(newText, DateTimeFormatString) == null)
                        {
                            this.ToastMessage("该输入项不是有效的日期时间！");
                            return;
                        }
                    }
                    else if (IsSplitChar(array[careIndex + 1]))
                    {
                        var dt = GetDateTimeByString(newText, DateTimeFormatString);
                        //如果验证不通过，则截取有效位再验证一次
                        if (dt == null)
                        {
                            dt = GetDateTimeByString(newText.Substring(0, careIndex + 1), DateTimeFormatString.Substring(0, careIndex + 1));
                            if (dt == null)
                            {
                                this.ToastMessage("该输入项不是有效的日期时间！");
                                return;
                            }
                            newText = dt.Value.ToString(DateTimeFormatString);
                        }
                    }
                    this.ToastMessage(null);
                    tb_Host.Text = newText;
                    careIndex++;
                    while (array.Length > careIndex && IsSplitChar(array[careIndex]))
                        careIndex++;
                    tb_Host.CaretIndex = careIndex;
                    tb_Host.Select(careIndex, 1);
                }
            }
        }

        /// <summary>
        /// 获取格式化后的数据文本
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public string GetTextByFormat(DateTime date)
        {
            return date.ToString(DateTimeFormatString);
        }

        /// <summary>
        /// 是否为规定的分隔符
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
        private bool IsSplitChar(char c)
        {
            return SplitChars.Contains(c);// c == '/' || c == ' ' || c == ':';
        }

        /// <summary>
        /// 将指定格式的日期字符串转换成对应的DateTime对象
        /// </summary>
        /// <param name="s"></param>
        /// <param name="format"></param>
        /// <returns></returns>
        public static DateTime? GetDateTimeByString(string s, string format)
        {
            DateTime dt;
            if (DateTime.TryParseExact(s, format, null, System.Globalization.DateTimeStyles.None, out dt))
                return dt;
            return null;
        }
        #endregion

        #region Prop
        /// <summary>
        /// 输入的格式化串
        /// </summary>
        private string _dateTimeFormatString;

        public string DateTimeFormatString
        {
            get { return _dateTimeFormatString; }
            set
            {
                _dateTimeFormatString = value;
                SplitChars = new string(_dateTimeFormatString.Where(x => !_resveredString.Contains(x)).ToArray());
                WaterMark = _dateTimeFormatString;
            }
        }

        private const string _resveredString = "yMdHhms";

        /// <summary>
        /// 定义的分隔符
        /// </summary>
        public string SplitChars { get; private set; }

        /// <summary>
        /// 最小值
        /// </summary>
        public DateTime MinValue { get; set; }

        /// <summary>
        /// 最大值
        /// </summary>
        public DateTime MaxValue { get; set; }
        #endregion

        #region Method
        public override int CompareTo(ComponentBase c)
        {
            var compare = c as DateTimeInput;
            if (compare == null)
                throw new ArgumentOutOfRangeException("参数必须为DateTimeInput类型！");
            if (!CurrentDateTime.HasValue || !compare.CurrentDateTime.HasValue)
                throw new ArgumentOutOfRangeException("存在值为空的情况，无法进行比较！");
            return this.CurrentDateTime.Value.CompareTo(compare.CurrentDateTime.Value);
        }

        public override string Value
        {
            get
            {
                return new string(tb_Host.Text.Where(
                    x => !SplitChars.Contains(x)).ToArray());
            }
            set
            {
                //构建纯有效数据的格式化串
                var format = new string(DateTimeFormatString.Where(x => !SplitChars.Contains(x)).ToArray());
                var datetime = GetDateTimeByString(value, format);
                if (datetime == null && !string.IsNullOrEmpty(value))
                    throw new ArgumentException("DateTimeInput.SetValue：无效的赋值！[" + value + "]");
                CurrentDateTime = datetime;
            }
        }

        /// <summary>
        /// 当前输入的日期对象
        /// </summary>
        public DateTime? CurrentDateTime
        {
            get
            {
                if (string.IsNullOrEmpty(tb_Host.Text))
                    return null;
                return GetDateTimeByString(tb_Host.Text, DateTimeFormatString);
            }
            set
            {
                if (value.HasValue && (value > MaxValue || value < MinValue))
                    throw new ArgumentOutOfRangeException("DateTimeInput.SetCurrentDateTime发生异常：超出设定的范围！[" + this.Title + "=" + value + "]");
                tb_Host.Text = value == null ? "" : value.Value.ToString(DateTimeFormatString);
            }
        }
        #endregion
    }
}
