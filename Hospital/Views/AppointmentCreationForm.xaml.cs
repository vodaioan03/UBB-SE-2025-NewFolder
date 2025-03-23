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
using System.Security.AccessControl;

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

        private AppointmentCreationForm(DepartmentManagerModel departmentManagerModel, MedicalProcedureManagerModel procedureManagerModel, DoctorManagerModel doctorManagerModel, ShiftManagerModel shiftManagerModel, AppointmentManagerModel appointmentManagerModel)
        {
            this.InitializeComponent();
            this.StyleTitleBar();

            //prepare view model
            _departmentManager = departmentManagerModel;
            _procedureManager = procedureManagerModel;
            _doctorManager = doctorManagerModel;
            _shiftManager = shiftManagerModel;
            _appointmentManager = appointmentManagerModel;

            //resize
            this.AppWindow.Resize(new(1000, 1400));
        }

        public static async Task<AppointmentCreationForm> CreateAppointmentCreationForm(DepartmentManagerModel departmentManagerModel, MedicalProcedureManagerModel procedureManagerModel, DoctorManagerModel doctorManagerModel, ShiftManagerModel shiftManagerModel, AppointmentManagerModel appointmentManagerModel)
        {
            AppointmentCreationForm form = new AppointmentCreationForm(departmentManagerModel, procedureManagerModel, doctorManagerModel, shiftManagerModel, appointmentManagerModel);


            form._viewModel = await AppointmentCreationFormViewModel.CreateViewModel(form._departmentManager, form._procedureManager, form._doctorManager, form._shiftManager, form._appointmentManager);

            //set data context
            form.AppointmentForm.DataContext = form._viewModel;
            form._viewModel.Root = form.Content.XamlRoot;

            //return value
            return form;
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
                var validationDialog = new ContentDialog
                {
                    Title = "Error",
                    Content = "Please fill all the fields!",
                    CloseButtonText = "OK"
                };
                validationDialog.XamlRoot = this.Content.XamlRoot;
                await validationDialog.ShowAsync();
                return;
            }

            // --------------------------------------------------------------

            // Create a dialog to show all the details of the appointment
            var dialogContent = new StackPanel
            {
                Spacing = 15
            };

            // Department with formatting
            dialogContent.Children.Add(new TextBlock
            {
                Text = $"Department: {_viewModel.SelectedDepartment.Name}",
                FontSize = 18
            });

            // Procedure with formatting
            dialogContent.Children.Add(new TextBlock
            {
                Text = $"Procedure: {_viewModel.SelectedProcedure.Name}",
                FontSize = 18
            });

            // Doctor with formatting
            dialogContent.Children.Add(new TextBlock
            {
                Text = $"Doctor: {_viewModel.SelectedDoctor?.DoctorName}",
                FontSize = 18
            });

            // Date with formatting
            dialogContent.Children.Add(new TextBlock
            {
                Text = $"Date: {_viewModel.SelectedCalendarDate?.ToString("MMMM dd, yyyy")}",
                FontSize = 18
            });

            // Time with formatting
            dialogContent.Children.Add(new TextBlock
            {
                Text = $"Time: {_viewModel.SelectedTime.ToString()}",
                FontSize = 18
            });

            // Style the confirmation dialog
            var confirmationDialog = new ContentDialog
            {
                Title = "Confirm Appointment",
                Content = dialogContent,
                CloseButtonText = "Cancel",
                PrimaryButtonText = "Confirm",
            };

            // Setting the dialog's XamlRoot for better visual performance
            confirmationDialog.XamlRoot = this.Content.XamlRoot;

            // Show the dialog and capture the result
            var result = await confirmationDialog.ShowAsync();

            // ------------------------------------------------------------------


            if (result == ContentDialogResult.Primary)
            {
                // Show appropriate dialog based on the result of booking the appointment
                ContentDialog successDialog;

                // User confirmed, now attempt to book the appointment
                try
                {
                    await _viewModel.BookAppointment();

                    successDialog = new ContentDialog
                    {
                        Title = "Success",
                        Content = "Appointment created successfully!",
                        CloseButtonText = "OK"
                    };

                }
                catch(Exception ex)
                {
                    successDialog = new ContentDialog
                    {
                        Title = "Error",
                        Content = "Error creating appointment!\n" + ex.Message,
                        CloseButtonText = "OK"
                    };
                }

                successDialog.XamlRoot = this.Content.XamlRoot;
                await successDialog.ShowAsync();

                this.Close(); // Close the current window after showing success/error
            }
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

        private async void DepartmentComboBox_SelectionChanged(object sender, object e)
        {
            try
            {
                await _viewModel.LoadProceduresAndDoctorsOfSelectedDepartment();
            }
            catch(Exception ex)
            {
                ContentDialog errorDialog = new ContentDialog
                {
                    Title = "Error",
                    Content = ex.Message,
                    CloseButtonText = "OK"
                };
                errorDialog.XamlRoot = this.Content.XamlRoot;
                await errorDialog.ShowAsync();
            }
        }

        private async void ProcedureComboBox_SelectionChanged(object sender, object e)
        {
            try
            {
                await _viewModel.LoadAvailableTimeSlots();

            }
            catch (Exception ex)
            {
                ContentDialog errorDialog = new ContentDialog
                {
                    Title = "Error",
                    Content = ex.Message,
                    CloseButtonText = "OK"
                };
                errorDialog.XamlRoot = this.Content.XamlRoot;
                await errorDialog.ShowAsync();
            }
        }

        private async void DoctorComboBox_SelectionChanged(object sender, object e)
        {
            try
            {
                await _viewModel.LoadDoctorSchedule();
                await _viewModel.LoadAvailableTimeSlots();

                //force a calendar reset in a dirty way can be left out
                CalendarDatePicker.MinDate = DateTime.Today.AddDays(1);
                CalendarDatePicker.MinDate = DateTime.Today;

            }
            catch (Exception ex)
            {
                ContentDialog errorDialog = new ContentDialog
                {
                    Title = "Error",
                    Content = ex.Message,
                    CloseButtonText = "OK"
                };
                errorDialog.XamlRoot = this.Content.XamlRoot;
                await errorDialog.ShowAsync();
            }

        }

        private void CalendarView_DayItemChanging(CalendarView sender, CalendarViewDayItemChangingEventArgs args)
        {
           DateTimeOffset date = args.Item.Date.Date;
            if (_viewModel.HighlightedDates.Any(d => d.Date == date))
            {

                args.Item.Background = new SolidColorBrush(Microsoft.UI.Colors.LightGreen); // Highlight date
                args.Item.Foreground = new SolidColorBrush(Microsoft.UI.Colors.Black);      // Ensure text is readable
            }
        }


    }
}
