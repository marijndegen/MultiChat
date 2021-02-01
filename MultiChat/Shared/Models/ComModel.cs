using System;
using System.Collections.Generic;
using System.Text;

namespace Shared.Models
{
    public interface ComModel
    {
        byte GetType();

        char[] toCharArray();
    }
}
