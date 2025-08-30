using CustomMessageBox;
using DentalCareSystem.Data.DAO.Exceptions;
using DentalCareSystem.Data.DAO.MySQL;
using DentalCareSystem.Data.DTO;
using DentalCareSystem.Forms.Windows.Modals;
using DentalCareSystem.Util;
using DentalCareSystem.ViewModel;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;

namespace DentalCareSystem.Forms.Pages
{
    public partial class AppointmentsPage : Page
    {
        private readonly GenericDataGridViewModel<Appointment> appointmentViewModel;
        private readonly AppointmentDAOImpl dao = new AppointmentDAOImpl();
        private readonly PatientDAOImpl patientDao = new PatientDAOImpl();
        private readonly EmployeeDAOImpl dentistDao = new EmployeeDAOImpl();
        private ObservableCollection<DentistComboBoxItem> dentistsList = new ObservableCollection<DentistComboBoxItem>();

        private DateTime currentWeekStart;
        private Dictionary<string, SolidColorBrush> dentistColors = new Dictionary<string, SolidColorBrush>();
        private List<SolidColorBrush> availableColors = new List<SolidColorBrush>
        {
            new SolidColorBrush(Color.FromRgb(239, 154, 154)),
            new SolidColorBrush(Color.FromRgb(129, 212, 250)), 
            new SolidColorBrush(Color.FromRgb(165, 214, 167)), 
            new SolidColorBrush(Color.FromRgb(255, 204, 128)), 
            new SolidColorBrush(Color.FromRgb(206, 147, 216)), 
            new SolidColorBrush(Color.FromRgb(255, 245, 157)), 
            new SolidColorBrush(Color.FromRgb(225, 190, 231)), 
            new SolidColorBrush(Color.FromRgb(197, 202, 233)), 
            new SolidColorBrush(Color.FromRgb(178, 223, 219)), 
            new SolidColorBrush(Color.FromRgb(255, 171, 145))  
        };

        private Appointment selectedAppointment;

        public AppointmentsPage()
        {
            InitializeComponent();

            currentWeekStart = GetStartOfWeek(DateTime.Today);
            UpdateWeekDisplay();

            LoadDentists();
            InitializeDentistColors();

            this.appointmentViewModel = new GenericDataGridViewModel<Appointment>()
            {
                Items = new ObservableCollection<Appointment>(dao.GetAppointments())
            };

            UpdateAppointmentDisplayData();
            DisplayAppointmentsForWeek();

            this.DataContext = appointmentViewModel;
            LocalizationProvider.UpdateAllObjects();
        }

        public class DentistComboBoxItem
        {
            public string JMBG { get; set; }
            public string DisplayName { get; set; }
        }

        private void InitializeDentistColors()
        {
            var dentists = dentistDao.GetDentists();
            int colorIndex = 0;

            foreach (var dentist in dentists)
            {
                if (!dentistColors.ContainsKey(dentist.JMBG))
                {
                    dentistColors[dentist.JMBG] = availableColors[colorIndex % availableColors.Count];
                    colorIndex++;
                }
            }
        }

        private DateTime GetStartOfWeek(DateTime date)
        {
            int diff = (7 + (date.DayOfWeek - DayOfWeek.Monday)) % 7;
            return date.AddDays(-1 * diff).Date;
        }

        private void UpdateWeekDisplay()
        {
            DateTime weekEnd = currentWeekStart.AddDays(6);
            WeekRangeText.Text = $"{currentWeekStart:dd.MM.yyyy} - {weekEnd:dd.MM.yyyy}";
        }

