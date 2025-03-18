using Hospital.Configs;
using Hospital.Models;
using Microsoft.Data.SqlClient;
using System;
using System.Threading.Tasks;

namespace Hospital.DatabaseServices
{
    class MedicalRecordDatabaseService
    {
        private readonly Config _config;

        public MedicalRecordDatabaseService(Config config)
        {
            _config = config;
        }

        public async Task<bool> AddMedicalRecord(MedicalRecord MedicalRecord)
        {
            const string queryAddMedicalRecord =
              "INSERT INTO MedicalRecord(DoctorId, PatientId, ProcedureId, Conclusion) " +
              "VALUES (@DoctorId, @PatientId, @ProcedureId, @Conclusion)";

            try
            {
                using var connection = new SqlConnection(_config.DatabaseConnection);

                // Open the database connection asynchronously
                await connection.OpenAsync().ConfigureAwait(false);
                Console.WriteLine("Connection established successfully.");

                // Create a command to execute the SQL query
                using var command = new SqlCommand(queryAddMedicalRecord, connection);

                // Add the parameters to the query with values from the appointment object
                command.Parameters.AddWithValue("@DoctorId", MedicalRecord.DoctorId);
                command.Parameters.AddWithValue("@PatientId", MedicalRecord.PatientId);
                command.Parameters.AddWithValue("@ProcedureId", MedicalRecord.ProcedureId);
                command.Parameters.AddWithValue("@Conclusion", MedicalRecord.Conclusion);

                // Execute the query asynchronously and check how many rows were affected
                int rowsAffected = await command.ExecuteNonQueryAsync().ConfigureAwait(false);

                // Close DB Connection
                connection.Close();

                // If at least one row was affected, the insert was successful
                return rowsAffected > 0;
            }
            catch (SqlException sqlException)
            {
                Console.WriteLine($"SQL Error: {sqlException.Message}");
                return false;
            }
            catch (Exception exception)
            {
                Console.WriteLine($"General Error: {exception.Message}");
                return false;
            }
        }
    }
}
