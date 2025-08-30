using CustomMessageBox;
using DentalCareSystem.Data.DAO.MySQL;
using DentalCareSystem.Data.DTO;
using DentalCareSystem.Util;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;

namespace DentalCareSystem.Forms.Windows.Modals
{
    public partial class AppointmentModal : Window, INotifyPropertyChanged
    {
        public bool AppointmentAdded { get; set; } = false;
        public bool AppointmentUpdated { get; set; } = false;

        private readonly bool isEditMode;
        private readonly Appointment originalAppointment;
        private readonly PatientDAOImpl patientDao = new PatientDAOImpl();
        private readonly EmployeeDAOImpl dentistDao = new EmployeeDAOImpl();
        private readonly PatientDAOImpl _patientDaoForSaving = new PatientDAOImpl();
        private readonly AppointmentDAOImpl appointmentDao = new AppointmentDAOImpl();
        public Patient PatientToAdd { get; set; }

        private DateTime appointmentDateTime = DateTime.Now;
        public DateTime AppointmentDateTime
        {
            get => appointmentDateTime;
            set { appointmentDateTime = value; OnPropertyChanged(); }
        }

        private string appointmentHour;
        public string AppointmentHour
        {
            get => appointmentHour;
            set { appointmentHour = value; OnPropertyChanged(); }
        }

        private string appointmentMinute;
        public string AppointmentMinute
        {
            get => appointmentMinute;
            set { appointmentMinute = value; OnPropertyChanged(); }
        }

        private string patientJMBG;
        public string PatientJMBG
        {
            get => patientJMBG;
            set { patientJMBG = value; OnPropertyChanged(); }
        }

        private string patientInfo;
        public string PatientInfo
        {
            get => patientInfo;
            set { patientInfo = value; OnPropertyChanged(); }
        }

        private ObservableCollection<Employee> dentists;
        public ObservableCollection<Employee> Dentists
        {
            get => dentists;
            set { dentists = value; OnPropertyChanged(); }
        }

        private string selectedDentistJMBG;
        public string SelectedDentistJMBG
        {
            get => selectedDentistJMBG;
            set { selectedDentistJMBG = value; OnPropertyChanged(); }
        }

        public Patient SelectedPatient { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public AppointmentModal()
        {
            InitializeComponent();
            isEditMode = false;
            originalAppointment = new Appointment();
            LoadDentists();
            this.DataContext = this;
            LocalizationProvider.UpdateAllObjects();

            AppointmentHour = AppointmentDateTime.Hour.ToString("D2");
            AppointmentMinute = AppointmentDateTime.Minute.ToString("D2");
            ModalTitle.Content = LocalizationProvider.GetLocalizedString("AddAppointmentTitle");
        }

        public AppointmentModal(Appointment appointment)
        {
            InitializeComponent();
            isEditMode = true;
            originalAppointment = appointment;
            LoadDentists();
            this.DataContext = this;

            AppointmentDateTime = appointment.AppointmentDateTime;
            PatientJMBG = appointment.PatientJMBG;

            AppointmentHour = AppointmentDateTime.Hour.ToString("D2");
            AppointmentMinute = AppointmentDateTime.Minute.ToString("D2");

            SelectedPatient = patientDao.GetPatientByJMBG(appointment.PatientJMBG);
            if (SelectedPatient != null)
            {
                PatientInfo = $"{SelectedPatient.FirstName} {SelectedPatient.LastName}";
            }

            SelectedDentistJMBG = appointment.DentistJMBG;

            LocalizationProvider.UpdateAllObjects();
            ModalTitle.Content = LocalizationProvider.GetLocalizedString("UpdateAppointmentTitle");
        }

        private void LoadDentists()
        {
            var loadedDentists = dentistDao.GetDentists();
            Dentists = new ObservableCollection<Employee>(loadedDentists);
        }

        public Appointment GetAppointment()
        {
            originalAppointment.AppointmentDateTime = AppointmentDateTime;
            originalAppointment.PatientJMBG = PatientJMBG;
            originalAppointment.DentistJMBG = SelectedDentistJMBG;
            return originalAppointment;
        }

        private void SearchPatient_Click(object sender, RoutedEventArgs e)
        {
            SearchPatientByJMBG();
        }

        private void PatientJMBGTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                SearchPatientByJMBG();
            }
        }

