using Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.Models
{
    public class ClientRecieveMemberListModel : IComModel
    {
        private static byte comType = 2;

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
