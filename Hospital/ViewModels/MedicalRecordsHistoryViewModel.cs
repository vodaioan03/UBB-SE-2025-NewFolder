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
        public ICommand ViewDetails { get; set; }

        public MedicalRecordsHistoryViewModel(MedicalRecordManagerModel medicalRecordManager, DocumentManagerModel documentManager)
        {
            _medicalRecordManager = medicalRecordManager;
            _documentManager = documentManager;
            // patient id will be substituted with the logged in user's id after login is implemented
            int patientId = 2;
            _medicalRecordManager.LoadMedicalRecordsForPatient(patientId).Wait();
            MedicalRecords = medicalRecordManager.s_medicalRecordList;
            ViewDetails = new RelayCommand(ShowMedicalRecordDetails);
        }

        public void ShowMedicalRecordDetails(Object obj)
        {
            if (obj is MedicalRecordJointModel medicalRecord)
            {
                Console.WriteLine($"Opening details for Medical Record ID: {medicalRecord.MedicalRecordId}");
                MedicalRecordDetailsView medicalRecordDetailsView = new MedicalRecordDetailsView(medicalRecord, _documentManager);
                medicalRecordDetailsView.Activate();
            }
        }
    }
}
