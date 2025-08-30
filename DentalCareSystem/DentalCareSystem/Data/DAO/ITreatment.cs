using DentalCareSystem.Data.DTO;

namespace DentalCareSystem.Data.DAO
{
    interface ITreatment
    {
        List<Treatment> GetTreatments();
        Treatment GetTreatmentById(int treatmentId);
        int AddTreatment(Treatment treatment);
        bool UpdateTreatment(Treatment treatment);
        bool DeleteTreatment(int treatmentId);
    }
}
