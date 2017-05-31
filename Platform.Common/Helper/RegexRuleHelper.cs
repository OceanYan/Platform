using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Platform.Common.Helper
{
    public class RegexRuleHelper
    {
        #region ctor
        private RegexRuleHelper()
        {
            _rules = new Dictionary<string, Tuple<string, string>>();
            //默认的验证规则
            AddRule("Date", @"^(?:(?:(?:(?:(?:1[6-9]|[2-9]\d)(?:0[48]|[2468][048]|[13579][26])|(?:(?:16|[2468][048]|[3579][26])00)))(?:0229))|(?:(?:(?:1[6-9]|[2-9]\d)\d{2})(?:(?:(?:0[13578]|1[02])31)|(?:(?:0[13-9]|1[0-2])(?:29|30))|(?:(?:0[1-9])|(?:1[0-2]))(?:0[1-9]|1\d|2[0-8]))))$", "日期格式20120101。");
            AddRule("Decimal", @"^[0-9]+\.{0,1}[0-9]{0,2}$", "请输入数字,格式000.00。");
            AddRule("Int", @"^\d+$", "请输入整数数字。");
            AddRule("Email", @"^\w+([-+.]\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*$", "请输入邮件地址类型。");
            AddRule("IDCard", @"^\d{17}[\d|x|X]$|^\d{15}$", "请输入有效身份证号。");
            AddRule("String", null, "");
        }

        /// <summary>
        /// 规则集合  Key-规则名称  Value.Item1-正则表达式   Value.Item2-错误提示
        /// </summary>
        private Dictionary<string, Tuple<string, string>> _rules;

        private static RegexRuleHelper _instance;

        /// <summary>
        /// 单例模式
        /// </summary>
        public static RegexRuleHelper Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new RegexRuleHelper();
                return _instance;
            }
        }
        #endregion

        #region Methods
        /// <summary>
        /// 添加指定的验证规则
        /// </summary>
        /// <param name="name"></param>
        /// <param name="regex"></param>
        /// <param name="errorMsg"></param>
        public void AddRule(string name, string regex, string errorMsg)
        {
            RemoveRule(name);
            _rules.Add(name, new Tuple<string, string>(regex, errorMsg));
        }

        /// <summary>
        /// 移除指定的验证规则
        /// </summary>
        /// <param name="name"></param>
        public void RemoveRule(string name)
        {
            if (_rules.ContainsKey(name))
                _rules.Remove(name);
        }

        /// <summary>
        /// 执行验证操作
        /// </summary>
        /// <param name="rule"></param>
        /// <param name="value"></param>
        /// <param name="error"></param>
        /// <returns></returns>
        public bool DoRegex(string rule, string value, out string error)
        {
            error = "不支持的验证规则：" + rule;
            if (_rules.ContainsKey(rule))
            {
                var data = _rules[rule];
                if (Regex.IsMatch(value, data.Item1))
                {
                    error = string.Empty;
                    return true;
                }
                error = data.Item2;
            }
            return false;
        }
        #endregion
    }
}
