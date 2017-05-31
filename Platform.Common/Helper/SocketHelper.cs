using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.IO;
using System.Threading;
using Platform.Common.LogSystem;

namespace Platform.Common.Helper
{
    /// <summary>
    /// 建立Socket连接的处理类
    /// </summary>
    public class SocketHelper
    {
        #region Ctor
        /// <summary>
        /// 默认超时时间
        /// </summary>
        private const int _defaultTimeout = 3 * 60 * 1000;

        /// <summary>
        /// 数据缓冲区深度
        /// </summary>
        private const int _bufferLength = 1024;

        /// <summary>
        /// 创建Socket处理类,默认收发超时时间皆为3分钟（-1）
        /// </summary>
        /// <param name="host">主机名</param>
        /// <param name="port">端口</param>
        public SocketHelper(string host, int port, int sendTimeout = -1, int receiveTimeout = -1)
        {
            Host = host;
            Port = port;
            //默认为3分钟
            if (sendTimeout == -1)
                sendTimeout = _defaultTimeout;
            if (receiveTimeout == -1)
                receiveTimeout = _defaultTimeout;
            baseSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            baseSocket.SendTimeout = sendTimeout;
            baseSocket.ReceiveTimeout = receiveTimeout;
        }

        /// <summary>
        /// 主机名
        /// </summary>
        public string Host { get; private set; }

        /// <summary>
        /// 端口
        /// </summary>
        public int Port { get; private set; }

        /// <summary>
        /// 发送超时时间(毫秒)
        /// </summary>
        public int SendTimeout
        {
            get
            {
                return baseSocket.SendTimeout;
            }
        }

        /// <summary>
        /// 接收超时时间(毫秒)
        /// </summary>
        public int ReceiveTimeout
        {
            get
            {
                return baseSocket.ReceiveTimeout;
            }
        }
        #endregion

        #region Private
        /// <summary>
        /// 实际进行通讯的Socket对象
        /// </summary>
        private Socket baseSocket;

        /// <summary>
        /// 发送数据并接收
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        private byte[] SendData(byte[] data)
        {
            byte[] result = null;
            try
            {
                if (baseSocket.Connected)
                {
                    baseSocket.Send(data);
                    //读取返回数据,按照缓冲大小持续接收，一直到接收不到数据
                    using (MemoryStream ms = new MemoryStream())
                    {
                        var length = 0;
                        var buffer = new byte[_bufferLength];
                        do
                        {
                            //buffer对象的数据每次都会被覆盖，故无需重复创建
                            length = baseSocket.Receive(buffer);
                            ms.Write(buffer, 0, length);
                        } while (length == _bufferLength);
                        result = ms.ToArray();
                    }
                    baseSocket.Close();
                }
            }
            catch (Exception e)
            {
                LogWriter.LogSystem("SocktHelper:SendData异常", e);
                baseSocket.Close();
                result = null;
            }
            return result;
        }
        #endregion

        #region Method
        public byte[] Send(byte[] data)
        {
            byte[] result = null;
            for (int i = 0; i < 3; i++)
            {
                try
                {
                    baseSocket.Connect(Host, Port);
                    break;
                }
                catch (Exception e)
                {
                    LogWriter.LogSystem("SocktHelper:Connect发送异常", e);
                }
                //第一次通讯不成功，等待100毫秒，继续执行
                Thread.Sleep(100);
            }
            result = SendData(data);
            return result;
        }
        #endregion
    }
}
