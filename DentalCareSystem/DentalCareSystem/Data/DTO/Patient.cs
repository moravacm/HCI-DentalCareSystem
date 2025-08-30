namespace DentalCareSystem.Data.DTO
{
    public class Patient 
    {
        public string JMBG { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public string MedicalHistory { get; set; }

        public override string ToString()
        {
            return $"{JMBG} {FirstName} {LastName} {PhoneNumber} {Email} {Address} {MedicalHistory}";
        }
    }
}
