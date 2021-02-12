using Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.Models
{
    public class ClientSendMessageModel : ISendMessageModel
    {
        private static byte comType = 3;

        private string message;

        public string Message
        {
            get { return message; }
            set { message = value; }
        }

        private MemberModel memberModel;

        public ClientSendMessageModel(string message, MemberModel memberModel)
        {
            this.message = message;
            this.memberModel = memberModel;
        }

        public byte GetComType()
        {
            return comType;
        }

        public DateTime GetDateTime()
        {
            throw new NotImplementedException();
        }

        public MemberModel GetMemberModel()
        {
            return memberModel;
        }

        public char[] GetText()
        {
            throw new NotImplementedException();
        }

        public char[] ToCharArray()
        {
            string com = $"^^^{comType}~{message}$$$";
            return com.ToCharArray();
        }
    }
}
