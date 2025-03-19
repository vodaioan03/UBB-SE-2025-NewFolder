using Hospital.Commands;
using Hospital.DatabaseServices;
using Hospital.Managers;
using Hospital.Models;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hospital.ViewModels
{
    class AppointmentCreationFormViewModel
    {
        // List Properties
        public ObservableCollection<Department>? DepartmentsList { get; set; }

        public ObservableCollection<Procedure>? ProceduresList { get; set; }
        public ObservableCollection<DoctorJointModel>? DoctorsList { get; set; }
        private List<Shift>? shiftsList { get; set; }
        private ObservableCollection<AppointmentJointModel>? AppointmentsList { get; set; }


        //Manager Models
        private DepartmentManagerModel _departmentManager;
        private MedicalProcedureManagerModel _procedureManager;
        private DoctorManagerModel _doctorManager;
        private ShiftManagerModel _shiftManager;
        private AppointmentManagerModel _appointmentManager;

        //public event
        public event PropertyChangedEventHandler PropertyChanged;

        //selected fields
        public Department? SelectedDepartment { get; set; }
        public Procedure? SelectedProcedure { get; set; }
        public DoctorJointModel? SelectedDoctor { get; set; }
        public DateTime? SelectedDate { get; set; }
        public TimeSpan? SelectedTime { get; set; }



        //commands
        public RelayCommand LoadProceduresCommand { get; set; }



        public AppointmentCreationFormViewModel(DepartmentManagerModel departmentManagerModel, MedicalProcedureManagerModel procedureManagerModel, DoctorManagerModel doctorManagerModel, ShiftManagerModel shiftManagerModel, AppointmentManagerModel appointmentManagerModel)
        {
            _departmentManager = departmentManagerModel;
            _procedureManager = procedureManagerModel;
            _doctorManager = doctorManagerModel;
            _shiftManager = shiftManagerModel;
            _appointmentManager = appointmentManagerModel;

            //initialize lists
            DepartmentsList = new ObservableCollection<Department>();
            ProceduresList = new ObservableCollection<Procedure>();
            DoctorsList = new ObservableCollection<DoctorJointModel>();

            //initialize commands
            //LoadProceduresCommand = new RelayCommand(LoadProceduresOfSelectedDepartment, CanLoadProcedures);

            //load data
            LoadDepartments();
        }

        

        private async void LoadDepartments()
        {
            DepartmentsList.Clear();
            await _departmentManager.LoadDepartments();
            foreach(Department dept in _departmentManager.GetDepartments())
            {
                DepartmentsList?.Add(dept);
            }
        }

        public async void LoadProceduresOfSelectedDepartment(object sender, SelectionChangedEventArgs e)
        {
            //clear the list
            ProceduresList.Clear();

            //load the procedures
            await _procedureManager.LoadProceduresByDepartmentId(SelectedDepartment.DepartmentId);
            foreach (Procedure proc in _procedureManager.GetProcedures())
            {
                ProceduresList?.Add(proc);  
            }
        }

        private bool CanLoadProcedures(object arg)
        {
            if(SelectedDepartment == null)
            {
                return false;
            }
            return true;
        }

    }
}
