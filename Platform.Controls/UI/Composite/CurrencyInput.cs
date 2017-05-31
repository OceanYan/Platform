using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using Platform.Common.Extension;

namespace Platform.Controls.UI.Composite
{
    public class CurrencyInput : NumericInput
    {
        #region Ctor
        public CurrencyInput()
        {
            Title = "金额域";
            Accuracy = 2;
            MinValue = 0;
            MaxValue = 99999999999999.99M;
            this.SetValue(Platform.Controls.UI.TextInput.PrefixProperty, "¥");
            this.SetTextBoxFont(22, FontWeights.Bold);
        }
        #endregion

        #region Prop
        #region IsShowUpperCNY - 是否显示大写中文后缀
        public bool IsShowUpperCNY
        {
            get { return (bool)GetValue(IsShowUpperCNYProperty); }
            set { SetValue(IsShowUpperCNYProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsShowUpperCNY.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsShowUpperCNYProperty =
            DependencyProperty.Register("IsShowUpperCNY", typeof(bool), typeof(CurrencyInput), new PropertyMetadata(false,
                new PropertyChangedCallback((d, e) =>
                {
                    var element = d as CurrencyInput;
                    var flag = (bool)e.NewValue;
                    if (flag)
                    {
                        element.ScaleSize = new Point(2, 1);
                        element.tb_Host.TextChanged += Tb_Host_TextChanged;
                    }
                    else
                    {
                        element.ScaleSize = new Point(1, 1);
                        element.tb_Host.TextChanged -= Tb_Host_TextChanged;
                    }
                })));

        private static void Tb_Host_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            var element = (sender as DependencyObject).FindVisualTreeAncestor(x => x is TextInput) as TextInput;
            if (element == null) return;
            decimal num;
            if (decimal.TryParse(element.Value, out num))
                element.Suffix = ChangeToCNY(num);
        }
        #endregion
        #endregion

        #region Method
        /// <summary>
        /// 将数字类型转换成大写金额字符串
        /// </summary>
        /// <param name="target"></param>
        /// <param name="num"></param>
        /// <returns></returns>
        public static string ChangeToCNY(decimal num)
        {
            string str1 = "零壹贰叁肆伍陆柒捌玖";            //0-9所对应的汉字
            string str2 = "万仟佰拾亿仟佰拾万仟佰拾元角分"; //数字位所对应的汉字
            string str3 = "";    //从原num值中取出的值
            string str4 = "";    //数字的字符串形式
            string str5 = "";  //人民币大写金额形式
            int i;    //循环变量
            int j;    //num的值乘以100的字符串长度
            string ch1 = "";    //数字的汉语读法
            string ch2 = "";    //数字位的汉字读法
            int nzero = 0;  //用来计算连续的零值是几个
            int temp;            //从原num值中取出的值

            num = Math.Round(Math.Abs(num), 2);    //将num取绝对值并四舍五入取2位小数
            str4 = ((long)(num * 100)).ToString();        //将num乘100并转换成字符串形式
            j = str4.Length;      //找出最高位
            if (j > 15)
                return string.Empty;

            str2 = str2.Substring(15 - j);   //取出对应位数的str2的值。如：200.55,j为5所以str2=佰拾元角分

            //循环取出每一位需要转换的值
            for (i = 0; i < j; i++)
            {
                str3 = str4.Substring(i, 1);          //取出需转换的某一位的值
                temp = Convert.ToInt32(str3);      //转换为数字
                if (i != (j - 3) && i != (j - 7) && i != (j - 11) && i != (j - 15))
                {
                    //当所取位数不为元、万、亿、万亿上的数字时
                    if (str3 == "0")
                    {
                        ch1 = "";
                        ch2 = "";
                        nzero = nzero + 1;
                    }
                    else
                    {
                        if (str3 != "0" && nzero != 0)
                        {
                            ch1 = "零" + str1.Substring(temp * 1, 1);
                            ch2 = str2.Substring(i, 1);
                            nzero = 0;
                        }
                        else
                        {
                            ch1 = str1.Substring(temp * 1, 1);
                            ch2 = str2.Substring(i, 1);
                            nzero = 0;
                        }
                    }
                }
                else
                {
                    //该位是万亿，亿，万，元位等关键位
                    if (str3 != "0" && nzero != 0)
                    {
                        ch1 = "零" + str1.Substring(temp * 1, 1);
                        ch2 = str2.Substring(i, 1);
                        nzero = 0;
                    }
                    else
                    {
                        if (str3 != "0" && nzero == 0)
                        {
                            ch1 = str1.Substring(temp * 1, 1);
                            ch2 = str2.Substring(i, 1);
                            nzero = 0;
                        }
                        else
                        {
                            if (str3 == "0" && nzero >= 3)
                            {
                                ch1 = "";
                                ch2 = "";
                                nzero = nzero + 1;
                            }
                            else
                            {
                                if (j >= 11)
                                {
                                    ch1 = "";
                                    nzero = nzero + 1;
                                    //修正bug 20150323
                                    //371508144.26
                                    //错误的："叁亿柒仟壹佰伍拾拾零捌仟壹佰肆拾肆元贰角陆分"
                                    //正确的："叁亿柒仟壹佰伍拾万零捌仟壹佰肆拾肆元贰角陆分"
                                    ch2 = str2.Substring(i, 1);
                                }
                                else
                                {
                                    ch1 = "";
                                    ch2 = str2.Substring(i, 1);
                                    nzero = nzero + 1;
                                }
                            }
                        }
                    }
                }
                if (i == (j - 11) || i == (j - 3) || (i == (i - 7) && nzero <= 3))
                {
                    nzero = 0;
                    //如果该位是亿位或元位，则必须写上
                    ch2 = str2.Substring(i, 1);
                }
                str5 = str5 + ch1 + ch2;

                if (i == j - 1 && str3 == "0")
                {
                    //最后一位（分）为0时，加上“整”
                    str5 = str5 + '整';
                }
            }
            if (num == 0)
            {
                // str5 = "零园整";
                str5 = "零元整";
            }
            return str5;
        }
        #endregion
    }
}
