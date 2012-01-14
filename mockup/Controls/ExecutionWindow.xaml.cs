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
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Reflection;
using System.Xml.Serialization;
using System.Xml;
using System.Xml.XPath;
using System.IO;
using Org.OpenEngSB.DotNet.Lib.MockupMonitor.Communication;
using System.ServiceModel;

namespace Org.OpenEngSB.DotNet.Lib.MockupMonitor.Controls
{
    /// <summary>
    /// Interaction logic for ResultsWindow.xaml
    /// </summary>
    public partial class ExecutionWindow : Window
    {
        private ClientMethodInfo _methodInfo;

        public ExecutionWindow(ClientMethodInfo cmi, object[] parameters = null)
        {
            _methodInfo = cmi;

            InitializeComponent();

            Title = _methodInfo.ToString();

            //XmlDataProvider provider = new XmlDataProvider();

            //XmlSerializer serializer = new XmlSerializer(method.ReturnType);
            //XmlDocument doc = new XmlDocument();
            //XPathNavigator navi = doc.CreateNavigator();
            //XmlWriter writer = navi.AppendChild();

            //serializer.Serialize(writer, returnValue);
            //provider.Document = doc;

            int index = 0;

            foreach (var param in _methodInfo.MethodInfo.Parameters)
            {
                TextBlock tb = new TextBlock() { Text = string.Format("{0} ({1}):", param.Name, param.ParameterType) };
                spParams.Children.Add(tb);

                UIElement input;
                
                if (param.ParameterType.IsEnum)
                {
                    Type t = param.ParameterType.Convert();
                    input = new ComboBox() { ItemsSource = t.GetEnumValues(), SelectedIndex = 0 };

                    if (parameters != null)
                    {
                        ((ComboBox)input).SelectedItem = Enum.Parse(t, parameters[index].ToString());
                        input.IsEnabled = false;
                    }
                }
                else
                {
                    input = new TextBox() 
                    { 
                        AcceptsReturn = true, 
                        AcceptsTab = true, 
                        TextWrapping = TextWrapping.Wrap, 
                        VerticalScrollBarVisibility = ScrollBarVisibility.Auto, 
                        VerticalAlignment = System.Windows.VerticalAlignment.Stretch, 
                        VerticalContentAlignment = System.Windows.VerticalAlignment.Top 
                    };

                    if (parameters != null)
                    {
                        if (parameters[index] == null)
                            parameters[index] = "<<NULL>>";

                        ((TextBox)input).Text = parameters[index].ToString();
                        ((TextBox)input).IsReadOnly = true;
                    }
                }

                spParams.Children.Add(input);
            }

            if (parameters != null)
            {
                txtRetValue.IsReadOnly = false;
                cmdExec.Visibility = System.Windows.Visibility.Collapsed;
                cmdRet.Visibility = System.Windows.Visibility.Visible;
            }
            //StringWriter sw = new StringWriter();
            //XmlSerializer serializer = new XmlSerializer(method.ReturnType);

            //serializer.Serialize(sw, returnValue);
            //txtRetValue.Text = serializer.ToString();

            //sw.Close();
        }

        private void cmdExec_Click(object sender, RoutedEventArgs e)
        {
            if (((ICommunicationObject)_methodInfo.Callback).State == CommunicationState.Opened)
            {
                int index = 0;
                object[] parameters = new object[_methodInfo.MethodInfo.Parameters.Length];
                var paramDefinitions = _methodInfo.MethodInfo.Parameters;
                Type t;

                foreach (var child in spParams.Children)
                {
                    if (child is TextBox)
                    {
                        TextBox tbChild = (TextBox)child;

                        if (string.IsNullOrEmpty(tbChild.Text))
                            parameters[index] = null;
                        else
                        {
                            t = paramDefinitions[index].ParameterType.Convert();
                            parameters[index] = tbChild.Text.ConvertTo(t);
                        }
                        index++;
                    }
                    else if (child is ComboBox)
                    {
                        parameters[index++] = ((ComboBox)child).SelectedItem.ToString();
                    }
                }

                object ret = _methodInfo.Callback.ExecuteMethod(_methodInfo.ID, parameters);
                XmlSerializer serializer = new XmlSerializer(_methodInfo.MethodInfo.ReturnType.Convert());
                StringWriter sw = new StringWriter();

                serializer.Serialize(sw, ret);
                txtRetValue.Text = sw.ToString();
            }
        }

        private void cmdRet_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
