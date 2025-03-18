using Hospital.Configs;
using Hospital.Models;
using Microsoft.Data.SqlClient;
using System;
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
    }
}