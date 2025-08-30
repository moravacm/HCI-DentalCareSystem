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
    public partial class PatientsPage : Page
    {
        private readonly GenericDataGridViewModel<Patient> patientViewModel;
        private readonly PatientDAOImpl dao = new PatientDAOImpl();

        public PatientsPage()
        {
            InitializeComponent();
            this.patientViewModel = new GenericDataGridViewModel<Patient>()
            {
                Items = new ObservableCollection<Patient>(dao.GetPatients())
            };

            this.DataContext = patientViewModel;
            LocalizationProvider.UpdateAllObjects();
        }

        public void Delete_Click(object sender, RoutedEventArgs e)
        {
            if (DataGridPatients.SelectedItems.Count == 0)
            {
                new MessageBoxCustom("NoSelection", MessageType.Info, MessageButtons.Ok).ShowDialog();
            }
            else
            {
                bool? Result = new MessageBoxCustom("ConfirmDelete", MessageType.Confirmation, MessageButtons.YesNo).ShowDialog();
                if (Result == false)
                    return;

                int res = 0, toDelete = DataGridPatients.SelectedItems.Count;
                List<Patient> selectedPatients = new List<Patient>();
                foreach (Patient patient in DataGridPatients.SelectedItems)
                    selectedPatients.Add(patient);

                try
                {
                    foreach (Patient patient in selectedPatients)
                    {
                        if (dao.DeletePatient(patient.JMBG) == true)
                            res++;

                        patientViewModel.Items.Remove(patient);
                    }
                    if (res == toDelete)
                        PatientsSnackbar.MessageQueue?.Enqueue(LocalizationProvider.GetLocalizedString("SuccessfullyDeleted"), null, null, null, false, true, TimeSpan.FromSeconds(3));
                    else
                        PatientsSnackbar.MessageQueue?.Enqueue(LocalizationProvider.GetLocalizedString("UnsuccessfullyDeleted"), null, null, null, false, true, TimeSpan.FromSeconds(3));
                }
                catch (DataAccessException ex)
                {
                    if (ex.InnerException?.Message.Contains("foreign key constraint fails") == true)
                    {
                        PatientsSnackbar.MessageQueue?.Enqueue(
                            LocalizationProvider.GetLocalizedString("DeleteNotAllowed"),
                            null, null, null, false, true, TimeSpan.FromSeconds(5));
                    }
                    else
                    {
                        PatientsSnackbar.MessageQueue?.Enqueue(
                            LocalizationProvider.GetLocalizedString("ErrorOccurred"),
                            null, null, null, false, true, TimeSpan.FromSeconds(5));
                    }
                }
            }
        }

        public void Add_Click(object sender, RoutedEventArgs e)
        {
            PatientModal modal = new PatientModal();
            modal.Owner = Window.GetWindow(this);
            modal.ShowDialog();
            if (modal.PatientAdded)
            {
                patientViewModel.Items.Add(modal.GetPatient());
                dao.AddPatient(modal.GetPatient());
                PatientsSnackbar.MessageQueue?.Enqueue(LocalizationProvider.GetLocalizedString("PatientAdded"), null, null, null, false, true, TimeSpan.FromSeconds(5));
            }
        }

        public void Update_Click(object sender, RoutedEventArgs e)
        {
            Patient selectedPatient = (Patient)DataGridPatients.SelectedItem;
            if (selectedPatient != null)
            {
                PatientModal modal = new PatientModal(selectedPatient);
                modal.Owner = Window.GetWindow(this);
                modal.ShowDialog();
                if (modal.PatientUpdated)
                {
                    patientViewModel.Items.Remove(selectedPatient);
                    patientViewModel.Items.Add(modal.GetPatient());
                    dao.UpdatePatient(modal.GetPatient());
                    PatientsSnackbar.MessageQueue?.Enqueue(LocalizationProvider.GetLocalizedString("PatientUpdated"), null, null, null, false, true, TimeSpan.FromSeconds(5));
                }
            }
            else
            {
                new MessageBoxCustom("NoSelection", MessageType.Info, MessageButtons.Ok).ShowDialog();
            }
        }

        private void SearchJMBG_Click(object sender, RoutedEventArgs e)
        {
            string searchJMBG = SearchJMBGTextBox.Text.Trim();
            if (string.IsNullOrEmpty(searchJMBG))
                return;

            try
            {
                Patient patient = dao.GetPatientByJMBG(searchJMBG);
                if (patient != null)
                {
                    patientViewModel.Items.Clear();
                    patientViewModel.Items.Add(patient);
                }
                else
                {
                    patientViewModel.Items.Clear();
                    PatientsSnackbar.MessageQueue?.Enqueue(LocalizationProvider.GetLocalizedString("PatientNotFound"), null, null, null, false, true, TimeSpan.FromSeconds(3));
                }
            }
            catch (DataAccessException)
            {
                PatientsSnackbar.MessageQueue?.Enqueue(LocalizationProvider.GetLocalizedString("ErrorSearchingPatient"), null, null, null, false, true, TimeSpan.FromSeconds(3));
            }
        }

        private void ResetSearch_Click(object sender, RoutedEventArgs e)
        {
            patientViewModel.Items.Clear();
            foreach (var p in dao.GetPatients())
            {
                patientViewModel.Items.Add(p);
            }
            SearchJMBGTextBox.Clear();
        }

    }
}
