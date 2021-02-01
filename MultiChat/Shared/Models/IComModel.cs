using System;
using System.Collections.Generic;
using System.Text;

namespace Shared.Models
{
    public interface IComModel
    {
        byte GetType();

        char[] toCharArray();
    }
}
