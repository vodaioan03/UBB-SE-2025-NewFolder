using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Hospital.Views;
using Hospital.DatabaseServices;
using Hospital.Managers;
using Hospital.ViewModels;
using System.Diagnostics;
using Windows.ApplicationModel.Appointments;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Hospital
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainWindow : Window
    {

        //DB Services
        private DepartmentsDatabaseService? departmentService;
        private MedicalProceduresDatabaseService? procedureService;
        private DoctorsDatabaseService? doctorService;
        private ShiftsDatabaseService? shiftService;
        private AppointmentsDatabaseService? appointmentService;

        //ManagerModels 
        private DepartmentManagerModel? DepartmentManager;
        private MedicalProcedureManagerModel? ProcedureManager;
        private DoctorManagerModel? DoctorManager;
        private ShiftManagerModel? ShiftManager;
        private AppointmentManagerModel? AppointmentManager;


        public MainWindow()
        {
            this.InitializeComponent();
            this.SetupDatabaseServices();
        }

        private void Patient1_Click(object sender, RoutedEventArgs e)
        {
            AppointmentCreationForm appointmentCreationForm = new AppointmentCreationForm(DepartmentManager, ProcedureManager, DoctorManager, ShiftManager, AppointmentManager);
            appointmentCreationForm.Activate();
        }

        private void Patient2_Click(object sender, RoutedEventArgs e)
        {
            //test ui of feature Patient2 here
        }

            private void Patient3_Click(object sender, RoutedEventArgs e)
            {
                MedicalRecordsDatabaseService medicalRecordsDatabaseService = new MedicalRecordsDatabaseService();
                MedicalRecordManagerModel medicalRecordManagerModel = new MedicalRecordManagerModel(medicalRecordsDatabaseService);
                // After login is implemented, the patient id will be passed as a parameter
                int loggedInUserId = 1;
                medicalRecordManagerModel.LoadMedicalRecordsForPatient(loggedInUserId).Wait();
                MedicalRecordsHistoryViewModel medicalRecordsHistoryViewModel = new MedicalRecordsHistoryViewModel(medicalRecordManagerModel);
                MedicalRecordsHistoryView medicalRecordsHistoryView = new MedicalRecordsHistoryView(medicalRecordsHistoryViewModel);
                medicalRecordsHistoryView.Activate();
            }

        private void Doctor1_Click(object sender, RoutedEventArgs e)
        {
            DoctorScheduleView doctorScheduleView = new DoctorScheduleView(AppointmentManager, ShiftManager);
            doctorScheduleView.Activate();
        }

        private void Doctor2_Click(object sender, RoutedEventArgs e)
        {
            //test ui of feature Patient3 here
        }

        private void SetupDatabaseServices()
        {
            //setup database services here
            departmentService = new DepartmentsDatabaseService();
            procedureService = new MedicalProceduresDatabaseService();
            doctorService = new DoctorsDatabaseService();
            shiftService = new ShiftsDatabaseService();
            appointmentService = new AppointmentsDatabaseService();

            //setup manager models here
            DepartmentManager = new DepartmentManagerModel(departmentService);
            ProcedureManager = new MedicalProcedureManagerModel(procedureService);
            DoctorManager = new DoctorManagerModel(doctorService);
            ShiftManager = new ShiftManagerModel(shiftService);
            AppointmentManager = new AppointmentManagerModel(appointmentService);
        }



    }
}