using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Hospital.Managers;
using Hospital.Models;
using System.Linq;
using System.Windows.Input;
using Microsoft.UI.Xaml;

namespace Hospital.ViewModels
{
    class PatientScheduleViewModel
    {
        private readonly AppointmentManagerModel _appointmentManager;
        public ObservableCollection<DateTime> Appointments { get; private set; }

        public PatientScheduleViewModel(AppointmentManagerModel appointmentManager)
        {
            _appointmentManager = appointmentManager;
            Appointments = new ObservableCollection<DateTime>();
        }

        public async Task LoadAppointmentsForPatient(int patientId)
        {
            await _appointmentManager.LoadAppointmentsForPatient(patientId);
            Appointments.Clear();
            foreach (var appointment in _appointmentManager.s_appointmentList)
            {
                Appointments.Add(appointment.Date.Date);
            }
        }
    }
}
