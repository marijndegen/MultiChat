using System;
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
        #region View Members
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
			set { messages = value; }
		}

		//TODO should be in the service
		private bool isHosting;

		public bool IsHosting
		{
			set { isHosting = value; OnPropertyChanged("HostingLabel"); }
		}

		private string hostingLabel;

		public string HostingLabel
		{
			get { return !isHosting ? "Start connection" : "Stop connection"; }
		}



		#endregion

		#region Connection Command
		private ConnectionCommand connectionCommand;

		public ConnectionCommand ConnectionCommand
		{
			get { return connectionCommand; }
		}

		public async void Connect()
		{
			Console.WriteLine("connecting...");
			connectionCommand.Enable = false;
			await Task.Delay(3 * 1000);
			connectionCommand.Enable = true;
			Console.WriteLine("connected!");
			IsHosting = !isHosting;
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

		public ServerViewModel()
		{
			ServerAddress = "127.0.0.1";
			ServerPort = 9000;
			BufferSize = 1024;

			connectionCommand = new ConnectionCommand(Connect);
		}
	}
}
