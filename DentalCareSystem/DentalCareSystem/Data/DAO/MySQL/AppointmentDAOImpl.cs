using DentalCareSystem.Data.DAO.Exceptions;
using DentalCareSystem.Data.DTO;
using MySql.Data.MySqlClient;

namespace DentalCareSystem.Data.DAO.MySQL
{
    public class AppointmentDAOImpl : IAppointment
    {
        private static readonly string SELECT_ALL = "SELECT * FROM `TERMIN`";
        private static readonly string SELECT_BY_ID = "SELECT * FROM `TERMIN` WHERE idTermina = @idTermina";
        private static readonly string SELECT_BY_DATE = "SELECT * FROM `TERMIN` WHERE DATE(DatumIVrijeme) = @DatumIVrijeme";
        private static readonly string SELECT_BY_PATIENT = "SELECT * FROM `TERMIN` WHERE JMBGPacijenta = @JMBGPacijenta";
        private static readonly string SELECT_BY_DENTIST = "SELECT * FROM `TERMIN` WHERE JMBGStomatologa = @JMBGStomatologa";
        private static readonly string INSERT = "INSERT INTO `TERMIN` (DatumIVrijeme, JMBGPacijenta, JMBGStomatologa, JMBGMedicinskogTehnicara) VALUES (@DatumIVrijeme, @JMBGPacijenta, @JMBGStomatologa, @JMBGMedicinskogTehnicara)";
        private static readonly string UPDATE = "UPDATE `TERMIN` SET DatumIVrijeme = @DatumIVrijeme, JMBGPacijenta = @JMBGPacijenta, JMBGStomatologa = @JMBGStomatologa, JMBGMedicinskogTehnicara = @JMBGMedicinskogTehnicara WHERE idTermina = @idTermina";
        private static readonly string DELETE = "DELETE FROM `TERMIN` WHERE idTermina = @idTermina";
        private static readonly string CHECK_TIME_SLOT = "SELECT COUNT(*) FROM `TERMIN` WHERE JMBGStomatologa = @JMBGStomatologa AND DatumIVrijeme = @DatumIVrijeme AND idTermina != @ExcludeAppointmentId";


        public List<Appointment> GetAppointments()
        {
            List<Appointment> result = new List<Appointment>();
            MySqlConnection conn = null;
            MySqlCommand cmd;
            MySqlDataReader reader = null;
            try
            {
                conn = MySQLUtil.GetMySQLConnection();
                cmd = conn.CreateCommand();
                cmd.CommandText = SELECT_ALL;
                reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    result.Add(new Appointment
                    {
                        AppointmentId = reader.GetInt32("idTermina"),
                        AppointmentDateTime = reader.GetDateTime("DatumIVrijeme"),
                        PatientJMBG = reader.GetString("JMBGPacijenta"),
                        DentistJMBG = reader.GetString("JMBGStomatologa"),
                        MedicalTechnicianJMBG = reader.IsDBNull(reader.GetOrdinal("JMBGMedicinskogTehnicara")) ? null : reader.GetString("JMBGMedicinskogTehnicara")
                    });
                }
            }
            catch (Exception ex)
            {
                throw new DataAccessException("Exception in AppointmentDAOImpl", ex);
            }
            finally
            {
                MySQLUtil.CloseQuietly(reader, conn);
            }
            return result;
        }

