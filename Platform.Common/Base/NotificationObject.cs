using System;
using System.ComponentModel;
using System.Windows;

namespace Platform.Common.Base
{
    /// <summary>
    /// 消息通知接口
    /// <para>在属性值更改的时候进行双向通知</para>
    /// </summary>
    [Serializable]
    public class NotificationObject : DependencyObject, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public void RaisePropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}