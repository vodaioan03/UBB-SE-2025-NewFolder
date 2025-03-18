using Hospital.DatabaseServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hospital.Managers
{
    class ShiftManagerModel
    {
        private ShiftsDatabaseService ShiftsDatabaseService;

        public ShiftManagerModel(ShiftsDatabaseService shiftsDatabaseService)
        {
            ShiftsDatabaseService = shiftsDatabaseService;
        }


    }
}
