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

        if(appointmentJointModels == null || appointmentJointModels.Count == 0)
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
        List<AppointmentJointModel> appointments =
            await _appointmentsDBService.GetAppointmentsByDoctorAndDate(doctorId, date)
            .ConfigureAwait(false);

        s_appointmentList.Clear();

        if (appointments == null || appointments.Count == 0)
        {
          return;
        }

        foreach (AppointmentJointModel appointment in appointments)
        {
          s_appointmentList.Add(appointment);
        }
      }
      catch (Exception ex)
      {
        Console.WriteLine($"Error loading doctor appointments: {ex.Message}");
        throw;
      }
    }

    public async Task LoadAppointmentsForPatient(int patientId)
    {
      try
      {
        List<AppointmentJointModel> appointments =
            await _appointmentsDBService.GetAppointmentsForPatient(patientId)
            .ConfigureAwait(false);

        s_appointmentList.Clear();

        if (appointments == null || appointments.Count == 0)
        {
          return;
        }

        foreach (AppointmentJointModel appointment in appointments)
        {
          if (appointment.Date > DateTime.Now && !appointment.Finished)
          {
            s_appointmentList.Add(appointment);
          }
        }
      }
      catch (Exception ex)
      {
        Console.WriteLine($"Error loading patient appointments: {ex.Message}");
        throw;
      }
    }

    public async Task LoadAppointmentsForDoctor(int doctorId)
    {
      try
      {
        List<AppointmentJointModel> appointments =
            await _appointmentsDBService.GetAppointmentsForDoctor(doctorId).ConfigureAwait(false);

        s_appointmentList.Clear();

        if (appointments == null || appointments.Count == 0)
        {
          return;
        }

        foreach (AppointmentJointModel appointment in appointments)
        {
          s_appointmentList.Add(appointment);
        }
      }
      catch (Exception ex)
      {
        Console.WriteLine($"Error loading appointments for doctor {doctorId}: {ex.Message}");
        throw;
      }
    }
  }
}