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
    public partial class ExaminationModal : Window, INotifyPropertyChanged
    {
        public bool ExaminationAdded { get; set; } = false;
        public bool ExaminationUpdated { get; set; } = false;

        private readonly bool isEditMode;
        private readonly Examination originalExamination;
        private readonly AppointmentDAOImpl appointmentDao = new AppointmentDAOImpl();
        private readonly EmployeeDAOImpl medicalTechnicianDao = new EmployeeDAOImpl();
        private readonly TreatmentDAOImpl treatmentDao = new TreatmentDAOImpl();
        private readonly MaterialDAOImpl materialDao = new MaterialDAOImpl();
        private readonly ExaminationDAOImpl examinationDao = new ExaminationDAOImpl();

        public ICommand SearchAppointmentCommand { get; }
        public ICommand BrowseAppointmentsCommand { get; }
        public ICommand SaveCommand { get; }
        public ICommand AddTreatmentCommand { get; }
        public ICommand RemoveTreatmentCommand { get; }
        public ICommand AddMaterialCommand { get; }
        public ICommand RemoveMaterialCommand { get; }

        private string diagnosis;
        public string Diagnosis
        {
            get => diagnosis;
            set { diagnosis = value; OnPropertyChanged(); }
        }

        private int appointmentId;
        public int AppointmentId
        {
            get => appointmentId;
            set { appointmentId = value; OnPropertyChanged(); }
        }

        private string appointmentInfo;
        public string AppointmentInfo
        {
            get => appointmentInfo;
            set { appointmentInfo = value; OnPropertyChanged(); }
        }

        private ObservableCollection<Employee> medicalTechnicians;
        public ObservableCollection<Employee> MedicalTechnicians
        {
            get => medicalTechnicians;
            set { medicalTechnicians = value; OnPropertyChanged(); }
        }

        private string selectedMedicalTechnicianJMBG;
        public string SelectedMedicalTechnicianJMBG
        {
            get => selectedMedicalTechnicianJMBG;
            set { selectedMedicalTechnicianJMBG = value; OnPropertyChanged(); }
        }

        private ObservableCollection<Treatment> availableTreatments;
        public ObservableCollection<Treatment> AvailableTreatments
        {
            get => availableTreatments;
            set { availableTreatments = value; OnPropertyChanged(); }
        }

        private ObservableCollection<Material> availableMaterials;
        public ObservableCollection<Material> AvailableMaterials
        {
            get => availableMaterials;
            set { availableMaterials = value; OnPropertyChanged(); }
        }

        public ObservableCollection<Treatment> selectedTreatments;
        public ObservableCollection<Treatment> SelectedTreatments
        {
            get => selectedTreatments;
            set { selectedTreatments = value; OnPropertyChanged(); }
        }

        public ObservableCollection<MaterialUsage> selectedMaterials;
        public ObservableCollection<MaterialUsage> SelectedMaterials
        {
            get => selectedMaterials;
            set { selectedMaterials = value; OnPropertyChanged(); }
        }

        private int selectedTreatmentId;
        public int SelectedTreatmentId
        {
            get => selectedTreatmentId;
            set { selectedTreatmentId = value; OnPropertyChanged(); }
        }

        private int selectedMaterialId;
        public int SelectedMaterialId
        {
            get => selectedMaterialId;
            set { selectedMaterialId = value; OnPropertyChanged(); }
        }

        private decimal materialQuantity;
        public decimal MaterialQuantity
        {
            get => materialQuantity;
            set { materialQuantity = value; OnPropertyChanged(); }
        }

        public Appointment SelectedAppointment { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public ExaminationModal()
        {
            SearchAppointmentCommand = new RelayCommand(SearchAppointmentById);
            BrowseAppointmentsCommand = new RelayCommand(BrowseAppointments);
            //SaveCommand = new RelayCommand(SaveExamination);

            AddTreatmentCommand = new RelayCommand(AddTreatment);
            RemoveTreatmentCommand = new RelayCommand(RemoveTreatment);
            AddMaterialCommand = new RelayCommand(AddMaterial);
            RemoveMaterialCommand = new RelayCommand(RemoveMaterial);

            InitializeComponent();
            isEditMode = false;
            originalExamination = new Examination();

            LoadData();
            this.DataContext = this;
            LocalizationProvider.UpdateAllObjects();

            ModalTitle.Content = LocalizationProvider.GetLocalizedString("AddExaminationTitle");
        }

        public ExaminationModal(Examination examination)
        {
            SearchAppointmentCommand = new RelayCommand(SearchAppointmentById);
            BrowseAppointmentsCommand = new RelayCommand(BrowseAppointments);
           // SaveCommand = new RelayCommand(SaveExamination);

            AddTreatmentCommand = new RelayCommand(AddTreatment);
            RemoveTreatmentCommand = new RelayCommand(RemoveTreatment);
            AddMaterialCommand = new RelayCommand(AddMaterial);
            RemoveMaterialCommand = new RelayCommand(RemoveMaterial);

            InitializeComponent();
            isEditMode = true;
            originalExamination = examination;

            LoadData();
            this.DataContext = this;

            Diagnosis = examination.Diagnosis;
            AppointmentId = examination.AppointmentId;

            SelectedAppointment = appointmentDao.GetAppointmentById(examination.AppointmentId);
            if (SelectedAppointment != null)
            {
                AppointmentInfo = $"{SelectedAppointment.AppointmentDateTime:dd.MM.yyyy HH:mm} - {SelectedAppointment.PatientName} ({SelectedAppointment.PatientJMBG})";
            }
            SelectedMedicalTechnicianJMBG = examination.MedicalTechnicianJMBG;

            LoadExistingExaminationData(examination.ExaminationId);

            LocalizationProvider.UpdateAllObjects();
            ModalTitle.Content = LocalizationProvider.GetLocalizedString("UpdateExaminationTitle");
        }


        private void LoadData()
        {
            var loadedTechnicians = medicalTechnicianDao.GetMedicalTechnicians();
            MedicalTechnicians = new ObservableCollection<Employee>(loadedTechnicians);

            var treatments = treatmentDao.GetTreatments();
            AvailableTreatments = new ObservableCollection<Treatment>(treatments);

            var materials = materialDao.GetMaterials();
            AvailableMaterials = new ObservableCollection<Material>(materials);

            SelectedTreatments = new ObservableCollection<Treatment>();
            SelectedMaterials = new ObservableCollection<MaterialUsage>();
        }

        private void LoadExistingExaminationData(int examinationId)
        {
            try
            {
                var treatments = examinationDao.GetTreatmentsForExamination(examinationId);
                foreach (var treatment in treatments)
                {
                    SelectedTreatments.Add(treatment);
                }

                var materialUsages = examinationDao.GetMaterialUsagesForExamination(examinationId);
                foreach (var material in materialUsages)
                {
                    SelectedMaterials.Add(material);
                }
            }
            catch (Exception)
            {
                ExaminationModalSnackbar.MessageQueue?.Enqueue(LocalizationProvider.GetLocalizedString("ErrorOccured"), null, null, null, false, true, TimeSpan.FromSeconds(5));
            }
        }

        private void AddTreatment()
        {
            if (SelectedTreatmentId > 0)
            {
                var treatment = AvailableTreatments.FirstOrDefault(t => t.TreatmentId == SelectedTreatmentId);
                if (treatment != null && !SelectedTreatments.Any(t => t.TreatmentId == treatment.TreatmentId))
                {
                    SelectedTreatments.Add(treatment);
                    SelectedTreatmentId = 0;
                }
                else if (treatment != null)
                {
                    ExaminationModalSnackbar.MessageQueue?.Enqueue(LocalizationProvider.GetLocalizedString("AlreadyAdded"), null, null, null, false, true, TimeSpan.FromSeconds(5));
                }
            }
        }

        private void RemoveTreatment(object parameter)
        {
            if (parameter is Treatment treatment)
            {
                SelectedTreatments.Remove(treatment);
            }
        }

        private void AddMaterial()
        {
            if (SelectedMaterialId > 0 && MaterialQuantity > 0)
            {
                var material = AvailableMaterials.FirstOrDefault(m => m.MaterialId == SelectedMaterialId);
                if (material != null)
                {
                    var existing = SelectedMaterials.FirstOrDefault(m => m.idMaterijala == SelectedMaterialId);
                    if (existing != null)
                    {
                        existing.Kolicina += MaterialQuantity;
                    }
                    else
                    {
                        SelectedMaterials.Add(new MaterialUsage
                        {
                            idMaterijala = SelectedMaterialId,
                            Naziv = material.Name,
                            Kolicina = MaterialQuantity
                        });
                    }
                    MaterialQuantity = 0;
                    SelectedMaterialId = 0;
                }
            }
            else
            {
                ExaminationModalSnackbar.MessageQueue?.Enqueue(LocalizationProvider.GetLocalizedString("ChooseMaterial"), null, null, null, false, true, TimeSpan.FromSeconds(5));
            }
        }

        private void RemoveMaterial(object parameter)
        {
            if (parameter is MaterialUsage materialUsage)
            {
                SelectedMaterials.Remove(materialUsage);
            }
        }

        public Examination GetExamination()
        {
            originalExamination.Diagnosis = Diagnosis;
            originalExamination.AppointmentId = AppointmentId;
            originalExamination.MedicalTechnicianJMBG = SelectedMedicalTechnicianJMBG;

            if (SelectedAppointment != null)
            {
                if (string.IsNullOrEmpty(originalExamination.PatientJMBG))
                {
                    originalExamination.PatientJMBG = SelectedAppointment.PatientJMBG;
                }

                if (string.IsNullOrEmpty(originalExamination.DentistJMBG))
                {
                    originalExamination.DentistJMBG = SelectedAppointment.DentistJMBG;
                }

                if (originalExamination.ExaminationDate == default)
                {
                    originalExamination.ExaminationDate = SelectedAppointment.AppointmentDateTime;
                }
            }

            return originalExamination;
        }

        private void SearchAppointment_Click(object sender, RoutedEventArgs e)
        {
            SearchAppointmentById();
        }

        private void AppointmentIdTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                SearchAppointmentById();
            }
        }

        private void SearchAppointmentById()
        {
            if (AppointmentId <= 0)
            {
                ExaminationModalSnackbar.MessageQueue?.Enqueue(LocalizationProvider.GetLocalizedString("EnterAppointmentId"), null, null, null, false, true, TimeSpan.FromSeconds(3));
                return;
            }

            SelectedAppointment = appointmentDao.GetAppointmentById(AppointmentId);
            if (SelectedAppointment != null)
            {
                AppointmentInfo = $"{SelectedAppointment.AppointmentDateTime:dd.MM.yyyy HH:mm} - {SelectedAppointment.PatientJMBG}";
                ExaminationModalSnackbar.MessageQueue?.Enqueue(LocalizationProvider.GetLocalizedString("AppointmentFound"), null, null, null, false, true, TimeSpan.FromSeconds(3));
            }
            else
            {
                AppointmentInfo = LocalizationProvider.GetLocalizedString("AppointmentNotFound");
                ExaminationModalSnackbar.MessageQueue?.Enqueue(LocalizationProvider.GetLocalizedString("AppointmentNotFound"), null, null, null, false, true, TimeSpan.FromSeconds(3));
            }
        }

        private void BrowseAppointments()
        {
            try
            {
                AppointmentSelectionModal appointmentSelectionModal = new AppointmentSelectionModal();
                appointmentSelectionModal.Owner = this;
                appointmentSelectionModal.ShowDialog();

                if (appointmentSelectionModal.AppointmentSelected)
                {
                    SelectedAppointment = appointmentSelectionModal.GetSelectedAppointment();
                    AppointmentId = SelectedAppointment.AppointmentId;
                    AppointmentInfo = $"{SelectedAppointment.AppointmentDateTime:dd.MM.yyyy HH:mm} - {SelectedAppointment.PatientJMBG}";

                    ExaminationModalSnackbar.MessageQueue?.Enqueue(
                        LocalizationProvider.GetLocalizedString("AppointmentSelected"),
                        null, null, null, false, true, TimeSpan.FromSeconds(3));
                }
            }
            catch (Exception)
            {
                ExaminationModalSnackbar.MessageQueue?.Enqueue(LocalizationProvider.GetLocalizedString("ErrorOccured"), null, null, null, false, true, TimeSpan.FromSeconds(5));
            }
        }

        private void BrowseAppointments_Click(object sender, RoutedEventArgs e)
        {
            BrowseAppointments();
        }
        

        private void AddOrUpdate_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(Diagnosis))
            {
                ExaminationModalSnackbar.MessageQueue?.Enqueue(LocalizationProvider.GetLocalizedString("EnterDiagnosis"), null, null, null, false, true, TimeSpan.FromSeconds(3));
                return;
            }

            if (SelectedAppointment == null || AppointmentId <= 0)
            {
                ExaminationModalSnackbar.MessageQueue?.Enqueue(LocalizationProvider.GetLocalizedString("SelectAppointment"), null, null, null, false, true, TimeSpan.FromSeconds(3));
                return;
            }

            if (string.IsNullOrEmpty(SelectedMedicalTechnicianJMBG))
            {
                ExaminationModalSnackbar.MessageQueue?.Enqueue(LocalizationProvider.GetLocalizedString("SelectMedicalTechnician"), null, null, null, false, true, TimeSpan.FromSeconds(3));
                return;
            }

            if (!isEditMode)
            {
                var existingExaminations = examinationDao.GetExaminations();
                if (existingExaminations.Any(ex => ex.AppointmentId == AppointmentId))
                {
                    ExaminationModalSnackbar.MessageQueue?.Enqueue(LocalizationProvider.GetLocalizedString("ExaminationAlreadyExists"), null, null, null, false, true, TimeSpan.FromSeconds(3));
                    return;
                }
            }

            if (isEditMode)
            {
                ExaminationUpdated = true;
            }
            else
            {
                ExaminationAdded = true;
            }

            this.Close();
            //SaveExamination();
        }

        /*private void SaveExamination()
        {
            try
            {
                int examinationId;

                if (isEditMode)
                {
                    // Ažuriraj postojeći pregled
                    var examination = GetExamination();
                    examinationDao.UpdateExamination(examination);
                    examinationId = examination.ExaminationId;

                    // Obriši postojeće tretmane i materijale
                    examinationDao.RemoveAllTreatmentsFromExamination(examinationId);
                    examinationDao.RemoveAllMaterialsFromExamination(examinationId);
                }
                else
                {
                    // Kreiraj novi pregled
                    var examination = GetExamination();
                    examinationId = examinationDao.AddExamination(examination);
                }

                // Dodaj tretmane
                foreach (var treatment in SelectedTreatments)
                {
                    examinationDao.AddTreatmentToExamination(examinationId, treatment.TreatmentId, treatment.Price);
                }

                // Dodaj materijale
                foreach (var material in SelectedMaterials)
                {
                    examinationDao.AddMaterialToExamination(examinationId, material.idMaterijala, material.Kolicina);
                }

                // Obavijesti korisnika o uspjehu
                ExaminationModalSnackbar.MessageQueue?.Enqueue(
                    isEditMode ? "Pregled je ažuriran!" : "Pregled je kreiran!",
                    null, null, null, false, true, TimeSpan.FromSeconds(3));

                if (isEditMode)
                {
                    ExaminationUpdated = true;
                }
                else
                {
                    ExaminationAdded = true;
                }

                this.Close();
            }
            catch (Exception ex)
            {
                ExaminationModalSnackbar.MessageQueue?.Enqueue(
                    $"Greška: {ex.Message}",
                    null, null, null, false, true, TimeSpan.FromSeconds(5));
            }
        }*/

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            ExaminationAdded = false;
            ExaminationUpdated = false;
            this.Close();
        }

        public class MaterialUsage : INotifyPropertyChanged
        {
            private int materialId;
            private string name;
            private decimal quantity;

            public int MaterialId
            {
                get => materialId;
                set { materialId = value; OnPropertyChanged(); }
            }

            public string Name
            {
                get => name;
                set { name = value; OnPropertyChanged(); }
            }
            public decimal Quantity
            {
                get => quantity;
                set { quantity = value; OnPropertyChanged(); }
            }

            public int idMaterijala
            {
                get => MaterialId;
                set => MaterialId = value;
            }

            public string Naziv
            {
                get => Name;
                set => Name = value;
            }

            public decimal Kolicina
            {
                get => Quantity;
                set => Quantity = value;
            }

            public event PropertyChangedEventHandler PropertyChanged;
            protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}