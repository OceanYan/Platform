using Platform.Common.Base;
using Platform.Controls.Core;
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

namespace Platform.Controls.Dialog
{
    /// <summary>
    /// ConfirmDialog.xaml 的交互逻辑
    /// </summary>
    public partial class ConfirmDialog : PageDialog
    {
        #region Ctor
        public ConfirmDialog()
        {
            InitializeComponent();
            this.Mode = DialogMode.Drawer;
            //加载时检查
            this.Loaded += (sender, e) =>
            {
                var fm = FlowManager.GetManagerInstance(this);
                if (fm == null) return;
                if (BtnGroup.Children.Count == 0)
                {
                    AddButton("确认", Key.None, null);
                    fm.ReSetFlow();
                }
                foreach (UI.Button btn in BtnGroup.Children)
                {
                    if ((Key)btn.Tag == DefaultKey)
                        fm.SetFlow(btn);
                }
            };
            //快捷键处理
            this.PreviewKeyDown += (sender, e) =>
            {
                if (e.KeyboardDevice.Modifiers != ModifierKeys.None) return;
                foreach (var item in BtnGroup.Children)
                {
                    var btn = item as UI.Button;
                    if (btn == null) return;
                    var key = (Key)btn.Tag;
                    if (e.Key == key)
                    {
                        btn.RaiseEvent(new RoutedEventArgs { RoutedEvent = UI.Button.ClickEvent });
                        e.Handled = true;
                        break;
                    }
                }
            };
        }
        #endregion

        #region Caption
        public string Caption
        {
            get { return (string)GetValue(CaptionProperty); }
            set { SetValue(CaptionProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Caption.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CaptionProperty =
            DependencyProperty.Register("Caption", typeof(string), typeof(ConfirmDialog), new PropertyMetadata("标题信息"));
        #endregion

        #region IconBrush
        public Brush IconBrush
        {
            get { return (Brush)GetValue(IconBrushProperty); }
            set { SetValue(IconBrushProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ImageSource.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IconBrushProperty =
            DependencyProperty.Register("IconBrush", typeof(Brush), typeof(ConfirmDialog), new PropertyMetadata(null, new PropertyChangedCallback((d, e) =>
            {
                var element = d as ConfirmDialog;
                if (element == null) return;
                var brush = e.NewValue as Brush;
                if (brush == null)
                    element.bd_Icon.Visibility = Visibility.Collapsed;
                else
                {
                    element.bd_Icon.Background = brush;
                    element.bd_Icon.Visibility = Visibility.Visible;
                }
            })));
        #endregion

        #region Message
        public object Message
        {
            get { return (object)GetValue(MessageProperty); }
            set { SetValue(MessageProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Message.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MessageProperty =
            DependencyProperty.Register("Message", typeof(object), typeof(ConfirmDialog), new PropertyMetadata(""));
        #endregion

        #region BtnGroup
        public void AddButton(string text, Key key, Action action)
        {
            var btn = new UI.Button { Value = string.Format("{0}({1})", text, key) };
            btn.Tag = key;
            btn.Click += (sender, e) =>
            {
                //判断动画是否还在执行
                if (this.State == DialogState.Ready)
                {
                    action?.Invoke();
                    ResultKey = (Key)((sender as UI.Button).Tag);
                    this.Close();
                }
            };
            BtnGroup.Children.Insert(0, btn);
        }

        public Key ResultKey { get; private set; }

        public Key DefaultKey { get; set; }
        #endregion

        #region Show
        public static MessageBoxResult Show(string message, string caption, MessageBoxButton button, MessageBoxImage icon, MessageBoxResult defaultResult)
        {
            var dialog = new ConfirmDialog();
            dialog.Title = "";
            dialog.Mode = DialogMode.Page;
            dialog.Caption = caption;
            dialog.Message = message;
            switch (button)
            {
                case MessageBoxButton.OK:
                    dialog.AddButton("确定", Key.O, null);
                    break;
                case MessageBoxButton.OKCancel:
                    dialog.AddButton("确定", Key.O, null);
                    dialog.AddButton("取消", Key.C, null);
                    break;
                case MessageBoxButton.YesNoCancel:
                    dialog.AddButton("是", Key.Y, null);
                    dialog.AddButton("否", Key.N, null);
                    dialog.AddButton("取消", Key.C, null);
                    break;
                case MessageBoxButton.YesNo:
                    dialog.AddButton("是", Key.Y, null);
                    dialog.AddButton("否", Key.N, null);
                    break;
                default:
                    throw new ArgumentException("");
            }
            //处理icon
            switch (icon)
            {
                case MessageBoxImage.Question:
                    dialog.IconBrush = IconBox.GetIconBrush("\uf29c", Brushes.SkyBlue);
                    break;
                case MessageBoxImage.Error:
                    dialog.IconBrush = IconBox.GetIconBrush("\uf05e", Brushes.Red);
                    break;
                case MessageBoxImage.Warning:
                    dialog.IconBrush = IconBox.GetIconBrush("\uf071", Brushes.Yellow);
                    break;
                case MessageBoxImage.Information:
                    dialog.IconBrush = IconBox.GetIconBrush("\uf0eb", Brushes.Yellow);
                    break;
                default:
                    break;
            }
            //定位defaultResult
            switch (defaultResult)
            {
                case MessageBoxResult.OK:
                    dialog.DefaultKey = Key.O;
                    break;
                case MessageBoxResult.Cancel:
                    dialog.DefaultKey = Key.C;
                    break;
                case MessageBoxResult.Yes:
                    dialog.DefaultKey = Key.Y;
                    break;
                case MessageBoxResult.No:
                    dialog.DefaultKey = Key.N;
                    break;
                default:
                    dialog.DefaultKey = Key.None;
                    break;
            }
            dialog.ShowDialog();
            var ret = MessageBoxResult.None;
            switch (dialog.ResultKey)
            {
                case Key.O:
                    ret = MessageBoxResult.OK;
                    break;
                case Key.C:
                    ret = MessageBoxResult.Cancel;
                    break;
                case Key.Y:
                    ret = MessageBoxResult.Yes;
                    break;
                case Key.N:
                    ret = MessageBoxResult.No;
                    break;
                default:
                    break;
            }
            return ret;
        }
        public static MessageBoxResult Show(string message, string caption, MessageBoxButton button, MessageBoxImage icon)
        {
            return Show(message, caption, button, icon, MessageBoxResult.None);
        }
        public static MessageBoxResult Show(string message, string caption, MessageBoxButton button)
        {
            return Show(message, caption, button, MessageBoxImage.None);
        }
        public static MessageBoxResult Show(string message, string caption)
        {
            return Show(message, caption, MessageBoxButton.OK);
        }
        public static MessageBoxResult Show(string message)
        {
            return Show(message, "提示信息");
        }
        #endregion
    }
}
