using Platform.Common.Configure;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Platform.Widget.Core
{
    public class PageFactory
    {
        #region 字段
        /// <summary>
        /// 存储交易实例类型字典
        /// </summary>
        private static readonly List<Tuple<string, string, Type>> LoadedActionTypes = new List<Tuple<string, string, Type>>();
        #endregion

        #region 方法
        /// <summary>
        /// 动态通过反射Dll来创建交易
        /// </summary>
        /// <param name="actionName">命名空间（例如：PlatformManage.T7803）</param>
        /// <returns>返回Ipage对象</returns>
        public static PageBase CreatePage(string actionName)
        {
            //构造交易实例
            Type actionType = null;
            actionType = GetActionType(actionName);
            if (actionType == null)
                return null;
            return Activator.CreateInstance(actionType) as PageBase;
        }

        /// <summary>
        /// 通过名称来构造交易类型实例
        /// </summary>
        /// <param name="actionName">交易类型名称(例如:PlatformManage.T7803)</param>
        /// <returns>交易类型实例</returns>
        private static Type GetActionType(string actionName)
        {
            //检查条件，文件更新以及缓存比对，该方法会清理缓存集合
            if (!CheckAction(actionName))
                return null;
            #region 加载dll
            var fileName = string.Concat(actionName, ".dll");
            var pagesDirectory = PlatformSettings.PagesDirectory;
            var file = new FileInfo(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, pagesDirectory, fileName));
            //文件不存在，返回null
            if (!file.Exists) return null;
            var md5 = GetFileMD5(file);
            //判断缓存对象
            var item = LoadedActionTypes.Find(x => x.Item1 == actionName && string.Compare(x.Item2, md5, true) == 0);
            //未找到缓存，开始进行加载dll
            if (item == null)
            {
                Assembly assembly = null;
                byte[] rawAssembly = File.ReadAllBytes(file.FullName);
                byte[] rawSymbolStore = null;
                if (PlatformSettings.IsDebugMode)
                {
                    var fileSymbolName = string.Concat(actionName, ".pdb");
                    var fileSymbol = new FileInfo(Path.Combine(file.DirectoryName, fileSymbolName));
                    if (fileSymbol.Exists)
                        rawSymbolStore = File.ReadAllBytes(fileSymbol.FullName);
                }
                assembly = AppDomain.CurrentDomain.Load(rawAssembly, rawSymbolStore);
                if (assembly == null) return null;
                var type = assembly.GetType(actionName + ".HomePage", false);
                if (type != null)
                    LoadedActionTypes.Add(new Tuple<string, string, Type>(actionName, md5, type));
                return type;
            }
            else
                return item.Item3;
            #endregion
        }

        private static bool CheckAction(string actionName)
        {
            //交易更新的条件，非debug模式
            if (!PlatformSettings.IsDebugMode)
            {
                //TODO:进行更新检查，校验文件版本是否与服务器上的一致，如果不一致，更新成服务器上的版本
                //TODO:如果进行了更新，则需要移除缓存

            }
            return true;
        }

        static string GetFileMD5(FileInfo fileInfo)
        {
            var MD5 = string.Empty;
            if (fileInfo != null && fileInfo.Exists)
            {
                var data = File.ReadAllBytes(fileInfo.FullName);
                using (var stream = new MemoryStream(data))
                {
                    var dataMD5 = new System.Security.Cryptography.MD5CryptoServiceProvider().ComputeHash(stream);
                    stream.Close();
                    MD5 = BitConverter.ToString(dataMD5).Replace("-", "");
                }
            }
            return MD5;
        }
        #endregion
    }
}
