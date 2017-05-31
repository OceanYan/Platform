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

namespace Platform.Controls.Layout
{
    public class AnchorAdorner : Adorner
    {
        #region Ctor 
        public AnchorAdorner(UIElement adornedElement) : base(adornedElement)
        {
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            if (this.AdornedElement == null || !this.AdornedElement.IsVisible) return;
            var pen = new Pen(new SolidColorBrush(Colors.Red), 1.0);
            Rect adornedElementRect = new Rect(this.AdornedElement.RenderSize);
            drawingContext.DrawLine(pen, new Point(adornedElementRect.TopLeft.X - 3, adornedElementRect.TopLeft.Y - 3), new Point(adornedElementRect.TopLeft.X + 5, adornedElementRect.TopLeft.Y - 3));
            drawingContext.DrawLine(pen, new Point(adornedElementRect.TopLeft.X - 3, adornedElementRect.TopLeft.Y - 3), new Point(adornedElementRect.TopLeft.X - 3, adornedElementRect.TopLeft.Y + 5));

            drawingContext.DrawLine(pen, new Point(adornedElementRect.TopRight.X + 3, adornedElementRect.TopRight.Y - 3), new Point(adornedElementRect.TopRight.X - 5, adornedElementRect.TopRight.Y - 3));
            drawingContext.DrawLine(pen, new Point(adornedElementRect.TopRight.X + 3, adornedElementRect.TopRight.Y - 3), new Point(adornedElementRect.TopRight.X + 3, adornedElementRect.TopRight.Y + 5));

            drawingContext.DrawLine(pen, new Point(adornedElementRect.BottomLeft.X - 3, adornedElementRect.BottomLeft.Y + 3), new Point(adornedElementRect.BottomLeft.X + 5, adornedElementRect.BottomLeft.Y + 3));
            drawingContext.DrawLine(pen, new Point(adornedElementRect.BottomLeft.X - 3, adornedElementRect.BottomLeft.Y + 3), new Point(adornedElementRect.BottomLeft.X - 3, adornedElementRect.BottomLeft.Y - 5));

            drawingContext.DrawLine(pen, new Point(adornedElementRect.BottomRight.X + 3, adornedElementRect.BottomRight.Y + 3), new Point(adornedElementRect.BottomRight.X - 5, adornedElementRect.BottomRight.Y + 3));
            drawingContext.DrawLine(pen, new Point(adornedElementRect.BottomRight.X + 3, adornedElementRect.BottomRight.Y + 3), new Point(adornedElementRect.BottomRight.X + 3, adornedElementRect.BottomRight.Y - 5));
        }
        #endregion

        #region IsKeyboardFocusWithin 瞄准器
        #region IsKeyboardFocusWithinAnchor  
        public static bool GetIsKeyboardFocusWithinAnchor(DependencyObject obj)
        {
            return (bool)obj.GetValue(IsKeyboardFocusWithinAnchorProperty);
        }

        public static void SetIsKeyboardFocusWithinAnchor(DependencyObject obj, bool value)
        {
            obj.SetValue(IsKeyboardFocusWithinAnchorProperty, value);
        }

        // Using a DependencyProperty as the backing store for IsKeyboardFocusInAnchor.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsKeyboardFocusWithinAnchorProperty =
            DependencyProperty.RegisterAttached("IsKeyboardFocusWithinAnchor", typeof(bool), typeof(AnchorAdorner), new PropertyMetadata(false,
                new PropertyChangedCallback((d, e) =>
                {
                    var element = d as UIElement;
                    if (element == null) return;
                    element.IsKeyboardFocusWithinChanged -= IsKeyboardFocusWithinAnchorChanged;
                    if ((bool)e.NewValue)
                        element.IsKeyboardFocusWithinChanged += IsKeyboardFocusWithinAnchorChanged;
                })));
        #endregion

        #region KeyboardWithinAnchor 
        private static AnchorAdorner GetKeyboardWithinAnchor(DependencyObject obj)
        {
            return (AnchorAdorner)obj.GetValue(KeyboardWithinAnchorProperty);
        }

        private static void SetKeyboardWithinAnchor(DependencyObject obj, AnchorAdorner value)
        {
            obj.SetValue(KeyboardWithinAnchorProperty, value);
        }

        // Using a DependencyProperty as the backing store for KeyboardWithinAnchor.  This enables animation, styling, binding, etc...
        private static readonly DependencyProperty KeyboardWithinAnchorProperty =
            DependencyProperty.RegisterAttached("KeyboardWithinAnchor", typeof(AnchorAdorner), typeof(AnchorAdorner), new PropertyMetadata(null));
        #endregion

        private static void IsKeyboardFocusWithinAnchorChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            var element = sender as FrameworkElement;
            if (element == null || DesignerProperties.GetIsInDesignMode(element)) return;

            //如果元素内部还包含该装饰器，为防止重叠，则不处理
            if (element.FirstVisualTreeChildren(x => AnchorAdorner.GetIsKeyboardFocusWithinAnchor(x)) != null)
                return;
            var flag = (bool)e.NewValue;
            element.Dispatcher.BeginInvoke(new Action(() =>
            {
                //安装或卸载装饰器
                var anchor = AnchorAdorner.GetKeyboardWithinAnchor(element);
                //若flag为false，且anchor不存在，则直接退出
                if (anchor == null && !flag) return;
                var adornerLayer = AdornerLayer.GetAdornerLayer(element);
                if (adornerLayer == null) return;
                if (anchor == null)
                {
                    anchor = new AnchorAdorner(element);
                    AnchorAdorner.SetKeyboardWithinAnchor(element, anchor);
                    //装饰器Visibility与依附元素的IsVisible绑定
                    anchor.SetBinding(FrameworkElement.VisibilityProperty,
                        new System.Windows.Data.Binding()
                        {
                            Source = element,
                            Converter = ValueConverter.BoolToVisibility,
                            Path = new PropertyPath(IsVisibleProperty)
                        });
                }
                adornerLayer.Remove(anchor);
                if (flag)
                    adornerLayer.Add(anchor);
            }), System.Windows.Threading.DispatcherPriority.Loaded);
        }
        #endregion
    }
}
