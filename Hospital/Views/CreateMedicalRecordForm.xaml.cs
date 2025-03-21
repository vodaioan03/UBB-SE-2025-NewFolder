using System;
using System.Collections.Generic;
using Windows.Storage.Pickers;
using Windows.Storage;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Hospital.ViewModels;
using Hospital.Models;
using System.Threading.Tasks;

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
            this.rootGrid.DataContext = _viewModel;
            this.AppWindow.Resize(new(700, 700));
            // Auto-fill fields
            _viewModel.PatientName = appointment.PatientName;
            _viewModel.DoctorName = appointment.DoctorName;
            _viewModel.AppointmentDate = appointment.Date;
            _viewModel.AppointmentTime = appointment.Date.ToString("hh:mm tt");
            _viewModel.Department = appointment.DepartmentName;
        }

        private async void SubmitButton_Click(object sender, RoutedEventArgs e)
        {
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
                this.Close(); // Close window if successful
            }
            else
            {
                await ShowErrorDialog("Failed to create medical record.");
            }
        }

        private async void UploadFiles_Click(object sender, RoutedEventArgs e)
        {
            var picker = new FileOpenPicker
            {
                ViewMode = PickerViewMode.Thumbnail,
                FileTypeFilter = { ".jpg", ".png", ".pdf", ".docx" }
            };

            var file = await picker.PickSingleFileAsync();
            if (file != null)
            {
                _viewModel.AddDocument(_appointment.AppointmentId, file.Path);
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
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
