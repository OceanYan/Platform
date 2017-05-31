using System;
using System.Runtime.CompilerServices;
using System.Windows.Threading;

namespace Platform.Common.Extension
{
    /// <summary>
    /// 基于DispatcherObject的扩展方法集合
    /// </summary>
    public static class DispatcherObjectExtension
    {
        public static object UIInvoke(this DispatcherObject target, Delegate method)
        {
            return UIInvoke<object>(target, method);
        }

        public static T UIInvoke<T>(this DispatcherObject target, Delegate method)
        {
            return target.Dispatcher.CheckAccess()
                ? (T)method.DynamicInvoke() : (T)target.Dispatcher.Invoke(method);
        }
    }
}
