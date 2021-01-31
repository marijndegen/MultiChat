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
        #region Hosting vars
        private bool isHosting = false;

        public bool IsHosting
        {
            get { return isHosting; }
            set { isHosting = value; }
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

        private TcpListener tcpListener;

        public ServerService(Action<string> addMessage, Action<bool, bool> updateVMState)
        {
            this.AddMessage = addMessage;
            this.UpdateVMState = updateVMState;
        }

        public async Task StartHosting(string serverAddress, int port, int bufferSize)
        {
            UpdateVMState(false, false);
            try
            {
                IPAddress address = IPAddress.Parse(serverAddress);
                if (port <= 0 || port >= 65535)
                    throw new Exception($"Port: {port} not valid");
                if (bufferSize <= 0 || bufferSize >= 10000)
                    throw new Exception("Buffersize should be between 1 and 10.000");

                this.bufferSize = bufferSize;
                tcpListener = new TcpListener(address, port);
                tcpListener.Start();

                isHosting = true;
                UpdateVMState(true, true);

                await Hosting();
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public void StopHosting()
        {
            UpdateVMState(false, false);
            tcpListener.Stop();
            tcpListener = null;
            isHosting = false;
            UpdateVMState(true, false);
        }

        public async Task Hosting()
        {
            int bufferSize = 1024;
            string message = "";
            byte[] buffer = new byte[bufferSize];

            try
            {
                while (isHosting)
                {

                    TcpClient tcpClient = await tcpListener.AcceptTcpClientAsync();
                    NetworkStream networkStream = tcpClient.GetStream();

                    Task messageListener = Task.Run(() => MessageListener(networkStream));

                }


            }
            catch (Exception ex)
            {
                IsHosting = false;
                Console.WriteLine($"In hosting: {ex.Message}");
                throw ex;
            }
        }

        //TODO place this method in member model class.
        public async Task MessageListener(NetworkStream networkStream)
        {
            Console.WriteLine("Message listner");
            int bufferSize = 1024;
            string message = "";
            byte[] buffer = new byte[bufferSize];

            try
            {
                while (isHosting)
                {
                    Console.WriteLine("in loop");
                    int readBytes = await networkStream.ReadAsync(buffer, 0, bufferSize);
                    message = Encoding.ASCII.GetString(buffer, 0, readBytes);
                    Console.Write("Recieved: ");
                    Console.WriteLine(message);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"In hosting: {ex.Message}");
                throw;
            }



        }




    }
}
