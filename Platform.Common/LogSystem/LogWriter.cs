using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using log4net;
using System.Diagnostics;

namespace Platform.Common.LogSystem
{
    public class LogWriter
    {
        static LogWriter()
        {
            //加载log4net配置文件
            log4net.Config.XmlConfigurator.Configure(new System.IO.FileInfo("log4net.xml"));
        }

        /// <summary>
        /// 获取系统日志记录器
        /// </summary>
        /// <returns></returns>
        private static ILog GetSystemLogger()
        {
            // TODO:需要从公共缓存中取对象，暂时写死
            return LogManager.GetLogger("system");
        }

        /// <summary>
        /// 获取业务日志记录器
        /// </summary>
        /// <returns></returns>
        private static ILog GetBusinessLogger()
        {
            // TODO:需要从公共缓存中取对象，暂时写死
            return LogManager.GetLogger("business");
        }

        /// <summary>
        /// 记录业务运行日志，默认INFO级别
        /// </summary>
        /// <param name="message"></param>
        public static void Log(object message)
        {
            Log(message, null, LogLevel.INFO);
        }

        /// <summary>
        /// 记录业务运行日志，默认ERROR级别
        /// </summary>
        /// <param name="message"></param>
        /// <param name="exception"></param>
        public static void Log(object message, Exception exception)
        {
            Log(message, exception, LogLevel.ERROR);
        }

        /// <summary>
        /// 记录业务运行日志
        /// </summary>
        /// <param name="message"></param>
        /// <param name="level"></param>
        public static void Log(object message, LogLevel level)
        {
            Log(message, null, level);
        }

        public static void Log(object message, Exception exception, LogLevel level)
        {
            ILog log = GetBusinessLogger();
            LogWrite(log, message, exception, level);
        }

        /// <summary>
        /// 记录系统运行日志，默认INFO级别
        /// </summary>
        /// <param name="message"></param>
        public static void LogSystem(object message)
        {
            LogSystem(message, null);
        }

        /// <summary>
        /// 记录系统运行日志，默认ERROR级别
        /// </summary>
        /// <param name="message"></param>
        /// <param name="exception"></param>
        public static void LogSystem(object message, Exception exception)
        {
            LogSystem(message, exception, LogLevel.ERROR);
        }

        /// <summary>
        /// 记录系统运行日志
        /// </summary>
        /// <param name="message"></param>
        /// <param name="level"></param>
        public static void LogSystem(object message, LogLevel level)
        {
            LogSystem(message, null, level);
        }

        /// <summary>
        /// 记录系统运行日志
        /// </summary>
        /// <param name="message"></param>
        /// <param name="level"></param>
        public static void LogSystem(object message, Exception exception, LogLevel level)
        {
            ILog log = GetSystemLogger();
            LogWrite(log, message, exception, level);
        }

        private static void LogWrite(ILog logger, Object message, Exception exception,
            LogLevel level)
        {
            switch (level)
            {
                case LogLevel.DEBUG:
                    logger.Debug(message, exception);
                    break;
                case LogLevel.INFO:
                    logger.Info(message, exception);
                    break;
                case LogLevel.WARN:
                    logger.Warn(message, exception);
                    break;
                case LogLevel.ERROR:
                    logger.Error(message, exception);
                    break;
                case LogLevel.FATAL:
                    logger.Fatal(message, exception);
                    break;
                default:
                    break;
            }
        }
    }
}
