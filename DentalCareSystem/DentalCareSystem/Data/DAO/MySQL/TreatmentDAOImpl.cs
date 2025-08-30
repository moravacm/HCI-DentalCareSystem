using DentalCareSystem.Data.DAO.Exceptions;
using DentalCareSystem.Data.DTO;
using MySql.Data.MySqlClient;

namespace DentalCareSystem.Data.DAO.MySQL
{
    public class TreatmentDAOImpl : ITreatment
    {
        private static readonly string SELECT_ALL = "SELECT * FROM TRETMAN";
        private static readonly string SELECT_BY_ID = "SELECT * FROM TRETMAN WHERE idTretmana = @idTretmana";
        private static readonly string INSERT = "INSERT INTO TRETMAN (Naziv, Cijena) VALUES (@Naziv, @Cijena); SELECT LAST_INSERT_ID();";
        private static readonly string UPDATE = "UPDATE TRETMAN SET Naziv = @Naziv, Cijena = @Cijena WHERE idTretmana = @idTretmana";
        private static readonly string DELETE = "DELETE FROM TRETMAN WHERE idTretmana = @idTretmana";

        public List<Treatment> GetTreatments()
        {
            List<Treatment> result = new List<Treatment>();
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
                    result.Add(new Treatment
                    {
                        TreatmentId = reader.GetInt32("idTretmana"),
                        Name = reader.GetString("Naziv"),
                        Price = reader.GetDecimal("Cijena")
                    });
                }
            }
            catch (Exception ex)
            {
                throw new DataAccessException("Exception in TreatmentDAOImpl", ex);
            }
            finally
            {
                MySQLUtil.CloseQuietly(reader, conn);
            }
            return result;
        }

        public Treatment GetTreatmentById(int treatmentId)
        {
            Treatment treatment = null;
            MySqlConnection conn = null;
            MySqlCommand cmd;
            MySqlDataReader reader = null;

            try
            {
                conn = MySQLUtil.GetMySQLConnection();
                cmd = conn.CreateCommand();
                cmd.CommandText = SELECT_BY_ID;
                cmd.Parameters.AddWithValue("@idTretmana", treatmentId);
                reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    treatment = new Treatment
                    {
                        TreatmentId = reader.GetInt32("idTretmana"),
                        Name = reader.GetString("Naziv"),
                        Price = reader.GetDecimal("Cijena")
                    };
                }
            }
            catch (Exception ex)
            {
                throw new DataAccessException("Exception in TreatmentDAOImpl", ex);
            }
            finally
            {
                MySQLUtil.CloseQuietly(reader, conn);
            }
            return treatment;
        }

        public int AddTreatment(Treatment treatment) 
        {
            int newId = 0; 
            MySqlConnection conn = null;
            MySqlCommand cmd;

            try
            {
                conn = MySQLUtil.GetMySQLConnection();
                cmd = conn.CreateCommand();
                cmd.CommandText = INSERT;

                cmd.Parameters.AddWithValue("@Naziv", treatment.Name);
                cmd.Parameters.AddWithValue("@Cijena", treatment.Price);

                newId = Convert.ToInt32(cmd.ExecuteScalar());
            }
            catch (Exception ex)
            {
                throw new DataAccessException("Exception in TreatmentDAOImpl", ex);
            }
            finally
            {
                MySQLUtil.CloseQuietly(conn);
            }
            return newId; 
        }

        public bool UpdateTreatment(Treatment treatment)
        {
            bool result = false;
            MySqlConnection conn = null;
            MySqlCommand cmd;

            try
            {
                conn = MySQLUtil.GetMySQLConnection();
                cmd = conn.CreateCommand();
                cmd.CommandText = UPDATE;

                cmd.Parameters.AddWithValue("@Naziv", treatment.Name);
                cmd.Parameters.AddWithValue("@Cijena", treatment.Price);
                cmd.Parameters.AddWithValue("@idTretmana", treatment.TreatmentId);

                result = cmd.ExecuteNonQuery() == 1;
            }
            catch (Exception ex)
            {
                throw new DataAccessException("Exception in TreatmentDAOImpl", ex);
            }
            finally
            {
                MySQLUtil.CloseQuietly(conn);
            }
            return result;
        }

        public bool DeleteTreatment(int treatmentId)
        {
            bool result = false;
            MySqlConnection conn = null;
            MySqlCommand cmd;

            try
            {
                conn = MySQLUtil.GetMySQLConnection();
                cmd = conn.CreateCommand();
                cmd.CommandText = DELETE;
                cmd.Parameters.AddWithValue("@idTretmana", treatmentId);

                result = cmd.ExecuteNonQuery() == 1;
            }
            catch (Exception ex)
            {
                throw new DataAccessException("Exception in TreatmentDAOImpl", ex);
            }
            finally
            {
                MySQLUtil.CloseQuietly(conn);
            }
            return result;
        }
    }
}