using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Platform.Common.Extension;
using System.Windows.Documents;
using System.Windows.Media;
using Platform.Controls.Layout;

namespace Platform.Controls.Core
{
    [DesignTimeVisible(false)]
    public class ComponentBase : ContentControl
    {
        #region Ctor 
        public ComponentBase()
        {
            this.Focusable = false;
            //默认开启瞄准器
            AnchorAdorner.SetIsKeyboardFocusWithinAnchor(this, true);
            //当焦点失去时，需要清空提示信息
            this.Unflowed += (sender, e) => { this.ToastMessage(null); };
        }
        #endregion

        #region FlowManager支持 
        #region Function
        /// <summary>
        /// 设置键盘焦点的方案
        /// </summary>
        internal virtual void SetFlowKeyboardFocus()
        {
            var focus = FindInternalFocusElement();
            if (focus != null) focus.Focus();
        }

        internal UIElement FindInternalFocusElement()
        {
            return this.FirstVisualTreeChildren(x =>
            {
                var item = x as FrameworkElement;
                if (item == null) return false;
                return item.Focusable && item.InputHitTest(new Point(item.ActualWidth / 2, item.ActualHeight / 2)) != null;
            }) as UIElement;
        }

        /// <summary>
        /// 接收到流程控制的按键事件。
        /// 返回值true:继续往下执行，false:停止后续步骤，注意此时e.Handled标志事件是否已处理
        /// </summary>
        /// <param name="e"></param>
        /// <returns>是否继续进行控制</returns>
        internal virtual bool OnFlowKeyDown(KeyEventArgs e)
        {
            return true;
        }
        #endregion

        #region FlowEvent
        #region PreviewFlowEnter 
        /// <summary>
        /// PreviewFlowEnter，流程控制进入准备
        /// </summary>
        public static readonly RoutedEvent PreviewFlowEnterEvent = EventManager.RegisterRoutedEvent("PreviewFlowEnter",
            RoutingStrategy.Direct, typeof(RoutedEventHandler), typeof(ComponentBase));

        /// <summary>
        /// 流程控制进入准备
        /// </summary>
        public event RoutedEventHandler PreviewFlowEnter
        {
            add { AddHandler(PreviewFlowEnterEvent, value); }
            remove { RemoveHandler(PreviewFlowEnterEvent, value); }
        }
        #endregion

        #region PreviewFlowLeave 
        /// <summary>
        /// PreviewFlowLeave，流程控制离开准备
        /// </summary>
        public static readonly RoutedEvent PreviewFlowLeaveEvent = EventManager.RegisterRoutedEvent("PreviewFlowLeave",
            RoutingStrategy.Direct, typeof(RoutedEventHandler), typeof(ComponentBase));

        /// <summary>
        /// 流程控制离开准备
        /// </summary>
        public event RoutedEventHandler PreviewFlowLeave
        {
            add { AddHandler(PreviewFlowLeaveEvent, value); }
            remove { RemoveHandler(PreviewFlowLeaveEvent, value); }
        }
        #endregion

        #region FlowEnter 
        /// <summary>
        /// FlowEnter，流程控制进入
        /// </summary>
        public static readonly RoutedEvent FlowEnterEvent = EventManager.RegisterRoutedEvent("FlowEnter",
            RoutingStrategy.Direct, typeof(RoutedEventHandler), typeof(ComponentBase));

        /// <summary>
        /// 流程控制进入
        /// </summary>
        public event RoutedEventHandler FlowEnter
        {
            add { AddHandler(FlowEnterEvent, value); }
            remove { RemoveHandler(FlowEnterEvent, value); }
        }
        #endregion

        #region FlowLeave 
        /// <summary>
        /// FlowLeave，流程控制离开
        /// </summary>
        public static readonly RoutedEvent FlowLeaveEvent = EventManager.RegisterRoutedEvent("FlowLeave",
            RoutingStrategy.Direct, typeof(RoutedEventHandler), typeof(ComponentBase));

        /// <summary>
        /// 流程控制离开
        /// </summary>
        public event RoutedEventHandler FlowLeave
        {
            add { AddHandler(FlowLeaveEvent, value); }
            remove { RemoveHandler(FlowLeaveEvent, value); }
        }
        #endregion

        #region Unflowed 
        /// <summary>
        /// Unflowed，失去流程控制
        /// </summary>
        public static readonly RoutedEvent UnflowedEvent = EventManager.RegisterRoutedEvent("Unflowed",
            RoutingStrategy.Direct, typeof(RoutedEventHandler), typeof(ComponentBase));

        /// <summary>
        /// 失去流程控制
        /// </summary>
        public event RoutedEventHandler Unflowed
        {
            add { AddHandler(UnflowedEvent, value); }
            remove { RemoveHandler(UnflowedEvent, value); }
        }
        #endregion
        #endregion
        #endregion

        #region ToastMessage 提示信息
        /// <summary>
        /// 发出提示信息
        /// </summary>
        /// <param name="message"></param>
        public void ToastMessage(string message)
        {
            ToastAdorner.SetToastText(this, message);
        }
        #endregion

        #region Text/Value - 交付继承元素自行实现的属性，virtual
        /// <summary>
        /// 获取要素参与打印动作时的输出
        /// </summary>
        /// <returns></returns>
        public virtual string Text
        {
            get
            {
                throw new NotImplementedException("不支持ComponentBase.Text：" + this.GetType().FullName);
            }
        }

        /// <summary>
        /// 元素的Value值，由子元素各自实现
        /// </summary>
        public virtual string Value
        {
            get
            {
                throw new NotImplementedException("不支持ComponentBase.GetValue：" + this.GetType().FullName);
            }
            set
            {
                throw new NotImplementedException("不支持ComponentBase.SetValue：" + this.GetType().FullName);
            }
        }

        public virtual int CompareTo(ComponentBase c)
        {
            return 0;
        }
        #endregion

        #region IsReadOnly
        public event RoutedEventHandler IsReadOnlyChanged;

        public bool IsReadOnly
        {
            get { return (bool)GetValue(IsReadOnlyProperty); }
            set { SetValue(IsReadOnlyProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsReadOnly.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsReadOnlyProperty =
            DependencyProperty.Register("IsReadOnly", typeof(bool), typeof(ComponentBase), new PropertyMetadata(false,
                new PropertyChangedCallback((d, e) =>
                {
                    var element = d as ComponentBase;
                    if (element == null) return;
                    element.IsReadOnlyChanged?.Invoke(element, new RoutedEventArgs());
                })));
        #endregion
    }
}
