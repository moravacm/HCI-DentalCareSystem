using DentalCareSystem.Data.DTO;

namespace DentalCareSystem.Data.DAO
{
    interface IEmployee
    {
        List<Employee> GetEmployees();
        List<Employee> GetDentists();
        List<Employee> GetAdmins();
        List<Employee> GetMedicalTechnicians();
        List<Employee> GetEmployeesByRole(string query);
        Employee GetEmployeeByJMBG(string employeeId);

        bool AddEmployee(Employee employee);

        bool DeleteEmployee(string employeeId);

        void UpdateEmployee(Employee employee);
    }
}
