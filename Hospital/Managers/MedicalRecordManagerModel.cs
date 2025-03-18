using Hospital.DatabaseServices;
using Hospital.Exceptions;
using Hospital.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection.Metadata;
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

        
        public MedicalRecordJointModel GetMedicalRecordById(int medicalRecordId)
        {
            try
            {
                MedicalRecordJointModel medicalRecord = _medicalRecordsDBService
                    .RetrieveMedicalRecordById(medicalRecordId)
                    .Result;
                return medicalRecord;
            }
            catch (MedicalRecordNotFoundException ex)
            {
                throw new MedicalRecordNotFoundException("No medical record found for the given id.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading medical record: {ex.Message}");
                return null;
            }
        }
    }
}
