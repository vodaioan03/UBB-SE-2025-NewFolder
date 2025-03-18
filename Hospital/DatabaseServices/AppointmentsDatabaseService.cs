using Hospital.Configs;
using Hospital.Models;
using Microsoft.Data.SqlClient;
using System;
using System.Threading.Tasks;

namespace Hospital.DatabaseServices
{
  class AppointmentsDatabaseService
  {
    private readonly Config _config;

    public AppointmentsDatabaseService(Config config)
    {
      _config = config;
    }

    public async Task<bool> AddAppointmentToDB(Appointment appointment)
    {
      const string query = 
        "INSERT INTO Appointments (PatientId, DoctorId, DateAndTime, ProcedureId, Finished) " +
        "VALUES (@PatientId, @DoctorId, @DateAndTime, @ProcedureId, @Finished)";

      try
      {
        using var connection = new SqlConnection(_config.DatabaseConnection);

        // Open the database connection asynchronously
        await connection.OpenAsync().ConfigureAwait(false);
        Console.WriteLine("Connection established successfully.");

        // Create a command to execute the SQL query
        using var command = new SqlCommand(query, connection);

        // Add the parameters to the query with values from the appointment object
        command.Parameters.AddWithValue("@PatientId", appointment.PatientId);
        command.Parameters.AddWithValue("@DoctorId", appointment.DoctorId);
        command.Parameters.AddWithValue("@DateAndTime", appointment.DateAndTime);
        command.Parameters.AddWithValue("@ProcedureId", appointment.ProcedureId);
        command.Parameters.AddWithValue("@Finished", appointment.Finished);

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
