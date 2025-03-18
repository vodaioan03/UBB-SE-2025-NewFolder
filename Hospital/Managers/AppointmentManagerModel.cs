using Hospital.DatabaseServices;
using Hospital.Models;
using System;
using System.Linq;
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

        

    public async Task<List<Appointment>> LoadAppointmentsForPatient(int patientId)
    {
          try
          {
                
                List<AppointmentJointModel> appointments =
                    await _appointmentsDBService.GetAppointmentsForPatient(patientId).ConfigureAwait(false);

               
                DateTime now = DateTime.Now;
                List<Appointment> upcomingAppointments = appointments
                    .Where(a => a.DateAndTime > now && !a.Finished)
                    .Select(a => new Appointment
                    {
                        AppointmentId = a.AppointmentId,
                        PatientId = a.PatientId,
                        DoctorId = a.DoctorId,
                        DateAndTime = a.DateAndTime,
                        ProcedureId = a.ProcedureId,
                        Finished = a.Finished
                    })
                    .ToList();

                if (!upcomingAppointments.Any())
                {
                    throw new NotFoundException($"No upcoming appointments found for Patient ID {patientId}.");
                }

                return upcomingAppointments;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading appointments: {ex.Message}");
                throw;
            }
    }
  }
}
