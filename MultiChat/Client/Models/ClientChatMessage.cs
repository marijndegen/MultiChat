using Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.Models.ClientCom
{
    public class ClientChatMessage :IChatMessage
    {

        private string message;

        public string Message
        {
            get { return message; }
            set { message = value; }
        }


        public ClientChatMessage(string message)
        {
            this.message = message;
        }

        public DateTime getDateTime()
        {
            throw new NotImplementedException();
        }

        public string getName()
        {
            throw new NotImplementedException();
        }

        public string getText()
        {
            throw new NotImplementedException();
        }
    }
}
