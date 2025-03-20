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

namespace Hospital.ViewModels
{
    public class MedicalRecordsHistoryViewModel
    {
        private readonly MedicalRecordManagerModel _medicalRecordManager;

        public List<MedicalRecordJointModel> MedicalRecords { get; private set; }
        public ICommand ViewDetails { get; set; }

        public MedicalRecordsHistoryViewModel(MedicalRecordManagerModel medicalRecordManager)
        {
            _medicalRecordManager = medicalRecordManager;
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
            }
        }
    }
}