        private void DisplayAppointmentsForWeek()
        {
            CalendarGrid.Children.Clear();

            DateTime today = DateTime.Today;

            UpdateDayHeaders(today);

            for (int i = 0; i < 7; i++)
            {
                DateTime currentDay = currentWeekStart.AddDays(i);

                StackPanel dayPanel = new StackPanel
                {
                    Margin = new Thickness(5),
                    Tag = currentDay
                };

                TextBlock dateText = new TextBlock
                {
                    Text = currentDay.ToString("dd.MM.yyyy"),
                    HorizontalAlignment = HorizontalAlignment.Center,
                    Margin = new Thickness(0, 0, 0, 5),
                    FontWeight = FontWeights.Bold
                };

                if (currentDay.Date == today.Date)
                {
                    dateText.Foreground = Brushes.Blue;
                    dateText.FontWeight = FontWeights.Bold;
                    dateText.Text = $"{currentDay.ToString("dd.MM.yyyy")}";

                    dayPanel.Background = new SolidColorBrush(Color.FromArgb(30, 0, 120, 215));
                }

                dayPanel.Children.Add(dateText);

                Grid.SetColumn(dayPanel, i);
                CalendarGrid.Children.Add(dayPanel);
            }

            var weekAppointments = appointmentViewModel.Items
                .Where(a => a.AppointmentDateTime.Date >= currentWeekStart &&
                           a.AppointmentDateTime.Date <= currentWeekStart.AddDays(6))
                .OrderBy(a => a.AppointmentDateTime)
                .ToList();

            foreach (var appointment in weekAppointments)
            {
                int dayIndex = (int)appointment.AppointmentDateTime.DayOfWeek - 1;
                if (dayIndex < 0) dayIndex = 6; 

                StackPanel dayPanel = CalendarGrid.Children
                    .OfType<StackPanel>()
                    .FirstOrDefault(p => ((DateTime)p.Tag).Date == appointment.AppointmentDateTime.Date);

                if (dayPanel != null)
                {
                    MaterialDesignThemes.Wpf.Card appointmentCard = new MaterialDesignThemes.Wpf.Card
                    {
                        Margin = new Thickness(0, 0, 0, 5),
                        Padding = new Thickness(10),
                        Background = dentistColors.ContainsKey(appointment.DentistJMBG)
                            ? dentistColors[appointment.DentistJMBG]
                            : Brushes.LightGray,
                        Tag = appointment 
                    };

                    appointmentCard.MouseDown += (s, e) =>
                    {
                        if (e.ChangedButton == System.Windows.Input.MouseButton.Left)
                        {
                            selectedAppointment = appointment;
                        }
                        else if (e.ChangedButton == System.Windows.Input.MouseButton.Right)
                        {
                            selectedAppointment = appointment;
                            CalendarGrid.ContextMenu.IsOpen = true;
                        }
                    };

                    StackPanel cardContent = new StackPanel();

                    TextBlock timeText = new TextBlock
                    {
                        Text = appointment.AppointmentDateTime.ToString("HH:mm"),
                        FontWeight = FontWeights.Bold,
                        Margin = new Thickness(0, 0, 0, 5)
                    };
                    cardContent.Children.Add(timeText);

                    TextBlock patientText = new TextBlock
                    {
                        Text = $"{appointment.PatientName}",
                        Margin = new Thickness(0, 0, 0, 2)
                    };
                    cardContent.Children.Add(patientText);

                    TextBlock jmbgText = new TextBlock
                    {
                        Text = $"{appointment.PatientJMBG}",
                        FontSize = 11,
                        Margin = new Thickness(0, 0, 0, 5)
                    };
                    cardContent.Children.Add(jmbgText);

                    TextBlock dentistText = new TextBlock
                    {
                        Text = $"{appointment.DentistName}",
                        FontStyle = FontStyles.Italic,
                        FontSize = 12
                    };
                    cardContent.Children.Add(dentistText);

                    appointmentCard.Content = cardContent;
                    dayPanel.Children.Add(appointmentCard);
                }
            }
        }

