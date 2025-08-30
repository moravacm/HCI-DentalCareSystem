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

namespace DentalCareSystem.Forms.Pages
{

    public partial class EmployeesPage : Page
    {
        private readonly GenericDataGridViewModel<Employee> employeeViewModel;
        private readonly EmployeeDAOImpl dao = new EmployeeDAOImpl();

        public EmployeesPage()
        {
            
            this.employeeViewModel = new GenericDataGridViewModel<Employee>()
            {
                Items = new ObservableCollection<Employee>(dao.GetEmployees())
            };

            this.DataContext = employeeViewModel;
            InitializeComponent();
            LocalizationProvider.UpdateAllObjects();
        }

        public void Delete_Click(object sender, RoutedEventArgs e)
        {
            if (DataGridEmployees.SelectedItems.Count == 0)
            {
                new MessageBoxCustom("NoSelection", MessageType.Info, MessageButtons.Ok).ShowDialog();
            }
            else
            {
                bool? Result = new MessageBoxCustom("ConfirmDelete", MessageType.Confirmation, MessageButtons.YesNo).ShowDialog();
                if (Result == false)
                    return;

                int res = 0, toDelete = DataGridEmployees.SelectedItems.Count;
                List<Employee> selectedEmployees = new List<Employee>();
                foreach (Employee employee in DataGridEmployees.SelectedItems)
                    selectedEmployees.Add(employee);
                try
                {
                    foreach (Employee employee in selectedEmployees)
                    {
                        if (dao.DeleteEmployee(employee.JMBG) == true)
                            res++;

                        employeeViewModel.Items.Remove(employee);
                    }
                    if (res == toDelete)
                        EmployeesSnackbar.MessageQueue?.Enqueue(LocalizationProvider.GetLocalizedString("SuccessfullyDeleted"), null, null, null, false, true, TimeSpan.FromSeconds(3));
                    else
                        EmployeesSnackbar.MessageQueue?.Enqueue(LocalizationProvider.GetLocalizedString("UnsuccessfullyDeleted"), null, null, null, false, true, TimeSpan.FromSeconds(3));
                }
                catch (DataAccessException ex)
                {
                    if (ex.InnerException?.Message.Contains("foreign key constraint fails") == true)
                    {
                        EmployeesSnackbar.MessageQueue?.Enqueue(
                            LocalizationProvider.GetLocalizedString("DeleteNotAllowed"),
                            null, null, null, false, true, TimeSpan.FromSeconds(5));
                    }
                    else
                    {
                        EmployeesSnackbar.MessageQueue?.Enqueue(
                            LocalizationProvider.GetLocalizedString("ErrorOccurred"),
                            null, null, null, false, true, TimeSpan.FromSeconds(5));
                    }
                }
            }
        }


        public void Add_Click(object sender, RoutedEventArgs e)
        {
            EmployeeModal modal = new EmployeeModal();
            modal.ShowDialog();
            if (modal.EmployeeAdded)
            {
                employeeViewModel.Items.Add(modal.GetEmployee());
                dao.AddEmployee(modal.GetEmployee());
                EmployeesSnackbar.MessageQueue?.Enqueue(LocalizationProvider.GetLocalizedString("EmployeeAdded"), null, null, null, false, true, TimeSpan.FromSeconds(5));
            }
        }

        private void Search_Click(object sender, RoutedEventArgs e)
        {
            string searchJMBG = SearchJMBGTextBox.Text.Trim();

            string selectedContent = (RoleComboBox.SelectedItem as ComboBoxItem)?.Content as string;

            try
            {
                employeeViewModel.Items.Clear();

                if (!string.IsNullOrEmpty(searchJMBG))
                {
                    Employee employee = dao.GetEmployeeByJMBG(searchJMBG);
                    if (employee != null)
                    {                   
                        employeeViewModel.Items.Add(employee);         
                    }
                    else
                    {
                        EmployeesSnackbar.MessageQueue?.Enqueue(LocalizationProvider.GetLocalizedString("EmployeeNotFound"), null, null, null, false, true, TimeSpan.FromSeconds(3));
                    }
                }
                else
                {
                    List<Employee> employees = null;
                    if (string.Equals(selectedContent, LocalizationProvider.GetLocalizedString("Dentist"), StringComparison.OrdinalIgnoreCase))
                    {
                        employees = dao.GetDentists();
                    }
                    else if (string.Equals(selectedContent, LocalizationProvider.GetLocalizedString("MedicalTechnician"), StringComparison.OrdinalIgnoreCase))
                    {
                        employees = dao.GetMedicalTechnicians();
                    }
                    else if (string.Equals(selectedContent, LocalizationProvider.GetLocalizedString("Administrator"), StringComparison.OrdinalIgnoreCase))
                    {
                        employees = dao.GetAdmins();
                    }
                    else
                    {
                        ResetSearch_Click(null, null);
                        return;
                    }

                    if (employees != null && employees.Any())
                    {
                        foreach (var emp in employees)
                        {
                            employeeViewModel.Items.Add(emp);
                        }
                    }
                    else
                    {
                        EmployeesSnackbar.MessageQueue?.Enqueue(LocalizationProvider.GetLocalizedString("NoEmployeesFound"), null, null, null, false, true, TimeSpan.FromSeconds(3));
                    }
                }
            }
            catch (DataAccessException)
            {
                EmployeesSnackbar.MessageQueue?.Enqueue(LocalizationProvider.GetLocalizedString("ErrorSearchingEmployee"), null, null, null, false, true, TimeSpan.FromSeconds(3));
            }
        }

        private void ResetSearch_Click(object sender, RoutedEventArgs e)
        {
            employeeViewModel.Items.Clear();
            foreach (var emp in dao.GetEmployees())
            {
                employeeViewModel.Items.Add(emp);
            }
            SearchJMBGTextBox.Clear();
            RoleComboBox.SelectedIndex = 0;
        }
    }
}