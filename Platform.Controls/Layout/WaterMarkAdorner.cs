using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;
using Platform.Common.Extension;
using System.Windows.Input;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Threading;

namespace Platform.Controls.Layout
{
    public class WaterMarkAdorner : Adorner
    {
        #region Ctor 
        public WaterMarkAdorner(UIElement adornedElement) : base(adornedElement)
        {
            IsHitTestVisible = false;
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            if (this.AdornedElement == null || !this.AdornedElement.IsVisible) return;
            var text = GetWaterMarkText(this.AdornedElement);
            if (string.IsNullOrEmpty(text))
                return;
            var fontFamily = this.AdornedElement.GetValue(TextBlock.FontFamilyProperty) as FontFamily;
            var fontSize = (double)this.AdornedElement.GetValue(TextBlock.FontSizeProperty);
            var formattedText = new FormattedText(text, CultureInfo.CurrentCulture,
                FlowDirection.LeftToRight, fontFamily.GetTypefaces().ElementAt(0), fontSize, Brushes.Silver);
            //计算位置
            Rect adornedElementRect = new Rect(this.AdornedElement.RenderSize);
            var point = new Point(2, (adornedElementRect.Height - formattedText.Height) / 2);
            drawingContext.DrawText(formattedText, point);
        }
        #endregion

        #region WaterMarkText
        public static string GetWaterMarkText(DependencyObject obj)
        {
            return (string)obj.GetValue(WaterMarkTextProperty);
        }

        public static void SetWaterMarkText(DependencyObject obj, string value)
        {
            obj.SetValue(WaterMarkTextProperty, value);
        }

        // Using a DependencyProperty as the backing store for WaterMarkText.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty WaterMarkTextProperty =
            DependencyProperty.RegisterAttached("WaterMarkText", typeof(string), typeof(WaterMarkAdorner), new PropertyMetadata("水印信息"));
        #endregion

        #region IsWaterMarkHost
        public static bool GetIsWaterMarkHost(DependencyObject obj)
        {
            return (bool)obj.GetValue(IsWaterMarkHostProperty);
        }

        public static void SetIsWaterMarkHost(DependencyObject obj, bool value)
        {
            obj.SetValue(IsWaterMarkHostProperty, value);
        }

        // Using a DependencyProperty as the backing store for IsWaterMarkHost.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsWaterMarkHostProperty =
            DependencyProperty.RegisterAttached("IsWaterMarkHost", typeof(bool), typeof(WaterMarkAdorner), new PropertyMetadata(false,
                new PropertyChangedCallback((d, e) =>
                {
                    var element = d as FrameworkElement;
                    if (element == null || DesignerProperties.GetIsInDesignMode(element)) return;
                    var flag = (bool)e.NewValue;
                    var handler = new Action(() =>
                    {
                        //安装或卸载装饰器
                        var adorner = WaterMarkAdorner.GetWaterMarkAdorner(element);
                        //若flag为false，且adorner不存在，则直接退出
                        if (adorner == null && !flag) return;
                        var adornerLayer = AdornerLayer.GetAdornerLayer(element);
                        if (adornerLayer == null) return;
                        if (adorner == null)
                        {
                            adorner = new WaterMarkAdorner(element);
                            WaterMarkAdorner.SetWaterMarkAdorner(element, adorner);
                            //装饰器Visibility与依附元素的IsVisible绑定
                            adorner.SetBinding(FrameworkElement.VisibilityProperty,
                                new System.Windows.Data.Binding()
                                {
                                    Source = element,
                                    Converter = ValueConverter.BoolToVisibility,
                                    Path = new PropertyPath(IsVisibleProperty)
                                });
                        }
                        adornerLayer.Remove(adorner);
                        if (flag)
                            adornerLayer.Add(adorner);
                    });
                    if (element.IsLoaded)
                        handler.Invoke();
                    else
                        element.Dispatcher.BeginInvoke(handler, DispatcherPriority.Loaded);
                })));
        #endregion

        #region WaterMarkAdorner
        public static WaterMarkAdorner GetWaterMarkAdorner(DependencyObject obj)
        {
            return (WaterMarkAdorner)obj.GetValue(WaterMarkAdornerProperty);
        }

        public static void SetWaterMarkAdorner(DependencyObject obj, WaterMarkAdorner value)
        {
            obj.SetValue(WaterMarkAdornerProperty, value);
        }

        // Using a DependencyProperty as the backing store for WaterMarkAdorner.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty WaterMarkAdornerProperty =
            DependencyProperty.RegisterAttached("WaterMarkAdorner", typeof(WaterMarkAdorner), typeof(WaterMarkAdorner), new PropertyMetadata(null));
        #endregion
    }
}
