using DentalCareSystem.Data.DTO;

namespace DentalCareSystem.Data.DAO
{
    interface IAppointment
    {
        List<Appointment> GetAppointments();
        Appointment GetAppointmentById(int appointmentId);
        List<Appointment> GetAppointmentsByDate(DateTime date);
        List<Appointment> GetAppointmentsByPatient(string jmbg);
        List<Appointment> GetAppointmentsByDentist(string jmbg);
        bool AddAppointment(Appointment appointment);
        void UpdateAppointment(Appointment appointment);
        bool DeleteAppointment(int appointmentId);
    }
}
