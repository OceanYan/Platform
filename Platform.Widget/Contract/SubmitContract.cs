using System;
using Platform.Widget.Core;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Configuration;
using Platform.Common.Extension;
using Platform.Widget.Contract;

namespace Platform.Widget.Contract
{
    /// <summary>
    /// 提交契约
    /// </summary>
    public class SubmitContract : ContractBase
    {
        #region Ctor
        public SubmitContract(ContractPage page) : base(page)
        {
        }

        /// <summary>
        /// 打印契约
        /// </summary>
        public PrintContract PrintContract { get; protected set; }

        /// <summary>
        /// 通讯输入定义
        /// </summary>
        public ApplicationSettingsBase DataInput { get; protected set; }

        /// <summary>
        /// 通讯输出定义
        /// </summary>
        public ApplicationSettingsBase DataOutput { get; protected set; }
        #endregion

        #region Virtual
        /// <summary>
        /// 准备进行提交动作
        /// </summary>
        /// <returns></returns>
        protected virtual bool PreviewSubmitting()
        {
            _transitMessage.Clear();
            if (DataInput != null)
            {
                foreach (SettingsProperty item in DataInput.Properties)
                {
                    var value = (string)DataInput[item.Name];
                    //判定属性是否为全局应用，标识该要素需要 进行数据转换
                    var app = item.Attributes.Values.Cast<Attribute>().FirstOrDefault(x => x is ApplicationScopedSettingAttribute);
                    if (app != null)
                    {
                        if (!string.IsNullOrEmpty(value))
                        {
                            //解析数据来源
                            var temp = value.Split(':');
                            if (temp.Length != 2)
                                throw new ArgumentOutOfRangeException("SubmitContract:不支持的DataInput配置！" + value);
                            switch (temp[0])
                            {
                                case "ui"://界面要素
                                    _transitMessage.SetDictionaryItem(item.Name, this.APIs.GetUI(temp[1]));
                                    break;
                                case "bus"://总线要素
                                    _transitMessage.SetDictionaryItem(item.Name, (string)PageData.DataBus[(temp[1])]);
                                    break;
                                default:
                                    throw new ArgumentOutOfRangeException("SubmitContract:不支持的DataInput配置！" + value);
                            }
                        }
                        else
                            _transitMessage.SetDictionaryItem(item.Name, string.Empty);
                    }
                    else
                        _transitMessage.SetDictionaryItem(item.Name, value);
                }
                return true;
            }
            return false;
        }

        /// <summary>
        /// 提交完毕，并验证结果
        /// </summary>
        /// <returns></returns>
        protected virtual bool SubmitComplete()
        {
            //TODO:此处需要进行默认的通讯结果判定
            return true;
        }

        /// <summary>
        /// 流程处理结束，执行后续操作
        /// </summary>
        /// <returns></returns>
        protected virtual void Finally()
        {
            //默认关闭界面动作
            this.PageData.ClosePage();
        }
        #endregion

        #region Overwrite
        protected Dictionary<string, string> _transitMessage = new Dictionary<string, string>();

        /// <summary>
        /// 触发提交流程
        /// </summary>
        public override bool Raise()
        {
            if (!PreviewSubmitting())
            {
                //组装失败
                Console.WriteLine("SubmitContract.PreviewSubmitting失败！");
                return false;
            }
            //执行通讯动作
            if (!DoTransit())
                return false;
            //判定结果
            if (!SubmitComplete())
                return false;
            //触发打印流程
            if (PrintContract != null)
                PrintContract.Raise();
            //交易结束，调用收尾处理
            Finally();
            return true;
        }

        public virtual bool DoTransit()
        {
            return false;
        }
        #endregion
    }
}
