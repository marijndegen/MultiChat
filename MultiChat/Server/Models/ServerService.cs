using Shared.Models;
using System;
using System.Collections.Concurrent;
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

        #region Service vars
        private ConcurrentDictionary<Guid, MemberModel> memberModels;

        public ConcurrentDictionary<Guid, MemberModel> MemberModels
        {
            get { return memberModels; }
            set { memberModels = value; }
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
            memberModels = new ConcurrentDictionary<Guid, MemberModel>();
        }

        #region Starthosting and Stophosting controls
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
        #endregion

        public async Task Hosting()
        {
            //int bufferSize = 1024;
            //string message = "";
            //byte[] buffer = new byte[bufferSize];

            byte newUserCount = 0;
            try
            {
                while (isHosting)
                {
                    
                    TcpClient tcpClient = await tcpListener.AcceptTcpClientAsync();
                    NetworkStream networkStream = tcpClient.GetStream();

                    newUserCount++;

                    MemberModel memberModel = new MemberModel($"New user: {newUserCount}", networkStream);
                    memberModels.TryAdd(memberModel.Guid, memberModel);

                    //MemberModel inDir = memberModels[memberModel.Guid];

                    Task messageListener = Task.Run(() => ComListener(memberModel));

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
        public async Task ComListener(MemberModel memberModel)
        {
            bool longtime = true;

            Console.WriteLine("Com listener");
            int bufferSize = 1024;
            string message = "";
            byte[] buffer = new byte[bufferSize];

            try
            {
                while (isHosting)
                {
                    int readBytes = await memberModel.NetworkStream.ReadAsync(buffer, 0, bufferSize);
                    message = Encoding.ASCII.GetString(buffer, 0, readBytes);

                    foreach (KeyValuePair<Guid, MemberModel> entry in memberModels)
                    {
                        MemberModel broadCastMember = entry.Value;
                        //if (entry.Value.Guid != memberModel.Guid && longtime)
                        if(longtime)
                        {
                            //longtime = false;
                            byte[] bufferToSend = Encoding.ASCII.GetBytes(message);
                            broadCastMember.NetworkStream.Write(bufferToSend, 0, bufferToSend.Length);
                        }

                    }

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
