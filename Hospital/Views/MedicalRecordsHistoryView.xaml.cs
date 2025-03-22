using Hospital.Managers;
using Hospital.Models;
using Hospital.ViewModels;
using Microsoft.UI;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Diagnostics;

namespace Hospital.Views
{
    /// <summary>
    /// A window where past medical records are listed for the logged in user.
    /// </summary>
    public sealed partial class MedicalRecordsHistoryView : Window
    {
        private MedicalRecordsHistoryViewModel _viewModel;

        public MedicalRecordsHistoryView(MedicalRecordManagerModel medicalRecordManager, DocumentManagerModel documentManager)
        {
            this.InitializeComponent();
            this.AppWindow.Resize(new(800, 600));
            this.StyleTitleBar();

            _viewModel = new MedicalRecordsHistoryViewModel(medicalRecordManager, documentManager);
            this.MedicalRecordsPanel.DataContext = _viewModel;
        }

        private async void ShowMedicalRecordDetails(object sender, RoutedEventArgs e)
        {
            try
            {
                var selectedRecord = MedicalRecordsListView.SelectedItem;
                if (selectedRecord is MedicalRecordJointModel medicalRecord)
                {
                    _viewModel.ShowMedicalRecordDetails(medicalRecord);
                }
                else if (selectedRecord == null)
                {
                    var validationDialog = new ContentDialog
                    {
                        Title = "No element selected",
                        Content = "Please select a medical record to view its details.",
                        CloseButtonText = "OK"
                    };
                    validationDialog.XamlRoot = this.Content.XamlRoot;
                    await validationDialog.ShowAsync();
                }
            }
            catch (Exception ex)
            {
                var validationDialog = new ContentDialog
                {
                    Title = "Error",
                    Content = $"Error: {ex.Message}",
                    CloseButtonText = "OK"
                };
                validationDialog.XamlRoot = this.Content.XamlRoot;
                await validationDialog.ShowAsync();
                return;
            }
        }

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


            /*arrivalCalendarDatePicker.MinDate = DateTime.Today;
            arrivalCalendarDatePicker.Date = DateTime.Today;
            arrivalCalendarDatePicker.MaxDate = DateTime.Today.AddMonths(1);*/

        }
    }
}