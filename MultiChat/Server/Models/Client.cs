using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Server.Models
{
    public class Client
    {
        private NetworkStream networkStream;

        public Client(NetworkStream networkStream)
        {
            this.networkStream = networkStream;
        }
    }
}
