using Client.Commands;
using Client.Models;
using Client.Models.ClientCom;
using Client.Services;
using Shared.Commands;
using Shared.ViewModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.ViewModels
{
    public class ClientViewModel : ViewModelBase
	{
		#region View values
		private string clientName;

		public string ClientName
		{
			get { return clientName; }
			set { clientName = value; OnPropertyChanged(); }
		}

		private string serverAddress;

		public string ServerAddress
		{
			get { return serverAddress; }
			set { serverAddress = value; OnPropertyChanged(); }
		}

		private int serverPort;

		public int ServerPort
		{
			get { return serverPort; }
			set { serverPort = value; OnPropertyChanged(); }
		}

		private int bufferSize;

		public int BufferSize
		{
			get { return bufferSize; }
			set { bufferSize = value; OnPropertyChanged(); }
		}


		private string message;

		public string Message
		{
			get { return message; }
			set { message = value; OnPropertyChanged(); }
		}

		private ObservableCollection<ClientChatMessage> messages;

		public ObservableCollection<ClientChatMessage> Messages
		{
			get { return messages; }
			set { messages = value; OnPropertyChanged(); }
		}

		#endregion

		#region View labels and State
		private string connectionLabel = "Start connection with host";

		public string ConnectionLabel
		{
			get { return connectionLabel; }
			set { connectionLabel = value; OnPropertyChanged(); }
		}

		private bool isIdle = true;

		public bool IsIdle
		{
			get { return isIdle; }
			set { isIdle = value; OnPropertyChanged(); OnPropertyChanged("IsActive"); }
		}
		public bool IsActive { get { return !IsIdle; } }

        private bool working = false;

        public bool Working
        {
            get { return working; }
            set { working = value; connectionCommand.RaiseCanExecuteChanged(); }
        }

        private bool operating = false;

        public bool Operating
        {
            get { return operating; }
            set { operating = value; messageCommand.RaiseCanExecuteChanged(); setBufferSizeCommand.RaiseCanExecuteChanged(); }
        }

		#endregion

		#region View operations
		private ConnectionCommand connectionCommand;

		public ConnectionCommand ConnectionCommand
		{
			get { return connectionCommand; }
		}

		public async void ConnectOrDisconnect()
		{
			Working = true;
			try
			{
				if (isIdle)
				{
					await clientService.StartConnectionToHost(clientName, ServerAddress, ServerPort, BufferSize);
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
			string messageToSend = Message;
			string[] charsToRemove = new string[] { "^", "$", "`" };
			foreach (string c in charsToRemove)
			{
				messageToSend = messageToSend.Replace(c, string.Empty);
			}

			Message = messageToSend;

			if(messageToSend.Length >= 1)
				await clientService.SendCom(message);

			Message = "";
		}

		private SetBufferSizeCommand setBufferSizeCommand;

		public SetBufferSizeCommand SetBufferSizeCommand
		{
			get { return setBufferSizeCommand; }
		}

		public void SetBufferSize()
		{
			clientService.SetBufferSize(bufferSize);
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

			connectionCommand = new ConnectionCommand(ConnectOrDisconnect, (_) => !working);
			messageCommand = new MessageCommand(SendMessage, (_) => operating);
			setBufferSizeCommand = new SetBufferSizeCommand(SetBufferSize, (_) => operating);

			clientService = new ClientService(AddMessage, UpdateVMState);

			messages = new ObservableCollection<ClientChatMessage>();

		}
        #endregion

        #region Service operations
        public void AddMessage(string text)
		{
			messages.Add(new ClientChatMessage(text));
			OnPropertyChanged("Messages");
		}

		public void UpdateVMState(bool enable, bool operating)
		{
			Operating = operating;
			if (!operating)
				ConnectionLabel = "Start connection with host";
			else
				ConnectionLabel = "Stop connection with host";
			IsIdle = !operating;
			Working = !enable;
		}
		#endregion
	}
}
