﻿using System;
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

        public static UserInput ClientValidateUserInput(string clientName, string serverAddress, int port, int bufferSize)
        {
            if (clientName.Length < 1)
                throw new Exception("Fill in a clientName");
                   
            IPAddress address = IPAddress.Parse(serverAddress);
            if (!ValidatePort(port))
                throw new Exception($"Port: {port} not valid");
            if (!ValidateBufferSize(bufferSize))
                throw new Exception("Buffersize should be between 1 and 10.000");
            return new UserInput(address, port, bufferSize);
        }

        public static UserInput ServerValidateUserInput(string serverAddress, int port, int bufferSize)
        {
            IPAddress address = IPAddress.Parse(serverAddress);
            if(!ValidatePort(port))
                throw new Exception($"Port: {port} not valid");
            if (!ValidateBufferSize(bufferSize))
                throw new Exception("Buffersize should be between 1 and 10.000");
            return new UserInput(address, port, bufferSize);
        }

        private static bool ValidatePort(int port)
        {
            return !(port <= 0 || port >= 65535);
        }

        public static bool ValidateBufferSize(int bufferSize)
        {
            return !(bufferSize <= 0 || bufferSize >= 10000);
        }
    }
}
