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
                s_medicalRecordList.Clear();
                foreach (MedicalRecordJointModel medicalRecord in medicalRecords)
                {
                    s_medicalRecordList.Add(medicalRecord);
                }
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

        public async Task<int> CreateMedicalRecord(AppointmentJointModel detailedAppointment)
        {
            // Create a new record with a default ID (0 or another placeholder)
            MedicalRecord medicalRecord = new MedicalRecord(
                0, // default or placeholder ID
                detailedAppointment.PatientId,
                detailedAppointment.DoctorId,
                detailedAppointment.ProcedureId,
                string.Empty
            );

            // Insert the record into the database and get the new ID
            int newMedicalRecordId = await _medicalRecordsDBService.AddMedicalRecord(medicalRecord)
                                                  .ConfigureAwait(false);

            // Optionally, update the record's ID property if you need to use the record further
            if (newMedicalRecordId > 0)
            {
                medicalRecord.MedicalRecordId = newMedicalRecordId;
                // And if required, update the ObservableCollection accordingly:
                // s_medicalRecordList.Add(new MedicalRecordJointModel(...));

                s_medicalRecordList.Add(GetMedicalRecordById(newMedicalRecordId));
            }

            return newMedicalRecordId;
        }

        public async Task LoadMedicalRecordsForDoctor(int doctorId)
        {
            try
            {
                List<MedicalRecordJointModel> medicalRecords = await _medicalRecordsDBService
                    .GetMedicalRecordsForDoctor(doctorId)
                    .ConfigureAwait(false);
                s_medicalRecordList.Clear();
                foreach (MedicalRecordJointModel medicalRecord in medicalRecords)
                {
                    s_medicalRecordList.Add(medicalRecord);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading medical records: {ex.Message}");
                return;
            }
        }

        public async Task<ObservableCollection<MedicalRecordJointModel>> getMedicalRecords()
        {
            return s_medicalRecordList;
        }
    }
}
