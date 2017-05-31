using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Platform.Controls.Core
{
    /// <summary>
    /// 按照步骤 1a 或 1b 操作，然后执行步骤 2 以在 XAML 文件中使用此自定义控件。
    ///
    /// 步骤 1a) 在当前项目中存在的 XAML 文件中使用该自定义控件。
    /// 将此 XmlNamespace 特性添加到要使用该特性的标记文件的根 
    /// 元素中: 
    ///
    ///     xmlns:MyNamespace="clr-namespace:Platform.Controls.Core"
    ///
    ///
    /// 步骤 1b) 在其他项目中存在的 XAML 文件中使用该自定义控件。
    /// 将此 XmlNamespace 特性添加到要使用该特性的标记文件的根 
    /// 元素中: 
    ///
    ///     xmlns:MyNamespace="clr-namespace:Platform.Controls.Core;assembly=Platform.Controls.Core"
    ///
    /// 您还需要添加一个从 XAML 文件所在的项目到此项目的项目引用，
    /// 并重新生成以避免编译错误: 
    ///
    ///     在解决方案资源管理器中右击目标项目，然后依次单击
    ///     “添加引用”->“项目”->[浏览查找并选择此项目]
    ///
    ///
    /// 步骤 2)
    /// 继续操作并在 XAML 文件中使用控件。
    ///
    ///     <MyNamespace:TitleComponent/>
    ///
    /// </summary>
    [DesignTimeVisible(false)]
    public class TitleComponent : ComponentBase
    {
        #region Ctor
        static TitleComponent()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(TitleComponent), new FrameworkPropertyMetadata(typeof(TitleComponent)));
        }

        /// <summary>
        /// 基数宽
        /// </summary>
        public const double _radixWidth = 350;

        /// <summary>
        /// 基数宽
        /// </summary>
        public const double _titleWidth = 160;

        /// <summary>
        /// 基数高
        /// </summary>
        public const double _radixHeight = 30;

        public TitleComponent()
        {
            this.Width = _radixWidth;
            this.Height = _radixHeight;
        }
        #endregion

        #region Scale
        public Point ScaleSize
        {
            get { return (Point)GetValue(ScaleSizeProperty); }
            set { SetValue(ScaleSizeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ScaleSize.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ScaleSizeProperty =
            DependencyProperty.Register("ScaleSize", typeof(Point), typeof(TitleComponent), new PropertyMetadata(new Point(1, 1),
                new PropertyChangedCallback((d, e) =>
                {
                    var element = d as TitleComponent;
                    var p = (Point)e.NewValue;
                    if (p.X == 0 || p.Y == 0) return;
                    element.Width = (_radixWidth - (element.Title == null ? _titleWidth : 0)) * p.X;
                    element.Height = _radixHeight * p.Y;
                })));
        #endregion

        #region Title
        /// <summary>
        /// 标题项
        /// </summary>
        public object Title
        {
            get { return (object)GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Title.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TitleProperty =
            DependencyProperty.Register("Title", typeof(object), typeof(TitleComponent), new PropertyMetadata("标题",
                new PropertyChangedCallback((d, e) =>
                {
                    var element = d as TitleComponent;
                    if (element == null) return;
                    if (e.NewValue == null && element.ScaleSize.X != 0)
                        element.Width = (_radixWidth - _titleWidth) * element.ScaleSize.X;
                })));
        #endregion
    }
}
