namespace DentalCareSystem.Data.DTO
{
    public class Appointment
    {
        public int AppointmentId { get; set; }
        public DateTime AppointmentDateTime { get; set; }
        public string PatientJMBG { get; set; }
        public string DentistJMBG { get; set; }
        public string? MedicalTechnicianJMBG { get; set; }
        public string? PatientName { get; set; }
        public string? DentistName { get; set; }


        public string DisplayPatient => PatientName ?? PatientJMBG;

        public string DisplayDentist => DentistName ?? DentistJMBG;

        public override string ToString()
        {
            return $"{AppointmentId}, {AppointmentDateTime}, {PatientJMBG}, {DentistJMBG}, {MedicalTechnicianJMBG}";
        }

    }
}
