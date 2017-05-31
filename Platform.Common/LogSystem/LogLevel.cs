using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace Platform.Common.LogSystem
{
    public enum LogLevel
    {
        //致命标识，系统流程发生异常，进行登记
        FATAL,
        //错误标识，系统流程发生异常，进行登记
        ERROR,
        //警示标识，系统执行关键流程，进行登记
        WARN,
        //信息标识，系统执行重要流程，进行登记
        INFO,
        //调试标识，系统执行普通流程，进行登记
        DEBUG
    }
}
