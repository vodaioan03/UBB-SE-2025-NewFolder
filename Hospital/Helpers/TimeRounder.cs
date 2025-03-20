using Hospital.Configs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hospital.Helpers
{

    public static class TimeRounder
    {
        public static TimeSpan RoundProcedureDuration(TimeSpan initialDuration)
        {
            Double slotDuration = Config.GetInstance().SlotDuration;

            int totalMinutes = (int)initialDuration.TotalMinutes;
            int roundedMinutes = (int)Math.Round(totalMinutes / slotDuration) * (int)slotDuration; // Round to nearest multiple of 30
            return TimeSpan.FromMinutes(roundedMinutes); // Convert back to TimeSpan
        }

    }
    
}
