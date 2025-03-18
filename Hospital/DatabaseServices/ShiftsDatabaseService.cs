using Hospital.Configs;
using Hospital.Models;
using Hospital.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hospital.DatabaseServices
{
    public class ShiftsDatabaseService
    {
        private Config configs;
        private List<Shift> shifts; //Can be deleted later -> used for hard coding the data at the moment
        private List<Schedule> schedules; //Can be deleted later -> used for hard coding the data at the moment

        public ShiftsDatabaseService()
        {
            this.configs = Config.GetInstance();
            this.shifts = new List<Shift> 
            {
                new Shift(1, DateTime.Today.AddDays(10), new TimeSpan(8, 0, 0), new TimeSpan(20, 0, 0)),
                new Shift(2, DateTime.Today.AddDays(12), new TimeSpan(20, 0, 0), new TimeSpan(8, 0, 0)),
                new Shift(3, DateTime.Today.AddDays(14), new TimeSpan(8, 0, 0), new TimeSpan(8, 0, 0)),
                new Shift(4, DateTime.Today.AddDays(16), new TimeSpan(8, 0, 0), new TimeSpan(20, 0, 0)),
                new Shift(5, DateTime.Today.AddDays(18), new TimeSpan(8, 0, 0), new TimeSpan(20, 0, 0)),
                new Shift(6, DateTime.Today.AddDays(19), new TimeSpan(8, 0, 0), new TimeSpan(8, 0, 0)),
                new Shift(7, DateTime.Today.AddDays(21), new TimeSpan(20, 0, 0), new TimeSpan(8, 0, 0))
            }; //Can be deleted later , after connecting to the database;

            this.schedules = new List<Schedule>
            {
                new Schedule(1,1),
                new Schedule(2,2),
                new Schedule(3,4),
                new Schedule(4,5)
            }; //Can be deleted later , after connecting to the database;
        }

        public List<Shift> GetShiftsByDoctorId(int doctorId)
        {
            return schedules.Where(schedule => schedule.DoctorId == doctorId).Join(shifts, schedule => schedule.ShiftId, shift => shift.ShiftId, (schedule, shift) => shift).ToList();
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
