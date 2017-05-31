using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using Platform.Common.Extension;

namespace Platform.Controls.Layout
{
    public class ToastAdorner : Adorner
    {
        #region Ctor 
        public ToastAdorner(UIElement adornedElement) : base(adornedElement)
        {
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            if (this.AdornedElement == null || !this.AdornedElement.IsVisible) return;
            var dock = GetToastDock(AdornedElement);
            var text = GetToastText(AdornedElement) ?? "";
            //如果提示信息为空，则不进行渲染
            if (string.IsNullOrEmpty(text)) return;
            Rect adornedElementRect = new Rect(this.AdornedElement.RenderSize);
            //计算绘制区域的宽，左右默认100，上下默认宿主宽度
            var width = adornedElementRect.Width;
            if (dock == Dock.Left || dock == Dock.Right) width = 100;
            //准备需要绘制的文字对象，根据宽确定高度
            var formattedText = new FormattedText(text, CultureInfo.CurrentCulture, FlowDirection.LeftToRight, new Typeface("simsun"), 14, Brushes.Red);
            formattedText.MaxTextWidth = width - 10;
            var height = formattedText.Height + 6;
            //通过Placement属性计算顶点
            var point = new Point();
            //与宿主间保持一定的间隙
            var gap = 6;
            switch (dock)
            {
                case Dock.Left:
                    point.X = adornedElementRect.Left - width - gap;
                    point.Y = adornedElementRect.Top;
                    break;
                case Dock.Top:
                    point.X = adornedElementRect.Left;
                    point.Y = adornedElementRect.Top - height - gap;
                    break;
                case Dock.Right:
                    point.X = adornedElementRect.Right + gap;
                    point.Y = adornedElementRect.Top;
                    break;
                case Dock.Bottom:
                    point.X = adornedElementRect.Left;
                    point.Y = adornedElementRect.Bottom + gap;
                    break;
                default:
                    return;
            }
            drawingContext.DrawRectangle(Brushes.White, new Pen(Brushes.Red, 1), new Rect(point, new Size(width, height + 8)));
            drawingContext.DrawText(formattedText, new Point(point.X + 8, point.Y + 8));
        }
        #endregion

        #region ToastText 提示信息
        public static string GetToastText(DependencyObject obj)
        {
            return (string)obj.GetValue(ToastTextProperty);
        }

        public static void SetToastText(DependencyObject obj, string value)
        {
            obj.SetValue(ToastTextProperty, value);
        }

        // Using a DependencyProperty as the backing store for ToastText.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ToastTextProperty =
            DependencyProperty.RegisterAttached("ToastText", typeof(string), typeof(ToastAdorner),
                new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.Inherits,
                    new PropertyChangedCallback((d, e) =>
                    {
                        var element = d as FrameworkElement;
                        if (element == null || DesignerProperties.GetIsInDesignMode(element)) return;
                        //承载对象必须具备IsToastAdornerHost属性
                        if (!GetIsToastAdornerHost(element)) return;
                        //避免元素还未显示，延迟到Loaded级别进行
                        element.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Loaded,
                            new Action<UIElement>(host =>
                            {
                                var flag = !string.IsNullOrEmpty(GetToastText(host));
                                var toast = GetToastAdorner(host);
                                //若flag为false，且toast不存在，则直接退出
                                if (!flag && toast == null) return;
                                var adornerLayer = AdornerLayer.GetAdornerLayer(host);
                                if (toast == null)
                                {
                                    toast = new ToastAdorner(host);
                                    SetToastAdorner(host, toast);
                                }
                                adornerLayer.Remove(toast);
                                if (flag)
                                    adornerLayer.Add(toast);
                            }), element);
                    })));
        #endregion

        #region ToastDock 显示位置
        public static Dock GetToastDock(DependencyObject obj)
        {
            return (Dock)obj.GetValue(ToastDockProperty);
        }

        public static void SetToastDock(DependencyObject obj, Dock value)
        {
            obj.SetValue(ToastDockProperty, value);
        }

        // Using a DependencyProperty as the backing store for ToastDock.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ToastDockProperty =
            DependencyProperty.RegisterAttached("ToastDock", typeof(Dock), typeof(ToastAdorner), new FrameworkPropertyMetadata(Dock.Bottom, FrameworkPropertyMetadataOptions.Inherits));
        #endregion

        #region IsToastAdornerHost
        public static bool GetIsToastAdornerHost(DependencyObject obj)
        {
            return (bool)obj.GetValue(IsToastAdornerHostProperty);
        }

        public static void SetIsToastAdornerHost(DependencyObject obj, bool value)
        {
            obj.SetValue(IsToastAdornerHostProperty, value);
        }

        // Using a DependencyProperty as the backing store for IsToastAdornerHost.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsToastAdornerHostProperty =
            DependencyProperty.RegisterAttached("IsToastAdornerHost", typeof(bool), typeof(ToastAdorner), new PropertyMetadata(false));
        #endregion

        #region ToastAdorner 
        private static ToastAdorner GetToastAdorner(DependencyObject obj)
        {
            return (ToastAdorner)obj.GetValue(ToastAdornerProperty);
        }

        private static void SetToastAdorner(DependencyObject obj, ToastAdorner value)
        {
            obj.SetValue(ToastAdornerProperty, value);
        }

        // Using a DependencyProperty as the backing store for ToastAdorner.  This enables animation, styling, binding, etc...
        private static readonly DependencyProperty ToastAdornerProperty =
            DependencyProperty.RegisterAttached("ToastAdorner", typeof(ToastAdorner), typeof(ToastAdorner), new PropertyMetadata(null));
        #endregion
    }
}
