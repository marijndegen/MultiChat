using Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Models
{
    class ServerRecieveHandshakeModel : IComModel
    {
        private static byte comType = 1;

        public ServerRecieveHandshakeModel()
        {

        }

        public byte GetComType()
        {
            return comType;
        }

        public char[] ToCharArray()
        {
            throw new NotImplementedException();
        }
    }
}
