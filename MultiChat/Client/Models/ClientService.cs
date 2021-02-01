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
            Console.WriteLine(DateTime.Now.ToString());
            Console.WriteLine(DateTime.Parse(DateTime.Now.ToString()));
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
                networkStream = tcpClient.GetStream();


                //TODO make listner new treadstart
                await Task.Run(() => this.ConnectionToHost());
            }
            catch (Exception ex)
            {
                Console.WriteLine("ERROR");
                Console.WriteLine(ex.Message);
                throw ex;
            }


        }

        public async Task ConnectionToHost()
        {

            int bufferSize = 1024;
            string message = "";
            byte[] buffer = new byte[bufferSize];

            while (clientActive)
            {
                int readBytes = await networkStream.ReadAsync(buffer, 0, bufferSize);
                message = Encoding.ASCII.GetString(buffer, 0, readBytes);
                Console.WriteLine(message);

            }


        }

        public void StopConnectionToHost()
        {

        }

        public async Task SendCom(string message)
        {
            try
            {
                //Console.WriteLine("connected");
                byte[] buffer = Encoding.ASCII.GetBytes(message);
                await networkStream.WriteAsync(buffer, 0, buffer.Length);
                Console.WriteLine("SENDED");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        
    }
}
