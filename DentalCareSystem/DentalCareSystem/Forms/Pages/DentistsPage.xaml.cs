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
    public partial class DentistsPage : Page
    {
        private readonly GenericDataGridViewModel<Employee> dentistViewModel;
        private readonly EmployeeDAOImpl dao = new EmployeeDAOImpl();

        public DentistsPage()
        {
            InitializeComponent();
            this.dentistViewModel = new GenericDataGridViewModel<Employee>()
            {
                Items = new ObservableCollection<Employee>(dao.GetDentists())
            };

            this.DataContext = dentistViewModel;
            LocalizationProvider.UpdateAllObjects();
        }

        private void SearchJMBG_Click(object sender, RoutedEventArgs e)
        {
            string searchJMBG = SearchJMBGTextBox.Text.Trim();
            if (string.IsNullOrEmpty(searchJMBG))
                return;

            try
            {
                Employee dentist = dao.GetEmployeeByJMBG(searchJMBG);
                if (dentist != null)
                {
                    dentistViewModel.Items.Clear();
                    dentistViewModel.Items.Add(dentist);
                }
                else
                {
                    dentistViewModel.Items.Clear();
                    DentistsSnackbar.MessageQueue?.Enqueue(LocalizationProvider.GetLocalizedString("DentistNotFound"), null, null, null, false, true, TimeSpan.FromSeconds(3));
                }
            }
            catch (DataAccessException)
            {
                DentistsSnackbar.MessageQueue?.Enqueue(LocalizationProvider.GetLocalizedString("ErrorSearchingDentist"), null, null, null, false, true, TimeSpan.FromSeconds(3));
            }
        }

        private void ResetSearch_Click(object sender, RoutedEventArgs e)
        {
            dentistViewModel.Items.Clear();
            foreach (var p in dao.GetDentists())
            {
                dentistViewModel.Items.Add(p);
            }
            SearchJMBGTextBox.Clear();
        }
    }
}
