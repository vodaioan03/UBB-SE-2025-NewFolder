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
    class ShiftManagerModel
    {
        private readonly ShiftsDatabaseService _shiftsDatabaseService;
        private List<Shift> _shifts;

        public ShiftManagerModel(ShiftsDatabaseService shiftsDatabaseService)
        {
            _shiftsDatabaseService = shiftsDatabaseService;
            _shifts = new List<Shift>();
        }

        public async Task<List<Shift>> LoadShifts(int doctorID)
        {
            try
            {
                _shifts = await _shiftsDatabaseService.GetShiftsByDoctorId(doctorID);
                return _shifts;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error loading shifts for doctor {doctorID}: {ex.Message}");
            }
        }


        public List<Shift> GetShifts()
        {
            return _shifts;
        }

        public Shift GetShiftByDay(DateTime day)
        {
            Shift shift = _shifts.FirstOrDefault(s => s.DateTime == day);
            if (shift == null)
                throw new ShiftNotFoundException(string.Format("Shift not found for date {0}", day.ToString()));
            return shift;
        }


    }
}
