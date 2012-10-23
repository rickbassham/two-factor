using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace TwoFactor
{
    public class OneTimePasswordException : Exception
    {
        public OneTimePasswordException()
            : base()
        {
        }

        public OneTimePasswordException(string message)
            : base(message)
        {
        }

        public OneTimePasswordException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        public OneTimePasswordException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
