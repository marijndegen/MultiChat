using Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Models
{
    class ServerSendMemberListModel : ISendComModel
    {
        private static byte comType = 2;

        private string[] names;

        public string[] Names
        {
            get { return names; }
            set { names = value; }
        }

        private MemberModel memberModel;

        public ServerSendMemberListModel(string[] names, MemberModel memberModel)
        {
            this.names = names;
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
            string implodedNames = String.Join("~", names);
            Console.WriteLine("implodednames");
            Console.WriteLine();
            implodedNames = implodedNames.ToString();
            string message = $"^^^{comType}~{implodedNames}$$$";
            return message.ToCharArray();
        }
    }
}
