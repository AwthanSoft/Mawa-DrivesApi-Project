using System;

using Mawa.Exceptions.Core;

namespace Mawa.Drives
{
    public class MawaDrivesException : AppExceptionCore
    {

        public MawaDrivesException()
        {

        }
        public MawaDrivesException(string message) : base(message)
        {

        }
        public MawaDrivesException(string message, Exception innerException) : base(message, innerException)
        {

        }

        public override string AppName => "Mawa-Drive";
    }
}
