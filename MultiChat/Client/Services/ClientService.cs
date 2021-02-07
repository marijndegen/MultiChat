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
using System.Threading;
using Shared.Models;
using Client.Models;

namespace Client.Services
{
    class ClientService
    {
        #region Delegates
        private Action<string> AddMessage;
        private Action<bool, bool> UpdateVMState;
        #endregion

        #region TCP vars and constructor 
        private TcpClient tcpClient;
        private NetworkStream networkStream;
        private ClientComService clientComService;
        private CancellationTokenSource clientTokenSource;
        private CancellationToken clientToken;

        public ClientService(Action<string> addMessage, Action<bool, bool> updateVMState)
        {
            this.AddMessage = addMessage;
            this.UpdateVMState = updateVMState;
            clientComService = new ClientComService(this.StopConnectionToHost);
            //Console.WriteLine(DateTime.Now.ToString());
            //Console.WriteLine(DateTime.Parse(DateTime.Now.ToString()));
        }

        #endregion

        #region Starthosting, stophosting, setBufferSize, Sendcom
        public async Task StartConnectionToHost(string clientName, string serverAddress, int port, int bufferSize)
        {
            try
            {
                UpdateVMState(false, false);
                UserInput userInput = Validation.ValidateUserInput(serverAddress, port, bufferSize);

                tcpClient = new TcpClient();
                await tcpClient.ConnectAsync(userInput.Address, userInput.Port);

                if (tcpClient.GetState() != TcpState.Established)
                {
                    throw new Exception();
                }

                networkStream = tcpClient.GetStream();

                clientTokenSource = new CancellationTokenSource();
                clientToken = clientTokenSource.Token;

                MemberModel memberModel = new MemberModel(clientName, tcpClient);
                clientComService.BufferSize = bufferSize;
                
                Task messageListner = Task.Run(() => clientComService.ConnectionToHost(memberModel, clientToken), clientToken);

                await clientComService.sendCom(new ClientSendHandshakeModel(memberModel));
                UpdateVMState(true, true);
            }
            catch (Exception ex)
            {
                UpdateVMState(true, false);
                MessageBox.Show("Couln't connect to the server", "Client", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Warning, System.Windows.MessageBoxResult.OK, MessageBoxOptions.DefaultDesktopOnly);
                throw ex;
            }
        }

        public void StopConnectionToHost()
        {
            Console.WriteLine("stopping!!");
            try
            {
                Application.Current.Dispatcher.BeginInvoke(new Action(() => UpdateVMState(false, false)));

                clientComService.ClientActive = false;

                tcpClient.GetStream().Close();
                tcpClient.Close();
                tcpClient = null;

            }
            catch (Exception ex)
            {
                Console.WriteLine("Error in stopping client ");
                Console.WriteLine(ex.Message);
            }
            finally
            {
                Application.Current.Dispatcher.BeginInvoke(new Action(() => UpdateVMState(true, false)));
            }
        }

        public void SetBufferSize(int bufferSize)
        {
            if (ValidateBufferSize(bufferSize))
                clientComService.BufferSize = bufferSize;
            else
                MessageBox.Show("Invalid bufferSize");
        }

        public async Task SendCom(string message)
        {
            //try
            //{
            //    //Console.WriteLine("connected");
            //    byte[] buffer = Encoding.ASCII.GetBytes(message);
            //    await networkStream.WriteAsync(buffer, 0, buffer.Length);
            //    Console.WriteLine("SENDED");
            //}
            //catch (Exception ex)
            //{
            //    throw ex;
            //}
        }
        #endregion

    }

    public class ClientComService
    {
        private bool isListening;

        public bool IsListening
        {
            get { return isListening; }
            set { isListening = value; }
        }

        private int bufferSize;

        public int BufferSize
        {
            get { return bufferSize; }
            set { bufferSize = value; }
        }

        private bool clientActive = false;

        public bool ClientActive
        {
            get { return clientActive; }
            set { clientActive = value; }
        }

        private Action stopConnectionToHost;

        public Action StopConnectionToHost
        {
            get { return stopConnectionToHost; }
        }


