using Hospital.DatabaseServices;
using System;
using Hospital.Exceptions;
using Hospital.Models;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hospital.Managers
{
    public class ShiftManagerModel
    {
        private readonly ShiftsDatabaseService _shiftsDatabaseService;
        private List<Shift> _shifts;

        public ShiftManagerModel(ShiftsDatabaseService shiftsDatabaseService)
        {
            _shiftsDatabaseService = shiftsDatabaseService;
            _shifts = new List<Shift>();
        }

        public async Task LoadShifts(int doctorID)
        {
            _shifts = await _shiftsDatabaseService.GetShiftsByDoctorId(doctorID);
        }


        public List<Shift> GetShifts()
        {
            return _shifts;
        }

        public Shift GetShiftByDay(DateTime day)
        {
            Shift shift = _shifts.FirstOrDefault(s => s.DateTime.Day == day.Day);
            if (shift == null)
                throw new ShiftNotFoundException(string.Format("Shift not found for date {0}", day.ToString()));
            return shift;
        }

        public async Task LoadUpcomingDoctorDayshifts(int doctorID)
        {

            _shifts = await _shiftsDatabaseService.GetDoctorDayShifts(doctorID);

        }


    }
}
