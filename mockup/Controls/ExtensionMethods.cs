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
using System.Windows.Controls;
using System.Reflection;
using System.Collections;
using System.Xml.Serialization;
using System.IO;

namespace Org.OpenEngSB.DotNet.Lib.MockupMonitor.Controls
{
    public static class ExtensionMethods
    {
        public static void DisposeChildren(this ItemsControl me)
        {
            foreach (object o in me.Items)
            {
                if (o is IDisposable)
                    ((IDisposable)o).Dispose();
            }

            me.Items.Clear();
        }

        public static bool IsIndexProperty(this PropertyInfo me)
        {
            return !me.GetIndexParameters().IsNullOrEmpty();
        }

        public static bool IsNullOrEmpty<T>(this IEnumerable<T> me)
        {
            return me == null || me.Count() == 0;
        }

        public static Type GetRealType(this IEnumerable me)
        {
            if (me == null)
                return null;

            Type t = me.GetType();

            if (t.IsArray)
                return t.GetElementType();

            while (t != typeof(IEnumerable))
            {
                if (t == typeof(IEnumerable<>))
                    return t.GetGenericTypeDefinition().GetGenericArguments()[0];

                t = t.BaseType;
            }

            return typeof(object);
        }

        public static T ConvertTo<T>(this string me)
        {
            return (T)me.ConvertTo(typeof(T));
        }

        public static object ConvertTo(this string me, Type type)
        {
            if (type == typeof(string))
                return me;

            Type converter = typeof(Convert);

            foreach (var method in converter.GetMethods(BindingFlags.Static))
            {
                var parameters = method.GetParameters();

                if (!parameters.IsNullOrEmpty() && parameters.Length == 1 && parameters[1].ParameterType == typeof(string))
                {
                    if (method.ReturnType == type)
                    {
                        return method.Invoke(null, new object[] { me });
                    }
                }
            }

            XmlSerializer serializer = new XmlSerializer(type);
            
            // just test if the object is ok
            serializer.Deserialize(new StringReader(me));

            return me;
        }
    }
}