        private void UpdateDayHeaders(DateTime today)
        {
            var dayHeaders = new List<TextBlock>();

            var headerGrid = FindVisualChild<Grid>(CalendarGrid.Parent as Grid, "DayHeadersGrid");

            if (headerGrid != null)
            {
                for (int i = 0; i < 7; i++)
                {
                    var textBlock = FindVisualChild<TextBlock>(headerGrid, $"DayHeader_{i}");
                    if (textBlock != null)
                    {
                        dayHeaders.Add(textBlock);
                    }
                }
            }

            if (dayHeaders.Count == 0)
            {
                return;
            }

            for (int i = 0; i < 7; i++)
            {
                DateTime currentDay = currentWeekStart.AddDays(i);
                if (currentDay.Date == today.Date && i < dayHeaders.Count)
                {
                    dayHeaders[i].Foreground = Brushes.Blue;
                    dayHeaders[i].FontWeight = FontWeights.Bold;
                }
                else if (i < dayHeaders.Count)
                {
                    dayHeaders[i].Foreground = Brushes.Black;
                    dayHeaders[i].FontWeight = FontWeights.Normal;
                }
            }
        }

        private T FindVisualChild<T>(DependencyObject parent, string name = null) where T : DependencyObject
        {
            if (parent == null) return null;

            T foundChild = null;
            int childrenCount = VisualTreeHelper.GetChildrenCount(parent);

            for (int i = 0; i < childrenCount; i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);
                T childType = child as T;

                if (childType == null)
                {
                    foundChild = FindVisualChild<T>(child, name);
                    if (foundChild != null) break;
                }
                else if (string.IsNullOrEmpty(name))
                {
                    foundChild = childType;
                    break;
                }
                else
                {
                    var frameworkElement = child as FrameworkElement;
                    if (frameworkElement != null && frameworkElement.Name == name)
                    {
                        foundChild = childType;
                        break;
                    }
                }
            }
            return foundChild;
        }

        private void LoadDentists()
        {
            var dentists = dentistDao.GetDentists();
            dentistsList.Clear();

            dentistsList.Add(new DentistComboBoxItem { JMBG = "", DisplayName = LocalizationProvider.GetLocalizedString("AllDentists") });

            foreach (var dentist in dentists)
            {
                dentistsList.Add(new DentistComboBoxItem
                {
                    JMBG = dentist.JMBG,
                    DisplayName = $"{dentist.FirstName} {dentist.LastName} ({dentist.JMBG})"
                });
            }

            DentistComboBox.ItemsSource = dentistsList;
            DentistComboBox.SelectedIndex = 0;
        }

        private void UpdateAppointmentDisplayData()
        {
            var patients = patientDao.GetPatients().ToDictionary(p => p.JMBG, p => $"{p.FirstName} {p.LastName}");
            var dentists = dentistDao.GetDentists().ToDictionary(d => d.JMBG, d => $"{d.FirstName} {d.LastName}");

            foreach (var appointment in appointmentViewModel.Items)
            {
                if (patients.ContainsKey(appointment.PatientJMBG))
                {
                    appointment.PatientName = patients[appointment.PatientJMBG];
                }
                if (dentists.ContainsKey(appointment.DentistJMBG))
                {
                    appointment.DentistName = dentists[appointment.DentistJMBG];
                }
            }
        }

        private void PreviousWeekButton_Click(object sender, RoutedEventArgs e)
        {
            currentWeekStart = currentWeekStart.AddDays(-7);
            UpdateWeekDisplay();
            DisplayAppointmentsForWeek();
        }

        private void NextWeekButton_Click(object sender, RoutedEventArgs e)
        {
            currentWeekStart = currentWeekStart.AddDays(7);
            UpdateWeekDisplay();
            DisplayAppointmentsForWeek();
        }

        private void TodayButton_Click(object sender, RoutedEventArgs e)
        {
            currentWeekStart = GetStartOfWeek(DateTime.Today);
            UpdateWeekDisplay();
            DisplayAppointmentsForWeek();
        }

