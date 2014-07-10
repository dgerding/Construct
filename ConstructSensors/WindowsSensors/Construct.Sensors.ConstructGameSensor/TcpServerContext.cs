using System;
using System.Net.Sockets;

namespace ConstructTcpServer
{
    public class TcpServerContext : IDisposable
    {
        public delegate void OnAfterConnect(TcpServerContext tcpContext);

        public int GameId;
        public string TcpHost;
        public int TcpPort;
        public TcpClient ClientSocket;
        public NetworkStream ClientStream;
        public OnAfterConnect AfterConnectAction;

        public TcpServerContext(int iGameId, string strTcpHost, int iTcpPort, OnAfterConnect delAfterConnectAction)
        {
            GameId = iGameId;
            TcpHost = strTcpHost;
            TcpPort = iTcpPort;
            ClientSocket = null;
            ClientStream = null;
            AfterConnectAction = delAfterConnectAction;
        }

        ~TcpServerContext()
        {
            Dispose();
        }

        #region IDisposable Members

        public void Dispose()
        {
            if (ClientStream != null)
            {
                ClientStream.Close();
                ClientStream = null;
            }
        }
        #endregion
    }
}