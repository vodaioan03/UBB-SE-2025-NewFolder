using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hospital.Models
{
    public class MedicalRecord
    {
        public int MedicalRecordId { get; set; }
        public int PatientId { get; set; }
        public int DoctorId { get; set; }
        public int ProcedureId { get; set; }
        public string Conclusion { get; set; }


        public MedicalRecord(int medicalRecordId, int patientId, int doctorId, int procedureId, string conclusion)
        {
            MedicalRecordId = medicalRecordId;
            PatientId = patientId;
            DoctorId = doctorId;
            ProcedureId = procedureId;
            Conclusion = conclusion;
        }
    }
}
