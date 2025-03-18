using Hospital.Configs;
using Hospital.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;

namespace Hospital.DatabaseServices
{
    class DepartmentsDatabaseService
    {
        private readonly Config _config;

        public DepartmentsDatabaseService()
        {
            _config = Config.GetInstance();
        }

        // This method will be used to get the departments from the database
        public async Task<List<Department>> GetDepartmentsFromDB()
        {
            const string querySelectDepartments = "SELECT * FROM Departments";

            try
            {
                using SqlConnection connection = new SqlConnection(_config.DatabaseConnection);
                await connection.OpenAsync().ConfigureAwait(false);

                //Prepare the command
                SqlCommand selectCommand = new SqlCommand(querySelectDepartments, connection);
                SqlDataReader reader = await selectCommand.ExecuteReaderAsync().ConfigureAwait(false);


                //Prepare the list of departments
                List<Department> departmentList = new List<Department>();

                //Read the data from the database
                while (await reader.ReadAsync().ConfigureAwait(false))
                {
                    int departmentId = reader.GetInt32(0);
                    string departmentName = reader.GetString(1);
                    Department department = new Department(departmentId, departmentName);
                    departmentList.Add(department);
                }
                return departmentList;
            }
            catch(SqlException e)
            {
                Console.WriteLine($"SQL Exception: {e.Message}");
                return new List<Department>();
            }
            catch (Exception e)
            {
                Console.WriteLine($"General Exception: {e.Message}");
                return new List<Department>();
            }
        }
    }
}
