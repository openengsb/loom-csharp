using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Implementation.Communication;
using System.Reflection;
using Implementation.Communication.Json;

namespace Implementation.Common.RemoteObjects
{
    public abstract class IMethodCall
    {
        private Boolean? wrapped = null;
        /// <summary>
        /// Fully qualified class names of the arguments.
        /// </summary>
        public IList<string> classes { get; set; }
        /// <summary>
        /// Name of the method to be called.
        /// </summary>
        public string methodName { get; set; }

        /// <summary>
        /// Arguments of the call.
        /// </summary>
        public IList<object> args { get; set; }

        #region Public Method
        public Boolean isWrapped()
        {
            if (!wrapped.HasValue) 
                ConvertWrapper(new JsonMarshaller());
            return wrapped.Value;
        }
        /// <summary>
        /// Checks if the message is a wrapped method and set it when it  is one
        /// </summary>        
        public void ConvertWrapper(IMarshaller marshaller)
        {
            if (wrapped != null) return;
            List<Object> result = new List<Object>();
            int fails = 0;
            for (int i = 0; i < args.Count; i++)
            {
                try
                {
                    OpenEngSBModelWrapper element = (OpenEngSBModelWrapper)marshaller.UnmarshallObject(args[i].ToString(), typeof(OpenEngSBModelWrapper));
                    if (element != null)
                    {
                        result.Add(element);
                    }
                    else
                    {
                        fails++;
                        result.Add(args[i]);
                    }
                }
                catch (Exception ex)
                {
                    if (!ex.Source.ToString().ToUpper().Contains("JSON")) throw ex;
                    result.Add(args[i]);
                    fails++;
                }                
            }
            wrapped = fails < args.Count;
            if (wrapped.Value) args = result;           
        }
        /// <summary>
        /// Converts an object to an object to a OpenEngSBWrapper
        /// </summary>
        /// <param name="obj">object to convert</param>
        /// <param name="pos">position of the object in the list</param>
        /// <returns></returns>
        private Object ConvertToOpenEngsModelWrapper(Object obj,int pos)
        {
            OpenEngSBModelWrapper wrapper = new OpenEngSBModelWrapper();
            IList<OpenEngSBModelEntry> elements = new List<OpenEngSBModelEntry>();
            foreach (PropertyInfo info in obj.GetType().GetProperties())
            {
                elements.Add(OpenEngSBModelEntry.getInstance(info.Name, info.GetValue(obj, null).ToString(), info.GetType().ToString()));
            }
            wrapper.entries = elements;
            wrapper.modelClass = classes[pos];
            return wrapper;
        }
        #endregion

    }
}
