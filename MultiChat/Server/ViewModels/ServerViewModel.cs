using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.ComponentModel;
using Server.Models;
using System.Collections.ObjectModel;
using Shared.Commands;
using Server.Models.ServerCom;
using Shared.Models;
using Server.Services;
using Shared.HelperFunctions;
using static Shared.HelperFunctions.Validation;
using System.Windows;

namespace Server.ViewModels
{
    public class ServerViewModel : INotifyPropertyChanged
    {
        #region View values
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
			set { bufferSize = value; }
		}

		#endregion

		#region View labels and state
		private string connectionLabel = "Start hosting";

		public string ConnectionLabel
		{
			get { return connectionLabel; }
			set { connectionLabel = value; OnPropertyChanged("ConnectionLabel"); }
		}

		private bool isIdle = true;

		public bool IsIdle
		{
			get { return isIdle; }
			set { isIdle = value; OnPropertyChanged("IsIdle"); }
		}
		#endregion

		#region View operations
		private ConnectionCommand connectionCommand;

		public ConnectionCommand ConnectionCommand
		{
			get { return connectionCommand; }
		}

		public void ConnectOrDisconnect()
		{
			try
			{
				if (isIdle)
				{ 
					serverService.StartHosting(serverAddress, serverPort, bufferSize);
				}
				else
				{
					serverService.StopHosting();
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
			}

		}

		private SetBufferSizeCommand setBufferSizeCommand;

		public SetBufferSizeCommand SetBufferSizeCommand
		{
			get { return setBufferSizeCommand; }
		}

		public void SetBufferSize()
		{
			Console.WriteLine($"set buffersize: {bufferSize}");
			serverService.SetBufferSize(bufferSize);

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

        #region Service and constructor

        private ServerService serverService;

		public ServerViewModel()
		{
			ServerAddress = "127.0.0.1";
			ServerPort = 9000;
			BufferSize = 1024;

			connectionCommand = new ConnectionCommand(ConnectOrDisconnect);
			setBufferSizeCommand = new SetBufferSizeCommand(SetBufferSize);
			serverService = new ServerService(UpdateVMState);
		}
        #endregion

        #region Service operations
		public void UpdateVMState(bool enable, bool operating)
		{
			connectionCommand.Enable = enable;
			setBufferSizeCommand.Enable = operating;
			if (!operating)
				ConnectionLabel = "Start Hosting";
			else
				ConnectionLabel = "Stop hosting";
			IsIdle = !operating;
		}
        #endregion
    }
}
