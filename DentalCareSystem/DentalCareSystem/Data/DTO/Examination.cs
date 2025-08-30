using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace DentalCareSystem.Data.DTO
{
    public class Examination : INotifyPropertyChanged
    {
        private int examinationId;
        private string diagnosis;
        private int appointmentId;
        private string medicalTechnicianJMBG;
        private DateTime? examinationDate;
        private string patientJMBG;
        private string dentistJMBG;

        private string patientName;
        private string dentistName;
        private string medicalTechnicianName;

        public int ExaminationId
        {
            get => examinationId;
            set { examinationId = value; OnPropertyChanged(); }
        }

        public string Diagnosis
        {
            get => diagnosis;
            set { diagnosis = value; OnPropertyChanged(); }
        }

        public int AppointmentId
        {
            get => appointmentId;
            set { appointmentId = value; OnPropertyChanged(); }
        }

        public string MedicalTechnicianJMBG
        {
            get => medicalTechnicianJMBG;
            set { medicalTechnicianJMBG = value; OnPropertyChanged(); }
        }

        public DateTime? ExaminationDate
        {
            get => examinationDate;
            set { examinationDate = value; OnPropertyChanged(); }
        }

        public string PatientJMBG
        {
            get => patientJMBG;
            set { patientJMBG = value; OnPropertyChanged(); }
        }

        public string DentistJMBG
        {
            get => dentistJMBG;
            set { dentistJMBG = value; OnPropertyChanged(); }
        }

        public string PatientName
        {
            get => patientName;
            set { patientName = value; OnPropertyChanged(); }
        }

        public string DentistName
        {
            get => dentistName;
            set { dentistName = value; OnPropertyChanged(); }
        }

        public string MedicalTechnicianName
        {
            get => medicalTechnicianName;
            set { medicalTechnicianName = value; OnPropertyChanged(); }
        }

        public List<TreatmentItem> TreatmentItems { get; set; } = new List<TreatmentItem>();
        public List<MaterialItem> MaterialItems { get; set; } = new List<MaterialItem>();

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class TreatmentItem
    {
        public int TreatmentId { get; set; }
        public string TreatmentName { get; set; }
        public decimal Price { get; set; }
        public int ExaminationId { get; set; }
    }

    public class MaterialItem
    {
        public int MaterialId { get; set; }
        public string MaterialName { get; set; }
        public decimal Quantity { get; set; }
        public string UnitOfMeasure { get; set; }
        public decimal PricePerUnit { get; set; }
        public int ExaminationId { get; set; }
    }

    public class Treatment
    {
        public int TreatmentId { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
    }
}