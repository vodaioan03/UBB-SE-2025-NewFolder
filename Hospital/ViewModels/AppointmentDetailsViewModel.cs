using System;
using System.Windows.Input;
using Hospital.Models;
using Hospital.Managers;
using Hospital.Commands;

namespace Hospital.ViewModels
{
    public class AppointmentDetailsViewModel
    {
        public string AppointmentDate { get; private set; }
        public string DoctorName { get; private set; }
        public string Department { get; private set; }
        public string ProcedureName { get; private set; }
        public string ProcedureDuration { get; private set; }

        private readonly AppointmentManagerModel _appointmentManager;
        private readonly Action _closeWindowAction;
        public ICommand RemoveAppointmentCommand { get; }

        public AppointmentDetailsViewModel(AppointmentJointModel appointment, AppointmentManagerModel appointmentManager, Action closeWindow)
        {
            _appointmentManager = appointmentManager;
            _closeWindowAction = closeWindow;

            AppointmentDate = appointment.Date.ToString("f");
            DoctorName = $"Dr. {appointment.DoctorName}";
            Department = appointment.DepartmentName;
            ProcedureName = appointment.ProcedureName;
            ProcedureDuration = $"{appointment.ProcedureDuration.TotalMinutes} minutes";


            RemoveAppointmentCommand = new RelayCommand(ExecuteRemoveAppointment);
        }

        private async void ExecuteRemoveAppointment(object obj)
        {
            bool removed = await _appointmentManager.RemoveAppointment(obj as int? ?? 0);
            if (removed)
            {
                _closeWindowAction.Invoke();
            }
        }
    }
}
