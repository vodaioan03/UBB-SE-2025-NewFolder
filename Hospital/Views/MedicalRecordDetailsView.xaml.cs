using Hospital.Exceptions;
using Hospital.Managers;
using Hospital.Models;
using Hospital.ViewModels;
using Microsoft.UI;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Hospital.Views
{
    /// <summary>
    /// A window where the details of a medical record are displayed.
    /// </summary>
    public sealed partial class MedicalRecordDetailsView : Window
    {
        private MedicalRecordDetailsViewModel _viewModel;

        public MedicalRecordDetailsView(MedicalRecordJointModel medicalRecordJointModel, DocumentManagerModel documentManagerModel)
        {
            this.InitializeComponent();
            this.AppWindow.Resize(new(800, 600));
            this.StyleTitleBar();

            _viewModel = new MedicalRecordDetailsViewModel(medicalRecordJointModel, documentManagerModel);
            this.MedicalRecordDetailsPanel.DataContext = _viewModel;
        }

        private async void DownloadButton_Click(object sender, RoutedEventArgs e)
        {
            _viewModel.OnDownloadButtonClicked();
        }

        private async void FeedbackButton_Click(object sender, RoutedEventArgs e)
        {
            Debug.WriteLine("Feedback button clicked");
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
