using Client.Commands;
using Client.Models;
using Client.Models.ClientCom;
using Client.Services;
using Shared.Commands;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
			set { bufferSize = value; }
		}

		private string message;

		public string Message
		{
			get { return message; }
			set { message = value; }
		}

		private ObservableCollection<ClientChatMessage> messages;

		public ObservableCollection<ClientChatMessage> Messages
		{
			get { return messages; }
			set { messages = value; OnPropertyChanged("Messages"); }
		}


		#endregion

		#region View labels and State
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
			set { isIdle = value; OnPropertyChanged("IsIdle"); OnPropertyChanged("IsActive"); }
		}
		public bool IsActive { get { return !IsIdle; } }

		#endregion

		#region View operations
		private ConnectionCommand connectionCommand;

		public ConnectionCommand ConnectionCommand
		{
			get { return connectionCommand; }
		}

		public async void ConnectOrDisconnect()
		{
			try
			{
				if (isIdle)
				{
					await clientService.StartConnectionToHost(ServerAddress, ServerPort, BufferSize);
				}
				else
				{
					clientService.StopConnectionToHost();
				}
			}
			catch (Exception ex)
			{
				Console.Write("Connect or Disconnect: ");
				Console.WriteLine(ex.Message);
			}

		}

		private MessageCommand messageCommand;

		public MessageCommand MessageCommand
		{
			get { return messageCommand; }
		}

		public async void SendMessage()
		{
			await clientService.SendCom(message);
		}

		private SetBufferSizeCommand setBufferSizeCommand;

		public SetBufferSizeCommand SetBufferSizeCommand
		{
			get { return setBufferSizeCommand; }
		}

		public async void SetBufferSize()
		{
			clientService.SetBufferSize(bufferSize);
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
        private ClientService clientService;

		public ClientViewModel()
		{
			ClientName = "Client 1";
			ServerAddress = "127.0.0.1";
			ServerPort = 9000;
			BufferSize = 1024;
			Message = "Hello World!";

			connectionCommand = new ConnectionCommand(ConnectOrDisconnect);
			messageCommand = new MessageCommand(SendMessage);
			setBufferSizeCommand = new SetBufferSizeCommand(SetBufferSize);

			clientService = new ClientService(AddMessage, UpdateVMState);

		}
        #endregion

        #region Service operations
        public void AddMessage(string text)
		{
			
		}

		public void UpdateVMState(bool enable, bool operating)
		{
			connectionCommand.Enable = enable;
			messageCommand.Enable = operating;
			setBufferSizeCommand.Enable = operating;
			if (!operating)
				ConnectionLabel = "Start connection with host";
			else
				ConnectionLabel = "Stop connection with host";
			IsIdle = !operating;
		}
        #endregion
    }
}
