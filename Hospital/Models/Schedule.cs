using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hospital.Models
{
    public class Schedule
    {
        public int DoctorId { get; set; }
        public int ShiftId { get; set; }

        public Schedule(int doctorId, int shiftId)
        {
            DoctorId = doctorId;
            ShiftId = shiftId;
        }
    }
}