        private void ApplyFilter_Click(object sender, RoutedEventArgs e)
        {
            DateTime? selectedDate = DatePicker.SelectedDate;
            string patientJMBG = PatientFilterTextBox.Text.Trim();
            string dentistJMBG = DentistComboBox.SelectedValue?.ToString() ?? "";

            if (!selectedDate.HasValue && string.IsNullOrEmpty(patientJMBG) && string.IsNullOrEmpty(dentistJMBG))
            {
                CalendarView.Visibility = Visibility.Visible;
                Week.Visibility = Visibility.Visible;
                DataGridAppointments.Visibility = Visibility.Collapsed;

                appointmentViewModel.Items = new ObservableCollection<Appointment>(dao.GetAppointments());
                UpdateAppointmentDisplayData();
                DisplayAppointmentsForWeek();
                Snackbar.MessageQueue?.Enqueue(LocalizationProvider.GetLocalizedString("AllAppointmentsLoaded"), null, null, null, false, true, TimeSpan.FromSeconds(3));
            }
            else
            {
                CalendarView.Visibility = Visibility.Collapsed;
                Week.Visibility = Visibility.Collapsed;
                DataGridAppointments.Visibility = Visibility.Visible;

                var filteredList = new List<Appointment>();

                if (selectedDate.HasValue)
                {
                    filteredList = dao.GetAppointmentsByDate(selectedDate.Value);
                }
                else
                {
                    filteredList = dao.GetAppointments();
                }

                var finalFilteredList = filteredList.Where(appointment =>
                    (string.IsNullOrEmpty(patientJMBG) || appointment.PatientJMBG.Contains(patientJMBG)) &&
                    (string.IsNullOrEmpty(dentistJMBG) || appointment.DentistJMBG == dentistJMBG)
                ).ToList();

                appointmentViewModel.Items = new ObservableCollection<Appointment>(finalFilteredList);
                UpdateAppointmentDisplayData();

               PrepareDataGridForFilteredResults();

                if (finalFilteredList.Count == 0)
                {
                    string message = LocalizationProvider.GetLocalizedString("NoAppointmentsFound");

                    if (selectedDate.HasValue)
                    {
                        message += $" {LocalizationProvider.GetLocalizedString("ForDate")}: {selectedDate.Value.ToShortDateString()}";
                    }
                    if (!string.IsNullOrEmpty(patientJMBG))
                    {
                        message += $" {LocalizationProvider.GetLocalizedString("ForPatientJMBG")}: {patientJMBG}";
                    }
                    if (!string.IsNullOrEmpty(dentistJMBG))
                    {
                        var selectedDentist = dentistsList.FirstOrDefault(d => d.JMBG == dentistJMBG);
                        message += $" {LocalizationProvider.GetLocalizedString("ForDentist")}: {selectedDentist?.DisplayName}";
                    }

                    Snackbar.MessageQueue?.Enqueue(message, null, null, null, false, true, TimeSpan.FromSeconds(5));
                }
                else
                {
                    Snackbar.MessageQueue?.Enqueue(LocalizationProvider.GetLocalizedString("Filtered"), null, null, null, false, true, TimeSpan.FromSeconds(3));
                }
            }
            DataGridAppointments.ItemsSource = appointmentViewModel.Items;
            ResetFilters();      
        }

        private void ResetFilters()
        {
            DatePicker.SelectedDate = null;
            DatePicker.Text = string.Empty;
            PatientFilterTextBox.Text = string.Empty;
            DentistComboBox.SelectedIndex = 0;
            DisplayAppointmentsForWeek();
        }

        private void PopupBox_Closed(object sender, EventArgs e)
        {
            ResetFilters();
        }

        private void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            CalendarView.Visibility = Visibility.Visible;
            Week.Visibility= Visibility.Visible;
            DataGridAppointments.Visibility = Visibility.Collapsed;

            appointmentViewModel.Items.Clear();
            appointmentViewModel.Objects = dao.GetAppointments();
            foreach (Appointment appointment in appointmentViewModel.Objects)
            {
                appointmentViewModel.Items.Add(appointment);
            }
            UpdateAppointmentDisplayData();
            DisplayAppointmentsForWeek();

