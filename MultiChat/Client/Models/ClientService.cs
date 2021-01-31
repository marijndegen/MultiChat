using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Client.Models
{
    class ClientService
    {
        #region Client vars
        private bool clientActive = false;

        public bool ClientActive
        {
            get { return clientActive; }
            set { clientActive = value; }
        }

        private int bufferSize;

        public int BufferSize
        {
            get { return bufferSize; }
            set { bufferSize = value; }
        }

        #endregion

        #region Delegates
        private Action<string> AddMessage;
        private Action<bool, bool> UpdateVMState;
        #endregion

        private TcpClient tcpClient;
        private NetworkStream networkStream;

        public ClientService(Action<string> addMessage, Action<bool, bool> updateVMState)
        {
            this.AddMessage = addMessage;
            this.UpdateVMState = updateVMState;
        }

        public async Task StartConnectionToHost(string serverAddress, int port, int bufferSize)
        {
            try
            {
                IPAddress address = IPAddress.Parse(serverAddress);
                if (port <= 0 || port >= 65535)
                    throw new Exception($"Port: {port} not valid");
                if (bufferSize <= 0 || bufferSize >= 10000)
                    throw new Exception("Buffersize should be between 1 and 10.000");

                this.bufferSize = bufferSize;
                tcpClient = new TcpClient();
                tcpClient.Connect(address, port);
                clientActive = true;

                await ConnectionToHost();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw ex;
            }


        }

        public async Task ConnectionToHost()
        {
            try
            {
                //TODO EERSTE BERICHT VAN CLIENT NAAR SERVER STUREN WERKT, COMMITTEN EN DAARNA CLEANUP, VERVOLGENS PUSHEN

                //int bufferSize = 1024;
                //string message = "";
                //byte[] buffer = new byte[bufferSize];

                Console.WriteLine("connected");
                networkStream = tcpClient.GetStream();


                byte[] buffer = Encoding.ASCII.GetBytes("FIRST CLIENT MESSAGE");
                await networkStream.WriteAsync(buffer, 0, buffer.Length);
                Console.WriteLine("SENDED");

                //while (clientActive)
                //{
                //    int readBytes = networkStream.Read(buffer, 0, bufferSize);
                //    message = Encoding.ASCII.GetString(buffer, 0, readBytes);
                //}
                //Console.WriteLine(message);
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public void StopConnectionToHost()
        {

        }

        
    }
}
