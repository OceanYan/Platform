using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Configuration;
using System.Xml.Linq;
using Platform.Common.Helper;

namespace Platform.Common.Configure
{
    public class ConfigManager
    {
        #region 字段
        private const string _dirName = "Config";
        /// <summary>
        /// 平台文件信息
        /// </summary>
        private FileInfo _clientFileInfo = new FileInfo(_dirName + "/" + "Platform.Client.Config");
        /// <summary>
        /// 客户端配置项集合
        /// </summary>
        private Dictionary<string, string> _clientDictionary = new Dictionary<string, string>();

        /// <summary>
        /// 系统文件信息
        /// </summary>
        private FileInfo _systemFileInfo = new FileInfo(_dirName + "/" + "Platform.System.Config");
        /// <summary>
        /// 系统配置项集合
        /// </summary>
        private Dictionary<string, string> _systemDictionary = new Dictionary<string, string>();

        /// <summary>
        /// 用户文件信息
        /// </summary>
        private FileInfo _userFileInfo = new FileInfo(_dirName + "/" + "Platform.User.Config");
        /// <summary>
        /// 用户配置项集合
        /// </summary>
        private Dictionary<string, string> _userDictionary = new Dictionary<string, string>();
        #endregion

        #region 方法
        /// <summary>
        /// 构造方法
        /// </summary>
        private ConfigManager() { }

        /// <summary>
        /// 单例模型，获取唯一的配置管理类
        /// </summary>
        private static ConfigManager instance;
        public static ConfigManager GetInstance()
        {
            if (instance == null)
                instance = new ConfigManager();
            return instance;
        }

        /// <summary>
        /// 加载配置文件信息,具体实现方式在Load方法中
        /// </summary>
        /// <param name="configType">配置文件的类型，包括系统，平台，用户三种配置文件</param>
        /// <param name="isEncryptedFile"></param>
        /// <returns>加载成功返回true,否则返回false</returns>
        public bool LoadConfig(ConfigType configType, bool isEncryptedFile = false)
        {
            switch (configType)
            {
                case ConfigType.System:
                    return Load(_systemFileInfo, _systemDictionary, isEncryptedFile);
                case ConfigType.Client:
                    return Load(_clientFileInfo, _clientDictionary);
                case ConfigType.User:
                    return Load(_userFileInfo, _userDictionary, isEncryptedFile);
                default:
                    return false;
            }
        }

