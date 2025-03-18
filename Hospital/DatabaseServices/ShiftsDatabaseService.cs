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

        public List<Shift> GetShiftsByDoctorId(int doctorId)
        {

            const string query = @"
            SELECT s.ShiftId, s.Date, s.StartTime, s.EndTime
            FROM Shifts s
            JOIN Schedules sch ON s.ShiftId = sch.ShiftId
            WHERE sch.DoctorId = @DoctorId";

            List<Shift> shifts = new List<Shift>();

            using (SqlConnection conn = new SqlConnection(_config.DatabaseConnection))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@DoctorId", doctorId);
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Shift shift = new Shift(reader.GetInt32(0),reader.GetDateTime(1),reader.GetTimeSpan(2),reader.GetTimeSpan(3));
                            shifts.Add(shift);
                        }
                    }
                }
            }
            return shifts;
        }

        public Shift GetShiftByDoctorAndDate(int doctorId, DateTime date)
        {
            Schedule schedule = schedules.FirstOrDefault(s => s.DoctorId == doctorId);
            if (schedule == null)
                throw new ScheduleNotFoundException(string.Format("Schedule not found for doctor ID {0}", doctorId));

            Shift shift = shifts.FirstOrDefault(shift => shift.ShiftId == schedule.ShiftId && shift.DateTime.Date == date.Date);
            if (shift == null)
                throw new ShiftNotFoundException(string.Format("Shift not found for doctor ID {0} on date {1}", doctorId, date.ToString()));

            return shift;
        }

    }
}
