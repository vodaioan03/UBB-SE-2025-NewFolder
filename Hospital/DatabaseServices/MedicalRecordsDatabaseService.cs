using Hospital.Configs;
using Hospital.Exceptions;
using Hospital.Models;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Hospital.DatabaseServices
{
    class MedicalRecordsDatabaseService
    {
        private readonly Config _config;

        public MedicalRecordsDatabaseService()
        {
            _config = Config.GetInstance();
        }

        public async Task<int> AddMedicalRecord(MedicalRecord medicalRecord)
        {
            const string queryAddMedicalRecord =
              "INSERT INTO MedicalRecord(DoctorId, PatientId, ProcedureId, Conclusion) " +
              "OUTPUT INSERTED.MedicalRecordId " +
              "VALUES (@DoctorId, @PatientId, @ProcedureId, @Conclusion)";

            try
            {
                using var connection = new SqlConnection(_config.DatabaseConnection);

                // Open the database connection asynchronously
                await connection.OpenAsync().ConfigureAwait(false);
                Console.WriteLine("Connection established successfully.");

                // Create a command to execute the SQL query
                using var command = new SqlCommand(queryAddMedicalRecord, connection);

                // Add parameters to the query
                command.Parameters.AddWithValue("@DoctorId", medicalRecord.DoctorId);
                command.Parameters.AddWithValue("@PatientId", medicalRecord.PatientId);
                command.Parameters.AddWithValue("@ProcedureId", medicalRecord.ProcedureId);
                command.Parameters.AddWithValue("@Conclusion", medicalRecord.Conclusion ?? (object)DBNull.Value);

                // Execute the query asynchronously and retrieve the generated MedicalRecordId
                SqlDataReader result = await command.ExecuteReaderAsync().ConfigureAwait(false);
                int MedicalRecordId = -1;
                while (await result.ReadAsync().ConfigureAwait(false))
                {
                    MedicalRecordId = result.GetInt32(0);
                }


                // Return the generated MedicalRecordId or -1 if insertion failed
                return MedicalRecordId;
            }
            catch (SqlException sqlException)
            {
                Console.WriteLine($"SQL Error: {sqlException.Message}");
                return -1;
            }
            catch (Exception exception)
            {
                Console.WriteLine($"General Error: {exception.Message}");
                return -1;
            }
        }

        public async Task<List<MedicalRecordJointModel>> GetMedicalRecordsForPatient(int medicalRecordId)
        {
            const string queryGetMedicalRecord =
              "SELECT * FROM MedicalRecord WHERE MedicalRecordId = @MedicalRecordId";
            try
            {
                using var connection = new SqlConnection(_config.DatabaseConnection);
                
                // Open the database connection asynchronously
                await connection.OpenAsync().ConfigureAwait(false);
                Console.WriteLine("Connection established successfully.");
                
                // Create a command to execute the SQL query
                using var command = new SqlCommand(queryGetMedicalRecord, connection);
                
                // Add parameters to the query
                command.Parameters.AddWithValue("@MedicalRecordId", medicalRecordId);
                
                // Execute the query asynchronously and retrieve the MedicalRecord
                SqlDataReader result = await command.ExecuteReaderAsync().ConfigureAwait(false);

                List<MedicalRecordJointModel> medicalRecords = new List<MedicalRecordJointModel>();

                while (await result.ReadAsync().ConfigureAwait(false))
                {
                    medicalRecords.Add(new MedicalRecordJointModel(
                        result.GetInt32(0),
                        result.GetInt32(1),
                        result.GetString(2),
                        result.GetInt32(3),
                        result.GetString(4),
                        result.GetInt32(5),
                        result.GetString(6),
                        result.GetDateTime(7),
                        result.GetString(8)
                    ));
                }

                if (medicalRecords.Count == 0)
                {
                    throw new MedicalRecordNotFoundException("No medical records found for the given patient.");
                }

                return medicalRecords;
            }
            catch (SqlException sqlException)
            {
                Console.WriteLine($"SQL Error: {sqlException.Message}");
                return null;
            }
            catch (Exception exception)
            {
                Console.WriteLine($"General Error: {exception.Message}");
                return null;
            }
        }

        public async Task<MedicalRecordJointModel> RetrieveMedicalRecordById(int medicalRecordId)
        {
            const string queryRetrieveMedicalRecord =
              "SELECT * FROM MedicalRecord WHERE MedicalRecordId = @MedicalRecordId";
            try
            {
                using var connection = new SqlConnection(_config.DatabaseConnection);

                // Open the database connection asynchronously
                await connection.OpenAsync().ConfigureAwait(false);
                Console.WriteLine("Connection established successfully.");

                // Create a command to execute the SQL query
                using var command = new SqlCommand(queryRetrieveMedicalRecord, connection);

                // Add parameters to the query
                command.Parameters.AddWithValue("@MedicalRecordId", medicalRecordId);

                // Execute the query asynchronously and retrieve the MedicalRecord
                SqlDataReader result = await command.ExecuteReaderAsync().ConfigureAwait(false);
                MedicalRecordJointModel medicalRecord = null;
                while (await result.ReadAsync().ConfigureAwait(false))
                {
                    medicalRecord = new MedicalRecordJointModel(
                        result.GetInt32(0),
                        result.GetInt32(1),
                        result.GetString(2),
                        result.GetInt32(3),
                        result.GetString(4),
                        result.GetInt32(5),
                        result.GetString(6),
                        result.GetDateTime(7),
                        result.GetString(8)
                    );
                }
                if (medicalRecord == null)
                {
                    throw new MedicalRecordNotFoundException("No medical record found for the given ID.");
                }
                return medicalRecord;
            }
            catch (SqlException sqlException)
            {
                Console.WriteLine($"SQL Error: {sqlException.Message}");
                return null;
            }
            catch (Exception exception)
            {
                Console.WriteLine($"General Error: {exception.Message}");
                return null;
            }
        }

        public async Task<List<MedicalRecordJointModel>> GetMedicalRecordsForDoctor(int DoctorId)
        {
            const string queryGetMedicalRecordsForDoctor =
              "SELECT * FROM MedicalRecord WHERE DoctorId = @DoctorId";
            try
            {
                using var connection = new SqlConnection(_config.DatabaseConnection);
                // Open the database connection asynchronously
                await connection.OpenAsync().ConfigureAwait(false);
                Console.WriteLine("Connection established successfully.");
                // Create a command to execute the SQL query
                using var command = new SqlCommand(queryGetMedicalRecordsForDoctor, connection);
                // Add parameters to the query
                command.Parameters.AddWithValue("@DoctorId", DoctorId);
                // Execute the query asynchronously and retrieve the MedicalRecords
                SqlDataReader result = await command.ExecuteReaderAsync().ConfigureAwait(false);
                List<MedicalRecordJointModel> medicalRecords = new List<MedicalRecordJointModel>();
                while (await result.ReadAsync().ConfigureAwait(false))
                {
                    medicalRecords.Add(new MedicalRecordJointModel(
                        result.GetInt32(0),
                        result.GetInt32(1),
                        result.GetString(2),
                        result.GetInt32(3),
                        result.GetString(4),
                        result.GetInt32(5),
                        result.GetString(6),
                        result.GetDateTime(7),
                        result.GetString(8)
                    ));
                }
                if (medicalRecords.Count == 0)
                {
                    throw new MedicalRecordNotFoundException("No medical records found for the given doctor.");
                }
                return medicalRecords;
            }
            catch (SqlException sqlException)
            {
                Console.WriteLine($"SQL Error: {sqlException.Message}");
                return null;
            }
            catch (Exception exception)
            {
                Console.WriteLine($"General Error: {exception.Message}");
                return null;
            }
        }
    }
}