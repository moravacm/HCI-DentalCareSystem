using DentalCareSystem.Data.DTO;

namespace DentalCareSystem.Data.DAO
{
    interface IExamination
    {
        List<Examination> GetExaminations();
        List<Examination> GetExaminationsByPatient(string patientJMBG);
        List<Examination> GetExaminationsByDentist(string dentistJMBG);
        List<Examination> GetExaminationsByDate(DateTime date);
        List<MaterialItem> GetMaterialsForExamination(int examinationId);
        int AddExamination(Examination examination);
        void UpdateExamination(Examination examination);
        bool DeleteExamination(int examinationId);
    }
}
