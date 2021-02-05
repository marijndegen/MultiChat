using Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.Models
{
    public class ClientSendHandshakeModel : ISendComModel
    {
        private static byte comType = 1;

        private MemberModel memberModel;

        public ClientSendHandshakeModel(MemberModel memberModel)
        {
            this.memberModel = memberModel;
        }

        public byte GetComType()
        {
            return comType;
        }

        public MemberModel GetMemberModel()
        {
            return memberModel;
        }

        public char[] ToCharArray()
        {
            string name = new String(memberModel.Name);
            string message = $"^^^{comType}~~~{name}$$$";
            return message.ToCharArray();
        }
    }
}
