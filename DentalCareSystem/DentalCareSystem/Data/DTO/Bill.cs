namespace DentalCareSystem.Data.DTO
{
    public class Bill
    {
        public int BillId { get; set; }
        public double TotalPrice { get; set; }
        public DateTime DateTime { get; set; }
        public string PatientJMBG { get; set; }
        public int ExaminationId { get; set; }
        public string MedicalTechnicianJMBG { get; set; }

        public override string ToString()
        {
            return $"{BillId}, {TotalPrice}, {DateTime}, {PatientJMBG}, {ExaminationId}, {MedicalTechnicianJMBG}";
        }

    }
}
