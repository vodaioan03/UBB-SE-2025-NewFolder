using System;

namespace Hospital.Exceptions
{
    public class DatabaseOperationException : Exception
    {
        public DatabaseOperationException(string message) : base(message) { }
    }
}
