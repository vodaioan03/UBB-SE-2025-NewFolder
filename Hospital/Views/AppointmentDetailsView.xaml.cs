using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using Hospital.ViewModels;
using Hospital.Models;
using Hospital.Managers;
using Microsoft.UI.Windowing;
using Microsoft.UI;
using WinRT.Interop;
using Windows.Graphics;
using Hospital.Exceptions;


namespace Hospital.Views
{
    public sealed partial class AppointmentDetailsView : Window
    {
        private readonly AppointmentDetailsViewModel _viewModel;

        public AppointmentDetailsView(AppointmentJointModel appointment, AppointmentManagerModel appointmentManager, Action refreshAppointments)
        {
            this.InitializeComponent();
            this.Activate();

            _viewModel = new AppointmentDetailsViewModel(appointment, appointmentManager, CloseWindow, refreshAppointments);

            BindUI();
            ResizeAndCenterWindow(550, 475);
        }

        private void ResizeAndCenterWindow(int width, int height)
        {
            var hwnd = WindowNative.GetWindowHandle(this); // Get window handle
            var windowId = Win32Interop.GetWindowIdFromWindow(hwnd);
            var appWindow = AppWindow.GetFromWindowId(windowId);

            if (appWindow != null)
            {

                appWindow.Resize(new SizeInt32(width, height));

                var displayArea = DisplayArea.GetFromWindowId(windowId, DisplayAreaFallback.Primary);
                if (displayArea != null)
                {
                    int screenWidth = displayArea.WorkArea.Width;
                    int screenHeight = displayArea.WorkArea.Height;


                    int centerX = (screenWidth - width) / 2;
                    int centerY = (screenHeight - height) / 2;

                    appWindow.Move(new PointInt32(centerX, centerY));
                }
            }
        }

        private void BindUI()
        {
            AppointmentDateText.Text = _viewModel.AppointmentDate;
            DoctorNameText.Text = _viewModel.DoctorName;
            DepartmentText.Text = _viewModel.Department;
            ProcedureNameText.Text = _viewModel.ProcedureName;
            ProcedureDurationText.Text = _viewModel.ProcedureDuration;
        }

        private void GoBack_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private async void CancelAppointment_Click(object sender, RoutedEventArgs e)
        {
            ContentDialog confirmationDialog = new ContentDialog
            {
                Title = "Confirm Cancellation",
                Content = "Are you sure you want to cancel this appointment?",
                PrimaryButtonText = "Yes",
                CloseButtonText = "No",
                DefaultButton = ContentDialogButton.Close,
                XamlRoot = this.Content.XamlRoot
            };

            ContentDialogResult result = await confirmationDialog.ShowAsync();
            if (result == ContentDialogResult.Primary)
            {
                try
                {
                    _viewModel.CancelAppointmentCommand.Execute(null);
                }
                catch (CancellationNotAllowedException ex)
                {
                    ContentDialog errorDialog = new ContentDialog
                    {
                        Title = "Cancellation Failed",
                        Content = ex.Message,
                        CloseButtonText = "OK",
                        XamlRoot = this.Content.XamlRoot
                    };

                    await errorDialog.ShowAsync();
                }
            }
        }


        private void CloseWindow()
        {
            this.Close();
        }
    }
}
