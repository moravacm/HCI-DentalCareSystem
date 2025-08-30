using DentalCareSystem.Data.DTO;

namespace DentalCareSystem.Data.DAO
{
    interface IPatient
    {
        List<Patient> GetPatients();
        Patient GetPatientByJMBG(string patientId);
        bool AddPatient(Patient patient);
        void UpdatePatient(Patient patient);
        bool DeletePatient(string patientId);
    }
}
