using DentalCareSystem.Data.DAO.Exceptions;
using DentalCareSystem.Data.DTO;
using MySql.Data.MySqlClient;

namespace DentalCareSystem.Data.DAO.MySQL
{
    public class PatientDAOImpl : IPatient
    {
        private static readonly string SELECT = "SELECT * FROM `PACIJENT` ORDER BY Prezime";
        private static readonly string SELECT_BY_ID = "SELECT * FROM `PACIJENT` WHERE JMBG=@JMBG";
        private static readonly string DELETE = "DELETE FROM `PACIJENT` WHERE JMBG=@JMBG";
        private static readonly string UPDATE = "UPDATE `PACIJENT` SET Ime=@Ime, Prezime=@Prezime, BrojTelefona=@BrojTelefona, Adresa=@Adresa, Email=@Email, ZdravstvenaIstorija=@ZdravstvenaIstorija WHERE JMBG=@JMBG";
        private static readonly string INSERT = "INSERT INTO `PACIJENT`(JMBG, Ime, Prezime, BrojTelefona, Adresa, Email, ZdravstvenaIstorija) VALUES (@JMBG, @Ime, @Prezime, @BrojTelefona, @Adresa, @Email, @ZdravstvenaIstorija)";


        public List<Patient> GetPatients()
        {
            List<Patient> result = new List<Patient>();
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
                    Patient patient = new Patient()
                    {
                        JMBG = reader.GetString(0),
                        FirstName = reader.GetString(1),
                        LastName = reader.GetString(2),
                        PhoneNumber = reader.GetString(3),
                        Address = reader.GetString(4),
                        Email = reader.GetString(5),
                        MedicalHistory = reader.GetString(6)
                    };

                    result.Add(patient);
                }
            }
            catch (Exception ex)
            {
                throw new DataAccessException("Exception in PatientDAOImpl", ex);
            }
            finally
            {
                MySQLUtil.CloseQuietly(reader, conn);
            }
            return result;
        }

        public Patient GetPatientByJMBG(string patientId)
        {
            Patient result = null;
            MySqlConnection conn = null;
            MySqlCommand cmd;
            MySqlDataReader reader = null;
            try
            {
                conn = MySQLUtil.GetMySQLConnection();
                cmd = conn.CreateCommand();

                cmd.CommandText = SELECT_BY_ID;
                cmd.Parameters.AddWithValue("@JMBG", patientId);

                reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    result = new Patient()
                    {
                        JMBG = reader.GetString(0),
                        FirstName = reader.GetString(1),
                        LastName = reader.GetString(2),
                        PhoneNumber = reader.GetString(3),
                        Address = reader.GetString(4),
                        Email = reader.GetString(5),
                        MedicalHistory = reader.GetString(6)
                    };
                }
            }
            catch (Exception ex)
            {
                throw new DataAccessException("Exception in PatientDAOImpl", ex);
            }
            finally
            {
                MySQLUtil.CloseQuietly(reader, conn);
            }
            return result;
        }


        public bool AddPatient(Patient patient)
        {
            bool result = false;
            MySqlConnection conn = null;
            MySqlCommand cmd;
            try
            {
                conn = MySQLUtil.GetMySQLConnection();
                cmd = conn.CreateCommand();
                cmd.CommandText = INSERT;

                cmd.Parameters.AddWithValue("@JMBG", patient.JMBG);
                cmd.Parameters.AddWithValue("@Ime", patient.FirstName);
                cmd.Parameters.AddWithValue("@Prezime", patient.LastName);
                cmd.Parameters.AddWithValue("@BrojTelefona", patient.PhoneNumber);
                cmd.Parameters.AddWithValue("@Adresa", patient.Address);
                cmd.Parameters.AddWithValue("@Email", patient.Email);
                cmd.Parameters.AddWithValue("@ZdravstvenaIstorija", patient.MedicalHistory);

                result = cmd.ExecuteNonQuery() == 1;
            }
            catch (Exception ex)
            {
                throw new DataAccessException("Exception in PatientDAOImpl", ex);
            }
            finally
            {
                MySQLUtil.CloseQuietly(conn);
            }
            return result;
        }


        public bool DeletePatient(string patientId)
        {
            bool result = false;
            MySqlConnection conn = null;
            MySqlCommand cmd;
            try
            {
                conn = MySQLUtil.GetMySQLConnection();
                cmd = conn.CreateCommand();
                cmd.CommandText = DELETE;
                cmd.Parameters.AddWithValue("@JMBG", patientId);
                result = cmd.ExecuteNonQuery() == 1;
            }
            catch (Exception ex)
            {
                throw new DataAccessException("Exception in PatientDAOImpl", ex);
            }
            finally
            {
                MySQLUtil.CloseQuietly(conn);
            }
            return result;
        }


        public void UpdatePatient(Patient patient)
        {
            MySqlConnection conn = null;
            MySqlCommand cmd;
            try
            {
                conn = MySQLUtil.GetMySQLConnection();
                cmd = conn.CreateCommand();
                cmd.CommandText = UPDATE;

                cmd.Parameters.AddWithValue("@JMBG", patient.JMBG);
                cmd.Parameters.AddWithValue("@Ime", patient.FirstName);
                cmd.Parameters.AddWithValue("@Prezime", patient.LastName);
                cmd.Parameters.AddWithValue("@BrojTelefona", patient.PhoneNumber);
                cmd.Parameters.AddWithValue("@Adresa", patient.Address);
                cmd.Parameters.AddWithValue("@Email", patient.Email);
                cmd.Parameters.AddWithValue("@ZdravstvenaIstorija", patient.MedicalHistory);

                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                throw new DataAccessException("Exception in PatientDAOImpl", ex);
            }
            finally
            {
                MySQLUtil.CloseQuietly(conn);
            }
        }

    }
}