        public ClientComService(Action stopConnectionToHost)
        {
            this.stopConnectionToHost = stopConnectionToHost;
        }

        public async Task ConnectionToHost(MemberModel memberModel, CancellationToken clientToken)
        {
            int bufferSize = 1024;
            string message = "";
            byte[] buffer = new byte[bufferSize];
            clientActive = true;

            NetworkStream networkStream = memberModel.TcpClient.GetStream();

            try
            {
                while (clientActive && !clientToken.IsCancellationRequested)
                {
                    if (memberModel.TcpClient.GetState() == TcpState.Established)
                    {
                        clientToken.ThrowIfCancellationRequested();
                        IComModel comModel = await DecodeCom(networkStream);
                        

                    }
                    else
                    {
                        throw new Exception("No connection from server");
                    }
                }
            }
            catch (Exception ex)
            {
                    MessageBox.Show("Connection with the server is lost", "Client", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Warning, System.Windows.MessageBoxResult.OK, MessageBoxOptions.DefaultDesktopOnly);
                    StopConnectionToHost();
            }
        }

        public async Task<IComModel> DecodeCom(NetworkStream networkStream)
        {
            string completeMessage = "";
            string message;
            byte[] buffer = new byte[bufferSize];
            bool foundCompleteMessage = false;

            do
            {
                int readBytes = await networkStream.ReadAsync(buffer, 0, bufferSize);
                message = Encoding.ASCII.GetString(buffer, 0, readBytes);
                completeMessage = $"{completeMessage}{message}";

                if (completeMessage.Length > 6)
                {
                    Console.Write("Recieved: ");
                    Console.WriteLine(message.ToString());
                    Console.WriteLine("CompleteMessage");
                    Console.WriteLine();
                    foundCompleteMessage = completeMessage.Substring(completeMessage.Length - 3, 3) == "$$$";
                }

                if (foundCompleteMessage)
                {
                    Console.WriteLine("the complete message");
                    Console.WriteLine(completeMessage);
                }

            } while (!foundCompleteMessage);

            return null;
        }

        //todo refactor code.
        //todo implement the buffersize on the recieving end of client and the sending end of server

        public async Task sendCom(ISendComModel sendComModel)
        {
            try
            {
                NetworkStream networkStream = sendComModel.GetMemberModel().TcpClient.GetStream();
                char[] messageToSend = sendComModel.ToCharArray();
                string ms = new String(messageToSend);
                byte[] bufferToSend = Encoding.ASCII.GetBytes(messageToSend);
                //Console.WriteLine("THE MESSAGE");
                //Console.WriteLine($"Message {ms}");
                //Console.WriteLine(messageToSend.Length);

                //If the message is smaller than or equal to the specified buffersize, send the message as is to save characters
                if (bufferSize >= messageToSend.Length)
                {
                    await networkStream.WriteAsync(bufferToSend, 0, bufferToSend.Length);
                }
                //If the message is bigger than the specified buffersize, break the message up into smaller pieces 
                else
                {
                    int remainingChars = messageToSend.Length;
                    for (int i = 0; i < (int)Math.Ceiling((decimal)messageToSend.Length / bufferSize); i++)
                    {
                        int hallo = (int)Math.Ceiling((decimal)messageToSend.Length / bufferSize);
                        //Console.WriteLine($"iterations { hallo}");

                        //Console.WriteLine();

                        int roundIndex = (i * bufferSize);

                        int theRealBufferSize;
                        if(remainingChars > bufferSize)
                        {
                            theRealBufferSize = bufferSize;
                        }
                        else
                        {
                            theRealBufferSize = remainingChars;
                        }

                        remainingChars -= theRealBufferSize;
                        Console.WriteLine($"Remaining chars: {remainingChars}");

                        Console.WriteLine("writing");
                        Console.WriteLine(theRealBufferSize);
                        await networkStream.WriteAsync(bufferToSend, roundIndex, theRealBufferSize);

                    }
                }    
            }
            catch (Exception ex)
            {
                Console.WriteLine("TROEP");
                throw ex;
            }
            Console.WriteLine("SENDED");
        }

    }



}
