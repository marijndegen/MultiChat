using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Server.Models
{
    public class ServerService
    {
        private bool isHosting = false;

        public bool IsHosting
        {
            get { return isHosting; }
            set { isHosting = value; }
        }

        //addMessage
        private Action<string> AddMessage;
        private Action<bool, bool> SetStatus;

        public ServerService(Action<string> addMessage, Action<bool, bool> setStatus)
        {
            this.AddMessage = addMessage;
            this.SetStatus = setStatus;
        }

        private TcpListener tcpListener;

        public async Task StartHosting(string serverAddress, int port, int bufferSize)
        {
            SetStatus(false, false);
            try
            {
                IPAddress address = IPAddress.Parse(serverAddress);
                if (port <= 0 || port >= 65535)
                    throw new Exception($"Port: {port} not valid");
                if (bufferSize <= 0 || bufferSize >= 10000)
                    throw new Exception("Buffersize should be between 1 and 10.000");


                tcpListener = new TcpListener(address, port);
                tcpListener.Start();

                isHosting = true;
                SetStatus(true, true);

                await Hosting();
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public void StopHosting()
        {
            SetStatus(false, false);
            tcpListener.Stop();
            tcpListener = null;
            isHosting = false;
            SetStatus(true, false);
        }

        public async Task Hosting()
        {
            try
            {
                TcpClient tcpClient;
                NetworkStream networkStream;

                while (isHosting)
                {
                    tcpClient = await tcpListener.AcceptTcpClientAsync();
                    networkStream = tcpClient.GetStream();

                    byte[] buffer = Encoding.ASCII.GetBytes("you are connnected");
                    networkStream.Write(buffer, 0, buffer.Length);

                    networkStream.Close();
                    tcpClient.Close();
                }

                
            }
            catch (Exception ex)
            {
                IsHosting = false; 
                throw ex;
            }
        }

        public Task ClientService()
        {
            return null;
        }




    }
}
