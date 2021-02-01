﻿using Client.Commands;
using Client.Models;
using Shared.Commands;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.ViewModels
{
    public class ClientViewModel : INotifyPropertyChanged
    {
        #region View values
        private string clientName;

		public string ClientName
		{
			get { return clientName; }
			set { clientName = value; }
		}

		private string serverAddress;

		public string ServerAddress
		{
			get { return serverAddress; }
			set { serverAddress = value; }
		}

		private int serverPort;

		public int ServerPort
		{
			get { return serverPort; }
			set { serverPort = value; }
		}

		private int bufferSize;

		public int BufferSize
		{
			get { return bufferSize; }
			set { bufferSize = value; OnPropertyChanged("BufferSize"); }
		}

		private string message;

		public string Message
		{
			get { return message; }
			set { message = value; }
		}


		#endregion

		#region Viewlabels and State
		private string connectionLabel = "Start connection with host";

		public string ConnectionLabel
		{
			get { return connectionLabel; }
			set { connectionLabel = value; OnPropertyChanged("ConnectionLabel"); }
		}

		private bool isIdle = true;

		public bool IsIdle
		{
			get { return isIdle; }
			set { isIdle = value; }
		}

		#endregion

		private ConnectionCommand connectionCommand;

		public ConnectionCommand ConnectionCommand
		{
			get { return connectionCommand; }
		}

		private MessageCommand messageCommand;

		public MessageCommand MessageCommand
		{
			get { return messageCommand; }
		}


		#region INotifyPropertyChanged
		public event PropertyChangedEventHandler PropertyChanged;

		private void OnPropertyChanged(string propertyName)
		{
			if (PropertyChanged != null)
			{
				PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
			}
		}
		#endregion

		private ClientService clientService;

		public ClientViewModel()
		{
			ClientName = "Client 1";
			ServerAddress = "127.0.0.1";
			ServerPort = 9000;
			BufferSize = 1024;
			Message = "Hello World!";

			connectionCommand = new ConnectionCommand(StartConnection);
			messageCommand = new MessageCommand(SendMessage);	
			clientService = new ClientService(AddMessage, UpdateVMState);

		}


		public async void StartConnection()
		{
			if (isIdle)
			{
				//TODO pass clientname
				await clientService.StartConnectionToHost(ServerAddress, ServerPort, BufferSize);
			}
			else
			{

			}
		}

		public async void SendMessage() 
		{
			await clientService.SendCom(message);
		}

        #region Service functions
        public void AddMessage(string text)
		{
			
		}

		public void UpdateVMState(bool enable, bool operating)
		{
			connectionCommand.Enable = enable;
			if (!operating)
				ConnectionLabel = "Start connection with host";
			else
				ConnectionLabel = "Stop connection with host";
			IsIdle = !operating;
		}
        #endregion




    }
}
