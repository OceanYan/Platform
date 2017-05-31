using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;
using System.Windows.Controls;
using System.Windows;
using System.Data;
using System.Windows.Input;
using System.Collections;
using System.Windows.Controls.Primitives;
using Platform.Controls.Core;
using Platform.Controls.Dialog;
using Platform.Print;
using Platform.Common.Base;
using Platform.Common.Configure;

namespace Platform.Widget.Core
{
    /// <summary>
    /// 业务API的访问服务类
    /// </summary>
    public class APIsHelper
    {
        #region ctor & Property
        public APIsHelper(PageBase page)
        {
            Page = page;
        }
        public PageBase Page { get; private set; }
        #endregion

        #region Methods
        /// <summary>
        /// 获取界面中指定name的流程元素值
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public string GetUI(string name)
        {
            var fm = FlowManager.GetManagerInstance(Page);
            if (fm != null)
            {
                var item = fm.FlowList.FirstOrDefault(x => x.Name == name);
                if (item != null)
                    return item.Value;
            }
            return string.Empty;
        }

        public string GetUIText(string name)
        {
            var fm = FlowManager.GetManagerInstance(Page);
            if (fm != null)
            {
                var item = fm.FlowList.FirstOrDefault(x => x.Name == name);
                if (item != null)
                    return item.Text;
            }
            return string.Empty;
        }

        public void SetFlow(ComponentBase component = null)
        {
            var fm = FlowManager.GetManagerInstance(Page);
            if (fm == null) return;
            fm.SetFlow(component);
            fm.IsContinue = false;
        }

        public Key ShowMessage(string message, string caption = "提示信息", Brush icon = null, List<Tuple<string, Key, Action>> btns = null)
        {
            var dialog = new ConfirmDialog();
            dialog.Caption = caption;
            dialog.Message = message;
            dialog.IconBrush = icon;
            if (btns != null)
                foreach (var btn in btns)
                {
                    dialog.AddButton(btn.Item1, btn.Item2, btn.Item3);
                }
            dialog.ShowDialog();
            return dialog.ResultKey;
        }

        public MessageBoxResult ShowMessage(string message, MessageBoxButton btns, string caption = "提示信息", Brush icon = null)
        {
            var ret = Key.None;
            switch (btns)
            {
                case MessageBoxButton.OK:
                    if (icon == null) icon = IconBox.GetIconBrush("\uf0eb", Brushes.Yellow);
                    ShowMessage(message, caption, icon, null);
                    return MessageBoxResult.OK;
                case MessageBoxButton.OKCancel:
                    if (icon == null) icon = IconBox.GetIconBrush("\uf29c", Brushes.SkyBlue);
                    ret = ShowMessage(message, caption, icon, new List<Tuple<string, Key, Action>>
                    {
                         new Tuple<string, Key, Action>("确定",Key.O,null),
                         new Tuple<string, Key, Action>("取消",Key.C,null)
                    });
                    return ret == Key.O ? MessageBoxResult.OK : MessageBoxResult.Cancel;
                case MessageBoxButton.YesNoCancel:
                    if (icon == null) icon = IconBox.GetIconBrush("\uf29c", Brushes.SkyBlue);
                    ret = ShowMessage(message, caption, icon, new List<Tuple<string, Key, Action>>
                    {
                         new Tuple<string, Key, Action>("是",Key.Y,null),
                         new Tuple<string, Key, Action>("否",Key.N,null),
                         new Tuple<string, Key, Action>("取消",Key.C,null)
                    });
                    return ret == Key.Y ? MessageBoxResult.Yes : ret == Key.N ? MessageBoxResult.No : MessageBoxResult.Cancel;
                case MessageBoxButton.YesNo:
                    if (icon == null) icon = IconBox.GetIconBrush("\uf29c", Brushes.SkyBlue);
                    ret = ShowMessage(message, caption, icon, new List<Tuple<string, Key, Action>>
                    {
                         new Tuple<string, Key, Action>("是",Key.Y,null),
                         new Tuple<string, Key, Action>("否",Key.N,null)
                    });
                    return ret == Key.Y ? MessageBoxResult.Yes : MessageBoxResult.No;
                default:
                    throw new NotSupportedException("NotSupportedException！");
            }
        }

        public bool ShowPage(PageBase page, string title = "弹窗信息", Action callback = null)
        {
            var dialog = new PageDialog();
            dialog.Title = title;
            dialog.Content = page;
            //传递总线对象
            if (Page is Contract.ContractPage && page is Contract.ContractPage)
                (page as Contract.ContractPage).SetDataBus((Page as Contract.ContractPage).DataBus);
            page.PageClosed += (sender, e) =>
            {
                dialog.Close();
                callback?.Invoke();
            };
            return dialog.ShowDialog() ?? false;
        }

        /// <summary>
        /// 进度条效果
        /// </summary>
        /// <param name="action"></param>
        /// <param name="canCancel">是否允许中断</param>
        /// <param name="message"></param>
        /// <returns></returns>
        public bool ProgressWork(Action action, bool canCancel = false, string message = "数据正在处理，请稍候...")
        {
            return ProgressDialog.ProgressWork(action, canCancel, message);
        }

        public void PrintCertificate(PrintContainer[] prints)
        {
            if (prints == null || prints.Length == 0) return;
            PrintManager.Instance.DoPrint(prints);
        }
    }
    #endregion
}
