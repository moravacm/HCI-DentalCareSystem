using DentalCareSystem.Data.DTO;
using DentalCareSystem.Util;
using System.Windows;
using System.Windows.Controls;

namespace DentalCareSystem.Forms.Windows.Modals
{
    public partial class EmployeeModal : Window
    {
        private readonly Employee newEmployee = new Employee();
        public bool EmployeeAdded { get; set; } = false;

        public EmployeeModal()
        {
            this.DataContext = newEmployee;
            InitializeComponent();
            LocalizationProvider.UpdateAllObjects();
        }

        private void Add_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(NameTextBox.Text) ||
                string.IsNullOrEmpty(LastNameTextBox.Text) ||
                string.IsNullOrEmpty(UsernameTextBox.Text) ||
                string.IsNullOrEmpty(PasswordBox.Password) ||
                string.IsNullOrEmpty(PhoneNumberTextBox.Text) ||
                string.IsNullOrEmpty(EmailTextBox.Text) ||
                string.IsNullOrEmpty(IdTextBox.Text))
            {
                EmployeeModalSnackbar.MessageQueue?.Enqueue(LocalizationProvider.GetLocalizedString("FillFields"), null, null, null, false, true, TimeSpan.FromSeconds(3));
                return;
            }

            if (IdTextBox.Text.Length != 13 || !long.TryParse(IdTextBox.Text, out _))
            {
                EmployeeModalSnackbar.MessageQueue?.Enqueue(LocalizationProvider.GetLocalizedString("InvalidJmbgFormat"), null, null, null, false, true, TimeSpan.FromSeconds(3));
                return;
            }

            if (!string.IsNullOrEmpty(EmailTextBox.Text) && !EmailTextBox.Text.Contains("@"))
            {
                EmployeeModalSnackbar.MessageQueue?.Enqueue(LocalizationProvider.GetLocalizedString("InvalidEmailFormat"), null, null, null, false, true, TimeSpan.FromSeconds(3));
                return;
            }

            if (!string.IsNullOrEmpty(PhoneNumberTextBox.Text) && PhoneNumberTextBox.Text.Any(char.IsLetter))
            {
                EmployeeModalSnackbar.MessageQueue?.Enqueue(LocalizationProvider.GetLocalizedString("InvalidPhoneFormat"), null, null, null, false, true, TimeSpan.FromSeconds(3));
                return;
            }

            EmployeeAdded = true;
            newEmployee.Password = PasswordBox.Password;
            this.Close();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            EmployeeAdded = false;
            this.Close();
        }

        public Employee GetEmployee()
        {
            return newEmployee;
        }
    }
}