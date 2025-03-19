using System;

namespace Hospital.Exceptions
{
    public class CancellationNotAllowedException : Exception
    {
        public CancellationNotAllowedException(string message) : base(message) { }
    }
}