            Snackbar.MessageQueue?.Enqueue(LocalizationProvider.GetLocalizedString("Refreshed"), null, null, null, false, true, TimeSpan.FromSeconds(3));
        }


        public void AddAppointment_Click(object sender, RoutedEventArgs e)
        {
            AppointmentModal modal = new AppointmentModal();
            modal.ShowDialog();
            if (modal.AppointmentAdded)
            {
                var newAppointment = modal.GetAppointment();
                appointmentViewModel.Items.Add(newAppointment);
                dao.AddAppointment(newAppointment);
                UpdateAppointmentDisplayData();
                DisplayAppointmentsForWeek(); 
                Snackbar.MessageQueue?.Enqueue(LocalizationProvider.GetLocalizedString("AppointmentAdded"), null, null, null, false, true, TimeSpan.FromSeconds(3));
            }
        }

        public void UpdateAppointment_Click(object sender, RoutedEventArgs e)
        {
            if (selectedAppointment == null)
            {
                Snackbar.MessageQueue?.Enqueue(LocalizationProvider.GetLocalizedString("NoSelection"), null, null, null, false, true, TimeSpan.FromSeconds(3));
                return;
            }

            AppointmentModal modal = new AppointmentModal(selectedAppointment);
            modal.ShowDialog();
            if (modal.AppointmentUpdated)
            {
                var updatedAppointment = modal.GetAppointment();

                selectedAppointment.AppointmentDateTime = updatedAppointment.AppointmentDateTime;
                selectedAppointment.PatientJMBG = updatedAppointment.PatientJMBG;
                selectedAppointment.DentistJMBG = updatedAppointment.DentistJMBG;
                selectedAppointment.MedicalTechnicianJMBG = updatedAppointment.MedicalTechnicianJMBG;

                dao.UpdateAppointment(updatedAppointment);
                UpdateAppointmentDisplayData();
                DisplayAppointmentsForWeek(); 

                Snackbar.MessageQueue?.Enqueue(LocalizationProvider.GetLocalizedString("AppointmentUpdated"), null, null, null, false, true, TimeSpan.FromSeconds(3));
            }
        }

        public void DeleteAppointment_Click(object sender, RoutedEventArgs e)
        {
            if (selectedAppointment == null)
            {
                Snackbar.MessageQueue?.Enqueue(LocalizationProvider.GetLocalizedString("NoSelection"), null, null, null, false, true, TimeSpan.FromSeconds(3));
                return;
            }

            bool? result = new MessageBoxCustom("ConfirmDelete", MessageType.Confirmation, MessageButtons.YesNo).ShowDialog();
            if (result == false)
                return;

            try
            {
                if (dao.DeleteAppointment(selectedAppointment.AppointmentId))
                {
                    appointmentViewModel.Items.Remove(selectedAppointment);
                    UpdateAppointmentDisplayData();
                    DisplayAppointmentsForWeek(); 
                    selectedAppointment = null;

                    Snackbar.MessageQueue?.Enqueue(LocalizationProvider.GetLocalizedString("SuccessfullyDeleted"), null, null, null, false, true, TimeSpan.FromSeconds(3));
                }
                else
                {
                    Snackbar.MessageQueue?.Enqueue(LocalizationProvider.GetLocalizedString("UnsuccessfullyDeleted"), null, null, null, false, true, TimeSpan.FromSeconds(3));
                }
            }
            catch (DataAccessException ex)
            {
                if (ex.InnerException?.Message.Contains("foreign key constraint fails") == true)
                {
                    Snackbar.MessageQueue?.Enqueue(
                        LocalizationProvider.GetLocalizedString("DeleteNotAllowed"),
                        null, null, null, false, true, TimeSpan.FromSeconds(5));
                }
                else
                {
                    Snackbar.MessageQueue?.Enqueue(
                        LocalizationProvider.GetLocalizedString("ErrorOccurred"),
                        null, null, null, false, true, TimeSpan.FromSeconds(5));
                }
            }
        }

        private void PrepareDataGridForFilteredResults()
        {
            DataGridAppointments.RowStyle = new Style(typeof(DataGridRow));
            DataGridAppointments.RowStyle.Setters.Add(new Setter(DataGridRow.BackgroundProperty,
                new Binding("DentistJMBG")
                {
                    Converter = new DentistColorConverter(),
                    ConverterParameter = dentistColors
                }));
        }
    }
}