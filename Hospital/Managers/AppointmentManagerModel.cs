using Hospital.DatabaseServices;
using Hospital.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace Hospital.Managers
{
  class AppointmentManagerModel
  {
    public ObservableCollection<AppointmentJointModel> s_appointmentList { get; private set; }
    private readonly AppointmentsDatabaseService _appointmentsDBService;

    public AppointmentManagerModel(AppointmentsDatabaseService dbService)
    {
      _appointmentsDBService = dbService;
      s_appointmentList = new ObservableCollection<AppointmentJointModel>();
    }

    public async Task<ObservableCollection<AppointmentJointModel>> GetAppointments()
    {
      try
      {
        List<AppointmentJointModel> appointmentJointModels =
            await _appointmentsDBService.GetAppointments().ConfigureAwait(false);

        if (appointmentJointModels == null)
        {
          return new ObservableCollection<AppointmentJointModel>();
        }

        return new ObservableCollection<AppointmentJointModel>(appointmentJointModels);
      }
      catch (Exception ex)
      {
        Console.WriteLine($"Error getting appointments: {ex.Message}");
        return new ObservableCollection<AppointmentJointModel>();
      }
    }

    public async Task LoadDoctorAppointmentsOnDate(int doctorId, DateTime date)
    {
      try
      {
        List<AppointmentJointModel> appointments = await _appointmentsDBService
            .GetAppointmentsByDoctorAndDate(doctorId, date)
            .ConfigureAwait(false);

        s_appointmentList = new ObservableCollection<AppointmentJointModel>(appointments);
      }
      catch (Exception ex)
      {
        Console.WriteLine($"Error loading doctor appointments: {ex.Message}");
        return;
      }
    }
  }
}
