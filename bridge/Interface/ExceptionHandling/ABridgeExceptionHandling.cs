#region Copyright
// <copyright file="ABridgeExceptionHandling.cs" company="OpenEngSB">
// Licensed to the Austrian Association for Software Tool Integration (AASTI)
// under one or more contributor license agreements. See the NOTICE file
// distributed with this work for additional information regarding copyright
// ownership. The AASTI licenses this file to you under the Apache License,
// Version 2.0 (the "License"); you may not use this file except in compliance
// with the License. You may obtain a copy of the License at
//
// http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// </copyright>
#endregion
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Dispatcher;
using System.Text;
using Org.Openengsb.Loom.CSharp.Bridge.Implementation.Common;

namespace Org.Openengsb.Loom.CSharp.Bridge.Interface.ExceptionHandling
{
    public abstract class ABridgeExceptionHandling
    {
        #region Propreties

        /// <summary>
        /// When the stop method has been invoked. The ExceptionHandler should stop.
        /// </summary>
        public Boolean Stop
        {
            get;
            set;
        }

        public ABridgeExceptionHandling()
        {
            Stop = false;
        }

        #endregion

        #region Abstract Methods

        public abstract Object HandleException(Exception exception, params object[] parameters);

        #endregion

        #region Event And Delegat types
        public delegate Object ThrowExceptionMethod(params object[] obj);

        public event ThrowExceptionMethod Changed;

        #endregion

        #region Methods

        /// <summary>
        /// Invokes the method, that triggered the exception again
        /// </summary>
        /// <param name="obj">(Failed) Method parameter</param>
        /// <returns>Method result</returns>
        protected Object Invoke(Object[] obj)
        {
            if (Stop)
            {
                throw new InvalidOperationException("The Exceptionhandler has been stopped manually");
            }

            if (Changed == null)
            {
                throw new InvalidOperationException("An exception occured without a mehtod call");
            }
            
            return Changed.Invoke(obj);
        }

        #endregion
    }
}