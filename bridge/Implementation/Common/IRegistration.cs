using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Org.Openengsb.Loom.CSharp.Bridge.Interface.Common.xlink;

namespace Org.Openengsb.Loom.CSharp.Bridge.Interface.Common
{
    public interface IRegistration
    {
        Boolean Registered { get; }
        void CreateRemoteProxy();
        void DeleteRemoteProxy();
        XLinkTemplate ConnectToXLink(string toolName, ModelToViewsTuple[] modelsToViews);
        void DisconnectFromXLink();
        void RegisterConnector(String serviceId);
        void UnRegisterConnector();
        String ServiceID { get; }
        void Stop();
    }
}