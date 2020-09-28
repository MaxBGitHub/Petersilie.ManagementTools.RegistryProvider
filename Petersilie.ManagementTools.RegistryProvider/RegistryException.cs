using System;
using System.Collections.Specialized;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Petersilie.ManagementTools.RegistryProvider
{
    public class UnexpectedRegistryException : Exception
    {
        /// <summary>
        /// The error code that was caught.
        /// </summary>
        public int ErrorCode { get; }
        /// <summary>
        /// The method in which the error occured.
        /// </summary>
        public string Method { get; }
        /// <summary>
        /// The input parameters which where passed to the method.
        /// </summary>
        public NameValueCollection Parameters { get; }


        public UnexpectedRegistryException() 
            : base()
        {
        }


        public UnexpectedRegistryException(string message)
            : base(message)
        {
        }


        public UnexpectedRegistryException(string message, Exception inner)
            : base(message, inner)
        {
        }


        /// <summary>
        /// Throws a new <see cref="UnexpectedRegistryException"/>
        /// </summary>
        /// <param name="errCode">Error code that was caught</param>
        /// <param name="method">Method which raised the error</param>
        /// <param name="parameters">Method parameters</param>
        public UnexpectedRegistryException(int errCode, string method, 
            NameValueCollection parameters)
        : this()
        {
            ErrorCode   = errCode;
            Method      = method;
            Parameters  = parameters;
        }


        /// <summary>
        /// Throws a new <see cref="UnexpectedRegistryException"/>
        /// </summary>
        /// <param name="errCode">Error code that was caught</param>
        /// <param name="method">Method which raised the error</param>
        /// <param name="parameters">Method parameters</param>
        /// <param name="message">Message to display</param>
        public UnexpectedRegistryException(int errCode, string method, string message,
            NameValueCollection parameters)
        : this(message)
        {
            ErrorCode = errCode;
            Method = method;
            Parameters = parameters;
        }
    }
}
