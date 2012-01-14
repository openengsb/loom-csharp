/***
 * Licensed to the Austrian Association for Software Tool Integration (AASTI)
 * under one or more contributor license agreements. See the NOTICE file
 * distributed with this work for additional information regarding copyright
 * ownership. The AASTI licenses this file to you under the Apache License,
 * Version 2.0 (the "License"); you may not use this file except in compliance
 * with the License. You may obtain a copy of the License at
 * 
 * http://www.apache.org/licenses/LICENSE-2.0
 * 
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 ***/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Remoting.Messaging;

namespace Org.OpenEngSB.Loom.Csharp.Common.Bridge.Interface
{
    public class MessageMethodInvocation : MethodInvocation
    {
        private IMessage _message;
        private LogicalCallContext _callContext;

        /// <summary>
        /// Extracts and interprets fields from "IMessage"
        /// 
        /// Samle IMassage when the method "ToString" is called:
        ///     msg.Properties["__Uri"]	
        ///         null	
        ///         object
        ///     msg.Properties["__MethodName"]	
        ///         "ToString"	
        ///         object {string}
        ///     msg.Properties["__MethodSignature"]	
        ///         {System.Type[0]}	
        ///         object {System.Type[]}
        ///     msg.Properties["__TypeName"]	
        ///         "System.Object, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089"	
        ///         object {string}
        ///     msg.Properties["__Args"]	
        ///         {object[0]}	
        ///         object {object[]}
        ///     msg.Properties["__CallContext"]	
        ///         {System.Runtime.Remoting.Messaging.LogicalCallContext}	
        ///         object {System.Runtime.Remoting.Messaging.LogicalCallContext}
        ///     Type.GetType((string)msg.Properties["__TypeName"]) == typeof(object)	
        ///         true	
        ///         bool
        /// </summary>
        /// <param name="msg"></param>
        public MessageMethodInvocation(IMessage msg) :
            base(msg.Properties["__TypeName"] as string,
                 msg.Properties["__MethodName"] as string,
                 msg.Properties["__MethodSignature"] as Type[],
                 msg.Properties["__Args"] as object[])
        {
            _message = msg;
            _callContext = _message.Properties["__CallContext"] as LogicalCallContext;
        }

        public object Invoke(object targetObject)
        {
            return Method.Invoke(targetObject, Arguments);
        }

        public ReturnMessage ReturnValue(object retValue, params object[] outArgs)
        {
            int outArgsCount = outArgs == null ? 0 : outArgs.Length;

            return new ReturnMessage(retValue, outArgs, outArgsCount, _callContext, _message as IMethodCallMessage);
        }

        public ReturnMessage ThrowException(Exception ex)
        {
            return new ReturnMessage(ex, _message as IMethodCallMessage);
        }
    }
}
