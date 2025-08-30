using CustomMessageBox;
using DentalCareSystem.Data.DAO.Exceptions;
using DentalCareSystem.Data.DAO.MySQL;
using DentalCareSystem.Data.DTO;
using DentalCareSystem.Forms.Windows.Modals;
using DentalCareSystem.Util;
using DentalCareSystem.ViewModel;
using System.Collections.ObjectModel;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace DentalCareSystem.Forms.Pages
{
    public partial class ExaminationPage : Page
    {

        public static event Action ExaminationAdded;

        private readonly GenericDataGridViewModel<Examination> examinationViewModel;
        private readonly ExaminationDAOImpl examinationDao = new ExaminationDAOImpl();
        private readonly PatientDAOImpl patientDao = new PatientDAOImpl();
        private readonly EmployeeDAOImpl dentistDao = new EmployeeDAOImpl();
        private ObservableCollection<DentistComboBoxItem> dentistsList = new ObservableCollection<DentistComboBoxItem>();

        public ExaminationPage()
        {
            InitializeComponent();
            LoadDentists();

            this.examinationViewModel = new GenericDataGridViewModel<Examination>()
            {
                Items = new ObservableCollection<Examination>(examinationDao.GetExaminations())
            };

            UpdateExaminationDisplayData();
            this.DataContext = examinationViewModel;
            LocalizationProvider.UpdateAllObjects();
        }

        public class DentistComboBoxItem
        {
            public string JMBG { get; set; }
            public string DisplayName { get; set; }
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

        private void UpdateExaminationDisplayData()
        {
            try
            {
                var patients = patientDao.GetPatients().ToDictionary(p => p.JMBG, p => $"{p.FirstName} {p.LastName}");
                var dentists = dentistDao.GetDentists().ToDictionary(d => d.JMBG, d => $"{d.FirstName} {d.LastName}");
                var medicalTechnicians = dentistDao.GetMedicalTechnicians().ToDictionary(mt => mt.JMBG, mt => $"{mt.FirstName} {mt.LastName}");

                foreach (var examination in examinationViewModel.Items)
                {
                    if (examination != null)
                    {
                        if (!string.IsNullOrEmpty(examination.PatientJMBG))
                        {
                            if (patients.ContainsKey(examination.PatientJMBG))
                            {
                                examination.PatientName = patients[examination.PatientJMBG];
                            }
                            else
                            {
                                examination.PatientName = "Nepoznat pacijent";
                                Console.WriteLine($"Patient JMBG {examination.PatientJMBG} not found in dictionary!");
                            }
                        }
                        else
                        {
                            examination.PatientName = "Nepoznat pacijent";
                            Console.WriteLine("PatientJMBG is null or empty!");
                        }

                        if (!string.IsNullOrEmpty(examination.DentistJMBG))
                        {
                            if (dentists.ContainsKey(examination.DentistJMBG))
                            {
                                examination.DentistName = dentists[examination.DentistJMBG];
                            }
                            else
                            {
                                examination.DentistName = "Nepoznat stomatolog";
                                Console.WriteLine($"Dentist JMBG {examination.DentistJMBG} not found in dictionary!");
                            }
                        }
                        else
                        {
                            examination.DentistName = "Nepoznat stomatolog";
                            Console.WriteLine("DentistJMBG is null or empty!");
                        }

                        if (!string.IsNullOrEmpty(examination.MedicalTechnicianJMBG))
                        {
                            if (medicalTechnicians.ContainsKey(examination.MedicalTechnicianJMBG))
                            {
                                examination.MedicalTechnicianName = medicalTechnicians[examination.MedicalTechnicianJMBG];
                            }
                            else
                            {
                                examination.MedicalTechnicianName = "Nepoznat medicinski tehničar";
                                Console.WriteLine($"MedTech JMBG {examination.MedicalTechnicianJMBG} not found in dictionary!");
                            }
                        }
                        else
                        {
                            examination.MedicalTechnicianName = "Nepoznat medicinski tehničar";
                            Console.WriteLine("MedicalTechnicianJMBG is null or empty!");
                        }
                    }
                    else
                    {
                        Console.WriteLine("Examination object is null!");
                    }
                }
            }
            catch (Exception)
            {
                Snackbar.MessageQueue?.Enqueue(LocalizationProvider.GetLocalizedString("ErrorOccured"), null, null, null, false, true, TimeSpan.FromSeconds(5));
            }
        }

        private void ApplyFilter_Click(object sender, RoutedEventArgs e)
        {
            DateTime? selectedDate = DatePicker.SelectedDate;
            string patientJMBG = PatientFilterTextBox.Text.Trim();
            string dentistJMBG = DentistComboBox.SelectedValue?.ToString() ?? "";

            if (!selectedDate.HasValue && string.IsNullOrEmpty(patientJMBG) && string.IsNullOrEmpty(dentistJMBG))
            {
                examinationViewModel.Items = new ObservableCollection<Examination>(examinationDao.GetExaminations());
                UpdateExaminationDisplayData();
                Snackbar.MessageQueue?.Enqueue(LocalizationProvider.GetLocalizedString("AllExaminationsLoaded"), null, null, null, false, true, TimeSpan.FromSeconds(3));
            }
            else
            {
                var filteredList = new List<Examination>();

                if (selectedDate.HasValue)
                {
                    filteredList = examinationDao.GetExaminationsByDate(selectedDate.Value);
                }
                else
                {
                    filteredList = examinationDao.GetExaminations();
                }

                var finalFilteredList = filteredList.Where(examination =>
                    (string.IsNullOrEmpty(patientJMBG) || examination.PatientJMBG.Contains(patientJMBG)) &&
                    (string.IsNullOrEmpty(dentistJMBG) || examination.DentistJMBG == dentistJMBG)
                ).ToList();

                examinationViewModel.Items = new ObservableCollection<Examination>(finalFilteredList);
                UpdateExaminationDisplayData();

                if (finalFilteredList.Count == 0)
                {
                    string message = LocalizationProvider.GetLocalizedString("NoExaminationsFound");

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

            DataGridExaminations.ItemsSource = examinationViewModel.Items;
            ResetFilters();
        }

        private void ResetFilters()
        {
            DatePicker.SelectedDate = null;
            DatePicker.Text = string.Empty;
            PatientFilterTextBox.Text = string.Empty;
            DentistComboBox.SelectedIndex = 0;
        }

        private void PopupBox_Closed(object sender, EventArgs e)
        {
            ResetFilters();
        }

        private void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            examinationViewModel.Items.Clear();
            var examinations = examinationDao.GetExaminations();
            foreach (var examination in examinations)
            {
                examinationViewModel.Items.Add(examination);
            }
            UpdateExaminationDisplayData();

            Snackbar.MessageQueue?.Enqueue(LocalizationProvider.GetLocalizedString("Refreshed"), null, null, null, false, true, TimeSpan.FromSeconds(3));
        }

        private void ApplySortButton_Click(object sender, RoutedEventArgs e)
        {
            if (SortComboBox.SelectedItem is ComboBoxItem selectedItem)
            {
                int sortOption = int.Parse(selectedItem.Tag.ToString());

                if (examinationViewModel.Items == null || examinationViewModel.Items.Count == 0)
                    return;

                switch (sortOption)
                {
                    case 0:
                        examinationViewModel.Items = new ObservableCollection<Examination>(
                            examinationViewModel.Items.OrderBy(ex => ex.ExaminationDate));
                        Snackbar.MessageQueue?.Enqueue(LocalizationProvider.GetLocalizedString("Sorted"), null, null, null, false, true, TimeSpan.FromSeconds(2));
                        break;
                    case 1:
                        examinationViewModel.Items = new ObservableCollection<Examination>(
                            examinationViewModel.Items.OrderByDescending(ex => ex.ExaminationDate));
                        Snackbar.MessageQueue?.Enqueue(LocalizationProvider.GetLocalizedString("Sorted"), null, null, null, false, true, TimeSpan.FromSeconds(2));
                        break;
                }

                DataGridExaminations.ItemsSource = examinationViewModel.Items;
            }
        }

        public void AddExamination_Click(object sender, RoutedEventArgs e)
        {
            ExaminationModal modal = new ExaminationModal();
            modal.ShowDialog();

            if (modal.ExaminationAdded)
            {
                try
                {
                    var newExamination = modal.GetExamination();
                    int newId = examinationDao.AddExamination(newExamination);
                    newExamination.ExaminationId = newId;

                    foreach (var treatment in modal.SelectedTreatments)
                    {
                        examinationDao.AddTreatmentToExamination(newId, treatment.TreatmentId, treatment.Price);
                    }

                    foreach (var material in modal.SelectedMaterials)
                    {
                        string errorMessage;
                        bool success = examinationDao.AddMaterialToExamination(
                            newId, material.idMaterijala, material.Kolicina, out errorMessage);

                        if (success && !string.IsNullOrEmpty(errorMessage))
                        {
                            Snackbar.MessageQueue?.Enqueue(
                                $"{errorMessage}", 
                                null, null, null, false, true, TimeSpan.FromSeconds(5));
                        }
                        else if (!success)
                        {
                            Snackbar.MessageQueue?.Enqueue(
                                $"{errorMessage}", 
                                null, null, null, false, true, TimeSpan.FromSeconds(5));
                        }
                    }

                    examinationViewModel.Items.Add(newExamination);
                    UpdateExaminationDisplayData();
                    ExaminationAdded?.Invoke();

                    Snackbar.MessageQueue?.Enqueue(LocalizationProvider.GetLocalizedString("ExaminationAdded"), null, null, null, false, true, TimeSpan.FromSeconds(5));
                }
                catch (DataAccessException)
                {
                    Snackbar.MessageQueue?.Enqueue(LocalizationProvider.GetLocalizedString("ErrorOccured"), null, null, null, false, true, TimeSpan.FromSeconds(5));
                }
            }
        }

        public void UpdateExamination_Click(object sender, RoutedEventArgs e)
        {
            Examination selectedExamination = (Examination)DataGridExaminations.SelectedItem;
            if (selectedExamination == null)
            {
                Snackbar.MessageQueue?.Enqueue(LocalizationProvider.GetLocalizedString("NoSelection"), null, null, null, false, true, TimeSpan.FromSeconds(3));
                return;
            }

            ExaminationModal modal = new ExaminationModal(selectedExamination);
            modal.ShowDialog();

            if (modal.ExaminationUpdated)
            {
                try
                {
                    var updatedExamination = modal.GetExamination();

                    examinationDao.RemoveAllTreatmentsFromExamination(updatedExamination.ExaminationId);
                    examinationDao.RemoveAllMaterialsFromExamination(updatedExamination.ExaminationId);

                    foreach (var treatment in modal.SelectedTreatments)
                    {
                        examinationDao.AddTreatmentToExamination(updatedExamination.ExaminationId, treatment.TreatmentId, treatment.Price);
                    }

                    examinationDao.UpdateExamination(updatedExamination);

                    StringBuilder materialMessages = new StringBuilder();

                    foreach (var material in modal.SelectedMaterials)
                    {
                        string errorMessage;
                        bool success = examinationDao.AddMaterialToExamination(
                            updatedExamination.ExaminationId, material.idMaterijala, material.Kolicina, out errorMessage);

                        if (!success)
                        {
                            materialMessages.AppendLine($"{material.Naziv}: {errorMessage}");
                        }
                        else if (!string.IsNullOrEmpty(errorMessage))
                        {
                            materialMessages.AppendLine($"Upozorenje - {material.Naziv}: {errorMessage}");
                        }
                    }

                    if (materialMessages.Length > 0)
                    {
                        Snackbar.MessageQueue?.Enqueue(materialMessages.ToString(), null, null, null, false, true, TimeSpan.FromSeconds(10));
                    }
                    else
                    {
                        Snackbar.MessageQueue?.Enqueue(
                            LocalizationProvider.GetLocalizedString("ExaminationUpdated"),
                            null, null, null, false, true, TimeSpan.FromSeconds(3));
                    }

                    UpdateExaminationDisplayData();
                    DataGridExaminations.Items.Refresh();
                }
                catch (DataAccessException)
                {
                    UpdateExaminationDisplayData();
                    Snackbar.MessageQueue?.Enqueue(LocalizationProvider.GetLocalizedString("MaterialErrorUpdate"), null, null, null, false, true, TimeSpan.FromSeconds(5));
                }
            }
        }

        /*public void AddExamination_Click(object sender, RoutedEventArgs e)
        {
            ExaminationModal modal = new ExaminationModal();
            modal.ShowDialog();
            if (modal.ExaminationAdded)
            {
                var newExamination = modal.GetExamination();
                int newId = examinationDao.AddExamination(newExamination);
                newExamination.ExaminationId = newId;
                examinationViewModel.Items.Add(newExamination);
                UpdateExaminationDisplayData();
                Snackbar.MessageQueue?.Enqueue(LocalizationProvider.GetLocalizedString("ExaminationAdded"), null, null, null, false, true, TimeSpan.FromSeconds(3));
            }
        }

        public void UpdateExamination_Click(object sender, RoutedEventArgs e)
        {
            Examination selectedExamination = (Examination)DataGridExaminations.SelectedItem;
            if (selectedExamination == null)
            {
                Snackbar.MessageQueue?.Enqueue(LocalizationProvider.GetLocalizedString("NoSelection"), null, null, null, false, true, TimeSpan.FromSeconds(3));
                return;
            }

            ExaminationModal modal = new ExaminationModal(selectedExamination);
            modal.ShowDialog();
            if (modal.ExaminationUpdated)
            {
                var updatedExamination = modal.GetExamination();

                selectedExamination.Diagnosis = updatedExamination.Diagnosis;
                selectedExamination.AppointmentId = updatedExamination.AppointmentId;
                selectedExamination.MedicalTechnicianJMBG = updatedExamination.MedicalTechnicianJMBG;

                examinationDao.UpdateExamination(updatedExamination);
                UpdateExaminationDisplayData();

                DataGridExaminations.Items.Refresh();
                Snackbar.MessageQueue?.Enqueue(LocalizationProvider.GetLocalizedString("ExaminationUpdated"), null, null, null, false, true, TimeSpan.FromSeconds(3));
            }
        }*/

        public void DeleteExamination_Click(object sender, RoutedEventArgs e)
        {
            if (DataGridExaminations.SelectedItems.Count == 0)
            {
                Snackbar.MessageQueue?.Enqueue(LocalizationProvider.GetLocalizedString("NoSelection"), null, null, null, false, true, TimeSpan.FromSeconds(3));
            }
            else
            {
                bool? result = new MessageBoxCustom("ConfirmDelete", MessageType.Confirmation, MessageButtons.YesNo).ShowDialog();
                if (result == false)
                    return;

                int res = 0, toDelete = DataGridExaminations.SelectedItems.Count;
                List<Examination> selectedExaminations = new List<Examination>();
                foreach (Examination examination in DataGridExaminations.SelectedItems)
                    selectedExaminations.Add(examination);

                try
                {
                    foreach (Examination examination in selectedExaminations)
                    {
                        if (examinationDao.DeleteExamination(examination.ExaminationId))
                            res++;

                        examinationViewModel.Items.Remove(examination);
                    }

                    if (res == toDelete)
                    {
                        UpdateExaminationDisplayData();
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

        private void ViewDetails_Click(object sender, RoutedEventArgs e)
        {
            Examination selectedExamination = (Examination)DataGridExaminations.SelectedItem;
            if (selectedExamination == null)
            {
                Snackbar.MessageQueue?.Enqueue(LocalizationProvider.GetLocalizedString("NoSelection"),
                    null, null, null, false, true, TimeSpan.FromSeconds(3));
                return;
            }

            DetailedExaminationModal detailsModal = new DetailedExaminationModal(selectedExamination);
            detailsModal.Owner = Window.GetWindow(this);
            detailsModal.ShowDialog();
        }
    }
}