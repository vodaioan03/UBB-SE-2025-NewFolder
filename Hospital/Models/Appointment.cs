using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hospital.Models
{
    public class Appointment
    {
        public int AppointmentId { get; set; }
        public int DoctorId { get; set; }
        public int PatientId { get; set; }
        public DateTime DateAndTime { get; set; }
        public bool Finished { get; set; }
        public int ProcedureId { get; set; }

        public Appointment(int appointmentId, int doctorId, int patientId, DateTime dateAndTime, bool finished, int procedureId)
        {
            AppointmentId = appointmentId;
            DoctorId = doctorId;
            PatientId = patientId;
            DateAndTime = dateAndTime;
            Finished = finished;
            ProcedureId = procedureId;
        }
    }
}
