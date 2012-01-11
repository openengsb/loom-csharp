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

namespace Org.OpenEngSB.DotNet.Lib.MockupMonitor.Communication
{
    public class SimpleType
    {
        //
        // Summary:
        //     Gets the assembly-qualified name of the System.Type, which includes the name
        //     of the assembly from which the System.Type was loaded.
        //
        // Returns:
        //     The assembly-qualified name of the System.Type, which includes the name of
        //     the assembly from which the System.Type was loaded, or null if the current
        //     instance represents a generic type parameter.
        public string AssemblyQualifiedName { get; set; }
        //
        // Summary:
        //     Gets the type from which the current System.Type directly inherits.
        //
        // Returns:
        //     The System.Type from which the current System.Type directly inherits, or
        //     null if the current Type represents the System.Object class or an interface.
        public SimpleType BaseType { get; set; }
        //
        // Summary:
        //     Gets a value indicating whether the current System.Type object has type parameters
        //     that have not been replaced by specific types.
        //
        // Returns:
        //     true if the System.Type object is itself a generic type parameter or has
        //     type parameters for which specific types have not been supplied; otherwise,
        //     false.
        public bool ContainsGenericParameters { get; set; }
        //
        // Summary:
        //     Gets the fully qualified name of the System.Type, including the namespace
        //     of the System.Type but not the assembly.
        //
        // Returns:
        //     The fully qualified name of the System.Type, including the namespace of the
        //     System.Type but not the assembly; or null if the current instance represents
        //     a generic type parameter, an array type, pointer type, or byref type based
        //     on a type parameter, or a generic type that is not a generic type definition
        //     but contains unresolved type parameters.
        public string FullName { get; set; }
        //
        // Summary:
        //     Gets a combination of System.Reflection.GenericParameterAttributes flags
        //     that describe the covariance and special constraints of the current generic
        //     type parameter.
        //
        // Returns:
        //     A bitwise combination of System.Reflection.GenericParameterAttributes values
        //     that describes the covariance and special constraints of the current generic
        //     type parameter.
        //
        // Exceptions:
        //   System.InvalidOperationException:
        //     The current System.Type object is not a generic type parameter. That is,
        //     the System.Type.IsGenericParameter property returns false.
        //
        //   System.NotSupportedException:
        //     The invoked method is not supported in the base class.
        //public virtual GenericParameterAttributes GenericParameterAttributes { get; }
        //
        // Summary:
        //     Gets a value indicating whether the System.Type is an array.
        //
        // Returns:
        //     true if the System.Type is an array; otherwise, false.
        public bool IsArray { get; set; }
        //
        // Summary:
        //     Gets a value indicating whether the System.Type is a class; that is, not
        //     a value type or interface.
        //
        // Returns:
        //     true if the System.Type is a class; otherwise, false.
        public bool IsClass { get; set; }
        //
        // Summary:
        //     Gets a value indicating whether the current System.Type represents an enumeration.
        //
        // Returns:
        //     true if the current System.Type represents an enumeration; otherwise, false.
        public bool IsEnum { get; set; }
        //
        // Summary:
        //     Gets a value indicating whether the current type is a generic type.
        //
        // Returns:
        //     true if the current type is a generic type; otherwise, false.
        public bool IsGenericType { get; set; }
        //
        // Summary:
        //     Gets a value indicating whether the System.Type is an interface; that is,
        //     not a class or a value type.
        //
        // Returns:
        //     true if the System.Type is an interface; otherwise, false.
        public bool IsInterface { get; set; }
        //
        // Summary:
        //     Gets a value indicating whether the System.Type is one of the primitive types.
        //
        // Returns:
        //     true if the System.Type is one of the primitive types; otherwise, false.
        public bool IsPrimitive { get; set; }
        //
        // Summary:
        //     Gets a value indicating whether the System.Type is serializable.
        //
        // Returns:
        //     true if the System.Type is serializable; otherwise, false.
        public bool IsSerializable { get; set; }
        //
        // Summary:
        //     Gets a value indicating whether the System.Type is a value type.
        //
        // Returns:
        //     true if the System.Type is a value type; otherwise, false.
        public bool IsValueType { get; set; }
        //
        // Summary:
        //     Gets the namespace of the System.Type.
        //
        // Returns:
        //     The namespace of the System.Type; null if the current instance has no namespace
        //     or represents a generic parameter.
        public string Namespace { get; set; }

        public Type Convert()
        {
            Type ret = Type.GetType(FullName);

            if (ret == null)
            {
                // example: AssemblyQualifiedName = "Org.OpenEngSB.DotNet.Lib.MockupMonitor.MainWindow, MockupMonitor, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null"
                // AssemblyQualifiedName.Split(',')[1] = " MockupMonitor"
                // AssemblyQualifiedName.Split(',')[1].Trim() = "MockupMonitor"
                string assemblyName = AssemblyQualifiedName.Split(',')[1].Trim() + ".dll";

                ret = Assembly.LoadFile(System.IO.Path.GetFullPath(assemblyName)).GetType(FullName);
            }

            return ret;
        }

        public override string ToString()
        {
            return FullName;
        }
    }
}
