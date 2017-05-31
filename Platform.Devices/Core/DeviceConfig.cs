using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Platform.Common.Configure;
using Platform.Common.Helper;

namespace Platform.Devices.Core
{
    public static class DeviceConfig
    {
        /// <summary>
        /// 加载该设备的配置项
        /// </summary>
        public static void LoadConfig(DeviceBase device)
        {
            device.Config.Clear();
            var config = ConfigManager.GetInstance().GetConfigValue(device.DeviceKind + "_" + device.DeviceName, ConfigType.User);
            if (!string.IsNullOrEmpty(config))
            {
                try
                {
                    var dict = JsonHelper.ParseJson2Dictionary(config);
                    foreach (var item in dict)
                        device.Config.Add(item.Key, item.Value.ToString());
                    return;
                }
                catch
                { }
            }
        }

        /// <summary>
        /// 存储该设备的配置项
        /// </summary>
        public static bool SaveConfig(DeviceBase device)
        {
            //检查配置项
            var dict = new Dictionary<string, object>();
            foreach (var item in device.ConfigSet)
            {
                var value = device.Config[item.Key];
                if (!string.IsNullOrEmpty(value))
                {
                    if (item.Value.Item2 != null)
                    {
                        if (!item.Value.Item2.ContainsValue(value))
                            return false;
                        dict[item.Key] = value;
                    }
                    else
                    {
                        dict[item.Key] = value;
                    }
                }
            }
            var config = JsonHelper.ParseDictionary2Json(dict);
            return ConfigManager.GetInstance().SetConfigValue(device.DeviceKind + "_" + device.DeviceName, config, ConfigType.User);
        }

        /// <summary>
        /// 获取配置项定义
        /// </summary>
        /// <param name="device"></param>
        public static Dictionary<string, Tuple<string, Dictionary<string, string>>> GetConfigSet(DeviceBase device)
        {
            return device.ConfigSet;
        }

        /// <summary>
        /// 获取设备种类的当前品牌配置
        /// </summary>
        /// <param name="kind"></param>
        /// <returns></returns>
        public static string GetDeviceName(string kind)
        {
            return ConfigManager.GetInstance().GetConfigValue(kind, ConfigType.User);
        }

        /// <summary>
        /// 设置设备种类的当前品牌配置
        /// </summary>
        /// <param name="kind"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static bool SetDeviceName(string kind, string name)
        {
            return ConfigManager.GetInstance().SetConfigValue(kind, name, ConfigType.User);
        }
    }
}
