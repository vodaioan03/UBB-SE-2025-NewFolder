using Hospital.Configs;
using Hospital.Models;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hospital.DatabaseServices
{
    class MedicalProceduresDatabaseService
    {
        private readonly Config _config;

        public MedicalProceduresDatabaseService()
        {
            _config = Config.GetInstance();
        }

        // This method will be used to get the procedures from the database
        public async Task<List<Procedure>> GetProceduresByDepartmentId(int departmentId)
        {
            const string querySelectProcedures = @"SELECT * FROM Procedures WHERE DepartmentId = @departmentId";

            try
            {
                using SqlConnection connection = new SqlConnection(_config.DatabaseConnection);
                await connection.OpenAsync().ConfigureAwait(false);

                //Prepare the command
                SqlCommand selectCommand = new SqlCommand(querySelectProcedures, connection);
                selectCommand.Parameters.AddWithValue("@departmentId", departmentId);


                SqlDataReader reader = await selectCommand.ExecuteReaderAsync().ConfigureAwait(false);


                //Prepare the list of procedures
                List<Procedure> procedureList = new List<Procedure>();

                //Read the data from the database
                while (await reader.ReadAsync().ConfigureAwait(false))
                {
                    int procedureId = reader.GetInt32(0);
                    string procedureName = reader.GetString(1);
                    TimeSpan procedureDuration = reader.GetTimeSpan(2);
                    Procedure medicalProcedure = new Procedure(procedureId, departmentId, procedureName, procedureDuration);
                    procedureList.Add(medicalProcedure);
                }
                return procedureList;
            }
            catch (SqlException e)
            {
                Console.WriteLine($"SQL Exception: {e.Message}");
                return new List<Procedure>();
            }
            catch (Exception e)
            {
                Console.WriteLine($"General Exception: {e.Message}");
                return new List<Procedure>();
            }
        }
    }
}
