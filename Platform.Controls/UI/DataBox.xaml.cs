using Platform.Controls.Core;
using Platform.Controls.Layout;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
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
using Platform.Common.Extension;

namespace Platform.Controls.UI
{
    /// <summary>
    /// DataBox.xaml 的交互逻辑
    /// </summary>
    public partial class DataBox
    {
        #region Ctor
        public DataBox()
        {
            InitializeComponent();
            this.Height = 300;
            isMultiSelected = false;
            dg_Host.ItemsSource = rows;
        }

        private DataTemplate CheckHeaderTemplate;
        private DataTemplate RadioHeaderTemplate;
        private Style RowHeaderStyle;
        private Style ColumnElementStyle;
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            CheckHeaderTemplate = FindResource("CheckHeaderTemplate") as DataTemplate;
            RadioHeaderTemplate = FindResource("RadioHeaderTemplate") as DataTemplate;
            RowHeaderStyle = FindResource("RowHeaderStyle") as Style;
            ColumnElementStyle = FindResource("ColumnElementStyle") as Style;
        }

        private ListCollectionView rows = new ListCollectionView(new List<DataBoxRow>());

        public void DataBinding(DataTable table)
        {
            text_Filter.Text = "";
            dg_Host.SelectedItem = null;
            (rows.SourceCollection as IList).Clear();
            (rows.SourceCollection as List<DataBoxRow>).AddRange(table.Rows.Cast<DataRow>().Select(x =>
            {
                var item = new DataBoxRow(x);
                item.PropertyChanged += DataBoxRow_PropertyChanged;
                return item;
            }));
            if (IsShowPager)
            {
                ResetPager();
            }
            //自动创建列绑定
            InitColumns(table.Columns);
        }

