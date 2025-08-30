namespace DentalCareSystem.Data.DTO
{
    public class DetailedBill
    {
        public int BillId { get; set; }
        public double TotalPrice { get; set; }
        public DateTime DateTime { get; set; }
        public List<TreatmentItem> Items = new List<TreatmentItem>();
        public String MedichalTechnicianFirstName { get; set; }
        public String MedichalTechnicianLastName { get; set; }
        public String PatientFirstName { get; set; }
        public String PatientLastName { get; set; }

        public string FullNameM
        {
            get
            {
                return MedichalTechnicianFirstName + " " + MedichalTechnicianLastName;
            }
        }

        public string FullNameP
        {
            get
            {
                return PatientFirstName + " " +PatientLastName;
            }
        }

    }
}
