using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogonTimes.DateHandling
{
    public interface IDates
    {
        DateTime Now { get; }
        DateTime Today { get; }
    }
}
