using Shared.HelperFunctions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using static Shared.HelperFunctions.Validation;
using Shared.ExtensionMethods;
using System.Windows;
using System.Net.NetworkInformation;

namespace Client.Services
{
    class ClientService
    {
        #region Service vars
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

        #region TCP vars and constructor 
        private TcpClient tcpClient;
        private NetworkStream networkStream;

        public ClientService(Action<string> addMessage, Action<bool, bool> updateVMState)
        {
            this.AddMessage = addMessage;
            this.UpdateVMState = updateVMState;
            //Console.WriteLine(DateTime.Now.ToString());
            //Console.WriteLine(DateTime.Parse(DateTime.Now.ToString()));
        }

        #endregion

        public async Task StartConnectionToHost(string serverAddress, int port, int bufferSize)
        {
            UpdateVMState(false, false);
            try
            {
                UserInput userInput = Validation.ValidateUserInput(serverAddress, port, bufferSize);
                this.bufferSize = userInput.BufferSize;
                
                tcpClient = new TcpClient();
                await tcpClient.ConnectAsync(userInput.Address, userInput.Port);

                if(tcpClient.GetState() != TcpState.Established)
                {
                    throw new Exception();
                }
                
                clientActive = true;
                UpdateVMState(true, true);

                networkStream = tcpClient.GetStream();

                Task messageListner = Task.Run(() => this.ConnectionToHost());
            }
            catch (Exception ex)
            {
                UpdateVMState(true, false);
                MessageBox.Show("Couln't connect to the server");
                throw ex;
            }
        }

        public void StopConnectionToHost()
        {
            UpdateVMState(false, false);
            clientActive = false;
            tcpClient.GetStream().Close();
            tcpClient.Close();
            tcpClient = null;
            UpdateVMState(true, false);
        }

        public async Task ConnectionToHost()
        {

            int bufferSize = 1024;
            string message = "";
            byte[] buffer = new byte[bufferSize];

            while (clientActive)
            {
                if (tcpClient.GetState() == TcpState.Established)
                {
                    Console.WriteLine("reading");
                    int readBytes = await networkStream.ReadAsync(buffer, 0, bufferSize);
                    message = Encoding.ASCII.GetString(buffer, 0, readBytes);
                    Console.WriteLine(message);
                }
                else
                {
                    MessageBox.Show("The server disconnected");
                    StopConnectionToHost();
                    break;
                }


            }
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

    public class ClientComService
    {

    }
}
