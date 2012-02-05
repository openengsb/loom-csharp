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
using System.Reflection;

namespace Org.OpenEngSB.Loom.Csharp.Common.Bridge.Interface
{
    public class MethodInvocation
    {
        public string TypeName { get; private set; }
        public string MethodName { get; private set; }
        public Type[] MethodSignature { get; private set; }

        public Type TargetType { get; private set; }
        public MethodInfo Method { get; private set; }

        public object[] Arguments { get; private set; }

        public MethodInvocation(string typeName, string methodName, Type[] methodSignature, object[] arguments = null)
        {
            if (typeName == null)
                throw new ArgumentNullException("typeName");
            if (methodName == null)
                throw new ArgumentNullException("methodName");

            TypeName = typeName;
            MethodName = methodName;

            if (methodSignature == null)
                MethodSignature = Type.EmptyTypes;
            else
                MethodSignature = methodSignature;

            TargetType = Type.GetType(TypeName);
            Method = TargetType.GetMethod(MethodName, MethodSignature);

            if (arguments == null)
                Arguments = new object[0];
            else
                Arguments = arguments;
        }
    }
}
