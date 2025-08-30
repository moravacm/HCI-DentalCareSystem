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
    public partial class TreatmentsPage : Page
    {
        private readonly GenericDataGridViewModel<Treatment> treatmentViewModel;
        private readonly TreatmentDAOImpl dao = new TreatmentDAOImpl();

        public TreatmentsPage()
        {
            InitializeComponent();
            this.treatmentViewModel = new GenericDataGridViewModel<Treatment>
            {
                Items = new ObservableCollection<Treatment>(dao.GetTreatments())
            };
            this.DataContext = treatmentViewModel;
            LocalizationProvider.UpdateAllObjects();
        }

        private void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var allTreatments = new ObservableCollection<Treatment>(dao.GetTreatments());
                treatmentViewModel.Items.Clear();
                foreach (var treatment in allTreatments)
                {
                    treatmentViewModel.Items.Add(treatment);
                }
                Snackbar.MessageQueue?.Enqueue(LocalizationProvider.GetLocalizedString("Refreshed"), null, null, null, false, true, TimeSpan.FromSeconds(3));
            }
            catch (DataAccessException ex)
            {
                Snackbar.MessageQueue?.Enqueue(LocalizationProvider.GetLocalizedString("ErrorRefreshingData") + ": " + ex.Message, null, null, null, false, true, TimeSpan.FromSeconds(5));
            }
        }

        private void ApplySortButton_Click(object sender, RoutedEventArgs e)
        {
            if (SortComboBox.SelectedIndex == 0)
            {
                var sortedList = treatmentViewModel.Items.OrderBy(t => t.Price).ToList();

                treatmentViewModel.Items.Clear();
                foreach (var item in sortedList)
                {
                    treatmentViewModel.Items.Add(item);
                }

                Snackbar.MessageQueue?.Enqueue(LocalizationProvider.GetLocalizedString("Sorted"), null, null, null, false, true, TimeSpan.FromSeconds(3));
            }
            else if (SortComboBox.SelectedIndex == 1)
            {
                var sortedList = treatmentViewModel.Items.OrderByDescending(t => t.Price).ToList();

                treatmentViewModel.Items.Clear();
                foreach (var item in sortedList)
                {
                    treatmentViewModel.Items.Add(item);
                }

                Snackbar.MessageQueue?.Enqueue(LocalizationProvider.GetLocalizedString("Sorted"), null, null, null, false, true, TimeSpan.FromSeconds(3));
            }
        }

        private void ClearSortButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var originalList = new ObservableCollection<Treatment>(dao.GetTreatments());
                treatmentViewModel.Items.Clear();
                foreach (var treatment in originalList)
                {
                    treatmentViewModel.Items.Add(treatment);
                }

                SortComboBox.SelectedIndex = -1;
                Snackbar.MessageQueue?.Enqueue(LocalizationProvider.GetLocalizedString("SortCleared"), null, null, null, false, true, TimeSpan.FromSeconds(3));
            }
            catch (DataAccessException ex)
            {
                Snackbar.MessageQueue?.Enqueue(LocalizationProvider.GetLocalizedString("ErrorRefreshingData") + ": " + ex.Message, null, null, null, false, true, TimeSpan.FromSeconds(5));
            }
        }

        public void Delete_Click(object sender, RoutedEventArgs e)
        {
            if (DataGridTreatments.SelectedItems.Count == 0)
            {
                new MessageBoxCustom(LocalizationProvider.GetLocalizedString("NoSelection"), MessageType.Info, MessageButtons.Ok).ShowDialog();
            }
            else
            {
                bool? result = new MessageBoxCustom(LocalizationProvider.GetLocalizedString("ConfirmDelete"), MessageType.Confirmation, MessageButtons.YesNo).ShowDialog();
                if (result == false)
                    return;

                int successfulDeletions = 0;
                List<Treatment> selectedTreatments = DataGridTreatments.SelectedItems.Cast<Treatment>().ToList();

                try
                {
                    foreach (Treatment treatment in selectedTreatments)
                    {
                        if (dao.DeleteTreatment(treatment.TreatmentId))
                        {
                            treatmentViewModel.Items.Remove(treatment);
                            successfulDeletions++;
                        }
                    }
                    if (successfulDeletions == selectedTreatments.Count)
                    {
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
        }

        public void Add_Click(object sender, RoutedEventArgs e)
        {
            TreatmentModal modal = new TreatmentModal();
            if (modal.ShowDialog() == true)
            {
                Treatment newTreatment = modal.Treatment;
                try
                {
                    dao.AddTreatment(newTreatment);
                    treatmentViewModel.Items.Add(newTreatment);
                    Snackbar.MessageQueue?.Enqueue(LocalizationProvider.GetLocalizedString("TreatmentAdded"), null, null, null, false, true, TimeSpan.FromSeconds(5));
                }
                catch (DataAccessException ex)
                {
                    Snackbar.MessageQueue?.Enqueue(LocalizationProvider.GetLocalizedString("ErrorOccured") + ": " + ex.Message, null, null, null, false, true, TimeSpan.FromSeconds(5));
                }
            }
        }

        public void Update_Click(object sender, RoutedEventArgs e)
        {
            Treatment selectedTreatment = (Treatment)DataGridTreatments.SelectedItem;
            if (selectedTreatment != null)
            {
                TreatmentModal modal = new TreatmentModal(selectedTreatment);
                if (modal.ShowDialog() == true)
                {
                    Treatment updatedTreatment = modal.Treatment;
                    try
                    {
                        dao.UpdateTreatment(updatedTreatment);

                        selectedTreatment.Name = updatedTreatment.Name;
                        selectedTreatment.Price = updatedTreatment.Price;

                        Snackbar.MessageQueue?.Enqueue(LocalizationProvider.GetLocalizedString("TreatmentUpdated"), null, null, null, false, true, TimeSpan.FromSeconds(5));
                    }
                    catch (DataAccessException ex)
                    {
                        Snackbar.MessageQueue?.Enqueue(LocalizationProvider.GetLocalizedString("ErrorOccured") + ": " + ex.Message, null, null, null, false, true, TimeSpan.FromSeconds(5));
                    }
                }
            }
            else
            {
                new MessageBoxCustom(LocalizationProvider.GetLocalizedString("NoSelection"), MessageType.Info, MessageButtons.Ok).ShowDialog();
            }
        }
    }
}