using DentalCareSystem.Data.DAO.Exceptions;
using DentalCareSystem.Data.DTO;
using MySql.Data.MySqlClient;

namespace DentalCareSystem.Data.DAO.MySQL
{
    public class EmployeeDAOImpl : IEmployee
    {
        private static readonly string SELECT = "SELECT * FROM `ZAPOSLENI` ORDER BY Uloga";
        private static readonly string DELETE = "DELETE FROM `ZAPOSLENI` WHERE JMBG=@JMBG";
        private static readonly string UPDATE = "UPDATE `ZAPOSLENI` SET Uloga=@Uloga, Ime=@Ime, Prezime=@Prezime, KorisnickoIme=@KorisnickoIme, Lozinka=@Lozinka, BrojTelefona=@BrojTelefona, Email=@Email, Specijalizacija=@Specijalizacija WHERE JMBG=@JMBG";
        private static readonly string INSERT = "INSERT INTO `ZAPOSLENI`(JMBG, Uloga, Ime, Prezime, KorisnickoIme, Lozinka, BrojTelefona, Email, Specijalizacija) VALUES (@JMBG, @Uloga, @Ime, @Prezime, @KorisnickoIme, @Lozinka, @BrojTelefona, @Email, @Specijalizacija)";
        private static readonly string SELECT_DENTISTS = "SELECT * FROM `ZAPOSLENI` WHERE LOWER(Uloga) IN ('stomatolog', 'dentist') ORDER BY Prezime";
        private static readonly string SELECT_TECHNICIANS = "SELECT * FROM `ZAPOSLENI` WHERE LOWER(Uloga) IN ('medicinski tehničar', 'medicinski tehnicar', 'medical technician') ORDER BY Prezime";
        private static readonly string SELECT_ADMIN = "SELECT * FROM `ZAPOSLENI` WHERE LOWER(Uloga) IN ('admin', 'administrator') ORDER BY Prezime";
        private static readonly string SELECT_BY_ID = "SELECT * FROM `ZAPOSLENI` WHERE JMBG=@JMBG";


        public List<Employee> GetDentists()
        {
            return GetEmployeesByRole(SELECT_DENTISTS);
        }

        public List<Employee> GetMedicalTechnicians()
        {
            return GetEmployeesByRole(SELECT_TECHNICIANS);
        }
        public List<Employee> GetAdmins()
        {
            return GetEmployeesByRole(SELECT_ADMIN);
        }

        public List<Employee> GetEmployeesByRole(string query)
        {
            List<Employee> result = new List<Employee>();
            MySqlConnection conn = null;
            MySqlCommand cmd;
            MySqlDataReader reader = null;

            try
            {
                conn = MySQLUtil.GetMySQLConnection();
                cmd = conn.CreateCommand();
                cmd.CommandText = query;
                reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    Employee employee = new Employee()
                    {
                        JMBG = reader.GetString(0),
                        Role = reader.GetString(1),
                        FirstName = reader.GetString(2),
                        LastName = reader.GetString(3),
                        Username = reader.GetString(4),
                        Password = reader.GetString(5),
                        PhoneNumber = reader.GetString(6),
                        Email = reader.GetString(7),
                        Specialization = reader.IsDBNull(8) ? null : reader.GetString(8)
                    };
                    result.Add(employee);
                }
            }
            catch (Exception ex)
            {
                throw new DataAccessException("Exception in GetEmployeesByRole", ex);
            }
            finally
            {
                MySQLUtil.CloseQuietly(reader, conn);
            }

            return result;
        }


