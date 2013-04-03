using Org.Openengsb.Loom.CSharp.Bridge.Implementation.Exceptions;
using Org.Openengsb.Loom.CSharp.Bridge.Interface.ExceptionHandling;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BridgeTests.TestExceptionHandlers
{
    [ExcludeFromCodeCoverageAttribute()]
    public class TestCustomExceptionHandler : ABridgeExceptionHandling
    {
        /// <summary>
        /// Used for to know the test execution (test cases)
        /// </summary>
        public static int executions = 0;

        /// <summary>
        /// Directly forwards the exception to user user of the .Net Bridge
        /// </summary>
        /// <param name="exception"></param>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override Object HandleException(Exception exception, params Object[] obj)
        {
            executions++;
            if (executions < 2)
            {
                return Invoke(obj);
            }
            return "TestCase";
        }
    }
}