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

namespace Server.Services
{
    public class ServerService
    {
        #region Service vars
        private int bufferSize;

        public int BufferSize
        {
            get { return bufferSize; }
            set { bufferSize = value; }
        }


        #endregion

        #region Delegates
        private Action<IChatMessage> AddMessage;
        private Action<bool, bool> UpdateVMState;
        #endregion

        #region TCP vars and constructor
        private TcpListener tcpListener;
        private ServerComService serverComService;
        private Task comListner;
        private CancellationTokenSource hostingTokenSource;
        private CancellationToken hostingToken;

        public ServerService(Action<IChatMessage> addMessage, Action<bool, bool> updateVMState)
        {
            this.AddMessage = addMessage;
            this.UpdateVMState = updateVMState;
        }

        #endregion

        #region Starthosting and Stophosting controls
        public void StartHosting(UserInput input)
        {

            try
            {
                UpdateVMState(false, false);
                serverComService = new ServerComService();
                this.bufferSize = input.BufferSize;
                tcpListener = new TcpListener(input.Address, input.Port);
                tcpListener.Start();

                hostingTokenSource = new CancellationTokenSource();
                hostingToken = hostingTokenSource.Token;

                comListner = Task.Run(() => serverComService.Hosting(tcpListener, hostingToken), hostingToken);
                UpdateVMState(true, true);
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public void StopHosting()
        {
            hostingTokenSource.Cancel();
            UpdateVMState(false, false);
            tcpListener.Stop();
            tcpListener = null;
            UpdateVMState(true, false);
        }
        #endregion

    }

    class ServerComService
    {
        private bool isHosting;

        public bool IsHosting
        {
            get { return isHosting; }
            set { isHosting = value; }
        }

        private ConcurrentDictionary<Guid, MemberModel> memberModels;

        public ConcurrentDictionary<Guid, MemberModel> MemberModels
        {
            get { return memberModels; }
            set { memberModels = value; }
        }


        public async Task Hosting(TcpListener tcpListener, CancellationToken hostingToken)
        {
            Console.WriteLine("hosting");

            isHosting = true;
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

                    Task messageListener = Task.Run(() => ComListener(memberModel));
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
                if (tcpClient.GetType() == typeof(TcpClient))
                    tcpClient.Close();
            }
        }

        //TODO place this method in member model class.
        public async Task ComListener(MemberModel memberModel)
        {
            Console.WriteLine($"We got a new client with the name: {memberModel.Name}");
            int bufferSize = 1024;
            string message;
            byte[] buffer = new byte[bufferSize];

            NetworkStream networkStream = memberModel.TcpClient.GetStream();

            try
            {
                while (isHosting)
                {
                    //todo these two lines of code should be producing a chatmessage class by decoding the content.
                    int readBytes = await networkStream.ReadAsync(buffer, 0, bufferSize);
                    message = Encoding.ASCII.GetString(buffer, 0, readBytes);

                    //todo this whole region has to be implemented in a function that will broadcast the message using the ISendMessageModel.cs
                    foreach (KeyValuePair<Guid, MemberModel> entry in memberModels)
                    {
                        
                        MemberModel broadCastMember = entry.Value;
                        if(broadCastMember.TcpClient.GetState() == TcpState.Established)
                        {
                            byte[] bufferToSend = Encoding.ASCII.GetBytes(message);
                            networkStream.Write(bufferToSend, 0, bufferToSend.Length);
                        }
                        else
                        {
                            throw new Exception($"A client {memberModel.Name}");
                        }
                    }

                    //for debugging purposes
                    if (message.Length > 0)
                    {
                        Console.Write("Recieved: ");
                        Console.WriteLine(message);
                    }
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

        public ServerComService()
        {

        }
    }
}