        public Appointment GetAppointmentById(int appointmentId)
        {
            Appointment result = null;
            MySqlConnection conn = null;
            MySqlCommand cmd;
            MySqlDataReader reader = null;
            try
            {
                conn = MySQLUtil.GetMySQLConnection();
                cmd = conn.CreateCommand();
                cmd.CommandText = SELECT_BY_ID;
                cmd.Parameters.AddWithValue("@idTermina", appointmentId);
                reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    result = new Appointment
                    {
                        AppointmentId = reader.GetInt32("idTermina"),
                        AppointmentDateTime = reader.GetDateTime("DatumIVrijeme"),
                        PatientJMBG = reader.GetString("JMBGPacijenta"),
                        DentistJMBG = reader.GetString("JMBGStomatologa"),
                        MedicalTechnicianJMBG = reader.IsDBNull(reader.GetOrdinal("JMBGMedicinskogTehnicara")) ? null : reader.GetString("JMBGMedicinskogTehnicara")
                    };
                }
            }
            catch (Exception ex)
            {
                throw new DataAccessException("Exception in AppointmentDAOImpl while getting appointment by ID", ex);
            }
            finally
            {
                MySQLUtil.CloseQuietly(reader, conn);
            }
            return result;
        }

        public List<Appointment> GetAppointmentsByDate(DateTime date)
        {
            List<Appointment> result = new List<Appointment>();
            MySqlConnection conn = null;
            MySqlCommand cmd;
            MySqlDataReader reader = null;
            try
            {
                conn = MySQLUtil.GetMySQLConnection();
                cmd = conn.CreateCommand();
                cmd.CommandText = SELECT_BY_DATE;
                cmd.Parameters.AddWithValue("@DatumIVrijeme", date.Date);
                reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    result.Add(new Appointment
                    {
                        AppointmentId = reader.GetInt32("idTermina"),
                        AppointmentDateTime = reader.GetDateTime("DatumIVrijeme"),
                        PatientJMBG = reader.GetString("JMBGPacijenta"),
                        DentistJMBG = reader.GetString("JMBGStomatologa"),
                        MedicalTechnicianJMBG = reader.IsDBNull(reader.GetOrdinal("JMBGMedicinskogTehnicara")) ? null : reader.GetString("JMBGMedicinskogTehnicara")
                    });
                }
            }
            catch (Exception ex)
            {
                throw new DataAccessException("Exception in AppointmentDAOImpl", ex);
            }
            finally
            {
                MySQLUtil.CloseQuietly(reader, conn);
            }
            return result;
        }

       
        public List<Appointment> GetAppointmentsByPatient(string jmbg)
        {
            List<Appointment> result = new List<Appointment>();
            MySqlConnection conn = null;
            MySqlCommand cmd;
            MySqlDataReader reader = null;
            try
            {
                conn = MySQLUtil.GetMySQLConnection();
                cmd = conn.CreateCommand();
                cmd.CommandText = SELECT_BY_PATIENT;
                cmd.Parameters.AddWithValue("@JMBGPacijenta", jmbg);
                reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    result.Add(new Appointment
                    {
                        AppointmentId = reader.GetInt32("idTermina"),
                        AppointmentDateTime = reader.GetDateTime("DatumIVrijeme"),
                        PatientJMBG = reader.GetString("JMBGPacijenta"),
                        DentistJMBG = reader.GetString("JMBGStomatologa"),
                        MedicalTechnicianJMBG = reader.IsDBNull(reader.GetOrdinal("JMBGMedicinskogTehnicara")) ? null : reader.GetString("JMBGMedicinskogTehnicara")
                    });
                }
            }
            catch (Exception ex)
            {
                throw new DataAccessException("Exception in AppointmentDAOImpl", ex);
            }
            finally
            {
                MySQLUtil.CloseQuietly(reader, conn);
            }
            return result;
        }

 
        public List<Appointment> GetAppointmentsByDentist(string jmbg)
        {
            List<Appointment> result = new List<Appointment>();
            MySqlConnection conn = null;
            MySqlCommand cmd;
            MySqlDataReader reader = null;
            try
            {
                conn = MySQLUtil.GetMySQLConnection();
                cmd = conn.CreateCommand();
                cmd.CommandText = SELECT_BY_DENTIST;
                cmd.Parameters.AddWithValue("@JMBGStomatologa", jmbg);
                reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    result.Add(new Appointment
                    {
                        AppointmentId = reader.GetInt32("idTermina"),
                        AppointmentDateTime = reader.GetDateTime("DatumIVrijeme"),
                        PatientJMBG = reader.GetString("JMBGPacijenta"),
                        DentistJMBG = reader.GetString("JMBGStomatologa"),
                        MedicalTechnicianJMBG = reader.IsDBNull(reader.GetOrdinal("JMBGMedicinskogTehnicara")) ? null : reader.GetString("JMBGMedicinskogTehnicara")
                    });
                }
            }
            catch (Exception ex)
            {
                throw new DataAccessException("Exception in AppointmentDAOImpl", ex);
            }
            finally
            {
                MySQLUtil.CloseQuietly(reader, conn);
            }
            return result;
        }

        public bool AddAppointment(Appointment appointment)
        {
            bool result = false;
            MySqlConnection conn = null;
            MySqlCommand cmd;
            try
            {
                conn = MySQLUtil.GetMySQLConnection();
                cmd = conn.CreateCommand();
                cmd.CommandText = INSERT;

                cmd.Parameters.AddWithValue("@DatumIVrijeme", appointment.AppointmentDateTime);
                cmd.Parameters.AddWithValue("@JMBGPacijenta", appointment.PatientJMBG);
                cmd.Parameters.AddWithValue("@JMBGStomatologa", appointment.DentistJMBG);
                cmd.Parameters.AddWithValue("@JMBGMedicinskogTehnicara",
                string.IsNullOrEmpty(appointment.MedicalTechnicianJMBG)
                    ? DBNull.Value
                    : appointment.MedicalTechnicianJMBG);

                result = cmd.ExecuteNonQuery() == 1;
            }
            catch (Exception ex)
            {
                throw new DataAccessException("Exception in AppointmentDAOImpl", ex);
            }
            finally
            {
                MySQLUtil.CloseQuietly(conn);
            }
            return result;
        }

        public void UpdateAppointment(Appointment appointment)
        {
            MySqlConnection conn = null;
            MySqlCommand cmd;
            try
            {
                conn = MySQLUtil.GetMySQLConnection();
                cmd = conn.CreateCommand();
                cmd.CommandText = UPDATE;

                cmd.Parameters.AddWithValue("@idTermina", appointment.AppointmentId);
                cmd.Parameters.AddWithValue("@DatumIVrijeme", appointment.AppointmentDateTime);
                cmd.Parameters.AddWithValue("@JMBGPacijenta", appointment.PatientJMBG);
                cmd.Parameters.AddWithValue("@JMBGStomatologa", appointment.DentistJMBG);
                cmd.Parameters.AddWithValue("@JMBGMedicinskogTehnicara",
                string.IsNullOrEmpty(appointment.MedicalTechnicianJMBG)
                    ? DBNull.Value
                    : appointment.MedicalTechnicianJMBG);

                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                throw new DataAccessException("Exception in AppointmentDAOImpl while updating appointment", ex);
            }
            finally
            {
                MySQLUtil.CloseQuietly(conn);
            }
        }


        public bool DeleteAppointment(int appointmentId)
        {
            bool result = false;
            MySqlConnection conn = null;
            MySqlCommand cmd;
            try
            {
                conn = MySQLUtil.GetMySQLConnection();
                cmd = conn.CreateCommand();
                cmd.CommandText = DELETE;
                cmd.Parameters.AddWithValue("@idTermina", appointmentId);

                result = cmd.ExecuteNonQuery() == 1;
            }
            catch (Exception ex)
            {
                throw new DataAccessException("Exception in AppointmentDAOImpl", ex);
            }
            finally
            {
                MySQLUtil.CloseQuietly(conn);
            }
            return result;
        }

        public bool IsAppointmentTimeSlotTaken(DateTime appointmentDateTime, string dentistJMBG, int excludeAppointmentId = 0)
        {
            MySqlConnection conn = null;
            MySqlCommand cmd;

            try
            {
                conn = MySQLUtil.GetMySQLConnection();
                cmd = conn.CreateCommand();
                cmd.CommandText = CHECK_TIME_SLOT;

                cmd.Parameters.AddWithValue("@JMBGStomatologa", dentistJMBG);
                cmd.Parameters.AddWithValue("@DatumIVrijeme", appointmentDateTime);
                cmd.Parameters.AddWithValue("@ExcludeAppointmentId", excludeAppointmentId);

                long count = (long)cmd.ExecuteScalar();
                return count > 0;
            }
            catch (Exception ex)
            {
                throw new DataAccessException("Exception in AppointmentDAOImpl while checking time slot availability", ex);
            }
            finally
            {
                MySQLUtil.CloseQuietly(conn);
            }
        }
    }
}
