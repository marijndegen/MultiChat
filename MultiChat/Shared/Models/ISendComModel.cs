﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Shared.Models
{
    public interface ISendComModel : IComModel
    {
        MemberModel GetMemberModel();
    }
}
