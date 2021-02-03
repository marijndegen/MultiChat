//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Net.Sockets;
//using System.Text;
//using System.Threading.Tasks;

//namespace Server.Models
//{
//    public class Member
//    {
//        private NetworkStream networkStream;

//        public Member(NetworkStream networkStream)
//        {
//            this.networkStream = networkStream;
//        }

//        public void sendMessage(string message, int bufferSize)
//        {
//            Console.WriteLine("writing message");
//            byte[] buffer = Encoding.ASCII.GetBytes(message);
//            networkStream.Write(buffer, 0, buffer.Length);
//        }
//    }
//}
