using DentalCareSystem.Data.DTO;
using DentalCareSystem.Data.DAO.MySQL;
using System.Windows;
using DentalCareSystem.Util;

namespace DentalCareSystem.Forms.Windows.Modals
{
    public partial class DetailedExaminationModal : Window
    {
        private readonly Examination _examination;
        private readonly ExaminationDAOImpl _examinationDao = new ExaminationDAOImpl();
        private readonly PatientDAOImpl _patientDao = new PatientDAOImpl();
        private readonly EmployeeDAOImpl _employeeDao = new EmployeeDAOImpl();

        public DetailedExaminationModal(Examination examination)
        {
            InitializeComponent();
            _examination = examination;
            LoadExaminationDetails();

            LocalizationProvider.UpdateAllObjects();
        }

        private void LoadExaminationDetails()
        {
            var patient = _patientDao.GetPatientByJMBG(_examination.PatientJMBG);
            if (patient != null)
            {
                PatientNameTextBlock.Text = $"{patient.FirstName} {patient.LastName}";
                PatientJMBGTextBlock.Text = patient.JMBG;
                PatientEmailTextBlock.Text = patient.Email;
                PatientPhoneTextBlock.Text = patient.PhoneNumber;
            }

            var dentist = _employeeDao.GetEmployeeByJMBG(_examination.DentistJMBG);
            if (dentist != null)
            {
                DentistNameTextBlock.Text = $"{dentist.FirstName} {dentist.LastName}";
            }

            if (!string.IsNullOrEmpty(_examination.MedicalTechnicianJMBG))
            {
                var medicalTechnician = _employeeDao.GetEmployeeByJMBG(_examination.MedicalTechnicianJMBG);
                if (medicalTechnician != null)
                {
                    MedicalTechnicianNameTextBlock.Text = $"{medicalTechnician.FirstName} {medicalTechnician.LastName}";
                }
            }
            else
            {
                MedicalTechnicianPanel.Visibility = Visibility.Collapsed;
            }

            ExaminationDateTextBlock.Text = _examination.ExaminationDate.ToString();
            DiagnosisTextBlock.Text = _examination.Diagnosis;

            var treatments = _examinationDao.GetTreatmentItemsForExamination(_examination.ExaminationId);
            TreatmentsDataGrid.ItemsSource = treatments;
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
            this.Close();
        }
    }
}