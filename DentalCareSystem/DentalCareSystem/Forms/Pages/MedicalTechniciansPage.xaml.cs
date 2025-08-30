using DentalCareSystem.Data.DAO.Exceptions;
using DentalCareSystem.Data.DAO.MySQL;
using DentalCareSystem.Data.DTO;
using DentalCareSystem.Util;
using DentalCareSystem.ViewModel;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;

namespace DentalCareSystem.Forms.Pages
{
    public partial class MedichalTechniciansPage : Page
    {
        private readonly GenericDataGridViewModel<Employee> medichalTechnicianViewModel;
        private readonly EmployeeDAOImpl dao = new EmployeeDAOImpl();

        public MedichalTechniciansPage()
        {
            InitializeComponent();
            this.medichalTechnicianViewModel = new GenericDataGridViewModel<Employee>()
            {
                Items = new ObservableCollection<Employee>(dao.GetMedicalTechnicians())
            };

            this.DataContext = medichalTechnicianViewModel;
            LocalizationProvider.UpdateAllObjects();
        }

        private void SearchJMBG_Click(object sender, RoutedEventArgs e)
        {
            string searchJMBG = SearchJMBGTextBox.Text.Trim();
            if (string.IsNullOrEmpty(searchJMBG))
                return;

            try
            {
                Employee technician = dao.GetEmployeeByJMBG(searchJMBG);
                if (technician != null)
                {
                    medichalTechnicianViewModel.Items.Clear();
                    medichalTechnicianViewModel.Items.Add(technician);
                }
                else
                {
                    medichalTechnicianViewModel.Items.Clear();
                    MedichalTechniciansSnackbar.MessageQueue?.Enqueue(LocalizationProvider.GetLocalizedString("MedicalTechnicianNotFound"), null, null, null, false, true, TimeSpan.FromSeconds(3));
                }
            }
            catch (DataAccessException)
            {
                MedichalTechniciansSnackbar.MessageQueue?.Enqueue(LocalizationProvider.GetLocalizedString("ErrorSearchingMedicalTechnician"), null, null, null, false, true, TimeSpan.FromSeconds(3));
            }
        }

        private void ResetSearch_Click(object sender, RoutedEventArgs e)
        {
            medichalTechnicianViewModel.Items.Clear();
            foreach (var p in dao.GetDentists())
            {
                medichalTechnicianViewModel.Items.Add(p);
            }
            SearchJMBGTextBox.Clear();
        }
        
    }
}
