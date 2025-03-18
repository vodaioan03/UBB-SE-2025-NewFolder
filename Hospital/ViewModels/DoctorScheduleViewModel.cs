using Hospital.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hospital.DatabaseServices;
using Hospital.Managers;
using Hospital.Commands;


using System.Collections.ObjectModel;
using Windows.ApplicationModel.Appointments;
using System.Windows.Input;

namespace Hospital.ViewModels
{
    class DoctorScheduleViewModel 
    {
        private readonly AppointmentManagerModel _appointmentManager;
        private readonly ShiftManagerModel shiftManager;
        private ObservableCollection<AppointmentJointModel> _appointments { get; set }
        private List<Shift> _shifts;

        public ICommand OpenDetailsCommand { get; set }

        public DoctorScheduleViewModel(AppointmentManagerModel appointmentManager, ShiftManagerModel shiftManager)
        {
            _appointmentManager = appointmentManager;
            this.shiftManager = shiftManager;
            _appointments = new ObservableCollection<AppointmentJointModel>();
            _shifts = new List<Shift>();
            OpenDetailsCommand = new RelayCommand(OpenDetails);
        }



    }
}
