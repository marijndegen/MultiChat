using Shared.HelperFunctions;
using Shared.Models;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static Shared.HelperFunctions.Validation;
using Shared.ExtensionMethods;
using System.Windows;

namespace Server.Services
{
    public class ServerService
    {
        #region Delegates
        private Action<IChatMessage> addMessage;
        private Action<bool, bool> updateVMModel;
        #endregion

        #region TCP vars and constructor
        private TcpListener tcpListener;
        private ServerComService serverComService;
        private Task hostingTask;
        private CancellationTokenSource hostingTokenSource;
        private CancellationToken hostingToken;

        public ServerService(Action<IChatMessage> addMessage, Action<bool, bool> updateVMState)
        {
            this.addMessage = addMessage;
            this.updateVMModel = updateVMState;
            serverComService = new ServerComService();


        }

        #endregion

        #region Starthosting, Stophosting, BufferSize
        public void StartHosting(string serverAddress, int serverPort, int bufferSize)
        {

            try
            {
                updateVMModel(false, false);
                UserInput input = Validation.ValidateUserInput(serverAddress, serverPort, bufferSize);
                serverComService.IsHosting = true;
                serverComService.BufferSize = bufferSize;

                tcpListener = new TcpListener(input.Address, input.Port);
                tcpListener.Start();

                hostingTokenSource = new CancellationTokenSource();
                hostingToken = hostingTokenSource.Token;

                hostingTask = Task.Run(() => serverComService.Hosting(tcpListener, hostingToken), hostingToken);
                updateVMModel(true, true);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                updateVMModel(true, false);
                throw ex;
            }

        }

        public void StopHosting()
        {
            updateVMModel(false, false);

            serverComService.IsHosting = false;
            hostingTokenSource.Cancel();
            tcpListener.Stop();
            tcpListener = null;

            updateVMModel(true, false);
        }

        public void SetBufferSize(int bufferSize)
        {
            if (ValidateBufferSize(bufferSize))
                serverComService.BufferSize = bufferSize;
            else
                MessageBox.Show("Invalid bufferSize");
        }
        #endregion

    }

    class ServerComService
    {
        #region Com vars
        private bool isHosting;

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


        private ConcurrentDictionary<Guid, MemberModel> memberModels;

        public ConcurrentDictionary<Guid, MemberModel> MemberModels
        {
            get { return memberModels; }
            set { memberModels = value; }
        }

        #endregion

        public async Task Hosting(TcpListener tcpListener, CancellationToken hostingToken)
        {
            Console.WriteLine("hosting");
            memberModels = new ConcurrentDictionary<Guid, MemberModel>();

            byte newUserCount = 0;

            TcpClient tcpClient = null;
            try
            {
                while (isHosting && !hostingToken.IsCancellationRequested)
                {
                    hostingToken.ThrowIfCancellationRequested();
                    tcpClient = await tcpListener.AcceptTcpClientAsync();

                    newUserCount++;

                    MemberModel memberModel = new MemberModel($"New user: {newUserCount}", tcpClient);
                    memberModels.TryAdd(memberModel.Guid, memberModel);

                    Task messageListener = Task.Run(() => ComListener(memberModel, hostingToken), hostingToken);
                }
            }
            catch (Exception ex)
            {
                isHosting = false;
                Console.WriteLine($"In hosting: {ex.Message}");
                throw ex;
            }
            finally
            {
                foreach (KeyValuePair<Guid, MemberModel> entry in memberModels)
                {
                    MemberModel memberModel = entry.Value;
                    if (memberModel.TcpClient.GetType() == typeof(TcpClient))
                        memberModel.TcpClient.Close();
                }
            }
        }

        public async Task ComListener(MemberModel memberModel, CancellationToken hostingToken)
        {
            string clientName = new String(memberModel.Name);
            Console.WriteLine($"We got a new client with the name: {clientName}");

            //int bufferSize = 1024;
            string message;
            byte[] buffer = new byte[bufferSize];

            NetworkStream networkStream = memberModel.TcpClient.GetStream();

            try
            {
                while (isHosting)
                {
                    hostingToken.ThrowIfCancellationRequested();
                    IComModel messageModel = await DecodeCom(networkStream);
                }
                Console.WriteLine("Client disconnected");
            }
            catch (Exception ex)
            {
                memberModels.TryRemove(memberModel.Guid, out MemberModel memberModelToDelete);
                memberModel.TcpClient.Close();
                memberModel = null;
                Console.WriteLine($"In ComListener: {ex.Message}");
                throw;
            }
        }

        private async Task<IComModel> DecodeCom(NetworkStream networkStream)
        {
            //int bufferSize = 1;
            string completeMessage = "";
            string message;
            byte[] buffer = new byte[bufferSize];
            bool foundCompleteMessage = false;

            //todo these two lines of code should be producing a chatmessage class by decoding the content.

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




            //todo this whole region has to be implemented in a function that will broadcast the message using the ISendMessageModel.cs
            //foreach (KeyValuePair<Guid, MemberModel> entry in memberModels)
            //{

            //    MemberModel broadCastMember = entry.Value;
            //    if (broadCastMember.TcpClient.GetState() == TcpState.Established)
            //    {
            //        NetworkStream networkStreamToBroadCastTo = broadCastMember.TcpClient.GetStream();
            //        byte[] bufferToSend = Encoding.ASCII.GetBytes(message);
            //        await networkStreamToBroadCastTo.WriteAsync(bufferToSend, 0, bufferToSend.Length);
            //    }
            //}

            //for debugging purposes





            //string message = "";
            ////string memmoryMessage = "";
            ////string newMessage = "";
            //char[] decoded;
            //byte[] buffer = new byte[bufferSize];
            //bool messageCompleted = false;

            //while (!messageCompleted)
            //{
            //    int readBytes = await networkStream.ReadAsync(buffer, 0, bufferSize);
            //    message = Encoding.ASCII.GetString(buffer, 0, readBytes);
            //    //memmoryMessage = $"{memmoryMessage}{newMessage}";


            //    if (message.Length > 0)
            //    {
            //        Console.Write("Recieved: ");
            //        Console.WriteLine(message.ToString());
            //    }

            //    //if (memmoryMessage.Length > 6)
            //    //{
            //    //    Console.WriteLine(memmoryMessage.Substring(memmoryMessage.Length - 4, 3));
            //    //}

            //}

            return null;



        }

    }
}
