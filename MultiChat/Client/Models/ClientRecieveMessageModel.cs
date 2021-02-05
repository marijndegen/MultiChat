using Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.Models
{
    public class ClientRecieveMessageModel : IMessageModel
    {
        private static byte comType = 4;

        public ClientRecieveMessageModel()
        {

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
