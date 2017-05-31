using System;
using System.Collections.Generic;
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
using System.Windows.Xps;
using System.Windows.Xps.Packaging;
using System.IO;
using System.Xml.Linq;
using System.ComponentModel;

namespace Platform.Print
{
    /// <summary>
    /// 按照步骤 1a 或 1b 操作，然后执行步骤 2 以在 XAML 文件中使用此自定义控件。
    ///
    /// 步骤 1a) 在当前项目中存在的 XAML 文件中使用该自定义控件。
    /// 将此 XmlNamespace 特性添加到要使用该特性的标记文件的根 
    /// 元素中:
    ///
    ///     xmlns:MyNamespace="clr-namespace:Platform.Print"
    ///
    ///
    /// 步骤 1b) 在其他项目中存在的 XAML 文件中使用该自定义控件。
    /// 将此 XmlNamespace 特性添加到要使用该特性的标记文件的根 
    /// 元素中:
    ///
    ///     xmlns:MyNamespace="clr-namespace:Platform.Print;assembly=Platform.Print"
    ///
    /// 您还需要添加一个从 XAML 文件所在的项目到此项目的项目引用，
    /// 并重新生成以避免编译错误:
    ///
    ///     在解决方案资源管理器中右击目标项目，然后依次单击
    ///     “添加引用”->“项目”->[选择此项目]
    ///
    ///
    /// 步骤 2)
    /// 继续操作并在 XAML 文件中使用控件。
    ///
    ///     <MyNamespace:CustomControl1/>
    ///
    /// </summary>
    [DesignTimeVisible(false)]
    public class PrintContainer : ContentControl
    {
        #region 属性
        /// <summary>
        /// 表单名称
        /// </summary>
        public string DocumentName { get; set; }

        /// <summary>
        /// 表单描述
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// 表示此表单在系统中唯一编号，打印控制中使用此编号来对表单是否可以打印、补打等控制
        /// 大部分情况下都无需设置此属性，只有当同一个交易需要打印同一个表单模板两次及以上情
        /// 况才需要设置此表单为不同的值用以在控制中区分这两个表单达到控制目的。
        /// </summary>
        public string DocumentId { get; set; }
        #endregion

        #region 方法
        /// <summary>
        /// 构造函数，创建一个空白表单容器，其Content为表单打印元素
        /// </summary>
        public PrintContainer()
        {
            //所有打印的界面都采用宋体12进行显示
            this.FontFamily = new System.Windows.Media.FontFamily("SimSun");
            this.FontSize = 12;
            //默认值
            DocumentName = "默认表单";
            Description = "";
            DocumentId = this.GetType().ToString();
        }
        #endregion

        #region 打印
        /// <summary>
        /// 生成打印xps文件
        /// </summary>
        /// <returns>Xps文件的具体信息</returns>
        public FileInfo PrintToXps()
        {
            var fileInfo = new FileInfo(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "temp", Guid.NewGuid().ToString("N")));
            if (!fileInfo.Directory.Exists) fileInfo.Directory.Create();
            if (fileInfo.Exists) fileInfo.Delete();
            //获取背景画刷
            var background = this.Background;
            this.Background = Brushes.Transparent;
            this.InvalidateVisual();
            //呈现打印容器
            CheckVisual();
            XpsDocument xd = new XpsDocument(fileInfo.FullName, System.IO.FileAccess.ReadWrite);
            xd.CoreDocumentProperties.Title = DocumentName;
            xd.CoreDocumentProperties.Description = Description;
            XpsDocumentWriter xpsdw = XpsDocument.CreateXpsDocumentWriter(xd);
            //xd.CoreDocumentProperties.Subject = "StaticResource_" + BackgroundBrush;
            xpsdw.Write(this);
            xd.Close();
            this.Background = background;
            return fileInfo;
        }

        /// <summary>
        /// 检查显示方式
        /// </summary>
        private void CheckVisual()
        {
            if (!this.IsLoaded)
            {
                var window = new Window { Content = new ScrollViewer { Content = this, VerticalScrollBarVisibility = ScrollBarVisibility.Visible, HorizontalScrollBarVisibility = ScrollBarVisibility.Visible }, SizeToContent = System.Windows.SizeToContent.WidthAndHeight };
                window.ShowInTaskbar = false;
                window.WindowState = WindowState.Normal;
                window.Top = 0 - 768;
                window.Left = 0 - 1024;
                window.Show();
                window.Close();
            }
        }

        /// <summary>
        /// 生成打印png图片资源，由于使用针式打印机效果太差，不建议采用
        /// </summary>
        /// <param name="isPrintBackground"></param>
        /// <returns></returns>
        public ImageSource PrintToPng()
        {
            CheckVisual();
            var rtb = new RenderTargetBitmap((int)this.ActualWidth, (int)this.ActualHeight, 96, 96, PixelFormats.Pbgra32);
            rtb.Render(this);
            return rtb;
        }
        #endregion
    }
}
