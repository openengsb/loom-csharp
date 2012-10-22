using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;

namespace Org.Openengsb.Loom.CSharp.Bridge.Implementation.Common
{
    public class NetMethodCallMessage : IMethodCallMessage
    {
        private Object[] args;
        private String methodname;
        public NetMethodCallMessage(Object[] args)
        {
            this.args = args;
        }
        public object GetInArg(int argNum)
        {
            throw new NotImplementedException();
        }

        public string GetInArgName(int index)
        {
            throw new NotImplementedException();
        }

        public int InArgCount
        {
            get { throw new NotImplementedException(); }
        }

        public object[] InArgs
        {
            get { throw new NotImplementedException(); }
        }

        public int ArgCount
        {
            get { return args.Length; }
        }

        public object[] Args
        {
            get { return args; }
        }

        public object GetArg(int argNum)
        {
            throw new NotImplementedException();
        }

        public string GetArgName(int index)
        {
            throw new NotImplementedException();
        }

        public bool HasVarArgs
        {
            get { throw new NotImplementedException(); }
        }

        public LogicalCallContext LogicalCallContext
        {
            get { throw new NotImplementedException(); }
        }

        public System.Reflection.MethodBase MethodBase
        {
            get { throw new NotImplementedException(); }
        }

        public string MethodName
        {
            get { return methodname; }
        }

        public object MethodSignature
        {
            get { throw new NotImplementedException(); }
        }

        public string TypeName
        {
            get { throw new NotImplementedException(); }
        }

        public string Uri
        {
            get { throw new NotImplementedException(); }
        }

        public System.Collections.IDictionary Properties
        {
            get { throw new NotImplementedException(); }
        }
    }
}
