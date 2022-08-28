using System;

namespace Mawa.Drives
{
    public class DriveDiskException : MawaDrivesException
    {

        public DriveDiskException()
        {

        }
        public DriveDiskException(string message) : base(message)
        {

        }
        public DriveDiskException(string message, Exception innerException) : base(message, innerException)
        {

        }

        public override string AppName => "Mawa-Drives-Disk";
    }
}