        private void SearchPatientByJMBG()
        {
            if (string.IsNullOrEmpty(PatientJMBG))
            {
                AppointmentModalSnackbar.MessageQueue?.Enqueue(LocalizationProvider.GetLocalizedString("EnterJMBG"), null, null, null, false, true, TimeSpan.FromSeconds(3));
                return;
            }

            SelectedPatient = patientDao.GetPatientByJMBG(PatientJMBG);
            if (SelectedPatient != null)
            {
                PatientInfo = $"{SelectedPatient.FirstName} {SelectedPatient.LastName}";
                AppointmentModalSnackbar.MessageQueue?.Enqueue(LocalizationProvider.GetLocalizedString("PatientFound"), null, null, null, false, true, TimeSpan.FromSeconds(3));
            }
            else
            {
                bool? Result = new MessageBoxCustom("ConfirmAdd", MessageType.Confirmation, MessageButtons.YesNo).ShowDialog();
                if (Result == false)
                {
                    return;
                }
                else if (Result == true)
                {
                    PatientModal patientModal = new PatientModal();
                    patientModal.Owner = this;
                    patientModal.ShowDialog();

                    if (patientModal.PatientAdded)
                    {
                        SelectedPatient = patientModal.GetPatient();

                        try
                        {
                            _patientDaoForSaving.AddPatient(SelectedPatient);
                            PatientJMBG = SelectedPatient.JMBG;
                            PatientInfo = $"{SelectedPatient.FirstName} {SelectedPatient.LastName}";
                            AppointmentModalSnackbar.MessageQueue?.Enqueue(LocalizationProvider.GetLocalizedString("PatientAdded"), null, null, null, false, true, TimeSpan.FromSeconds(3));
                        }
                        catch (Exception)
                        {
                            AppointmentModalSnackbar.MessageQueue?.Enqueue(LocalizationProvider.GetLocalizedString("ErrorOccured"), null, null, null, false, true, TimeSpan.FromSeconds(3));
                        }
                    }
                }
            }
        }

        private void AddOrUpdate_Click(object sender, RoutedEventArgs e)
        {
            if (!int.TryParse(AppointmentHour, out int hour) || hour < 0 || hour > 23 ||
                !int.TryParse(AppointmentMinute, out int minute) || minute < 0 || minute > 59)
            {
                AppointmentModalSnackbar.MessageQueue?.Enqueue(LocalizationProvider.GetLocalizedString("InvalidTimeFormat"), null, null, null, false, true, TimeSpan.FromSeconds(3));
                return;
            }

            DateTime selectedDate = AppointmentDatePicker.SelectedDate ?? DateTime.Now.Date;
            AppointmentDateTime = new DateTime(selectedDate.Year, selectedDate.Month, selectedDate.Day, hour, minute, 0);

            if (SelectedPatient == null || string.IsNullOrEmpty(SelectedDentistJMBG))
            {
                AppointmentModalSnackbar.MessageQueue?.Enqueue(LocalizationProvider.GetLocalizedString("FillFields"), null, null, null, false, true, TimeSpan.FromSeconds(3));
                return;
            }

            if (AppointmentDateTime < DateTime.Now)
            {
                AppointmentModalSnackbar.MessageQueue?.Enqueue(LocalizationProvider.GetLocalizedString("AppointmentInPast"), null, null, null, false, true, TimeSpan.FromSeconds(3));
                return;
            }

            int excludeAppointmentId = isEditMode ? originalAppointment.AppointmentId : 0;
            bool isTimeSlotTaken = appointmentDao.IsAppointmentTimeSlotTaken(
                AppointmentDateTime,
                SelectedDentistJMBG,
                excludeAppointmentId);

            if (isTimeSlotTaken)
            {
                AppointmentModalSnackbar.MessageQueue?.Enqueue(
                    LocalizationProvider.GetLocalizedString("AppointmentTimeTaken"),
                    null, null, null, false, true, TimeSpan.FromSeconds(5));
                return;
            }

            if (isEditMode)
            {
                AppointmentUpdated = true;
            }
            else
            {
                AppointmentAdded = true;
            }

            this.Close();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            AppointmentAdded = false;
            AppointmentUpdated = false;
            this.Close();
        }
    }
}