using Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Models
{
    public class ServerRecieveMessageModel : IMessageModel
    {
        private static byte comType = 3;

        private string message;

        public string Message
        {
            get { return message; }
            set { message = value; }
        }


        public ServerRecieveMessageModel(string message)
        {
            this.message = message;
        }

        public byte GetComType()
        {
            return comType;
        }

        public DateTime GetDateTime()
        {
            throw new NotImplementedException();
        }

        public char[] GetText()
        {
            throw new NotImplementedException();
        }

        public char[] ToCharArray()
        {
            throw new NotImplementedException();
        }
    }
}
