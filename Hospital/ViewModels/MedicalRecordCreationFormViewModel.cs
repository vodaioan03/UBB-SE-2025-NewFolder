using Hospital.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hospital.Managers;
using Hospital.Commands;

namespace Hospital.ViewModels
{
    class MedicalRecordCreationFormViewModel
    {
        private readonly MedicalRecordManagerModel _medicalRecordManager;
        private readonly DocumentManagerModel _documentManager;

        int maxDocs = 10;
        public ObservableCollection<string> Documents { get; private set; }


        public MedicalRecordCreationFormViewModel(MedicalRecordManagerModel medicalRecordManager, DocumentManagerModel documentManagerModel)
        {
            _medicalRecordManager = medicalRecordManager;
            _documentManager = documentManagerModel;
            Documents = new ObservableCollection<string>();
        }

        public async Task<int> CreateMedicalRecord(AppointmentJointModel detailedAppointment, string conclusion)
        {
            try
            {
                detailedAppointment.Finished = true;
                detailedAppointment.Date = DateTime.Now;
                

                int medicalRecordId = await _medicalRecordManager.CreateMedicalRecord(detailedAppointment);
                return medicalRecordId;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error creating medical record: {ex.Message}");
                return -1;
            }
        }

        public void AddDocument(int medicalRecordId, string path)
        {
            Document doc = new Document(0, medicalRecordId, path);
            if (Documents.Count < maxDocs)
            {
                Documents.Add(path);
                _documentManager.AddDocumentToMedicalRecord(doc);
            }
        }
    }
}
