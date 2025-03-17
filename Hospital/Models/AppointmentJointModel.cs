using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hospital.Models
{
    public class AppointmentJointModel
    {
        public int AppointmentId { get; set; }
        public bool Finished { get; set; }
        public DateTime Date { get; set; }
        public int DepartmentId { get; set; }
        public string DepartmentName { get; set; }
        public int DoctorId { get; set; }
        public string DoctorName { get; set; }
        public int PatientId { get; set; }
        public string PatientName { get; set; }
        public int ProcedureId { get; set; }
        public string ProcedureName { get; set; }
        public TimeSpan ProcedureDuration { get; set; }

        public AppointmentJointModel(int appointmentId, bool finished, DateTime date, int departmentId, string departmentName, int doctorId, string doctorName, int patientId, string patientName, int procedureId, string procedureName, TimeSpan procedureDuration)
        {
            AppointmentId = appointmentId;
            Finished = finished;
            Date = date;
            DepartmentId = departmentId;
            DepartmentName = departmentName;
            DoctorId = doctorId;
            DoctorName = doctorName;
            PatientId = patientId;
            PatientName = patientName;
            ProcedureId = procedureId;
            ProcedureName = procedureName;
            ProcedureDuration = procedureDuration;
        }
    }
}
