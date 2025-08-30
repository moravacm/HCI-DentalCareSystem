using DentalCareSystem.Data.DAO.MySQL;
using DentalCareSystem.Data.DTO;
using DentalCareSystem.Util;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;

namespace DentalCareSystem.Forms.Windows.Modals
{
    public partial class AppointmentSelectionModal : Window, INotifyPropertyChanged
    {
        public bool AppointmentSelected { get; set; } = false;

        private readonly AppointmentDAOImpl appointmentDao = new AppointmentDAOImpl();
        private readonly PatientDAOImpl patientDao = new PatientDAOImpl();
        private readonly EmployeeDAOImpl dentistDao = new EmployeeDAOImpl();

        private ObservableCollection<Appointment> appointments;
        public ObservableCollection<Appointment> Appointments
        {
            get => appointments;
            set { appointments = value; OnPropertyChanged(); }
        }

        private Appointment selectedAppointment;
        public Appointment SelectedAppointment
        {
            get => selectedAppointment;
            set { selectedAppointment = value; OnPropertyChanged(); }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public AppointmentSelectionModal()
        {
            InitializeComponent();
            LoadAppointments();
            this.DataContext = this;
            LocalizationProvider.UpdateAllObjects();
        }

        private void LoadAppointments()
        {
            var loadedAppointments = appointmentDao.GetAppointments();

            var patients = patientDao.GetPatients().ToDictionary(p => p.JMBG, p => $"{p.FirstName} {p.LastName}");
            var dentists = dentistDao.GetDentists().ToDictionary(d => d.JMBG, d => $"{d.FirstName} {d.LastName}");

            foreach (var appointment in loadedAppointments)
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

            Appointments = new ObservableCollection<Appointment>(loadedAppointments);
        }

        public Appointment GetSelectedAppointment()
        {
            return SelectedAppointment;
        }

        private void SelectAppointment_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedAppointment == null)
            {
                AppointmentSelectionSnackbar.MessageQueue?.Enqueue(LocalizationProvider.GetLocalizedString("SelectAppointment"), null, null, null, false, true, TimeSpan.FromSeconds(3));
                return;
            }

            AppointmentSelected = true;
            this.Close();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            AppointmentSelected = false;
            this.Close();
        }
    }
}
