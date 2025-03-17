using System;
using System.Threading.Tasks;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Hospital.Models;
using Hospital.DatabaseServices;

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

    private async void myButton_Click(object sender, RoutedEventArgs e)
    {
      try
      {
        bool result = await AddAppointmentToDatabase();
        myButton.Content = result ? "Success" : "Failure";
      }
      catch (Exception exception)
      {
        myButton.Content = $"Error: {exception.Message}";
      }
    }
  }
}
