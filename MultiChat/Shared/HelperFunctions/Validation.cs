using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace Shared.HelperFunctions
{
    public class Validation
    {
        public struct UserInput
        {

            private IPAddress address;
            private int port;
            private int bufferSize;

            public IPAddress Address
            {
                get { return address; }
            }


            public int Port
            {
                get { return port; }
            }


            public int BufferSize
            {
                get { return bufferSize; }
            }

            public UserInput(IPAddress address, int port, int bufferSize)
            {
                this.address = address;
                this.port = port;
                this.bufferSize = bufferSize;
            }


        }

        public static UserInput ValidateUserInput(string serverAddress, int port, int bufferSize)
        {
            IPAddress address = IPAddress.Parse(serverAddress);
            if (port <= 0 || port >= 65535)
                throw new Exception($"Port: {port} not valid");
            if (bufferSize <= 0 || bufferSize >= 10000)
                throw new Exception("Buffersize should be between 1 and 10.000");
            return new UserInput(address, port, bufferSize);
        }
    }
}