        /// <summary>
        /// 将配置文件中的信息加载到相应的配置字典中
        /// </summary>
        /// <param name="from">配置文件</param>
        /// <param name="to">配置字典</param>
        /// <param name="isEncryptedFile"></param>
        /// <returns>加载成功返回true,否则返回false(文件不存在的情况)</returns>
        private bool Load(FileInfo from, Dictionary<string, string> to, bool isEncryptedFile = false)
        {
            try
            {
                if (from.Exists)
                {
                    XDocument doc;
                    byte[] data = File.ReadAllBytes(from.FullName);
                    if (isEncryptedFile)
                    {
                        data = EncryptionHelper.DesDecryptFixKey(data);
                    }
                    using (var ms = new MemoryStream(data))
                    {
                        doc = XDocument.Load(ms);
                    }
                    //解析
                    var ElementCollection = doc.Root.Elements();
                    foreach (var element in ElementCollection)
                    {
                        string elementKey = element.Attribute("key").Value;
                        string elementValue = element.Attribute("value").Value;
                        if (string.IsNullOrEmpty(elementKey))
                            break;
                        if (to.ContainsKey(elementKey))
                            to[elementKey] = elementValue;
                        else
                            to.Add(elementKey, elementValue);
                    }
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 保存文件,具体实现方式在Save方法中
        /// </summary>
        /// <param name="configType">保存文件类型（平台，系统，用户）</param>
        /// <param name="isEncryptedFile"></param>
        /// <returns>保存成功则返回文件的具体信息，否则返回null</returns>
        public FileInfo SaveConfig(ConfigType configType, bool isEncryptedFile = false)
        {
            switch (configType)
            {
                case ConfigType.System:
                    return Save(_systemDictionary, _systemFileInfo, isEncryptedFile);
                case ConfigType.Client:
                    return Save(_clientDictionary, _clientFileInfo);
                case ConfigType.User:
                    return Save(_userDictionary, _userFileInfo, isEncryptedFile);
                default:
                    return null;
            }
        }

        /// <summary>
        /// 将配置字典中的信息存储到相应的配置文件中
        /// </summary>
        /// <param name="from">配置字典</param>
        /// <param name="to">配置文件</param>
        /// <param name="isEncryptedFile"></param>
        /// <returns>返回配置文件的信息</returns>
        private FileInfo Save(Dictionary<string, string> from, FileInfo to, bool isEncryptedFile = false)
        {
            XDocument Document = new XDocument();
            Document.Declaration = new XDeclaration("1.0", "utf-8", null);
            Document.Add(new XElement("elements"));
            foreach (var item in from)
            {
                XElement xe = new XElement("element", new XAttribute("key", item.Key), new XAttribute("value", item.Value));
                Document.Root.Add(xe);
            }
            using (var ms = new MemoryStream())
            {
                Document.Save(ms);
                if (ms.Length == 0)
                    return null;
                var data = ms.ToArray();
                if (isEncryptedFile)
                    data = EncryptionHelper.DesEncryptFixKey(data);
                if (data.Length == 0)
                    return null;
                File.WriteAllBytes(to.FullName, data);
                return to;
            }
        }

        /// <summary>
        /// 获取配置信息
        /// </summary>
        /// <param name="key">配置信息的键</param>
        /// <param name="configType">配置信息类型，包括系统，平台，用户三种配置信息类型</param>
        /// <returns>如果配置信息的键存在则返回相应的值，否则返回null</returns>
        public string GetConfigValue(string key, ConfigType configType)
        {
            string ret = string.Empty;
            if (!string.IsNullOrEmpty(key))
            {
                switch (configType)
                {
                    case ConfigType.System:
                        if (_systemDictionary.ContainsKey(key))
                            ret = _systemDictionary[key];
                        break;
                    case ConfigType.Client:
                        if (_clientDictionary.ContainsKey(key))
                            ret = _clientDictionary[key];
                        break;
                    case ConfigType.User:
                        if (_userDictionary.ContainsKey(key))
                            ret = _userDictionary[key];
                        break;
                    default: break;
                }
            }
            return ret;
        }

        /// <summary>
        /// 设置配置信息
        /// </summary>
        /// <param name="key">配置信息的键</param>
        /// <param name="value">配置信息的键对应的值</param>
        /// <param name="configType">配置信息类型，包括系统，平台，用户三种配置信息类型</param>
        /// <returns>配置成功返回true,否则返回false(即键为null或string.Empty)</returns>
        public bool SetConfigValue(string key, string value, ConfigType configType)
        {
            bool ret = false;
            if (!string.IsNullOrEmpty(key))
            {
                switch (configType)
                {
                    case ConfigType.System:
                        if (_systemDictionary.ContainsKey(key))
                            _systemDictionary[key] = value;
                        else
                            _systemDictionary.Add(key, value);
                        ret = true;
                        break;
                    case ConfigType.Client:
                        if (_clientDictionary.ContainsKey(key))
                            _clientDictionary[key] = value;
                        else
                            _clientDictionary.Add(key, value);
                        ret = true;
                        break;
                    case ConfigType.User:
                        if (_userDictionary.ContainsKey(key))
                            _userDictionary[key] = value;
                        else
                            _userDictionary.Add(key, value);
                        ret = true;
                        break;
                    default: break;
                }
            }
            return ret;
        }
        #endregion
    }

    /// <summary>
    /// 配置项类型
    /// </summary>
    public enum ConfigType
    {
        /// <summary>
        /// 系统级，存储介质-Platform.System.Config，系统统一配置
        /// </summary>
        System,
        /// <summary>
        /// 客户端级，存储介质-Platform.Client.Config，客户端本地配置
        /// </summary>
        Client,
        /// <summary>
        /// 用户级，存储介质-Platform.User.Config，用户个性配置
        /// </summary>
        User
    }
}
