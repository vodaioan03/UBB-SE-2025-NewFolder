using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hospital.Models
{
    public class Document
    {
        public int DocumentId { get; set; }
        public int MedicalRecordId { get; set; }
        public string File { get; set; }


        public Document(int documentId, int medicalRecordId, string file)
        {
            DocumentId = documentId;
            MedicalRecordId = medicalRecordId;
            File = file;
        }
    }
}
