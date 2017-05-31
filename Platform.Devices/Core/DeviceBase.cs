using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Platform.Common.Configure;
using Platform.Common.Helper;
using Platform.Common.Extension;

namespace Platform.Devices.Core
{
    /// <summary>
    /// 抽象产品
    /// </summary>
    public abstract class DeviceBase
    {
        #region Ctor
        public DeviceBase(string kind, string name)
        {
            DeviceKind = kind;
            DeviceName = name;
            Config = new Dictionary<string, string>();
            ConfigSet = new Dictionary<string, Tuple<string, Dictionary<string, string>>>();
            DeviceConfig.LoadConfig(this);
        }
        #endregion

        #region Prop
        /// <summary>
        /// 设备种类
        /// </summary>
        public string DeviceKind { get; private set; }

        /// <summary>
        /// 设备名称
        /// </summary>
        public string DeviceName { get; private set; }
        #endregion

        #region Config
        /// <summary>
        /// 配置项
        /// </summary>
        internal Dictionary<string, string> Config { get; private set; }

        internal Dictionary<string, Tuple<string, Dictionary<string, string>>> ConfigSet { get; private set; }

        /// <summary>
        /// 注册控制参数及范围
        /// </summary>
        /// <param name="key"></param>
        /// <param name="set"></param>
        protected void Register(string key, string desc, Dictionary<string, string> set)
        {
            ConfigSet[key] = new Tuple<string, Dictionary<string, string>>(desc, set);
        }

        /// <summary>
        /// 获取或设置设备使用的配置项
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public string this[string key]
        {
            get { return Config.GetDictionaryItem(key); }
            set { Config.SetDictionaryItem(key, value); }
        }
        #endregion

        #region abstract
        /// <summary>
        /// 测试接口
        /// </summary>
        /// <returns></returns>
        public abstract DeviceSet<bool> Test();
        #endregion
    }
}
