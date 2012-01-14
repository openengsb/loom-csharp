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
using System.Windows;
using System.Windows.Controls;
using System.Collections;

namespace Org.OpenEngSB.DotNet.Lib.MockupMonitor.Controls
{
    public class ObjectTreeView : TreeView, IDisposable
    {
        public object SerializedObject
        {
            get { return (object)GetValue(SerializedObjectProperty); }
            set { SetValue(SerializedObjectProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SerializedObject.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SerializedObjectProperty =
            DependencyProperty.Register("SerializedObject", typeof(object), typeof(ObjectTreeView), new UIPropertyMetadata(null, SerializedObjectPropertyChangedCallback));

        private static void SerializedObjectPropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ObjectTreeView me = d as ObjectTreeView;

            if (me != null)
            {
                me.RebuildTreeView();
            }
        }

        private void RebuildTreeView()
        {
            this.DisposeChildren();

            if (SerializedObject != null)
            {
                Items.Add(new ObjectTreeViewItem(SerializedObject, SerializedObject.GetType()) { IsExpanded = true });
            }
        }

        public void Dispose()
        {
            this.DisposeChildren();
        }
    }
}
