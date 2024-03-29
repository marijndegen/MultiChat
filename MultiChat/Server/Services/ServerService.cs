﻿using Shared.HelperFunctions;
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
using Server.Models;

namespace Server.Services
{
    public class ServerService
    {
        #region Delegates
        private Action<bool, bool> updateVMModel;
        #endregion

        #region TCP vars and constructor
        private TcpListener tcpListener;
        private ServerComService serverComService;
        private Task hostingTask;
        private CancellationTokenSource hostingTokenSource;
        private CancellationToken hostingToken;

        public ServerService(Action<bool, bool> updateVMState)
        {
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
                UserInput input = Validation.ServerValidateUserInput(serverAddress, serverPort, bufferSize);
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
                Console.WriteLine($"An error occoured in hosting: {ex.Message}");
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

            NetworkStream networkStream = memberModel.TcpClient.GetStream();

            try
            {
                while (isHosting)
                {
                    hostingToken.ThrowIfCancellationRequested();
                    IComModel comModel = await DecodeCom(networkStream);

                    if(comModel is ServerRecieveHandshakeModel)
                    {
                        ServerRecieveHandshakeModel serverRecieveHandshakeModel = (ServerRecieveHandshakeModel)comModel;
                        Console.WriteLine(serverRecieveHandshakeModel.Name.ToCharArray());
                        memberModel.Name = serverRecieveHandshakeModel.Name.ToCharArray();
                        await BroadCastClientNames();
                    } else if (comModel is ServerRecieveMessageModel)
                    {
                        ServerRecieveMessageModel serverRecieveMessageModel = (ServerRecieveMessageModel)comModel;
                        Console.WriteLine(serverRecieveMessageModel.Message.ToCharArray());
                        await broadCastMessage(serverRecieveMessageModel);

                    }
                }
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
                    foundCompleteMessage = completeMessage.Substring(completeMessage.Length - 3, 3) == "$$$";
                }

                if (foundCompleteMessage)
                {
                    Console.WriteLine("the complete message");
                    Console.WriteLine(completeMessage);
                }

            } while (!foundCompleteMessage);

            string strippedMessage = completeMessage.Trim('^', '$');
            string[] protocolCom = strippedMessage.Split('~');

            if(protocolCom[0] == "1")
            {
                return new ServerRecieveHandshakeModel(protocolCom[1]);
            } else if (protocolCom[0] == "3")
            {
                return new ServerRecieveMessageModel(protocolCom[1]);
            }
            else
            {
                return null;
            }

        }
        
        private async Task BroadCastClientNames()
        {
            List<string> names = new List<string>();
            

            foreach (KeyValuePair<Guid, MemberModel> entry in memberModels)
            {
                MemberModel member = entry.Value;
                names.Add(new String(member.Name));
            }

            string[] arrayNames = names.ToArray();

            try
            {
                foreach (KeyValuePair<Guid, MemberModel> entry in memberModels)
                {

                    MemberModel broadCastMember = entry.Value;
                    if (broadCastMember.TcpClient.GetState() == TcpState.Established)
                    {
                        ISendComModel sendComModel = new ServerSendMemberListModel(arrayNames, broadCastMember);
                        await sendCom(sendComModel);
                    }
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            
        }

        private async Task broadCastMessage(ServerRecieveMessageModel serverRecieveMessageModel)
        {
            try
            {
                foreach (KeyValuePair<Guid, MemberModel> entry in memberModels)
                {

                    MemberModel broadCastMember = entry.Value;
                    if (broadCastMember.TcpClient.GetState() == TcpState.Established)
                    {
                        ISendComModel sendComModel = new ServerBroadcastMessageModel(serverRecieveMessageModel.Message, broadCastMember);
                        await sendCom(sendComModel);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private async Task sendCom(ISendComModel sendComModel)
        {
            try
            {
                NetworkStream networkStream = sendComModel.GetMemberModel().TcpClient.GetStream();
                char[] messageToSend = sendComModel.ToCharArray();
                string ms = new String(messageToSend);
                byte[] bufferToSend = Encoding.ASCII.GetBytes(messageToSend);
                
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

                        int roundIndex = (i * bufferSize);

                        int theRealBufferSize;
                        if (remainingChars > bufferSize)
                        {
                            theRealBufferSize = bufferSize;
                        }
                        else
                        {
                            theRealBufferSize = remainingChars;
                        }

                        remainingChars -= theRealBufferSize;
                        await networkStream.WriteAsync(bufferToSend, roundIndex, theRealBufferSize);

                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception in sendCom");
                throw ex;
            }
            Console.WriteLine("SENDED");
        }

    }
}
