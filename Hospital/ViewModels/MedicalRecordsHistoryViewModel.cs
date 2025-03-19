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
    class MedicalRecordsHistoryViewModel
    {
        private readonly MedicalRecordManagerModel _medicalRecordManager;

        public ObservableCollection<MedicalRecordJointModel> MedicalRecords { get; private set; }
        public ICommand ViewDetails { get; set; }

        public MedicalRecordsHistoryViewModel(MedicalRecordManagerModel medicalRecordManager)
        {
            _medicalRecordManager = medicalRecordManager;
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
