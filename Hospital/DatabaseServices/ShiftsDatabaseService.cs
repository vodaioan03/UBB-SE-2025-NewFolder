using Hospital.Configs;
using Hospital.Models;
using Hospital.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;

namespace Hospital.DatabaseServices
{
    public class ShiftsDatabaseService
    {
        private readonly Config _config;
      
        public ShiftsDatabaseService()
        {
            this._config = Config.GetInstance();
        }

        public async Task<List<Shift>> GetShifts()
        {
            const string GetShiftsQuery = "SELECT ShiftId, Date, StartTime, EndTime FROM Shifts";
            List<Shift> shifts = new List<Shift>();

            try
            {
                using SqlConnection conn = new SqlConnection(_config.DatabaseConnection);
                await conn.OpenAsync();

                using SqlCommand cmd = new SqlCommand(GetShiftsQuery, conn);
                using SqlDataReader reader = await cmd.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    shifts.Add(new Shift(
                        reader.GetInt32(0),
                        reader.GetDateTime(1),
                        reader.GetTimeSpan(2),
                        reader.GetTimeSpan(3)
                    ));
                }
            }
            catch (SqlException sqlEx)
            {
                Console.WriteLine($"SQL Error: {sqlEx.Message}");
                throw;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"General Error: {ex.Message}");
                throw;
            }

            return shifts;
        }

        public async Task<List<Schedule>> GetSchedules()
        {
            const string getSchedulesQuery = "SELECT DoctorId, ShiftId FROM Schedules";
            List<Schedule> schedules = new List<Schedule>();

            try
            {
                using SqlConnection conn = new SqlConnection(_config.DatabaseConnection);
                await conn.OpenAsync();

                using SqlCommand cmd = new SqlCommand(getSchedulesQuery, conn);
                using SqlDataReader reader = await cmd.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    schedules.Add(new Schedule(reader.GetInt32(0), reader.GetInt32(1)));
                }
            }
            catch (SqlException sqlEx)
            {
                Console.WriteLine($"SQL Error: {sqlEx.Message}");
                throw;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"General Error: {ex.Message}");
                throw;
            }

            return schedules;
        }

        public async Task<List<Shift>> GetShiftsByDoctorId(int doctorId)
        {
            const string GetShiftByDoctorIdQuery = @"
            SELECT s.ShiftId, s.Date, s.StartTime, s.EndTime
            FROM Shifts s
            JOIN Schedules sch ON s.ShiftId = sch.ShiftId
            WHERE sch.DoctorId = @DoctorId";

            List<Shift> shifts = new List<Shift>();

            try
            {
                using SqlConnection conn = new SqlConnection(_config.DatabaseConnection);
                await conn.OpenAsync();

                using SqlCommand cmd = new SqlCommand(GetShiftByDoctorIdQuery, conn);
                cmd.Parameters.AddWithValue("@DoctorId", doctorId);

                using SqlDataReader reader = await cmd.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    shifts.Add(new Shift(
                        reader.GetInt32(0),
                        reader.GetDateTime(1),
                        reader.GetTimeSpan(2),
                        reader.GetTimeSpan(3)
                    ));
                }
            }
            catch (SqlException sqlEx)
            {
                Console.WriteLine($"SQL Error: {sqlEx.Message}");
                throw;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"General Error: {ex.Message}");
                throw;
            }

            return shifts;
        }

        public async Task<List<Shift>> GetDoctorDayShifts(int doctorId)
        {
            const string GetShiftByDoctorIdQuery = @"
            SELECT s.ShiftId, s.Date, s.StartTime, s.EndTime
            FROM Shifts s
            JOIN Schedules sch ON s.ShiftId = sch.ShiftId
            WHERE sch.DoctorId = @DoctorId AND s.StartTime < '20:00:00'
            AND CAST(s.Date AS DATE) >= CAST(GETDATE() AS DATE)";

            List<Shift> shifts = new List<Shift>();

            try
            {
                using SqlConnection conn = new SqlConnection(_config.DatabaseConnection);
                await conn.OpenAsync();

                using SqlCommand cmd = new SqlCommand(GetShiftByDoctorIdQuery, conn);
                cmd.Parameters.AddWithValue("@DoctorId", doctorId);

                using SqlDataReader reader = await cmd.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    shifts.Add(new Shift(
                        reader.GetInt32(0),
                        reader.GetDateTime(1),
                        reader.GetTimeSpan(2),
                        reader.GetTimeSpan(3)
                    ));
                }
            }
            catch (SqlException sqlEx)
            {
                Console.WriteLine($"SQL Error: {sqlEx.Message}");
                throw;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"General Error: {ex.Message}");
                throw;
            }

            return shifts;
        }
    }


}

