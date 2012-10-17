using System;

namespace Org.Openengsb.Loom.CSharp.Bridge.Implementation.Common
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