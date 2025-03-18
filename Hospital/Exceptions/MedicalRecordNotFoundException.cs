using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hospital.Exceptions
{
    class MedicalRecordNotFoundException : Exception
    {
        public MedicalRecordNotFoundException(string message) : base(message) { }
    }
}
