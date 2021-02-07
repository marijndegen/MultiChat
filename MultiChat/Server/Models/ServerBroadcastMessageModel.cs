using Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Models
{
    //todo implement interfaces on each class, make a listner and a sender on each class.
    public class ServerBroadcastMessageModel : ISendMessageModel
    {
        private static byte comType = 4;

        private string message;

        public string Message
        {
            get { return message; }
        }

        private MemberModel memberModel;

        public MemberModel MemberModel
        {
            get { return memberModel; }
            set { memberModel = value; }
        }



        public ServerBroadcastMessageModel(string message, MemberModel memberModel)
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
