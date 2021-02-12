using Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Models
{
    public class ServerRecieveHandshakeModel : IComModel
    {
        private static byte comType = 1;

        private string name;

        public string Name
        {
            get { return name; }
        }


        public ServerRecieveHandshakeModel(string name)
        {
            this.name = name;
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
