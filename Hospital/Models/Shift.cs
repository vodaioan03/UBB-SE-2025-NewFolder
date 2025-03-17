using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hospital.Models
{
    public class Shift
    {
        public int ShiftId { get; set; }
        public DateTime DateTime { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan Endtime { get; set; }

        public Shift(int shiftId, DateTime dateTime, TimeSpan startTime, TimeSpan endtime)
        {
            ShiftId = shiftId;
            DateTime = dateTime;
            StartTime = startTime;
            Endtime = endtime;
        }
    }
}
