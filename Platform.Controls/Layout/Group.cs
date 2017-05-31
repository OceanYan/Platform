using Platform.Common.Base;
using Platform.Controls.Core;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Media;

namespace Platform.Controls.Layout
{
    public class Group : WrapPanel
    {
        #region Ctor
        public Group()
        {
            //初始化_host_Title
            _host_Title = CreateTitle();
            Margin = new Thickness(0, 0, 0, 5);
            SetIsFullLine(this, true);
        }
        #endregion

        #region Layout
        private readonly UIElement _host_Title;

        private readonly IsFullLineConverter _isFullLineConverter = new IsFullLineConverter();

        private UIElement CreateTitle()
        {
            var border = new Border { Padding = new Thickness(10, 5, 0, 5), BorderBrush = Brushes.Silver, BorderThickness = new Thickness(0, 0, 0, 1) };
            var panel = new StackPanel { Orientation = Orientation.Horizontal };
            panel.SetValue(TextBlock.ForegroundProperty, Brushes.SkyBlue);
            //添加图标
            var icon = new IconBox { Code = "\uf044", Width = 20, Margin = new Thickness(0, 0, 10, 0) };
            panel.Children.Add(icon);
            //添加标题
            var title = new TextBlock { FontSize = 16, FontWeight = FontWeights.Bold, VerticalAlignment = VerticalAlignment.Center };
            title.SetBinding(TextBlock.TextProperty, new Binding { Source = this, Path = new PropertyPath(Group.TitleProperty) });
            panel.Children.Add(title);
            border.Child = panel;
            SetIsFullLine(border, true);
            return border;
        }

        protected override Size MeasureOverride(Size constraint)
        {
            //如果子项第一位元素不是标题，则插入
            if (Children.Count == 0 || Children[0] != _host_Title)
                Children.Insert(0, _host_Title);
            //若标题为空，则移除
            if (string.IsNullOrEmpty(Title))
                Children.Remove(_host_Title);
            foreach (FrameworkElement item in this.Children)
            {
                //消除Margin属性的影响
                item.SetValue(FrameworkElement.MarginProperty, new Thickness(0, 5, 0, 5));
                //占据整行的处理
                if (GetIsFullLine(item))
                {
                    var binding = new Binding();
                    binding.Source = this;
                    binding.Path = new PropertyPath(FrameworkElement.ActualWidthProperty);
                    if (double.IsNaN(item.Width))
                    {
                        item.SetBinding(FrameworkElement.WidthProperty, binding);
                        item.SetValue(FrameworkElement.MarginProperty, new Thickness(0));
                    }
                    else
                    {
                        binding.Converter = _isFullLineConverter;
                        binding.ConverterParameter = item;
                        item.SetBinding(FrameworkElement.MarginProperty, binding);
                    }
                }
            }
            return base.MeasureOverride(constraint);
        }

        protected override void OnRender(DrawingContext dc)
        {
            base.OnRender(dc);
            //若不需要标题，则无需边框
            if (!string.IsNullOrEmpty(Title))
            {
                //绘制矩形包围框
                //var rect = new Rect(new Size(this.ActualWidth, this.ActualHeight));
                //dc.DrawRoundedRectangle(null, new Pen(Brushes.Silver, 2), rect, 5, 5);
            }
        }
        #endregion

        #region IsFullLine 占据一整行的位置
        public static bool GetIsFullLine(DependencyObject obj)
        {
            return (bool)obj.GetValue(IsFullLineProperty);
        }

        public static void SetIsFullLine(DependencyObject obj, bool value)
        {
            obj.SetValue(IsFullLineProperty, value);
        }

        // Using a DependencyProperty as the backing store for IsFullLine.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsFullLineProperty =
            DependencyProperty.RegisterAttached("IsFullLine", typeof(bool), typeof(Group), new PropertyMetadata(false));

        public class IsFullLineConverter : IValueConverter
        {
            public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
            {
                var thickness = new Thickness(0);
                var item = parameter as FrameworkElement;
                var width = (double)value;
                //要素已经设定了Width,才执行处理
                if (item != null && !double.IsNaN(item.Width))
                    thickness = new Thickness(0, 0, width - item.Width, 0);
                return thickness;
            }

            public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            {
                throw new NotImplementedException();
            }
        }
        #endregion

        #region Title 标题
        public string Title
        {
            get { return (string)GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Title.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TitleProperty =
            DependencyProperty.Register("Title", typeof(string), typeof(Group), new FrameworkPropertyMetadata("Group标题"));
        #endregion
    }
}
