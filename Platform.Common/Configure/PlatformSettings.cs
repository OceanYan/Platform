using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Platform.Common.LogSystem;

namespace Platform.Common.Configure
{
    public class PlatformSettings
    {
        #region 属性
        private static Encoding _encoding;
        /// <summary>
        /// 字符编码，默认值是系统默认
        /// </summary>
        public static Encoding Encoding
        {
            get
            {
                string encName = ConfigManager.GetInstance().GetConfigValue("Encoding", ConfigType.Client);
                if (_encoding == null)
                    _encoding = Encoding.GetEncoding("gb18030");
                if (encName != _encoding.EncodingName)
                {
                    try
                    {
                        //若编码发生变化，则更新
                        _encoding = Encoding.GetEncoding(encName);
                    }
                    catch (Exception e)
                    {
                        LogWriter.LogSystem(string.Format("PlatformSettings.Encoding系统参数转换异常！{0}", encName), e);
                    }
                }
                return _encoding;
            }
        }

        /// <summary>
        /// 是否启用调试模式,默认为不启用
        /// </summary>
        public static bool IsDebugMode
        {
            get
            {                
                bool debugModeBoolean;
                string debugMode = ConfigManager.GetInstance().GetConfigValue("IsDebugMode", ConfigType.System);
                bool.TryParse(debugMode, out debugModeBoolean);
                return debugModeBoolean;
            }
        }

        /// <summary>
        /// 平台项目名称，默认为空
        /// </summary>
        public static string PlatformTitle
        {
            get
            {
                return ConfigManager.GetInstance().GetConfigValue("PlatformTitle", ConfigType.System);
            }
        }

        /// <summary>
        /// 服务器端通讯地址
        /// </summary>
        public static string ServiceAddress
        {
            get
            {
                return ConfigManager.GetInstance().GetConfigValue("SocketServiceAddress", ConfigType.Client);
            }
        }

        /// <summary>
        /// 通讯超时时间,默认是1分钟,单位是毫秒
        /// </summary>
        public static int Timeout
        {
            get
            {
                int timeout;
                string time = ConfigManager.GetInstance().GetConfigValue("SocketTimeOut", ConfigType.Client);
                if (!int.TryParse(time, out timeout))
                {
                    timeout = 6000;
                }
                return timeout;
            }
        }

        /// <summary>
        /// 交易文件dll路径，默认为Pages
        /// </summary>
        public static string PagesDirectory
        {
            get
            {
                string procName = ConfigManager.GetInstance().GetConfigValue("PagesDirectory", ConfigType.System);
                if (string.IsNullOrEmpty(procName))
                {
                    procName = "Pages";
                }
                return procName;
            }
        }

        /// <summary>
        /// 是否对报文加密标志
        /// </summary>
        public static bool IsEncrypt
        {
            get
            {
                bool EncryptBoolean;
                string Encrypt = ConfigManager.GetInstance().GetConfigValue("IsEncrypt", ConfigType.Client);
                if (!bool.TryParse(Encrypt, out EncryptBoolean))
                {
                    EncryptBoolean = false;
                }
                return EncryptBoolean;
            }
        }

        /// <summary>
        /// 服务端通讯报文加密KEY
        /// </summary>
        public static string EncryptsKey
        {
            get
            {
                string encryptskey = ConfigManager.GetInstance().GetConfigValue("EncryptsKey", ConfigType.Client);
                if (string.IsNullOrEmpty(encryptskey))
                {
                    encryptskey = "DHCCSoft";
                }
                return encryptskey;
            }
        }
        #endregion
    }
}
