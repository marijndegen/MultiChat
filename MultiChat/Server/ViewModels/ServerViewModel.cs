﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.ComponentModel;
using Server.Models;
using System.Collections.ObjectModel;
using Server.Commands;

namespace Server.ViewModels
{
    public class ServerViewModel : INotifyPropertyChanged
    {
        #region View values
        private string serverAdress;

		public string ServerAddress
		{
			get { return serverAdress; }
			set { serverAdress = value; }
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
			set { bufferSize = value; }
		}

		private ObservableCollection<Message> messages;

		public ObservableCollection<Message> Messages
		{
			get { return messages; }
			set { messages = value; OnPropertyChanged("EmployeesList"); }
		}

		#endregion

		#region View Labels and state
		private string hostingLabel = "Start hosting";

		public string HostingLabel
		{
			get { return hostingLabel; }
			set { hostingLabel = value; OnPropertyChanged("HostingLabel"); }
		}

		private bool isIdle = true;

		public bool IsIdle
		{
			get { return isIdle; }
			set { isIdle = value; OnPropertyChanged("IsIdle"); }
		}

		#endregion

		#region Connection Operation
		private ConnectionCommand connectionCommand;

		public ConnectionCommand ConnectionCommand
		{
			get { return connectionCommand; }
		}

		public async void Connect()
		{
			try
			{
				if (!serverService.IsHosting)
					await serverService.StartHosting(ServerAddress, ServerPort, BufferSize);
				else
					serverService.StopHosting();
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
			}

		}

		#endregion

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

		private ServerService serverService;

		public ServerViewModel()
		{
			ServerAddress = "127.0.0.1";
			ServerPort = 9000;
			BufferSize = 1024;

			connectionCommand = new ConnectionCommand(Connect);
			serverService = new ServerService(AddMessage, SetStatus);
		}

		public void AddMessage(string message)
		{
			messages.Add(new Message(message));
			OnPropertyChanged("EmployeesList");
		}

		public void SetStatus(bool enable, bool hosting)
		{
			connectionCommand.Enable = enable;
			if (!hosting)
				HostingLabel = "Start Hosting";
			else
				HostingLabel = "Stop hosting";
			IsIdle = !hosting;
		}
	}
}
