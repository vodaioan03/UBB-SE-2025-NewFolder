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
using Microsoft.UI;
using Microsoft.UI.Windowing;
using Hospital.Managers;
using Hospital.ViewModels;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Windows.UI.Popups;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Hospital.Views
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class AppointmentCreationForm : Window
    {

        //Manager Models
        private DepartmentManagerModel _departmentManager;
        private MedicalProcedureManagerModel _procedureManager;
        private DoctorManagerModel _doctorManager;
        private ShiftManagerModel _shiftManager;
        private AppointmentManagerModel _appointmentManager;

        private AppointmentCreationFormViewModel _viewModel;

        public AppointmentCreationForm(DepartmentManagerModel departmentManagerModel, MedicalProcedureManagerModel procedureManagerModel, DoctorManagerModel doctorManagerModel, ShiftManagerModel shiftManagerModel, AppointmentManagerModel appointmentManagerModel)
        {
            this.InitializeComponent();
            this.StyleTitleBar();

            //prepare view model
            _departmentManager = departmentManagerModel;
            _procedureManager = procedureManagerModel;
            _doctorManager = doctorManagerModel;
            _shiftManager = shiftManagerModel;
            _appointmentManager = appointmentManagerModel;
            _viewModel = new AppointmentCreationFormViewModel(_departmentManager, _procedureManager, _doctorManager, _shiftManager, _appointmentManager);

            //set data context
            this.AppointmentForm.DataContext = _viewModel;
            _viewModel.Root = this.Content.XamlRoot;

            //resize
            this.AppWindow.Resize(new(1000, 1400));
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private async void ConfirmButton_Click(object sender, RoutedEventArgs e)
        {
            // Validate appointment input using the ViewModel method
            if (!_viewModel.ValidateAppointment())
            {
                var dialog = new ContentDialog
                {
                    Title = "Error",
                    Content = "Please fill all the fields!",
                    CloseButtonText = "OK"
                };

                dialog.XamlRoot = this.Content.XamlRoot;

                await dialog.ShowAsync();
                return;
            }

            // Create new appointment and call the ViewModel method to add it to the database
            bool result = await _viewModel.BookAppointment();
            if (result) {
                var dialog = new ContentDialog
                {
                    Title = "Success",
                    Content = "Appointment created successfully!",
                    CloseButtonText = "OK"
                };

                dialog.XamlRoot = this.Content.XamlRoot;
                await dialog.ShowAsync();
            }
            else
            {
                var dialog = new ContentDialog
                {
                    Title = "Error",
                    Content = "Error creating appointment!",
                    CloseButtonText = "OK"
                };

                dialog.XamlRoot = this.Content.XamlRoot;
                await dialog.ShowAsync();
            }

            this.Close();
        }

        //this method is used to style the title bar of the window
        private void StyleTitleBar()
        {

            // Get the title bar of the app window.
            AppWindow m_Window = this.AppWindow;
            AppWindowTitleBar m_TitleBar = m_Window.TitleBar;

            // Set title bar colors.
            m_TitleBar.ForegroundColor = Colors.White;
            m_TitleBar.BackgroundColor = Colors.Green;

            // Set button colors.
            m_TitleBar.ButtonForegroundColor = Colors.White;
            m_TitleBar.ButtonBackgroundColor = Colors.SeaGreen;

            // Set button hover colors.
            m_TitleBar.ButtonHoverForegroundColor = Colors.Gainsboro;
            m_TitleBar.ButtonHoverBackgroundColor = Colors.DarkSeaGreen;
            m_TitleBar.ButtonPressedForegroundColor = Colors.Gray;
            m_TitleBar.ButtonPressedBackgroundColor = Colors.LightGreen;

            // Set inactive window colors.
            // Note: No effect when app is running on Windows 10
            // because color customization is not supported.
            m_TitleBar.InactiveForegroundColor = Colors.Gainsboro;
            m_TitleBar.InactiveBackgroundColor = Colors.SeaGreen;
            m_TitleBar.ButtonInactiveForegroundColor = Colors.Gainsboro;
            m_TitleBar.ButtonInactiveBackgroundColor = Colors.SeaGreen;
        }

        private void DepartmentComboBox_DropDownClosed(object sender, object e)
        {
            _viewModel.LoadProceduresAndDoctorsOfSelectedDepartment();
        }

        private void ProcedureComboBox_SelectionChanged(object sender, object e)
        {
            _viewModel.LoadAvailableTimeSlots();
        }

        private void DoctorComboBox_SelectionChanged(object sender, object e)
        {
            _viewModel.LoadAvailableTimeSlots();
        }
        private void CalendarDatePicker_DateChanged(CalendarDatePicker sender, CalendarDatePickerDateChangedEventArgs args)
        {
            if(_viewModel == null)
            {
                return;
            }
            _viewModel.LoadAvailableTimeSlots();
        }

    }
}
