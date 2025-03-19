using Microsoft.UI.Xaml;
using System;
using Hospital.Managers;
using Hospital.ViewModels;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hospital.Views
{
    class DoctorScheduleView : Window
    {
        private readonly AppointmentManagerModel _appointmentManager;
        private readonly ShiftManagerModel _shiftManager;

        private DoctorScheduleViewModel _viewModel;

        public DoctorScheduleView(AppointmentManagerModel appointmentManager, ShiftManagerModel shiftManager)
        {

            _appointmentManager = appointmentManager;
            _shiftManager = shiftManager;
            _viewModel = new DoctorScheduleViewModel(_appointmentManager, _shiftManager);
        }
    }
}
