using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Platform.Controls.Layout
{
    public class Toolbar : StackPanel
    {
        public Toolbar()
        {
            this.Orientation = Orientation.Horizontal;
            this.FlowDirection = System.Windows.FlowDirection.RightToLeft;
            Group.SetIsFullLine(this, true);
        }

        protected override Size MeasureOverride(Size constraint)
        {
            foreach (UIElement item in Children)
            {
                item.SetValue(FrameworkElement.FlowDirectionProperty, FlowDirection.LeftToRight);
                item.SetValue(FrameworkElement.MarginProperty, new Thickness(10, 0, 10, 0));
            }
            return base.MeasureOverride(constraint);
        }
    }
}
