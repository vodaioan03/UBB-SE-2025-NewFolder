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
    public class MedicalRecordManagerModel
    {
        public List<MedicalRecordJointModel> s_medicalRecordList { get; private set; }
        private readonly MedicalRecordsDatabaseService _medicalRecordsDBService;

        public MedicalRecordManagerModel(MedicalRecordsDatabaseService dbService)
        {
            _medicalRecordsDBService = dbService;
            s_medicalRecordList = new List<MedicalRecordJointModel>();
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

        public async Task<int> CreateMedicalRecord(AppointmentJointModel detailedAppointment, string conclusion)
        {
            try
            {
                // Create a new MedicalRecord instance with the provided conclusion.
                MedicalRecord medicalRecord = new MedicalRecord(
                    0, // Placeholder MedicalRecordId; the DB will generate the actual ID.
                    detailedAppointment.PatientId,
                    detailedAppointment.DoctorId,
                    detailedAppointment.ProcedureId,
                    conclusion
                );

                // Insert the new record into the database and get the generated ID.
                int newMedicalRecordId = await _medicalRecordsDBService.AddMedicalRecord(medicalRecord)
                                                          .ConfigureAwait(false);

                // If the record was successfully added, update the in-memory list.
                if (newMedicalRecordId > 0)
                {
                    medicalRecord.MedicalRecordId = newMedicalRecordId;
                    // Optionally, retrieve the full record from the database (with join data) and add it.
                    s_medicalRecordList.Add(GetMedicalRecordById(newMedicalRecordId));
                }

                return newMedicalRecordId;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error creating medical record: {ex.Message}");
                return -1;
            }
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

        public async Task<List<MedicalRecordJointModel>> getMedicalRecords()
        {
            return s_medicalRecordList;
        }
    }
}
