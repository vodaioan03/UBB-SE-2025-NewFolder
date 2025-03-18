using Hospital.Configs;
using Hospital.Models;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace Hospital.DatabaseServices
{
  class DoctorsDatabaseService
  {
    private readonly Config _config;

    public DoctorsDatabaseService()
    {
      _config = Config.GetInstance();
    }

    public async Task<List<DoctorJointModel>> GetDoctorsByDepartment(int departmentId)
    {
      const string query = @"
        SELECT 
            doc.DoctorId,
            doc.UserId,
            doc.DepartmentId,
            u.Username AS DoctorName,
            doc.Rating,
            doc.LicenseNumber
        FROM Doctors doc
        JOIN Users u ON doc.UserId = u.UserId
        WHERE doc.DepartmentId = @DepartmentId
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
        command.Parameters.AddWithValue("@DepartmentId", departmentId);

        // Execute the command and obtain a SqlDataReader.
        using SqlDataReader reader = await command.ExecuteReaderAsync().ConfigureAwait(false);

        // Load the results into a DataTable.
        await Task.Run(() => dt.Load(reader)).ConfigureAwait(false);

        List<DoctorJointModel> doctors = new List<DoctorJointModel>();
        foreach (DataRow row in dt.Rows)
        {
          doctors.Add(new DoctorJointModel
          (
              (int) row["DoctorId"],
              (int) row["UserId"],
              (int) row["DepartmentId"],
              (float) row["Rating"],
              (string) row["LicenseNumber"]
          ));
        }

        connection.Close();
        return doctors;
      }
      catch (SqlException sqlException)
      {
        Console.WriteLine($"SQL Error: {sqlException.Message}");
        return new List<DoctorJointModel>();
      }
      catch (Exception exception)
      {
        Console.WriteLine($"General Error: {exception.Message}");
        return new List<DoctorJointModel>();
      }
    }
  }
}
