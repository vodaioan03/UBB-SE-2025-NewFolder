using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hospital.Exceptions
{
    class DocumentNotFoundException : Exception
    {
        public DocumentNotFoundException(string message) : base(message) {}
    }
}
