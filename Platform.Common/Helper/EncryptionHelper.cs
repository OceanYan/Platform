using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;
using System.IO;
using Platform.Common.Configure;

namespace Platform.Common.Helper
{
    public static class EncryptionHelper
    {
        #region DES加解密
        //加密KEY
        private static string KEY = PlatformSettings.EncryptsKey;

        /// <summary>
        /// 加密报文
        /// </summary>
        /// <param name="msg">报文</param>
        /// <returns>加密后的报文</returns>
        public static byte[] DesEncryptFixKey(byte[] msg)
        {
            byte[] byKey = null;
            byte[] IV = { 0x12, 0x34, 0x56, 0x78, 0x90, 0xAB, 0xCD, 0xEF };
            byKey = PlatformSettings.Encoding.GetBytes(KEY);
            DESCryptoServiceProvider des = new DESCryptoServiceProvider();
            MemoryStream ms = new MemoryStream();
            CryptoStream cs = new CryptoStream(ms, des.CreateEncryptor(byKey, IV), CryptoStreamMode.Write);
            cs.Write(msg, 0, msg.Length);
            cs.FlushFinalBlock();
            return PlatformSettings.Encoding.GetBytes(Convert.ToBase64String(ms.ToArray()));
        }

        /// <summary>
        /// 解密报文
        /// </summary>
        /// <param name ="msg">加了密的报文</param>
        public static byte[] DesDecryptFixKey(byte[] msg)
        {
            byte[] byKey = null;
            byte[] IV = { 0x12, 0x34, 0x56, 0x78, 0x90, 0xAB, 0xCD, 0xEF };
            byKey = PlatformSettings.Encoding.GetBytes(KEY);
            DESCryptoServiceProvider des = new DESCryptoServiceProvider();
            byte[] inputByteArray = Convert.FromBase64String(PlatformSettings.Encoding.GetString(msg));
            MemoryStream ms = new MemoryStream();
            CryptoStream cs = new CryptoStream(ms, des.CreateDecryptor(byKey, IV), CryptoStreamMode.Write);
            cs.Write(inputByteArray, 0, inputByteArray.Length);
            cs.FlushFinalBlock();
            return ms.ToArray();
        }
        #endregion
    }
}
