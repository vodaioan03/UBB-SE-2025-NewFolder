using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hospital.Models
{
    public class MedicalRecordJointModel
    {
        public int MedicalRecordId { get; set; }
        public int PatientId { get; set; }
        public string PatientName { get; set; }
        public int DoctorId { get; set; }
        public string DoctorName { get; set; }
        public int ProcedureId { get; set; }
        public string ProcedureName { get; set; }
        public DateTime Date { get; set; }
        public string Conclusion { get; set; }

        public MedicalRecordJointModel(int medicalRecordId, int patientId, string patientName, int doctorId, string doctorName, int procedureId, string procedureName, DateTime date, string conclusion)
        {
            MedicalRecordId = medicalRecordId;
            PatientId = patientId;
            PatientName = patientName;
            DoctorId = doctorId;
            DoctorName = doctorName;
            ProcedureId = procedureId;
            ProcedureName = procedureName;
            Date = date;
            Conclusion = conclusion;
        }
    }
}
