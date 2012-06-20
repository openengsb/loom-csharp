using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Implementation.Common
{
    public interface IRegistration:IStoppable
    {
        Boolean Registered { get; }
    }
}
