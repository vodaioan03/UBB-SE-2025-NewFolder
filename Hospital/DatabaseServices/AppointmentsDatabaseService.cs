using Hospital.Configs;
using Hospital.Models;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
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
        using SqlConnection connection = new SqlConnection(_config.DatabaseConnection);

        // Open the database connection asynchronously
        await connection.OpenAsync().ConfigureAwait(false);
        Console.WriteLine("Connection established successfully.");

        // Create a command to execute the SQL query
        using SqlCommand command = new SqlCommand(query, connection);

        // Add the parameters to the query with values from the appointment object
        command.Parameters.AddWithValue("@PatientId", appointment.PatientId);
        command.Parameters.AddWithValue("@DoctorId", appointment.DoctorId);
        command.Parameters.AddWithValue("@DateAndTime", appointment.DateAndTime);
        command.Parameters.AddWithValue("@ProcedureId", appointment.ProcedureId);
        command.Parameters.AddWithValue("@Finished", appointment.Finished);

        // Execute the query asynchronously and check how many rows were affected
        int rowsAffected = await command.ExecuteNonQueryAsync().ConfigureAwait(false);

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

    public async Task<List<AppointmentJointModel>> GetAppointmentsForPatient(int patientId)
    {
      const string query = @"
        SELECT 
            a.AppointmentId,
            a.Finished,
            a.DateAndTime,
            d.DepartmentId,
            d.DepartmentName,
            doc.DoctorId,
            doc.DoctorName,
            p.PatientId,
            p.PatientName,
            pr.ProcedureId,
            pr.ProcedureName,
            pr.ProcedureDuration
        FROM Appointments a
        JOIN Doctors doc ON a.DoctorId = doc.DoctorId
        JOIN Departments d ON doc.DepartmentId = d.DepartmentId
        JOIN Patients p ON a.PatientId = p.PatientId
        JOIN Procedures pr ON a.ProcedureId = pr.ProcedureId
        ORDER BY a.AppointmentId;
        ";

      using DataTable dt = new DataTable();

      try
      {
        using SqlConnection connection = new SqlConnection(_config.DatabaseConnection);

        // Open the database connection asynchronously
        await connection.OpenAsync().ConfigureAwait(false);
        Console.WriteLine("Connection established successfully.");

        // Create a command to execute the SQL query
        using SqlCommand command = new SqlCommand(query, connection);

        // Get the results from running the command
        using SqlDataReader reader = await command.ExecuteReaderAsync().ConfigureAwait(false);

        // Load the results into the DataTable
        await Task.Run(() => dt.Load(reader)).ConfigureAwait(false);

        // Create a list to store the AppointmentJointModel objects
        List<AppointmentJointModel> appointments = new List<AppointmentJointModel>();
        foreach (DataRow row in dt.Rows)
        {
          appointments.Add(new AppointmentJointModel(
            (int)row["AppointmentId"],
            (bool)row["Finished"],
            (DateTime)row["DateAndTime"],
            (int)row["DepartmentId"],
            (string)row["DepartmentName"],
            (int)row["DoctorId"],
            (string)row["DoctorName"],
            (int)row["PatientId"],
            (string)row["PatientName"],
            (int)row["ProcedureId"],
            (string)row["ProcedureName"],
            (TimeSpan)row["ProcedureDuration"]
          ));
        }

        return appointments;
      }
      catch (SqlException sqlException)
      {
        Console.WriteLine($"SQL Error: {sqlException.Message}");
        return new List<AppointmentJointModel>();
      }
      catch (Exception exception)
      {
        Console.WriteLine($"General Error: {exception.Message}");
        return new List<AppointmentJointModel>();
      }
    }

    public async Task<List<AppointmentJointModel>> GetAppointmentsByDoctorAndDate(int doctorId, DateTime date)
    {
      const string query = @"
          SELECT 
              a.AppointmentId,
              a.Finished,
              a.DateAndTime,
              d.DepartmentId,
              d.DepartmentName,
              doc.DoctorId,
              doc.DoctorName,
              p.PatientId,
              p.PatientName,
              pr.ProcedureId,
              pr.ProcedureName,
              pr.ProcedureDuration
          FROM Appointments a
          JOIN Doctors doc ON a.DoctorId = doc.DoctorId
          JOIN Departments d ON doc.DepartmentId = d.DepartmentId
          JOIN Patients p ON a.PatientId = p.PatientId
          JOIN Procedures pr ON a.ProcedureId = pr.ProcedureId
          WHERE a.DoctorId = @DoctorId
            AND CONVERT(DATE, a.DateAndTime) = @Date
          ORDER BY a.DateAndTime;
      ";

      using DataTable dt = new DataTable();

      try
      {
        using SqlConnection connection = new SqlConnection(_config.DatabaseConnection);

        // Open the database connection asynchronously
        await connection.OpenAsync().ConfigureAwait(false);
        Console.WriteLine("Connection established successfully.");

        // Create a command to execute the SQL query
        using SqlCommand command = new SqlCommand(query, connection);

        // Add parameters for filtering by doctor and date
        command.Parameters.AddWithValue("@DoctorId", doctorId);
        command.Parameters.AddWithValue("@Date", date.Date);

        // Get the results from running the command
        using SqlDataReader reader = await command.ExecuteReaderAsync().ConfigureAwait(false);

        // Load the results into the DataTable
        await Task.Run(() => dt.Load(reader)).ConfigureAwait(false);

        // Create a list to store the AppointmentJointModel objects
        List<AppointmentJointModel> appointments = new List<AppointmentJointModel>();
        foreach (DataRow row in dt.Rows)
        {
          appointments.Add(new AppointmentJointModel(
            (int)row["AppointmentId"],
            (bool)row["Finished"],
            (DateTime)row["DateAndTime"],
            (int)row["DepartmentId"],
            (string)row["DepartmentName"],
            (int)row["DoctorId"],
            (string)row["DoctorName"],
            (int)row["PatientId"],
            (string)row["PatientName"],
            (int)row["ProcedureId"],
            (string)row["ProcedureName"],
            (TimeSpan)row["ProcedureDuration"]
          ));
        }

        return appointments;
      }
      catch (SqlException sqlException)
      {
        Console.WriteLine($"SQL Error: {sqlException.Message}");
        return new List<AppointmentJointModel>();
      }
      catch (Exception exception)
      {
        Console.WriteLine($"General Error: {exception.Message}");
        return new List<AppointmentJointModel>();
      }
    }


    // GetAppointmentsForDoctor
    public async Task<List<AppointmentJointModel>> GetAppointmentsForDoctor(int doctorId)
    {
      const string query = @"
          SELECT 
              a.AppointmentId,
              a.Finished,
              a.DateAndTime,
              d.DepartmentId,
              d.DepartmentName,
              doc.DoctorId,
              doc.DoctorName,
              p.PatientId,
              p.PatientName,
              pr.ProcedureId,
              pr.ProcedureName,
              pr.ProcedureDuration
          FROM Appointments a
          JOIN Doctors doc ON a.DoctorId = doc.DoctorId
          JOIN Departments d ON doc.DepartmentId = d.DepartmentId
          JOIN Patients p ON a.PatientId = p.PatientId
          JOIN Procedures pr ON a.ProcedureId = pr.ProcedureId
          WHERE a.DoctorId = @DoctorId
          ORDER BY a.DateAndTime;
      ";

      using DataTable dt = new DataTable();

      try
      {
        using SqlConnection connection = new SqlConnection(_config.DatabaseConnection);

        // Open the database connection asynchronously
        await connection.OpenAsync().ConfigureAwait(false);
        Console.WriteLine("Connection established successfully.");

        // Create a command to execute the SQL query
        using SqlCommand command = new SqlCommand(query, connection);

        // Add parameters for filtering by doctor
        command.Parameters.AddWithValue("@DoctorId", doctorId);

        // Get the results from running the command
        using SqlDataReader reader = await command.ExecuteReaderAsync().ConfigureAwait(false);

        // Load the results into the DataTable
        await Task.Run(() => dt.Load(reader)).ConfigureAwait(false);

        // Create a list to store the AppointmentJointModel objects
        List<AppointmentJointModel> appointments = new List<AppointmentJointModel>();
        foreach (DataRow row in dt.Rows)
        {
          appointments.Add(new AppointmentJointModel(
            (int)row["AppointmentId"],
            (bool)row["Finished"],
            (DateTime)row["DateAndTime"],
            (int)row["DepartmentId"],
            (string)row["DepartmentName"],
            (int)row["DoctorId"],
            (string)row["DoctorName"],
            (int)row["PatientId"],
            (string)row["PatientName"],
            (int)row["ProcedureId"],
            (string)row["ProcedureName"],
            (TimeSpan)row["ProcedureDuration"]
          ));
        }

        return appointments;
      }
      catch (SqlException sqlException)
      {
        Console.WriteLine($"SQL Error: {sqlException.Message}");
        return new List<AppointmentJointModel>();
      }
      catch (Exception exception)
      {
        Console.WriteLine($"General Error: {exception.Message}");
        return new List<AppointmentJointModel>();
      }
    }
  }
}
