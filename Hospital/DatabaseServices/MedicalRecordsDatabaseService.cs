using Hospital.Configs;
using Hospital.Exceptions;
using Hospital.Models;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Hospital.DatabaseServices
{
    public class MedicalRecordsDatabaseService
    {
        private readonly Config _config;

        public MedicalRecordsDatabaseService()
        {
            _config = Config.GetInstance();
        }

        public async Task<int> AddMedicalRecord(MedicalRecord medicalRecord)
        {
            DateTime recordDate = DateTime.Now;
            const string queryAddMedicalRecord =
                "INSERT INTO MedicalRecords(DoctorId, PatientId, ProcedureId, Conclusion, DateAndTime) " +
                "OUTPUT INSERTED.MedicalRecordId " +
                "VALUES (@DoctorId, @PatientId, @ProcedureId, @Conclusion, @DateAndTime)";

            try
            {
                using var connection = new SqlConnection(_config.DatabaseConnection);
                await connection.OpenAsync().ConfigureAwait(false);
                Console.WriteLine("Connection established successfully.");

                using var command = new SqlCommand(queryAddMedicalRecord, connection);
                command.Parameters.AddWithValue("@DoctorId", medicalRecord.DoctorId);
                command.Parameters.AddWithValue("@PatientId", medicalRecord.PatientId);
                command.Parameters.AddWithValue("@ProcedureId", medicalRecord.ProcedureId);
                command.Parameters.AddWithValue("@Conclusion", medicalRecord.Conclusion ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@DateAndTime", recordDate); // Pass the record's date

                object result = await command.ExecuteScalarAsync().ConfigureAwait(false);
                int medicalRecordId = result != null ? Convert.ToInt32(result) : -1;
                return medicalRecordId;
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

        public async Task<List<MedicalRecordJointModel>> GetMedicalRecordsForPatient(int patientId)
        {
            const string queryGetMedicalRecord =
              "SELECT " +
              "     mr.MedicalRecordId, " +
              "     mr.PatientId, " +
              "     p.Name AS PatientName, " +
              "     mr.DoctorId, " +
              "     d.Name AS DoctorName, " +
              "     pr.DepartmentId, " +
              "     dept.DepartmentName, " +
              "     mr.ProcedureId, " +
              "     pr.ProcedureName, " +
              "     mr.DateAndTime, " +
              "     mr.Conclusion " +
              "FROM MedicalRecords mr " +
              "JOIN Users p ON mr.PatientId = p.UserId " +
              "JOIN Users d ON mr.DoctorId = d.UserId " +
              "JOIN Procedures pr ON mr.ProcedureId = pr.ProcedureId " +
              "JOIN Departments dept ON pr.DepartmentId = dept.DepartmentId " +
              "WHERE mr.PatientId = @PatientId";
            try
            {
                using var connection = new SqlConnection(_config.DatabaseConnection);
                
                // Open the database connection asynchronously
                await connection.OpenAsync().ConfigureAwait(false);
                Console.WriteLine("Connection established successfully.");
                
                // Create a command to execute the SQL query
                using var command = new SqlCommand(queryGetMedicalRecord, connection);
                
                // Add parameters to the query
                command.Parameters.AddWithValue("@PatientId", patientId);
                
                // Execute the query asynchronously and retrieve the MedicalRecord
                SqlDataReader result = await command.ExecuteReaderAsync().ConfigureAwait(false);

                List<MedicalRecordJointModel> medicalRecords = new List<MedicalRecordJointModel>();

                while (await result.ReadAsync().ConfigureAwait(false))
                {
                    medicalRecords.Add(new MedicalRecordJointModel(
                        result.GetInt32(0),     // MedicalRecordId
                        result.GetInt32(1),     // PatientId
                        result.GetString(2),    // PatientName
                        result.GetInt32(3),     // DoctorId
                        result.GetString(4),    // DoctorName
                        result.GetInt32(5),     // DepartmentId
                        result.GetString(6),    // DepartmentName
                        result.GetInt32(7),     // ProcedureId
                        result.GetString(8),    // ProcedureName
                        result.GetDateTime(9),  // Date
                        result.GetString(10)    // Conclusion
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
              "SELECT " +
              "     mr.MedicalRecordId, " +
              "     mr.PatientId, " +
              "     p.Name AS PatientName, " +
              "     mr.DoctorId, " +
              "     d.Name AS DoctorName, " +
              "     pr.DepartmentId, " +
              "     dept.DepartmentName, " +
              "     mr.ProcedureId, " +
              "     pr.ProcedureName, " +
              "     mr.DateAndTime, " +
              "     mr.Conclusion " +
              "FROM MedicalRecords mr " +
              "JOIN Users p ON mr.PatientId = p.UserId " +
              "JOIN Users d ON mr.DoctorId = d.UserId " +
              "JOIN Procedures pr ON mr.ProcedureId = pr.ProcedureId " +
              "JOIN Departments dept ON pr.DepartmentId = dept.DepartmentId " +
              "WHERE MedicalRecordId = @MedicalRecordId";
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
                        result.GetInt32(0),     // MedicalRecordId
                        result.GetInt32(1),     // PatientId
                        result.GetString(2),    // PatientName
                        result.GetInt32(3),     // DoctorId
                        result.GetString(4),    // DoctorName
                        result.GetInt32(5),     // DepartmentId
                        result.GetString(6),    // DepartmentName
                        result.GetInt32(7),     // ProcedureId
                        result.GetString(8),    // ProcedureName
                        result.GetDateTime(9),  // Date
                        result.GetString(10)     // Conclusion
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
              "SELECT " +
              "     mr.MedicalRecordId, " +
              "     mr.PatientId, " +
              "     p.Name AS PatientName, " +
              "     mr.DoctorId, " +
              "     d.Name AS DoctorName, " +
              "     pr.DepartmentId, " +
              "     dept.DepartmentName, " +
              "     mr.ProcedureId, " +
              "     pr.ProcedureName, " +
              "     mr.DateAndTime, " +
              "     mr.Conclusion " +
              "FROM MedicalRecords mr " +
              "JOIN Users p ON mr.PatientId = p.UserId " +
              "JOIN Users d ON mr.DoctorId = d.UserId " +
              "JOIN Procedures pr ON mr.ProcedureId = pr.ProcedureId " +
              "JOIN Departments dept ON pr.DepartmentId = dept.DepartmentId " +
              "WHERE DoctorId = @DoctorId";
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
                        result.GetInt32(0),     // MedicalRecordId
                        result.GetInt32(1),     // PatientId
                        result.GetString(2),    // PatientName
                        result.GetInt32(3),     // DoctorId
                        result.GetString(4),    // DoctorName
                        result.GetInt32(5),     // DepartmentId
                        result.GetString(6),    // DepartmentName
                        result.GetInt32(7),     // ProcedureId
                        result.GetString(8),    // ProcedureName
                        result.GetDateTime(9),  // Date
                        result.GetString(10)    // Conclusion
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