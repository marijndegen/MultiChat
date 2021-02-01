using System;
using System.Collections.Generic;
using System.Text;

namespace Shared.Models
{
    public interface IChatMessage
    {
        DateTime getDateTime();

        string getName();

        string getText();
    }
}
