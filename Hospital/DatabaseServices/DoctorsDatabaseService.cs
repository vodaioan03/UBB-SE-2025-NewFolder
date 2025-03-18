using Hospital.Configs;
using Hospital.Models;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Windows.System;

namespace Hospital.DatabaseServices
{
    class DoctorsDatabaseService
    {
        private readonly Config _config;

        public DoctorsDatabaseService()
        {
            _config = Config.GetInstance();
        }

        // This method will be used to get the doctors from the database
        public async Task<List<DoctorJointModel>> GetDoctorsByDepartment(int departmentId)
        {
            const string querySelectDepartments = @"SELECT
                d.DoctorId,
                d.UserId,
                u.Username,
                d.DepartmentId,
                d.DoctorRating,
                d.LicenseNumber
                FROM Doctors d
                INNER JOIN Users u
                ON d.UserId = u.UserId
                WHERE DepartmentId = @departmentId";

            try
            {
                using SqlConnection connection = new SqlConnection(_config.DatabaseConnection);
                await connection.OpenAsync().ConfigureAwait(false);

                //Prepare the command
                SqlCommand selectCommand = new SqlCommand(querySelectDepartments, connection);

                //Insert parameters
                selectCommand.Parameters.AddWithValue("@departmentId", departmentId);

                SqlDataReader reader = await selectCommand.ExecuteReaderAsync().ConfigureAwait(false);


                //Prepare the list of doctors
                List<DoctorJointModel> doctorList = new List<DoctorJointModel>();

                //Read the data from the database
                while (await reader.ReadAsync().ConfigureAwait(false))
                {
                    int doctorId = reader.GetInt32(0);
                    int userId = reader.GetInt32(1);
                    string doctorName = reader.GetString(2);
                    int depId = reader.GetInt32(3);
                    double rating = reader.GetDouble(4);
                    string licenseNumber = reader.GetString(5);
                    DoctorJointModel doctor = new DoctorJointModel(doctorId, userId, doctorName, departmentId, rating, licenseNumber);
                    doctorList.Add(doctor);
                }
                return doctorList;
            }
            catch (SqlException e)
            {
                Console.WriteLine($"SQL Exception: {e.Message}");
                return new List<DoctorJointModel>();
            }
            catch (Exception e)
            {
                Console.WriteLine($"General Exception: {e.Message}");
                return new List<DoctorJointModel>();
            }
        }
    }
}
