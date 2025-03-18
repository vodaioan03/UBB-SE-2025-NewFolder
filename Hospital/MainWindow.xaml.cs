using System;
using System.Threading.Tasks;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Hospital.Models;
using Hospital.DatabaseServices;
using System.Collections.Generic;

namespace Hospital
{
  public sealed partial class MainWindow : Window
  {
    public MainWindow()
    {
      InitializeComponent();
    }

    // Function to test adding an appointment to database
    public async Task<bool> AddAppointmentToDatabase()
    {
      try
      {
        Appointment appointmentToAdd = new Appointment(
            appointmentId: 1,
            doctorId: 1,
            patientId: 1,
            dateAndTime: new DateTime(
                DateTime.Now.Year,
                DateTime.Now.Month,
                DateTime.Now.Day,
                12,
                0,
                0
            ),
            finished: false,
            procedureId: 1
        );

        AppointmentsDatabaseService appointmentDatabaseService = new AppointmentsDatabaseService(Configs.Config.GetInstance());

        bool result = await appointmentDatabaseService.AddAppointmentToDB(appointmentToAdd);
        return result;
      }
      catch (Exception exception)
      {
        Console.WriteLine($"Error adding appointment: {exception.Message}");
        return false;
      }
    }

    public async Task<List<AppointmentJointModel>> GetAppointmentJointTable()
    {
      try
      {
        AppointmentsDatabaseService appointmentDatabaseService = new AppointmentsDatabaseService(Configs.Config.GetInstance());
        //List<AppointmentJointModel> list = await appointmentDatabaseService.GetAppointmentsForPatient(1);
        //List<AppointmentJointModel> list = await appointmentDatabaseService.GetAppointmentsByDoctorAndDate(1, new DateTime(2023, 3, 17));
        List<AppointmentJointModel> list = await appointmentDatabaseService.GetAppointmentsForDoctor(3);
        return list;
      } 
      catch(Exception exception)
      {
        Console.WriteLine($"Error getting appointment joint table: {exception.Message}");
        return null;  
      }
    }

    private async void myButton_Click(object sender, RoutedEventArgs e)
    {
      try
      {
        //bool result = await AddAppointmentToDatabase();
        //myButton.Content = result ? "Success" : "Failure";

        List<AppointmentJointModel> list = await GetAppointmentJointTable();
        myButton.Content = list != null ? "Success" : "Failure";
      }
      catch (Exception exception)
      {
        myButton.Content = $"Error: {exception.Message}";
      }
    }
  }
}
