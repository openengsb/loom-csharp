﻿#region Copyright
// <copyright file="IMarshaller.cs" company="OpenEngSB">
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

namespace Org.Openengsb.Loom.CSharp.Bridge.Implementation.Communication
{
    /// <summary>
    /// This interface specifies operations for marshalling objects
    /// in an arbitrary format. This is usually necessary for serializing
    /// and sending objects over any communication channels.
    /// </summary>
    public interface IMarshaller
    {
        #region Methods
        /// <summary>
        /// Serialize a object
        /// </summary>
        /// <param name="obj">Object to serialize</param>
        /// <returns>Serialized string</returns>
        string MarshallObject(object obj);

        /// <summary>
        /// Deserialze a object
        /// </summary>
        /// <param name="jsonText">Json object in string format</param>
        /// <param name="objectType">Type of the object to return</param>
        /// <returns>The deserialized object</returns>
        object UnmarshallObject(string jsonText, Type objectType);

        /// <summary>
        /// Deserialze a object
        /// </summary>
        /// <typeparam name="ObjectTyp">Type of the object</typeparam>
        /// <param name="jsonText">Json object in string format</param>
        /// <returns>The deserialized object</returns>
        ObjectTyp UnmarshallObject<ObjectTyp>(string jsonText);
        #endregion
    }
}