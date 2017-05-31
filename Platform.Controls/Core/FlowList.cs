using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using Platform.Common.Extension;
using System.Windows.Media;
using System.Collections;

namespace Platform.Controls.Core
{
    /// <summary>
    /// 焦点列表
    /// </summary>
    public class FlowList : IEnumerable<ComponentBase>
    {
        #region base
        /// <summary>
        /// 初始化元素加载列表
        /// </summary>
        private readonly IList<ComponentBase> _internalList;
        /// <summary>
        /// 当前排序的根元素
        /// </summary>
        private readonly FrameworkElement _rootElement;
        private FlowList(FrameworkElement rootElement)
        {
            _internalList = new List<ComponentBase>();
            _rootElement = rootElement;
            Refresh();
        }
        #endregion

        #region sort
        /// <summary>
        /// 按位置比较
        /// </summary>
        /// <param name="targetElement">目标元素</param>
        /// <param name="currentElement">当前元素</param>
        /// <returns>目标元素位于当前元素之前返回true</returns>
        private bool CompareByLocation(FrameworkElement targetElement, FrameworkElement currentElement)
        {
            //先查看要比较顺序的控件外部是否有ScrollViewer
            Predicate<DependencyObject> handle = x => x is ScrollViewer;
            var targetScrollViewer = targetElement.FindVisualTreeAncestor(handle) as ScrollViewer;
            var currentScrollViewer = currentElement.FindVisualTreeAncestor(handle) as ScrollViewer;
            //如果两个都有并且不是同一个，看下两个ScrollViewer是否有父子关系
            if (targetScrollViewer != null && currentScrollViewer != null && targetScrollViewer != currentScrollViewer)
            {
                if (targetScrollViewer.IsAncestorOf(currentScrollViewer))
                    currentElement = currentScrollViewer;
                else
                    targetElement = targetScrollViewer;
            }
            else if (targetScrollViewer == null || currentScrollViewer == null || targetScrollViewer != currentScrollViewer)
            {
                if (targetScrollViewer != null) targetElement = targetScrollViewer;
                if (currentScrollViewer != null) currentElement = currentScrollViewer;
            }

            var locationPoint = new Point(0, 0);
            var targetElementOffset = targetElement.TranslatePoint(locationPoint, _rootElement);
            var currentElementOffset = currentElement.TranslatePoint(locationPoint, _rootElement);

            //目标元素上边线位于当前元素上边线之上
            if ((targetElementOffset.Y + targetElement.ActualHeight <= currentElementOffset.Y + currentElement.ActualHeight / 2))
                return true;

            //目标元素位于当前元素左边（按最左边线进行比较）并且目标元素上边线位于当前元素中线之上
            return (targetElementOffset.X <= currentElementOffset.X
                && targetElementOffset.Y <= currentElementOffset.Y + currentElement.ActualHeight / 2);
        }

        /// <summary>
        /// 计算过程中，被改变了Visibility属性的要素集合
        /// </summary>
        private Stack<FrameworkElement> _stackOperatorVisible = new Stack<FrameworkElement>(100);

        private void GetFlowChildren(FrameworkElement rootElement)
        {
            if (rootElement == null) return;

            //由于Visibility为Collapsed时，可视化树上没有子控件，需要强制显示后进行计算
            if (rootElement.Visibility == Visibility.Collapsed)
            {
                this._stackOperatorVisible.Push(rootElement);
                rootElement.Visibility = Visibility.Hidden;
                rootElement.UpdateLayout();
            }
            //遍历视觉树子项
            var childrenCount = VisualTreeHelper.GetChildrenCount(rootElement);
            //先遍历一次，让子元素都正常显示
            var flag = false;
            for (var i = 0; i < childrenCount; i++)
            {
                var child = VisualTreeHelper.GetChild(rootElement, i) as FrameworkElement;
                if (child == null) continue;
                //如果容器中的控件隐藏，则会出现焦点顺序错误的情况；
                if (child.Visibility == Visibility.Collapsed)
                {
                    this._stackOperatorVisible.Push(child);
                    child.Visibility = Visibility.Visible;
                    flag = true;
                }
            }
            if (flag) rootElement.UpdateLayout();
            //再遍历一次，处理子元素
            for (var i = 0; i < childrenCount; i++)
            {
                var child = VisualTreeHelper.GetChild(rootElement, i) as FrameworkElement;
                if (child == null) continue;
                //如果元素不需要纳入到流程控制，则继续递归
                if (child is ComponentBase)
                {
                    //特殊处理InternalFlowComponent,跳过
                    if (child is InternalFlowComponent)
                        GetFlowChildren(child);
                    else
                        _internalList.Insert(GetInsertIndex(child), child as ComponentBase);
                }
                else
                    GetFlowChildren(child);
            }
        }

        private int GetInsertIndex(FrameworkElement targetElement)
        {
            int currentIndex;
            int left = 0, right = _internalList.Count - 1;
            while (left <= right)
            {
                currentIndex = (left + right) / 2;
                FrameworkElement tmpElement = _internalList[currentIndex];
                if (CompareByLocation(targetElement, tmpElement))
                    right = currentIndex - 1;
                else
                    left = currentIndex + 1;
            }
            return left;
        }
        #endregion

        #region public
        public static FlowList Create(FrameworkElement rootElement)
        {
            return new FlowList(rootElement);
        }

        /// <summary>
        /// 刷新焦点列表,如果在运行中修改界面控件位置或者添加新控件,请开发人员手动调用此方法刷新
        /// </summary>
        public void Refresh()
        {
            //只有在根结点元素已经加载和呈现的情况下，刷新操作才进行
            if (_rootElement.IsVisible && _rootElement.IsLoaded)
            {
                //清理缓存
                _stackOperatorVisible.Clear();
                _internalList.Clear();
                //进行计算排序
                GetFlowChildren(_rootElement);
                //完成后再次设置回来
                foreach (FrameworkElement current in _stackOperatorVisible)
                    current.Visibility = Visibility.Collapsed;
            }
        }

        public int Count
        {
            get { return _internalList.Count; }
        }

        public ComponentBase this[int i]
        {
            get { return _internalList[i]; }
        }

        public int IndexOf(ComponentBase element)
        {
            return _internalList.IndexOf(element);
        }

        public IEnumerator<ComponentBase> GetEnumerator()
        {
            return _internalList.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _internalList.GetEnumerator();
        }
        #endregion
    }
}
