using Platform.Common.Base;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows;

namespace Platform.ViewModel.Menu
{
    /// <summary>
    /// 交易的模型
    /// </summary>
    public class TradeViewModel : NotificationObject
    {
        #region Ctor
        public TradeViewModel(TradeModel model)
        {
            Model = model;
            Children = new ObservableCollection<TradeViewModel>();
        }
        #endregion

        #region Prop
        /// <summary>
        /// 数据模型
        /// </summary>
        public TradeModel Model { get; private set; }

        /// <summary>
        /// 子元素集合
        /// </summary>
        public ObservableCollection<TradeViewModel> Children { get; private set; }

        #region IsSelected - 是否选中
        public bool IsSelected
        {
            get { return (bool)GetValue(IsSelectedProperty); }
            set { SetValue(IsSelectedProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsSelected.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsSelectedProperty =
            DependencyProperty.Register("IsSelected", typeof(bool), typeof(TradeViewModel), new PropertyMetadata(false));
        #endregion
        #endregion


    }
}
