using DentalCareSystem.Data.DTO;
using DentalCareSystem.Util;
using System.Windows;

namespace DentalCareSystem.Forms.Windows.Modals
{
    public partial class PatientModal : Window
    {
        private readonly Patient newPatient = new Patient();
        public bool PatientAdded { get; set; } = false;
        public bool PatientUpdated { get; set; } = false;
        private readonly bool isEditMode = false;

        public PatientModal()
        {
            this.DataContext = newPatient;
            isEditMode = false;
            InitializeComponent();
            LocalizationProvider.UpdateAllObjects();
            ModalTitle.Content = LocalizationProvider.GetLocalizedString("AddPatientTitle");
        }

        public PatientModal(Patient patient)
        {
            this.DataContext = patient;
            isEditMode = true;
            InitializeComponent();
            newPatient.JMBG = patient.JMBG;
            newPatient.FirstName = patient.FirstName;
            newPatient.LastName = patient.LastName;
            newPatient.PhoneNumber = patient.PhoneNumber;
            newPatient.Email = patient.Email;
            newPatient.Address = patient.Address;
            newPatient.MedicalHistory = patient.MedicalHistory;
            LocalizationProvider.UpdateAllObjects();
            ModalTitle.Content = LocalizationProvider.GetLocalizedString("UpdatePatientTitle");
        }

        private void AddOrUpdate_Click(object sender, RoutedEventArgs e)
        {
            if (String.IsNullOrEmpty(FirstNameTextBox.Text) || String.IsNullOrEmpty(LastNameTextBox.Text) ||
                String.IsNullOrEmpty(PhoneNumberTextBox.Text) || String.IsNullOrEmpty(EmailTextBox.Text) ||
                String.IsNullOrEmpty(AddressTextBox.Text) || String.IsNullOrEmpty(IdTextBox.Text))
            {
                PatientModalSnackbar.MessageQueue?.Enqueue(LocalizationProvider.GetLocalizedString("FillFields"), null, null, null, false, true, TimeSpan.FromSeconds(3));
                return;
            }

            if (IdTextBox.Text.Length != 13 || !long.TryParse(IdTextBox.Text, out _))
            {
                PatientModalSnackbar.MessageQueue?.Enqueue(LocalizationProvider.GetLocalizedString("InvalidJmbgFormat"), null, null, null, false, true, TimeSpan.FromSeconds(3));
                return;
            }

            if (!string.IsNullOrEmpty(EmailTextBox.Text) && !EmailTextBox.Text.Contains("@"))
            {
                PatientModalSnackbar.MessageQueue?.Enqueue(LocalizationProvider.GetLocalizedString("InvalidEmailFormat"), null, null, null, false, true, TimeSpan.FromSeconds(3));
                return;
            }

            if (!string.IsNullOrEmpty(PhoneNumberTextBox.Text) && PhoneNumberTextBox.Text.Any(char.IsLetter))
            {
                PatientModalSnackbar.MessageQueue?.Enqueue(LocalizationProvider.GetLocalizedString("InvalidPhoneFormat"), null, null, null, false, true, TimeSpan.FromSeconds(3));
                return;
            }

            newPatient.JMBG = IdTextBox.Text;
            newPatient.FirstName = FirstNameTextBox.Text;
            newPatient.LastName = LastNameTextBox.Text;
            newPatient.PhoneNumber = PhoneNumberTextBox.Text;
            newPatient.Email = EmailTextBox.Text;
            newPatient.Address = AddressTextBox.Text;
            newPatient.MedicalHistory = MedicalHistoryTextBox.Text;

            if (isEditMode)
            {
                PatientUpdated = true;
            }
            else
            {
                PatientAdded = true;
            }

            this.Close();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            PatientAdded = false;
            PatientUpdated = false;
            this.Close();
        }

        public Patient GetPatient()
        {
            return newPatient;
        }


    }
}
