using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;

namespace Shared.Models
{
    public class MemberModel
    {
		private Guid guid;

		public Guid Guid
		{
			get { return guid; }
			set { guid = value; }
		}

		private char[] name;

		public char[] Name
		{
			get { return name; }
			set { name = value; }
		}

		private TcpClient tcpClient;

		public TcpClient TcpClient
		{
			get { return tcpClient; }
			set { tcpClient = value; }
		}



		public MemberModel(string name, TcpClient tcpClient)
		{
			guid = Guid.NewGuid();
			this.name = name.ToCharArray();
			this.tcpClient = tcpClient;
		}

	}
}
