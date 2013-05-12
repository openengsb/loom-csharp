using OpenEngSBCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Org.Openengsb.Loom.CSharp.Bridge.Implementation.Common
{
    /// <summary>
    /// Is derived from the OpenEngSBModel on the openengsb-framework
    /// </summary>
    public interface OpenEngSBModel
    {

        List<openEngSBModelEntry> openEngSBModelTail { get; set; }

    }
}
