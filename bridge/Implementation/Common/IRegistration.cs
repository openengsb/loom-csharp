﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Implementation.Common
{
    public interface IRegistration
    {
        Boolean Registered { get; }
        void CreateRemoteProxy();
        void DeleteRemoteProxy();
        void RegisterConnector(String serviceId);
        void UnRegisterConnector();
        String ServiceID { get; }
        void Stop();
    }
}