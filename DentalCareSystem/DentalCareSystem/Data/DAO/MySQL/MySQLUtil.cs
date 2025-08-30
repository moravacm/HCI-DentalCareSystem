using DentalCareSystem.Data.DAO.Exceptions;
using DentalCareSystem.Data.DTO;
using MySql.Data.MySqlClient;
using System.Data;
using System.Windows;

namespace DentalCareSystem.Data.DAO.MySQL
{
    public class MySQLUtil
    {
        private static readonly string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["connectionString"].ConnectionString;

        public static MySqlConnection GetMySQLConnection()
        {
            MySqlConnection conn = null;
            try
            {
                conn = new MySqlConnection(connectionString);
                conn.Open();
            }
            catch (MySqlException ex)
            {
                MessageBox.Show(ex.Message);
            }
            return conn;
        }

        public static void CloseQuietly(MySqlConnection conn)
        {
            if (conn != null)
            {
                conn.Close();
            }
        }

        public static void CloseQuietly(MySqlDataReader reader)
        {
            if (reader != null)
            {
                reader.Close();
            }
        }

        public static void CloseQuietly(MySqlDataReader reader, MySqlConnection conn)
        {
            CloseQuietly(reader);
            CloseQuietly(conn);
        }

        public static Employee SignIn(String username, String password)
        {
            bool signIn = false;
            MySqlConnection conn = null;
            MySqlCommand cmd;
            try
            {
                conn = MySQLUtil.GetMySQLConnection();
                cmd = new MySqlCommand("prijava", conn);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;

                cmd.Parameters.Add("@korisnickoIme", MySqlDbType.String).Value = username;
                cmd.Parameters.Add("@lozinka", MySqlDbType.String).Value = password;

                cmd.Parameters.Add("@prijava", MySqlDbType.Bit).Direction = ParameterDirection.Output;
                cmd.Parameters.Add("@idZaposleni", MySqlDbType.String).Direction = ParameterDirection.Output; 
                cmd.Parameters.Add("@uloga", MySqlDbType.String).Direction = ParameterDirection.Output;
                cmd.Parameters.Add("@ime", MySqlDbType.String).Direction = ParameterDirection.Output;
                cmd.Parameters.Add("@prezime", MySqlDbType.String).Direction = ParameterDirection.Output;
                cmd.Parameters.Add("@brojTelefona", MySqlDbType.String).Direction = ParameterDirection.Output;
                cmd.Parameters.Add("@email", MySqlDbType.String).Direction = ParameterDirection.Output;
                cmd.Parameters.Add("@specijalizacija", MySqlDbType.String).Direction = ParameterDirection.Output;

                cmd.ExecuteNonQuery();

                if (cmd.Parameters["@prijava"].Value != DBNull.Value)
                    signIn = Convert.ToBoolean(cmd.Parameters["@prijava"].Value);

                if (signIn)
                {
                    Employee e = new Employee()
                    {
                        JMBG = Convert.ToString(cmd.Parameters["@idZaposleni"].Value),
                        Role = Convert.ToString(cmd.Parameters["@uloga"].Value),
                        FirstName = Convert.ToString(cmd.Parameters["@ime"].Value),
                        LastName = Convert.ToString(cmd.Parameters["@prezime"].Value),
                        Username = username,
                        Password = password,
                        PhoneNumber = Convert.ToString(cmd.Parameters["@brojTelefona"].Value),
                        Email = Convert.ToString(cmd.Parameters["@email"].Value),
                        Specialization = cmd.Parameters["@specijalizacija"].Value == DBNull.Value ? null : Convert.ToString(cmd.Parameters["@specijalizacija"].Value)
                    };
                    return e;
                }
                else
                {
                    return new Employee();
                }
            }
            catch (Exception ex)
            {
                throw new DataAccessException("Exception in MySQLUtil", ex);
            }
            finally
            {
                MySQLUtil.CloseQuietly(conn);
            }
        }
 
    }
}