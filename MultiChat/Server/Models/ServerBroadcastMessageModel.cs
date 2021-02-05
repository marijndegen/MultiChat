﻿using Shared.Models;
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

        public ServerBroadcastMessageModel()
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

        public MemberModel GetMemberModel()
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
