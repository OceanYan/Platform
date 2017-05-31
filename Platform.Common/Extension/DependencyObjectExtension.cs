using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;
using System.Windows.Input;

namespace Platform.Common.Extension
{
    /// <summary>
    /// 基于DependencyObject的扩展方法集合
    /// </summary>
    public static class DependencyObjectExtension
    {
        #region VisualTree相关
        /// <summary>
        /// 沿着视觉树查找符合条件的子元素集合
        /// </summary>
        /// <param name="target"></param>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public static List<DependencyObject> FindVisualTreeChildren(this DependencyObject target, Predicate<DependencyObject> predicate = null)
        {
            var result = new List<DependencyObject>();
            if (target != null)
            {
                var count = VisualTreeHelper.GetChildrenCount(target);
                for (int i = 0; i < count; i++)
                {
                    var element = VisualTreeHelper.GetChild(target, i);
                    if (element != null)
                    {
                        if (predicate == null || predicate(element)) result.Add(element);
                        result.AddRange(element.FindVisualTreeChildren(predicate));
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// 沿着视觉树查找符合条件的首个子元素
        /// </summary>
        /// <param name="target"></param>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public static DependencyObject FirstVisualTreeChildren(this DependencyObject target, Predicate<DependencyObject> predicate = null)
        {
            DependencyObject result = null;
            if (target != null)
            {
                var count = VisualTreeHelper.GetChildrenCount(target);
                for (int i = 0; i < count; i++)
                {
                    var element = VisualTreeHelper.GetChild(target, i);
                    if (element != null)
                    {
                        if (predicate == null || predicate(element))
                        {
                            result = element;
                            break;
                        }
                        result = element.FirstVisualTreeChildren(predicate);
                        if (result != null) break;
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// 沿着视觉树查找符合条件的父元素
        /// </summary>
        /// <param name="target"></param>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public static DependencyObject FindVisualTreeAncestor(this DependencyObject target, Predicate<DependencyObject> predicate = null)
        {
            DependencyObject result = null;
            if (target != null)
            {
                var parent = VisualTreeHelper.GetParent(target);
                if (predicate == null)
                    result = parent;
                else if (parent != null)
                    result = predicate(parent) ? parent : parent.FindVisualTreeAncestor(predicate);
            }
            return result;
        }
        #endregion

        #region 路由事件相关
        /// <summary>
        /// 尝试触发路由事件，不保证成功
        /// </summary>
        /// <param name="target"></param>
        /// <param name="args"></param>
        public static void RaiseEvent(this DependencyObject target, RoutedEventArgs args)
        {
            if (target is UIElement)
            {
                (target as UIElement).RaiseEvent(args);
            }
            else if (target is ContentElement)
            {
                (target as ContentElement).RaiseEvent(args);
            }
        }

        /// <summary>
        /// 尝试添加路由事件，不保证成功
        /// </summary>
        /// <param name="element"></param>
        /// <param name="routedEvent"></param>
        /// <param name="handler"></param>
        public static void AddHandler(this DependencyObject element, RoutedEvent routedEvent, Delegate handler)
        {
            var uiElement = element as UIElement;
            if (uiElement != null)
            {
                uiElement.AddHandler(routedEvent, handler);
            }
            else
            {
                var contentElement = element as ContentElement;
                if (contentElement != null)
                {
                    contentElement.AddHandler(routedEvent, handler);
                }
            }
        }

        /// <summary>
        /// 尝试移除路由事件，不保证成功
        /// </summary>
        /// <param name="element"></param>
        /// <param name="routedEvent"></param>
        /// <param name="handler"></param>
        public static void RemoveHandler(this DependencyObject element, RoutedEvent routedEvent, Delegate handler)
        {
            var uiElement = element as UIElement;
            if (uiElement != null)
            {
                uiElement.RemoveHandler(routedEvent, handler);
            }
            else
            {
                var contentElement = element as ContentElement;
                if (contentElement != null)
                {
                    contentElement.RemoveHandler(routedEvent, handler);
                }
            }
        }
        #endregion
    }
}
