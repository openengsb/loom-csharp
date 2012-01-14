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
using System.Threading;
using System.ServiceModel;
using Org.OpenEngSB.DotNet.Lib.MockupMonitor.Communication;
using System.Collections.ObjectModel;
using Org.OpenEngSB.DotNet.Lib.MockupMonitor.Controls;

namespace Org.OpenEngSB.DotNet.Lib.MockupMonitor
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, ConcurrencyMode = ConcurrencyMode.Single)]
    public partial class MainWindow : Window, IMonitorService
    {
        //private Dictionary<IMonitorServiceCallback, Dictionary<int, SimpleMethodInfo>> _methods = new Dictionary<IMonitorServiceCallback, Dictionary<int, SimpleMethodInfo>>();
        private ObservableCollection<ClientMethodInfo> _methods = new ObservableCollection<ClientMethodInfo>();
        private Dictionary<IMonitorServiceCallback, int> _clientIDs = new Dictionary<IMonitorServiceCallback,int>();
        private int _nextClientID = 1;

        public MainWindow()
        {
            InitializeComponent();

            CommunicationService.StartService(this);

            lstMethods.ItemsSource = _methods;
        }

        protected override void OnClosed(EventArgs e)
        {
            CommunicationService.StopService();

            base.OnClosed(e);
        }

        public string TestMethod(string value)
        {
            return "test " + value;
        }

        public void AddMethod(int id, SimpleMethodInfo method, MethodType type)
        {
            IMonitorServiceCallback channel = GetCallbackChannel();

            if (!_clientIDs.ContainsKey(channel))
            {
                AddStatusText(string.Format("New Client (#{0})", _nextClientID));
                _clientIDs.Add(channel, _nextClientID++);
            }

            AddStatusText(string.Format("New Method (#{0}, {1})", _clientIDs[channel], id));
            _methods.Add(new ClientMethodInfo() { Callback = channel, ID = id, MethodInfo = method, ClientID = _clientIDs[channel], Type = type });
        }

        public void RemoveMethod(int id)
        {
            throw new NotImplementedException();
        }

        public object MethodExecuted(int id, object[] parameters)
        {
            var cmi = _methods.FirstOrDefault((o) => { return o.ID == id; });

            ExecutionWindow ew = new ExecutionWindow(cmi, parameters);
            
            ew.ShowDialog();

            string ret = ew.txtRetValue.Text;

            if (string.IsNullOrEmpty(ret) || ret.Trim() == string.Empty)
                ret = null;

            return ret;
        }

        public void Unsubscribe()
        {
            IMonitorServiceCallback clb = GetCallbackChannel();

            _clientIDs.Remove(clb);

            for (int i = _methods.Count - 1; i >= 0; i--)
            {
                if (_methods[i].Callback.Equals(clb))
                    _methods.RemoveAt(i);
            }
        }

        private IMonitorServiceCallback GetCallbackChannel()
        {
            return OperationContext.Current.GetCallbackChannel<IMonitorServiceCallback>();
        }

        private void AddStatusText(string text)
        {
            tbStatus.Text = string.Format("{0}; {1}", text, tbStatus.Text);
        }

        private void lstMethods_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            ClientMethodInfo cmi = lstMethods.SelectedItem as ClientMethodInfo;

            if (cmi != null && cmi.Type == MethodType.Registered)
            {
                new ExecutionWindow(cmi).Show();
            }
        }
    }
}
