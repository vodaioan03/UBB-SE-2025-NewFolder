using Hospital.DatabaseServices;
using Hospital.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hospital.Managers
{
    class MedicalRecordManagerModel
    {
        public ObservableCollection<MedicalRecordJointModel> s_medicalRecordList { get; private set; }
        private readonly MedicalRecordsDatabaseService _medicalRecordsDBService;

        public MedicalRecordManagerModel(MedicalRecordsDatabaseService dbService)
        {
            _medicalRecordsDBService = dbService;
            s_medicalRecordList = new ObservableCollection<MedicalRecordJointModel>();
        }

        //Create the “LoadMedicalRecordsForPatient” method which receives as input parameter the patientId(int) and will return a Task that loads the list of medical records for a specific patient.
        public async Task LoadMedicalRecordsForPatient(int patientId)
        {
            try
            {
                List<MedicalRecordJointModel> medicalRecords = await _medicalRecordsDBService
                    .GetMedicalRecordsForPatient(patientId)
                    .ConfigureAwait(false);
                s_medicalRecordList = new ObservableCollection<MedicalRecordJointModel>(medicalRecords);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading medical records: {ex.Message}");
                return;
            }
        }
    }
}
