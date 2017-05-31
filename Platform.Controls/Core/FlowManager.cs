using Platform.Common.Extension;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;

namespace Platform.Controls.Core
{
    public class FlowManager : DependencyObject
    {
        #region 属性 
        /// <summary>
        /// 焦点控制控件列表
        /// </summary> 
        public FlowList FlowList { get; private set; }

        /// <summary>
        /// 焦点控制的根元素
        /// </summary>
        private readonly FrameworkElement _target;

        /// <summary>
        /// 当前焦点元素
        /// </summary>
        public ComponentBase CurrentElement
        {
            get { return (ComponentBase)GetValue(CurrentElementProperty); }
            private set { SetValue(CurrentElementProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CurrentElement.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CurrentElementProperty =
            DependencyProperty.Register("CurrentElement", typeof(ComponentBase), typeof(FlowManager),
                new PropertyMetadata(null, new PropertyChangedCallback((d, e) =>
            {
                //当元素发生变化时，赋予新元素键盘焦点
                var oldValue = e.OldValue as ComponentBase;
                var newValue = e.NewValue as ComponentBase;
                if (newValue != null) newValue.SetFlowKeyboardFocus();
                if (oldValue != null) oldValue.RaiseEvent(new RoutedEventArgs(ComponentBase.UnflowedEvent));
            })));

        /// <summary>
        /// 标识此时能否接受用户输入
        /// </summary>
        public bool CanAcceptUserInput { get; set; }

        /// <summary>
        /// 是否继续流程执行
        /// </summary>
        public bool IsContinue { get; set; }

        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="target"></param>
        private FlowManager(FrameworkElement target)
        {
            _target = target;
            CanAcceptUserInput = true;
        }
        #endregion

        #region 附加属性集合 
        #region IsFlowScope-是否启用流程控制
        /// <summary>
        /// 是否焦点控制
        /// </summary>
        public static readonly DependencyProperty IsFlowScopeProperty =
            DependencyProperty.RegisterAttached("IsFlowScope", typeof(bool), typeof(FlowManager),
            new PropertyMetadata((bool)false, new PropertyChangedCallback((d, e) =>
           {
               var element = d as FrameworkElement;
               if (element == null) return;
               var instance = GetManagerInstance(d);
               if (instance == null)
               {
                   instance = new FlowManager(element);
                   SetManagerInstance(d, instance);
               }
               //注册/卸载 焦点控制行为
               if ((bool)e.NewValue)
                   instance.Registe();
               else
                   instance.UnRegiste();
           })));

        public static bool GetIsFlowScope(DependencyObject obj)
        {
            return (bool)obj.GetValue(IsFlowScopeProperty);
        }

        public static void SetIsFlowScope(DependencyObject obj, bool value)
        {
            obj.SetValue(IsFlowScopeProperty, value);
        }
        #endregion

        #region CanSkipFlow - 是否允许跳过流程控制
        public static bool GetCanSkipFlow(DependencyObject obj)
        {
            return (bool)obj.GetValue(CanSkipFlowProperty);
        }

        public static void SetCanSkipFlow(DependencyObject obj, bool value)
        {
            obj.SetValue(CanSkipFlowProperty, value);
        }

        // Using a DependencyProperty as the backing store for CanSkipFlow.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CanSkipFlowProperty =
            DependencyProperty.RegisterAttached("CanSkipFlow", typeof(bool), typeof(FlowManager), new PropertyMetadata(false));
        #endregion

        #region FlowMouseDownEvent - 路由事件:焦点外鼠标按下
        /// <summary>
        ///  路由事件:焦点外鼠标按下
        /// </summary>
        public static readonly RoutedEvent FlowMouseDownEvent = EventManager.RegisterRoutedEvent("FlowMouseDown",
            RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(FlowManager));

        /// <summary>
        /// 焦点外鼠标按下事件
        /// </summary>
        public event RoutedEventHandler FlowMouseDown
        {
            add { this.AddHandler(FlowMouseDownEvent, value); }
            remove { this.RemoveHandler(FlowMouseDownEvent, value); }
        }
        #endregion

        #region FlowManager-实例对象
        /// <summary>
        /// FlowManager的实例
        /// </summary>
        private static readonly DependencyPropertyKey ManagerInstancePropertyKey
            = DependencyProperty.RegisterAttachedReadOnly("ManagerInstance", typeof(FlowManager), typeof(FlowManager),
                new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.Inherits));

        public static readonly DependencyProperty ManagerInstanceProperty
            = ManagerInstancePropertyKey.DependencyProperty;

        public static FlowManager GetManagerInstance(DependencyObject d)
        {
            return (FlowManager)d.GetValue(ManagerInstanceProperty);
        }

        internal static void SetManagerInstance(DependencyObject d, FlowManager value)
        {
            d.SetValue(ManagerInstancePropertyKey, value);
        }
        #endregion
        #endregion

        #region 注册/取消组册
        /// <summary>
        /// 创建输入场列表，并注册焦点控制的三个入口点
        /// </summary>
        private void Registe()
        {
            _target.Loaded -= _target_Ready;
            //如果已经加载，直接调用初始化
            if (!_target.IsLoaded)
                _target.Loaded += _target_Ready;
            else
                _target_Ready(_target, new EventArgs());
            _target.MouseEnter -= OnMouseEnter;
            _target.MouseEnter += OnMouseEnter;
            _target.PreviewKeyDown -= OnPreviewKeyDown;
            _target.PreviewKeyDown += OnPreviewKeyDown;
            _target.PreviewMouseDown -= OnPreviewMouseDown;
            _target.PreviewMouseDown += OnPreviewMouseDown;
        }

        private void OnMouseEnter(object sender, MouseEventArgs e)
        {
            //throw new NotImplementedException();
            if (CurrentElement != null && !CurrentElement.IsKeyboardFocusWithin)
                CurrentElement.SetFlowKeyboardFocus();
        }

        /// <summary>
        /// 清理操作
        /// </summary>
        private void UnRegiste()
        {
            _target.MouseEnter -= OnMouseEnter;
            _target.PreviewKeyDown -= OnPreviewKeyDown;
            _target.PreviewMouseDown -= OnPreviewMouseDown;
            SetManagerInstance(_target, null);
        }

        void _target_Ready(object sender, EventArgs e)
        {
            _target.Loaded -= _target_Ready;
            //检查上层是否存在开启焦点的控件，防止嵌套
            if (_target.FindVisualTreeAncestor(x => FlowManager.GetIsFlowScope(x)) != null)
                throw new NotSupportedException("上层已经开启焦点流程控制，系统不支持嵌套使用！目标:" + _target);
            //如果区域的背景为空，会导致鼠标点击事件的穿透，影响控制效果
            if (_target.GetValue(Control.BackgroundProperty) == null)
            {
                _target.SetValue(Control.BackgroundProperty, Brushes.Transparent);
            }
            FlowList = FlowList.Create(_target);
            //foreach (var item in FlowList)
            //{
            //    SetManagerInstance(item, this);
            //}
            //如果已经设置好焦点控件，则无须重置
            if (CurrentElement == null)
                this.MoveFocusHost(FlowNavigation.Top);
        }
        #endregion

        #region 触发事件
        /// <summary>
        /// 触发路由事件，与FocusManager相关
        /// </summary>
        /// <param name="target"></param>
        /// <param name="routedEvent"></param>
        /// <returns></returns>
        bool RaiseFocusManagerEvent(DependencyObject target, RoutedEvent routedEvent)
        {
            if (target == null) return false;
            IsContinue = true;
            var args = new RoutedEventArgs { RoutedEvent = routedEvent };
            target.RaiseEvent(args);
            //若事件标志已经处理，则也认为IsContinue =false
            if (args.Handled == true)
                IsContinue = false;
            //执行完毕后，将IsContinue重置为false，故可以用将IsContinue认定是否执行事件的流程过程
            var flag = IsContinue;
            IsContinue = false;
            return flag;
        }

        /// <summary>
        /// 执行焦点进入事件
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        bool RaiseEnterEvent(DependencyObject target)
        {
            return RaiseFocusManagerEvent(target, ComponentBase.PreviewFlowEnterEvent) &&
                   RaiseFocusManagerEvent(target, ComponentBase.FlowEnterEvent);
        }

        /// <summary>
        /// 执行焦点离开事件
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        bool RaiseLeaveEvent(FrameworkElement target)
        {
            return RaiseFocusManagerEvent(target, ComponentBase.PreviewFlowLeaveEvent) &&
                   RaiseFocusManagerEvent(target, ComponentBase.FlowLeaveEvent);
        }
        #endregion

        #region 设置焦点-对外公开的方法
        /// <summary>
        /// 聚焦第一个可获得焦点的控件
        /// </summary>
        /// <param name="Instance"></param>
        public void ReSetFlow()
        {
            this.FlowList.Refresh();
            this.MoveFocusHost(FlowNavigation.Top);
        }

        /// <summary>
        /// 设置焦点，若参数为null，则保持当前焦点位置
        /// </summary>
        /// <param name="target">设置的元素</param>
        public void SetFlow(ComponentBase target)
        {
            if (target == null) return;
            target.UIInvoke(new Action(() =>
            {
                if (CurrentElement != null && target == CurrentElement) return;
                //若传入要素的焦点控制器与当前的不一致，则不执行跳转
                var fm = FlowManager.GetManagerInstance(target);
                if (fm != this) return;
                //直接跳转到目标元素
                if (IsFlowable(target))
                {
                    RaiseEnterEvent(target);
                    CurrentElement = target;
                }
                else if (CurrentElement == null)
                    ReSetFlow();
            }));
        }
        #endregion

        #region 鼠标和键盘操作
        /// <summary>
        /// 鼠标操作
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnPreviewMouseDown(object sender, RoutedEventArgs e)
        {
            if (_target != sender) return;
            e.Handled = true;
            //如果外部已经禁用用户输入或内部正在处理，终结事件并返回
            if (!CanAcceptUserInput)
                return;
            #region 获取点击的元素
            var source = e.OriginalSource as FrameworkElement;
            var clickedElement = FlowList.FirstOrDefault(x => x == source || x.IsAncestorOf(source));
            if (clickedElement == null)
            {
                if (source != null)
                {
                    //若点击元素为滚动条内部，则需要支持继续路由
                    e.Handled = source.FindVisualTreeAncestor(x => x is ScrollBar) == null;
                    //触发路由事件FlowMouseDownEvent
                    source.RaiseEvent(new RoutedEventArgs { RoutedEvent = FlowMouseDownEvent });
                }
                return;
            }
            #endregion
            //进行移动操作
            FlowTo(clickedElement);
            //如果没移动或移动成功，则标志鼠标事件未处理
            if (CurrentElement == clickedElement)
                e.Handled = false;
            return;
        }

        /// <summary>
        /// 尝试焦点移动到目标元素
        /// </summary>
        /// <param name="target"></param>
        public void FlowTo(ComponentBase target)
        {
            //如果目标不在列表中，则不处理
            var targetIndex = FlowList.IndexOf(target);
            if (targetIndex < 0)
                return;
            //若当前要素为空，则重置到顶点
            var currentIndex = FlowList.IndexOf(CurrentElement);
            if (currentIndex < 0)
            {
                MoveFocusHost(FlowNavigation.Top);
                return;
            }
            //开始组织焦点移动 
            if (currentIndex > targetIndex)
            {
                //尝试往目标进行转移
                SetFlow(target);
            }
            else if (currentIndex < targetIndex)
            {
                //检查是否允许跳过控制
                if (GetCanSkipFlow(target))
                    SetFlow(target);
                else
                {
                    while (true)
                    {
                        if (!MoveFocusHost(FlowNavigation.Forward))
                            break;
                        var index = FlowList.IndexOf(CurrentElement);
                        if (targetIndex <= index)
                            break;
                    }
                }
            }
        }

        /// <summary>
        /// 键盘操作
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnPreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (_target != sender) return;
            e.Handled = true;
            //如果外部已经禁用用户输入或内部正在处理，终结事件并返回
            if (!CanAcceptUserInput || CurrentElement == null)
                return;
            #region 输入检查
            //控件内部需求检查
            e.Handled = false;
            //返回值true:继续往下执行，false:停止后续步骤，注意e.Handled决定是否继续内部传递
            if (!CurrentElement.OnFlowKeyDown(e))
                return;
            e.Handled = true;
            var pressKey = "";
            if (e.Key == Key.Enter || e.Key == Key.Down || e.Key == Key.Right || e.Key == Key.Tab)
                pressKey = "DOWN";
            else if (e.Key == Key.Up || e.Key == Key.Left)
                pressKey = "UP";
            #endregion
            if (pressKey == "DOWN")
                MoveFocusHost(FlowNavigation.Forward);
            else if (pressKey == "UP")
                MoveFocusHost(FlowNavigation.Backward);
            else
                e.Handled = false;
        }
        #endregion

        #region 焦点切换主方法
        /// <summary>
        /// 按照参数指定的方向进行一次移动
        /// </summary>
        public bool MoveFocusHost(FlowNavigation navigation)
        {
            //当前焦点元素的位置
            var currentIndex = FlowList.IndexOf(CurrentElement);
            switch (navigation)
            {
                case FlowNavigation.Top:
                    //清除当前焦点元素，然后重新往下移动
                    CurrentElement = null;
                    return MoveFocusHost(FlowNavigation.Forward);
                case FlowNavigation.Backward:
                    //若当前元素不存在，则移动到顶端 
                    if (currentIndex < 0)
                        return MoveFocusHost(FlowNavigation.Top);
                    //从当前位置倒序查找回退位置，若当前位置为0，则不移动
                    while (currentIndex > 0)
                    {
                        currentIndex--;
                        var target = FlowList[currentIndex];
                        if (IsFlowable(target))
                        {
                            //位置合适，进行移动，此时触发Enter系列事件
                            if (!RaiseEnterEvent(target))
                                return false;
                            CurrentElement = target;
                            return true;
                        }
                    }
                    FlowToTop?.Invoke(this, new RoutedEventArgs());
                    return false;
                case FlowNavigation.Forward:
                    if (CurrentElement != null && !RaiseLeaveEvent(CurrentElement))
                        return false;
                    //沿着流程列表往下找焦点控件
                    while (currentIndex + 1 < FlowList.Count)
                    {
                        currentIndex++;
                        var target = FlowList[currentIndex];
                        if (IsFlowable(target))
                        {
                            if (!RaiseEnterEvent(target))
                                return false;
                            CurrentElement = target;
                            return true;
                        }
                    }
                    //触发移动到最后的事件
                    FlowToEnd?.Invoke(this, new RoutedEventArgs());
                    return false;
                default: return false;
            }
        }

        /// <summary>
        /// 移动到最顶端
        /// </summary>
        public event RoutedEventHandler FlowToTop;

        /// <summary>
        /// 移动到最末尾
        /// </summary>
        public event RoutedEventHandler FlowToEnd;

        /// <summary>
        /// 判断元素是否流程停驻
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        bool IsFlowable(ComponentBase element)
        {
            if (element == null) return false;
            return element.FindInternalFocusElement() != null;
        }
        #endregion
    }


    /// <summary>
    /// 焦点转移方向
    /// </summary>
    public enum FlowNavigation
    {
        Top,
        Backward,
        Forward
    }

    /// <summary>
    /// 焦点处理函数返回结果
    /// </summary>
    public enum FlowResult
    {
        Self,//自己处理
        Forward,//前进
        Backward,//后退
        System//系统处理
    }
}
