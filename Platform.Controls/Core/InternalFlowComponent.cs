using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;
using Platform.Common.Extension;

namespace Platform.Controls.Core
{
    public class InternalFlowComponent : TitleComponent
    {
        public InternalFlowComponent()
        {
        }

        #region FlowEnterComponent
        public ComponentBase FlowEnterComponent
        {
            get { return (ComponentBase)GetValue(FlowEnterComponentProperty); }
            set { SetValue(FlowEnterComponentProperty, value); }
        }

        // Using a DependencyProperty as the backing store for FlowEnterComponent.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty FlowEnterComponentProperty =
            DependencyProperty.Register("FlowEnterComponent", typeof(ComponentBase), typeof(InternalFlowComponent), new PropertyMetadata(null,
                new PropertyChangedCallback((d, e) =>
                {
                    var element = d as InternalFlowComponent;
                    if (element == null) return;
                    var oldItem = e.OldValue as ComponentBase;
                    var newItem = e.NewValue as ComponentBase;
                    if (oldItem != null)
                        oldItem.FlowEnter -= InternalFlowComponent_FlowEnter;
                    if (newItem != null)
                        newItem.FlowEnter += InternalFlowComponent_FlowEnter;
                })));

        private static void InternalFlowComponent_FlowEnter(object sender, RoutedEventArgs e)
        {
            var component = (sender as ComponentBase).FindVisualTreeAncestor(x => x is InternalFlowComponent) as InternalFlowComponent;
            if (component == null)
                throw new ArgumentOutOfRangeException("InternalFlowComponent:无法找到FlowEnterComponent的关联关系！");
            //触发component的FlowEnterEvent事件
            var args = new RoutedEventArgs { RoutedEvent = FlowEnterEvent };
            component.RaiseEvent(args);
            e.Handled = args.Handled;
        }
        #endregion

        #region FlowLeaveComponent
        public ComponentBase FlowLeaveComponent
        {
            get { return (ComponentBase)GetValue(FlowLeaveComponentProperty); }
            set { SetValue(FlowLeaveComponentProperty, value); }
        }

        // Using a DependencyProperty as the backing store for FlowLeaveComponent.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty FlowLeaveComponentProperty =
            DependencyProperty.Register("FlowLeaveComponent", typeof(ComponentBase), typeof(ComponentBase), new PropertyMetadata(null,
                  new PropertyChangedCallback((d, e) =>
                  {
                      var element = d as InternalFlowComponent;
                      if (element == null) return;
                      var oldItem = e.OldValue as ComponentBase;
                      var newItem = e.NewValue as ComponentBase;
                      if (oldItem != null)
                          oldItem.FlowLeave -= InternalFlowComponent_FlowLeave;
                      if (newItem != null)
                          newItem.FlowLeave += InternalFlowComponent_FlowLeave;
                  })));

        private static void InternalFlowComponent_FlowLeave(object sender, RoutedEventArgs e)
        {
            var component = (sender as ComponentBase).FindVisualTreeAncestor(x => x is InternalFlowComponent) as InternalFlowComponent;
            if (component == null)
                throw new ArgumentOutOfRangeException("InternalFlowComponent:无法找到FlowLeaveComponent的关联关系！");
            //触发component的FlowLeaveEvent事件
            var args = new RoutedEventArgs { RoutedEvent = FlowLeaveEvent };
            component.RaiseEvent(args);
            e.Handled = args.Handled;
        }
        #endregion
    }
}
