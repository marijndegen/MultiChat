﻿using Shared.Models;
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

        public ServerSendMemberListModel()
        {

        }

        public byte GetComType()
        {
            return comType;
        }

        public MemberModel GetMemberModel()
        {
            throw new NotImplementedException();
        }

        public char[] ToCharArray()
        {
            throw new NotImplementedException();
        }
    }
}