        private void DataBoxRow_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            var row = sender as DataBoxRow;
            if (e.PropertyName == "IsChecked")
                SetCheckedAllState();
        }
        #region IsMultiSelected
        private bool? isMultiSelected;

        /// <summary>
        /// 是否采用多选模式
        /// </summary>
        public bool? IsMultiSelected
        {
            get { return isMultiSelected; }
            set
            {
                isMultiSelected = value;
                InitRowHeader();
            }
        }
        #endregion

        #region IsCheckedAll
        public bool? IsCheckedAll
        {
            get { return (bool?)GetValue(IsCheckedAllProperty); }
            set { SetValue(IsCheckedAllProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsCheckedAll.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsCheckedAllProperty =
            DependencyProperty.Register("IsCheckedAll", typeof(bool?), typeof(DataBox), new PropertyMetadata(false,
                new PropertyChangedCallback((d, e) =>
                {
                    var element = d as DataBox;
                    if (element == null) return;
                    if (e.NewValue == null) return;
                    var flag = (bool)e.NewValue;
                    //仅控制当前数据视图中的数据,其他数据置换成不选状态
                    var list = element.rows.Cast<DataBoxRow>().ToList();
                    element.CheckRows(x =>
                    {
                        if (list.Exists(row => row.Row == x))
                            return flag;
                        return false;
                    });
                })));
        #endregion

        #region IsShowExport

        /// <summary>
        /// 是否显示导出按钮
        /// </summary>
        public bool IsShowExport
        {
            get { return btn_Export.Visibility == Visibility.Visible; }
            set
            {
                if (value) btn_Export.Visibility = Visibility.Visible;
                else btn_Export.Visibility = Visibility.Hidden;
            }
        }
        #endregion

        #region IsShowPrint

        /// <summary>
        /// 是否显示打印按钮
        /// </summary>
        public bool IsShowPrint
        {
            get { return btn_Print.Visibility == Visibility.Visible; }
            set
            {
                if (value) btn_Print.Visibility = Visibility.Visible;
                else btn_Print.Visibility = Visibility.Hidden;
            }
        }
        #endregion

        private void InitRowHeader()
        {
            //选择列
            dg_Host.RowHeaderStyle = null;
            dg_Host.RowHeaderTemplate = null;
            if (IsMultiSelected != null)
            {
                //RowHeader默认不继承数据上下文，需要RowHeaderStyle指定
                dg_Host.RowHeaderStyle = RowHeaderStyle;
                dg_Host.RowHeaderTemplate = IsMultiSelected.Value ? CheckHeaderTemplate : RadioHeaderTemplate;
                dg_Host.SelectionMode = IsMultiSelected.Value ? DataGridSelectionMode.Extended : DataGridSelectionMode.Single;
            }
        }

        private void InitColumns(DataColumnCollection columns)
        {
            dg_Host.Columns.Clear();
            //选择列
            InitRowHeader();
            //数据列
            foreach (DataColumn column in columns)
            {
                //如果列标识为Hidden，则不显示该列
                if (column.ColumnMapping == MappingType.Hidden)
                    continue;
                var dgColumn = new DataGridTextColumn();
                dgColumn.ElementStyle = ColumnElementStyle;
                dgColumn.Header = column.Caption;
                dgColumn.Binding = new Binding("Row[" + column.ColumnName + "]");
                dg_Host.Columns.Add(dgColumn);
            }
            //自定义列
            if (CustomizeColumn != null)
                dg_Host.Columns.Add(CustomizeColumn);
            dg_Host.Items.Refresh();
        }
        #endregion

        #region CustomizeColumn
        public DataGridTemplateColumn CustomizeColumn
        {
            get { return (DataGridTemplateColumn)GetValue(CustomizeColumnProperty); }
            set { SetValue(CustomizeColumnProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CustomizeColumn.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CustomizeColumnProperty =
            DependencyProperty.Register("CustomizeColumn", typeof(DataGridTemplateColumn), typeof(DataBox), new PropertyMetadata(null));
        #endregion

        #region Events
        private void dg_Host_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var mouseOver = Mouse.DirectlyOver as DependencyObject;
            if (dg_Host.SelectedItem == null) return;
            var row = dg_Host.ItemContainerGenerator.ContainerFromItem(dg_Host.SelectedItem) as DataGridRow;
            if (row.IsAncestorOf(mouseOver))
            {
                this.Dispatcher.BeginInvoke(new Action(() =>
                {
                    var fm = FlowManager.GetManagerInstance(this);
                    if (fm != null) fm.MoveFocusHost(FlowNavigation.Forward);
                }), System.Windows.Threading.DispatcherPriority.Loaded);
            }
        }

        private void text_filter_TextChanged(object sender, TextChangedEventArgs e)
        {
            WaterMarkAdorner.SetIsWaterMarkHost(text_Filter, text_Filter.Text.Length == 0);
            var filterText = text_Filter.Text;
            var filter = string.IsNullOrWhiteSpace(filterText) ? null : new Predicate<object>(x =>
                  {
                      var row = x as DataBoxRow;
                      foreach (var item in row.Row.ItemArray)
                      {
                          if (item != null && item.ToString().Contains(filterText))
                              return true;
                      }
                      return false;
                  });
            if (!IsShowPager)
                dg_Host.Items.Filter = filter;
            else
            {
                rows.Filter = filter;
                ResetPager();
            }
            SetCheckedAllState();
        }

        private bool _canSetCheckedAllState = true;

        /// <summary>
        /// 设置全选状态标志
        /// </summary>
        private void SetCheckedAllState()
        {
            if (_canSetCheckedAllState)
            {
                //仅检查筛选过后的视图数据
                var list = rows.Cast<DataBoxRow>().ToList();// rows.SourceCollection as List<DataBoxRow>;
                var checkedFlag = list.Exists(x => x.IsChecked);
                var uncheckedFlag = list.Exists(x => !x.IsChecked);
                if (checkedFlag && uncheckedFlag)
                    IsCheckedAll = null;
                else
                    IsCheckedAll = checkedFlag ? true : false;
            }
        }
        #endregion

        #region Methods
        /// <summary>
        /// 获取选择的行数据
        /// </summary>
        /// <returns></returns>
        public List<DataRow> GetSelectedRows()
        {
            return ((IsMultiSelected ?? false) ?
                  (rows.SourceCollection as List<DataBoxRow>).Where(x => x.IsChecked) :
                  dg_Host.SelectedItems.Cast<DataBoxRow>()).Select(x => x.Row).ToList();
        }

        /// <summary>
        /// 获取所有的行数据
        /// </summary>
        /// <returns></returns>
        public List<DataRow> GetRows()
        {
            return (rows.SourceCollection as List<DataBoxRow>).Select(x => x.Row).ToList();
        }

        /// <summary>
        /// 将符合条件的行设置为选中状态
        /// </summary>
        /// <param name="predicate"></param>
        public void CheckRows(Predicate<DataRow> predicate)
        {
            if (predicate == null) throw new ArgumentNullException("DataBox.CheckRows：参数predicate不能为null！");
            //批量执行设定可选状态时，无需同步设定全选标志
            _canSetCheckedAllState = false;
            foreach (DataBoxRow item in rows.SourceCollection)
                item.IsChecked = predicate(item.Row);
            _canSetCheckedAllState = true;
            SetCheckedAllState();
        }

        /// <summary>
        /// 获取列对象，从而进行可见/长度等操作
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public DataGridColumn GetDataGridColumn(int index)
        {
            return dg_Host.Columns.Count > index ? dg_Host.Columns[index] : null;
        }

        public void SetComboBoxColumn(int index, Dictionary<string, string> dict)
        {
            var column = GetDataGridColumn(index) as DataGridTextColumn;
            if (column == null) return;
            var cbbColumn = new DataGridComboBoxColumn();
            cbbColumn.Header = column.Header;
            cbbColumn.ItemsSource = dict;
            cbbColumn.SelectedValuePath = "Value";
            cbbColumn.DisplayMemberPath = "Key";
            cbbColumn.SelectedValueBinding = column.Binding;
            dg_Host.Columns.Insert(index, cbbColumn);
            dg_Host.Columns.Remove(column);
        }
        #endregion

        #region Pager
        #region IsShowPager
        /// <summary>
        /// 是否显示分页效果
        /// </summary>
        public bool IsShowPager
        {
            get { return (bool)GetValue(IsShowPagerProperty); }
            set { SetValue(IsShowPagerProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsShowPager.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsShowPagerProperty =
            DependencyProperty.Register("IsShowPager", typeof(bool), typeof(DataBox), new PropertyMetadata(false,
                new PropertyChangedCallback((d, e) =>
                {
                    var element = d as DataBox;
                    if ((bool)e.NewValue)
                    {
                        element.grid_pager.Visibility = Visibility.Visible;
                        element.PageIndex = 0;
                        element.Paging();
                    }
                    else
                    {
                        element.grid_pager.Visibility = Visibility.Collapsed;
                        element.dg_Host.ItemsSource = element.rows;
                        element.dg_Host.Items.Refresh();
                    }
                })));
        #endregion

        /// <summary>
        /// 分页索引
        /// </summary>
        public int PageIndex { get; private set; }

        public int PageCount
        {
            get
            {
                var pageLines = int.Parse(cbb_PageLines.SelectedValue.ToString());
                return (rows.Count / pageLines) + (rows.Count % pageLines == 0 ? 0 : 1);
            }
        }

        /// <summary>
        /// 重置分页
        /// </summary>
        private void ResetPager()
        {
            //合计条目
            text_TotalCount.Text = string.Format("共{0}条记录", rows.Count);
            PageIndex = 0;
            Paging();
        }

        Brush enableBrush, disableBrush;
        /// <summary>
        /// 执行分页动作
        /// </summary>
        private void Paging()
        {
            //每页数目
            var pageLines = int.Parse(cbb_PageLines.SelectedValue.ToString());
            text_Pages.Text = string.Format("{0}/{1}", PageCount == 0 ? 0 : PageIndex + 1, PageCount);
            //取数据
            var list = rows.Cast<DataBoxRow>().Skip(PageIndex * pageLines).ToList();
            dg_Host.ItemsSource = list.Count > pageLines ? list.Take(pageLines) : list;
            dg_Host.Items.Refresh();
            //控制分页按钮是否可用
            if (enableBrush == null)
            {
                var rd = new ResourceDictionary();
                rd.Source = new Uri("/Platform.Controls;component/Themes/CommonResource.xaml", UriKind.RelativeOrAbsolute);
                enableBrush = rd["commonIconAmaranthRed"] as Brush;
                disableBrush = rd["commonDisableGray"] as Brush;
            }
            btn_Top.IsEnabled = btn_Post.IsEnabled = PageIndex < 1 ? false : true;
            btn_Top.Foreground = btn_Post.Foreground = PageIndex < 1 ? disableBrush : enableBrush;
            btn_Bottom.IsEnabled = btn_Next.IsEnabled = PageIndex + 1 == PageCount ? false : true;
            btn_Bottom.Foreground = btn_Next.Foreground = PageIndex + 1 == PageCount ? disableBrush : enableBrush;
        }

        private void btn_Post_Click(object sender, RoutedEventArgs e)
        {
            if (PageIndex == 0) return;
            PageIndex--;
            Paging();
        }

        private void btn_Next_Click(object sender, RoutedEventArgs e)
        {
            if (PageIndex == PageCount - 1) return;
            PageIndex++;
            Paging();
        }

        private void btn_Top_Click(object sender, RoutedEventArgs e)
        {
            PageIndex = 0;
            Paging();
        }

        private void btn_Bottom_Click(object sender, RoutedEventArgs e)
        {
            PageIndex = PageCount - 1;
            Paging();
        }

        private void cbb_PageLines_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this.IsLoaded)
            {
                PageIndex = 0;
                Paging();
            }
        }
        #endregion

        #region Override
        public override string Value { get; set; }

        internal override void SetFlowKeyboardFocus()
        {
            //base.SetFlowKeyboardFocus();
            if (dg_Host.SelectedIndex == -1)
                dg_Host.SelectedIndex = 0;
            if (dg_Host.SelectedItem != null)
                dg_Host.ScrollIntoView(dg_Host.SelectedItem);
            dg_Host.Focus();
        }

        internal override bool OnFlowKeyDown(KeyEventArgs e)
        {
            //没有选项，继续往下运作
            if (dg_Host.Items.Count == 0) return true;
            //分页快捷键
            if (IsShowPager)
            {
                if (e.Key == Key.Home)
                {
                    btn_Top_Click(btn_Top, new RoutedEventArgs());
                    return false;
                }
                if (e.Key == Key.End)
                {
                    btn_Bottom_Click(btn_Bottom, new RoutedEventArgs());
                    return false;
                }
                if (e.Key == Key.PageUp)
                {
                    btn_Post_Click(btn_Post, new RoutedEventArgs());
                    return false;
                }
                if (e.Key == Key.PageDown)
                {
                    btn_Next_Click(btn_Next, new RoutedEventArgs());
                    return false;
                }
            }
            if (e.Key == Key.Up)
            {
                if (dg_Host.SelectedIndex <= 0)
                    return true;
                dg_Host.SelectedIndex--;
                dg_Host.ScrollIntoView(dg_Host.SelectedItem);
                e.Handled = true;
                return false;
            }
            if (e.Key == Key.Down)
            {
                if (dg_Host.SelectedIndex >= dg_Host.Items.Count - 1)
                    dg_Host.SelectedIndex = 0;
                else
                    dg_Host.SelectedIndex++;
                dg_Host.ScrollIntoView(dg_Host.SelectedItem);
                e.Handled = true;
                return false;
            }
            if (e.Key == Key.Enter)
            {
                return true;
            }
            if (IsMultiSelected == true && e.Key == Key.Space)
            {
                var row = (dg_Host.SelectedItem as DataBoxRow);
                row.IsChecked = !row.IsChecked;
                e.Handled = true;
                return false;
            }
            if (e.Key == Key.Left || e.Key == Key.Right)
            {
                return false;
            }
            return base.OnFlowKeyDown(e);
        }
        #endregion

        private void dg_Host_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            Predicate<DependencyObject> handle = x => x is ScrollViewer && (x as ScrollViewer).ComputedVerticalScrollBarVisibility == Visibility.Visible;
            //内部的滚动条是否显示
            var sv_inner = dg_Host.FirstVisualTreeChildren(handle) as ScrollViewer;
            if (sv_inner == null)
            {
                var args = new MouseWheelEventArgs(e.MouseDevice, e.Timestamp, e.Delta);
                args.RoutedEvent = UIElement.MouseWheelEvent;
                args.Source = sender;
                var sv_Host = this.FindVisualTreeAncestor(handle) as ScrollViewer;
                if (sv_Host != null)
                    sv_Host.RaiseEvent(args);
                e.Handled = true;
            }
        }
    }

    #region DataBoxRow - 显示包装类
    /// <summary>
    /// DataBox的行包装类
    /// </summary>
    public class DataBoxRow : Platform.Common.Base.NotificationObject
    {
        #region Ctor 
        public DataBoxRow(DataRow row)
        {
            Row = row;
        }

        /// <summary>
        /// 实际的行数据
        /// </summary>
        public DataRow Row { get; private set; }
        #endregion

        #region IsChecked - 是否选中
        private bool _isChecked;

        public bool IsChecked
        {
            get { return _isChecked; }
            set
            {
                _isChecked = value;
                this.RaisePropertyChanged("IsChecked");
            }
        }
        #endregion
    }
    #endregion
}
