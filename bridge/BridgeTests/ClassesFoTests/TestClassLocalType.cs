﻿#region Copyright
// <copyright file="TestClassLocalType.cs" company="OpenEngSB">
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
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace BridgeTests.TestClasses
{
    [XmlTypeAttribute(Namespace = "http://example.domain.test.org")]
    [ExcludeFromCodeCoverageAttribute]
    public class TestClassLocalType
    {
        #region Public Methods
        public void hasArraySpecified(String[] test, Boolean testSpecified)
        {
        }

        public void hasDoubleSpecified(Double test, Boolean testSpecified)
        {
        }

        public void hasFloatSpecified(float test, String testSpecified)
        {
        }

        public void hasIntSpecified(int test, Boolean testSpecified)
        {
        }

        public void hasStringSpecified(String test, String testSpecified)
        {
        }
        #endregion
    }
}