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
using Hospital.Models;

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
        private MedicalRecordsDatabaseService? medicalRecordsDatabaseService;
        private DocumentDatabaseService? documentService;


        //ManagerModels 
        private DepartmentManagerModel? DepartmentManager;
        private MedicalProcedureManagerModel? ProcedureManager;
        private DoctorManagerModel? DoctorManager;
        private ShiftManagerModel? ShiftManager;
        private AppointmentManagerModel? AppointmentManager;
        private MedicalRecordManagerModel? MedicalRecordManager;
        private DocumentManagerModel? DocumentManager;


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


        private void Patient3_Click(object sender, RoutedEventArgs e)
        {
            MedicalRecordsHistoryView medicalRecordsHistoryView = new MedicalRecordsHistoryView(MedicalRecordManager);
            medicalRecordsHistoryView.Activate();
        }


        private void DoctorScheduleButton(object sender, RoutedEventArgs e)
        {
            DoctorScheduleView doctorScheduleView = new DoctorScheduleView(AppointmentManager, ShiftManager);
            doctorScheduleView.Activate();
        }

        private void Doctor2_Click(object sender, RoutedEventArgs e)
        {
            
            if (MedicalRecordManager == null || DocumentManager == null) return;

            // Create ViewModel with required dependencies
            MedicalRecordCreationFormViewModel viewModel = new MedicalRecordCreationFormViewModel(MedicalRecordManager, DocumentManager);

            // Create a mock appointment for testing
            AppointmentJointModel mockAppointment = new AppointmentJointModel(
                appointmentId: 1,
                finished: false,
                date: DateTime.Now,
                departmentId: 1,
                departmentName: "Cardiology",
                doctorId: 1,
                doctorName: "Dr. Jane Doe",
                patientId: 1,
                patientName: "John Doe",
                procedureId: 1,
                procedureName: "Heart Checkup",
                procedureDuration: TimeSpan.FromMinutes(30)
            );

            // Create and show the MedicalRecordCreateView window
            CreateMedicalRecordForm medicalRecordCreateView = new CreateMedicalRecordForm(viewModel, mockAppointment);
            medicalRecordCreateView.Activate();
        }


        private void SetupDatabaseServices()
        {
            //setup database services here
            departmentService = new DepartmentsDatabaseService();
            procedureService = new MedicalProceduresDatabaseService();
            doctorService = new DoctorsDatabaseService();
            shiftService = new ShiftsDatabaseService();
            appointmentService = new AppointmentsDatabaseService();
            medicalRecordsDatabaseService = new MedicalRecordsDatabaseService();
            documentService = new DocumentDatabaseService();

            //setup manager models here
            DepartmentManager = new DepartmentManagerModel(departmentService);
            ProcedureManager = new MedicalProcedureManagerModel(procedureService);
            DoctorManager = new DoctorManagerModel(doctorService);
            ShiftManager = new ShiftManagerModel(shiftService);
            AppointmentManager = new AppointmentManagerModel(appointmentService);
            MedicalRecordManager = new MedicalRecordManagerModel(medicalRecordsDatabaseService);
            DocumentManager = new DocumentManagerModel(documentService);

        }

        private void PatientScheduleButton_Click(object sender, RoutedEventArgs e)
        {
            PatientScheduleView patientScheduleView = new PatientScheduleView();
            patientScheduleView.Activate();
        }




    }
}