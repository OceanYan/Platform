using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Platform.Common.Configure;

namespace Platform.Devices.Core
{
    /// <summary>
    /// 抽象工厂
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class DeviceFactory<T> where T : DeviceBase
    {
        public DeviceFactory(string kind)
        {
            Kind = kind;
        }

        /// <summary>
        /// 产品名称
        /// </summary>
        public string Kind { get; private set; }

        /// <summary>
        /// 产品制作方案，按照指定的种类进行生产
        /// </summary>
        /// <param name="kind"></param>
        /// <returns></returns>
        public abstract T Process(string name);

        /// <summary>
        /// 获取产品对象
        /// </summary>
        /// <returns></returns>
        public T GetDevice()
        {
            var name = DeviceConfig.GetDeviceName(Kind);// ConfigManager.GetInstance().GetConfigValue(Kind, ConfigType.User);
            var device = Process(name);
            //检查关键项
            if (device == null || device.DeviceKind != Kind || device.DeviceName != name)
                throw new NotSupportedException(string.Format("DeviceFactory NotSupported:Kind={0},Name={1}", Kind, name));
            return device;
        }
    }
}
