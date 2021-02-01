﻿using System;
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

		private NetworkStream networkStream;

		public NetworkStream NetworkStream
		{
			get { return networkStream; }
			set { networkStream = value; }
		}



		public MemberModel(string name, NetworkStream networkStream)
		{
			guid = Guid.NewGuid();
			this.name = name.ToCharArray();
			this.networkStream = networkStream;
		}

		public void Send(SendComModel message, int bufferSize)
		{
			throw new NotImplementedException();
		}

	}
}
