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
using System.ComponentModel;
using System.Collections;

namespace Org.OpenEngSB.DotNet.Lib.MockupMonitor.Controls
{
    public class ObjectTreeViewItem : TreeViewItem, IDisposable
    {
        private DependencyPropertyDescriptor _isExpandendDpd;

        public object ContainingObject { get; private set; }
        public Type BaseType { get; private set; }
        
        public Type ContainingType 
        { 
            get 
            {
                if (ContainingObject != null)
                    return ContainingObject.GetType();
                else
                    return null;
            } 
        }

        public ObjectTreeViewItem(object containingObject, Type baseType, string header = null)
        {
            Header = header;
            ContainingObject = containingObject;
            BaseType = baseType;

            _isExpandendDpd = DependencyPropertyDescriptor.FromProperty(TreeViewItem.IsExpandedProperty, GetType());
            _isExpandendDpd.AddValueChanged(this, IsExpandendChangedEventHandler); 
        }

        private void IsExpandendChangedEventHandler(object sender, EventArgs e)
        {
            if (Items.Count == 0 && ContainingObject != null)
            {
                foreach (var prop in ContainingType.GetProperties())
                {
                    if (!prop.IsIndexProperty())
                    {
                        object val;

                        try
                        {
                            val = prop.GetValue(ContainingObject, null);
                        }
                        catch (Exception ex)
                        {
                            val = ex;
                        }

                        Items.Add(new ObjectTreeViewItem(val, prop.PropertyType, prop.Name));
                    }
                }

                IEnumerable enumerableObject = ContainingObject as IEnumerable;

                if (enumerableObject != null)
                {
                    Type enumerableType = enumerableObject.GetRealType();
                    int index = 0;

                    foreach (object o in enumerableObject)
                    {
                        Items.Add(new ObjectTreeViewItem(o, enumerableType, string.Format("[{0}]", index++)));
                    }
                }
            }
        }

        public void Dispose()
        {
            if (_isExpandendDpd != null)
            {
                _isExpandendDpd.RemoveValueChanged(this, IsExpandendChangedEventHandler);
                _isExpandendDpd = null;

                ContainingObject = null;
                this.DisposeChildren();
            }
        }
    }
}
