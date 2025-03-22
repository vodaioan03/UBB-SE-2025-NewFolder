using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Hospital.Commands;
using Hospital.Managers;
using Hospital.Models;
using Hospital.Views;

namespace Hospital.ViewModels
{
    public class MedicalRecordsHistoryViewModel
    {
        private readonly MedicalRecordManagerModel _medicalRecordManager;
        private readonly DocumentManagerModel _documentManager;

        public List<MedicalRecordJointModel> MedicalRecords { get; private set; }

        public MedicalRecordsHistoryViewModel(int patientId, MedicalRecordManagerModel medicalRecordManager, DocumentManagerModel documentManager)
        {
            _medicalRecordManager = medicalRecordManager;
            _documentManager = documentManager;
            _medicalRecordManager.LoadMedicalRecordsForPatient(patientId).Wait();
            MedicalRecords = medicalRecordManager.s_medicalRecordList;
        }

        public void ShowMedicalRecordDetails(MedicalRecordJointModel medicalRecord)
        {
            Console.WriteLine($"Opening details for Medical Record ID: {medicalRecord.MedicalRecordId}");
            MedicalRecordDetailsView medicalRecordDetailsView = new MedicalRecordDetailsView(medicalRecord, _documentManager);
            medicalRecordDetailsView.Activate();
        }
    }
}
