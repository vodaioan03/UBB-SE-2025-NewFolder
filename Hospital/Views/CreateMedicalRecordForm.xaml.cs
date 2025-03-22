using System;
using System.Collections.Generic;
using Windows.Storage.Pickers;
using Windows.Storage;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Hospital.ViewModels;
using Hospital.Models;
using System.Threading.Tasks;
using WinRT.Interop;
using Microsoft.UI.Xaml;

namespace Hospital.Views
{
    public sealed partial class CreateMedicalRecordForm : Window
    {

        private readonly MedicalRecordCreationFormViewModel _viewModel;
        private readonly AppointmentJointModel _appointment;

        public CreateMedicalRecordForm(MedicalRecordCreationFormViewModel viewModel, AppointmentJointModel appointment)
        {
            this.InitializeComponent();
            _viewModel = viewModel;
            _appointment = appointment;

            // Populate ViewModel from the Appointment
            _viewModel.PatientName = appointment.PatientName;
            _viewModel.DoctorName = appointment.DoctorName;
            _viewModel.AppointmentDate = appointment.Date;
            _viewModel.AppointmentTime = appointment.Date.ToString("hh:mm tt");
            _viewModel.Department = appointment.DepartmentName;

            // Set the data context for binding
            this.rootGrid.DataContext = _viewModel;
        }

        

        private async void UploadFiles_Click(object sender, RoutedEventArgs e)
        {
            var picker = new FileOpenPicker
            {
                ViewMode = PickerViewMode.Thumbnail,
                FileTypeFilter = { ".jpg", ".png", ".pdf", ".docx" }
            };

            // Get the window's HWND
            var hwnd = WinRT.Interop.WindowNative.GetWindowHandle(this);

            // Initialize the picker with the window handle
            WinRT.Interop.InitializeWithWindow.Initialize(picker, hwnd);

            var files = await picker.PickMultipleFilesAsync();
            if (files != null && files.Count > 0)
            {
                foreach (var file in files)
                {

                    _viewModel.AddDocument(file.Path);
                }
            }
        }
        private async void SubmitButton_Click(object sender, RoutedEventArgs e)
        {
            if (_viewModel.Conclusion.Length > 255)
            {
                await ShowErrorDialog("Conclusion cannot exceed 100 characters.");
                return;
            }

            if (string.IsNullOrWhiteSpace(_viewModel.Conclusion))
            {
                await ShowErrorDialog("Conclusion cannot be empty.");
                return;
            }

            _appointment.Finished = true;
            _appointment.Date = DateTime.Now;

            int recordId = await _viewModel.CreateMedicalRecord(_appointment, _viewModel.Conclusion);

            if (recordId > 0)
            {
                // Add documents with the new MedicalRecordId
                foreach (var documentPath in _viewModel.DocumentPaths)
                {
                    _viewModel.AddDocument(recordId, documentPath);
                }

                await ShowSuccessDialog("Medical record created successfully!");
                this.Close();
            }
            else
            {
                await ShowErrorDialog("Failed to create medical record.");
            }
        }


        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        private async Task ShowSuccessDialog(string message)
        {
            ContentDialog successDialog = new ContentDialog
            {
                Title = "Success",
                Content = message,
                CloseButtonText = "OK",
                XamlRoot = this.Content.XamlRoot
            };
            await successDialog.ShowAsync();
        }
        private async Task ShowErrorDialog(string message)
        {
            ContentDialog errorDialog = new ContentDialog
            {
                Title = "Error",
                Content = message,
                CloseButtonText = "OK",
                XamlRoot = this.Content.XamlRoot
            };
            await errorDialog.ShowAsync();
        }
    }
}