        public List<Employee> GetEmployees()
        {
            List<Employee> result = new List<Employee>();
            MySqlConnection conn = null;
            MySqlCommand cmd;
            MySqlDataReader reader = null;
            try
            {
                conn = MySQLUtil.GetMySQLConnection();
                cmd = conn.CreateCommand();
                cmd.CommandText = SELECT;
                reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    Employee employee = new Employee()
                    {
                        JMBG = reader.GetString(0),
                        Role = reader.GetString(1),
                        FirstName = reader.GetString(2),
                        LastName = reader.GetString(3),
                        Username = reader.GetString(4),
                        Password = reader.GetString(5),
                        PhoneNumber = reader.GetString(6),
                        Email = reader.GetString(7)
                    };

                    if (employee.Role.ToLower() == "stomatolog" || employee.Role.ToLower() == "dentist")
                    {
                        employee.Specialization = reader.IsDBNull(8) ? null : reader.GetString(8);
                    }
                    else
                    {
                        employee.Specialization = null;
                    }

                    result.Add(employee);
                }
            }
            catch (Exception ex)
            {
                throw new DataAccessException("Exception in EmployeeDAOImpl", ex);
            }
            finally
            {
                MySQLUtil.CloseQuietly(reader, conn);
            }
            return result;
        }

        public Employee GetEmployeeByJMBG(string employeeId)
        {
            Employee result = null;
            MySqlConnection conn = null;
            MySqlCommand cmd;
            MySqlDataReader reader = null;
            try
            {
                conn = MySQLUtil.GetMySQLConnection();
                cmd = conn.CreateCommand();

                cmd.CommandText = SELECT_BY_ID;
                cmd.Parameters.AddWithValue("@JMBG", employeeId);

                reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    Employee employee = new Employee()
                    {
                        JMBG = reader.GetString(0),
                        Role = reader.GetString(1),
                        FirstName = reader.GetString(2),
                        LastName = reader.GetString(3),
                        Username = reader.GetString(4),
                        Password = reader.GetString(5),
                        PhoneNumber = reader.GetString(6),
                        Email = reader.GetString(7)
                    };

                    if (employee.Role.ToLower() == "stomatolog" || employee.Role.ToLower() == "dentist")
                    {
                        employee.Specialization = reader.IsDBNull(8) ? null : reader.GetString(8);
                    }
                    else
                    {
                        employee.Specialization = null;
                    }
                    result = employee;
                }
            }
            catch (Exception ex)
            {
                throw new DataAccessException("Exception in EmployeeDAOImpl", ex);
            }
            finally
            {
                MySQLUtil.CloseQuietly(reader, conn);
            }
            return result;
        }

        public bool AddEmployee(Employee employee)
        {
            bool result = false;
            MySqlConnection conn = null;
            MySqlCommand cmd;
            try
            {
                conn = MySQLUtil.GetMySQLConnection();
                cmd = conn.CreateCommand();
                cmd.CommandText = INSERT;

                if (employee.Role.ToLower() == "stomatolog")
                {
                    cmd.Parameters.AddWithValue("@Specijalizacija", string.IsNullOrEmpty(employee.Specialization) ? DBNull.Value : (object)employee.Specialization);
                }
                else
                {
                    cmd.Parameters.AddWithValue("@Specijalizacija", DBNull.Value);
                }

                cmd.Parameters.AddWithValue("@JMBG", employee.JMBG);
                cmd.Parameters.AddWithValue("@Uloga", employee.Role);
                cmd.Parameters.AddWithValue("@Ime", employee.FirstName);
                cmd.Parameters.AddWithValue("@Prezime", employee.LastName);
                cmd.Parameters.AddWithValue("@KorisnickoIme", employee.Username);
                cmd.Parameters.AddWithValue("@Lozinka", employee.Password);
                cmd.Parameters.AddWithValue("@BrojTelefona", employee.PhoneNumber);
                cmd.Parameters.AddWithValue("@Email", employee.Email);

                result = cmd.ExecuteNonQuery() == 1;
            }
            catch (Exception ex)
            {
                throw new DataAccessException("Exception in EmployeeDAOImpl", ex);
            }
            finally
            {
                MySQLUtil.CloseQuietly(conn);
            }
            return result;
        }


        public bool DeleteEmployee(string employeeId)
        {
            bool result = false;
            MySqlConnection conn = null;
            MySqlCommand cmd;
            try
            {
                conn = MySQLUtil.GetMySQLConnection();
                cmd = conn.CreateCommand();
                cmd.CommandText = DELETE;
                cmd.Parameters.AddWithValue("@JMBG", employeeId);
                result = cmd.ExecuteNonQuery() == 1;
            }
            catch (Exception ex)
            {
                throw new DataAccessException("Exception in EmployeeDAOImpl", ex);
            }
            finally
            {
                MySQLUtil.CloseQuietly(conn);
            }
            return result;
        }


        public void UpdateEmployee(Employee employee)
        {
            MySqlConnection conn = null;
            MySqlCommand cmd;
            try
            {
                conn = MySQLUtil.GetMySQLConnection();
                cmd = conn.CreateCommand();
                cmd.CommandText = UPDATE;

                if (employee.Role.ToLower() == "stomatolog")
                {
                    cmd.Parameters.AddWithValue("@Specijalizacija", string.IsNullOrEmpty(employee.Specialization) ? DBNull.Value : (object)employee.Specialization);
                }
                else
                {
                    cmd.Parameters.AddWithValue("@Specijalizacija", DBNull.Value);
                }

                cmd.Parameters.AddWithValue("@JMBG", employee.JMBG);
                cmd.Parameters.AddWithValue("@Uloga", employee.Role);
                cmd.Parameters.AddWithValue("@Ime", employee.FirstName);
                cmd.Parameters.AddWithValue("@Prezime", employee.LastName);
                cmd.Parameters.AddWithValue("@KorisnickoIme", employee.Username);
                cmd.Parameters.AddWithValue("@Lozinka", employee.Password);
                cmd.Parameters.AddWithValue("@BrojTelefona", employee.PhoneNumber);
                cmd.Parameters.AddWithValue("@Email", employee.Email);

                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                throw new DataAccessException("Exception in EmployeeDAOImpl", ex);
            }
            finally
            {
                MySQLUtil.CloseQuietly(conn);
            }
        }

    }
}
