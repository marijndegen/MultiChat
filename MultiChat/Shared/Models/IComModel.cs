using System;
using System.Collections.Generic;
using System.Text;

namespace Shared.Models
{
    public interface IComModel
    {
        byte GetComType();

        char[] ToCharArray();
    }
}
