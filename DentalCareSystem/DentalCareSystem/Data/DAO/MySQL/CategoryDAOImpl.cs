using DentalCareSystem.Data.DAO.Exceptions;
using DentalCareSystem.Data.DTO;
using MySql.Data.MySqlClient;

namespace DentalCareSystem.Data.DAO.MySQL
{
    public class CategoryDAOImpl : ICategory
    {
        private static readonly string SELECT_ALL = "SELECT * FROM KATEGORIJA ORDER BY Naziv";
        private static readonly string INSERT = "INSERT INTO KATEGORIJA (Naziv) VALUES (@Naziv)";
        private static readonly string UPDATE = "UPDATE KATEGORIJA SET Naziv = @Naziv WHERE idKategorije = @Id";
        private static readonly string DELETE = "DELETE FROM KATEGORIJA WHERE idKategorije = @Id";

        public List<Category> GetCategories()
        {
            List<Category> result = new List<Category>();
            MySqlConnection conn = null;
            MySqlDataReader reader = null;

            try
            {
                conn = MySQLUtil.GetMySQLConnection();
                var cmd = conn.CreateCommand();
                cmd.CommandText = SELECT_ALL;
                reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    Category cat = new Category
                    {
                        Id = reader.GetInt32("idKategorije"),
                        Name = reader.GetString("Naziv")
                    };
                    result.Add(cat);
                }
            }
            catch (Exception ex)
            {
                throw new DataAccessException("Greška u CategorylDAOImpl", ex);
            }
            finally
            {
                MySQLUtil.CloseQuietly(reader, conn);
            }

            return result;
        }

        public bool AddCategory(Category category)
        {
            MySqlConnection conn = null;
            MySqlCommand cmd;
            bool result = false;

            try
            {
                conn = MySQLUtil.GetMySQLConnection();
                cmd = conn.CreateCommand();
                cmd.CommandText = INSERT;
                cmd.Parameters.AddWithValue("@Naziv", category.Name);
                result = cmd.ExecuteNonQuery() == 1;
            }
            catch (Exception ex)
            {
                throw new DataAccessException("Greška u CategorylDAOImpl", ex);
            }
            finally
            {
                MySQLUtil.CloseQuietly(conn);
            }

            return result;
        }

        public void UpdateCategory(Category category)
        {
            MySqlConnection conn = null;
            MySqlCommand cmd;

            try
            {
                conn = MySQLUtil.GetMySQLConnection();
                cmd = conn.CreateCommand();
                cmd.CommandText = UPDATE;
                cmd.Parameters.AddWithValue("@Naziv", category.Name);
                cmd.Parameters.AddWithValue("@Id", category.Id);
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                throw new DataAccessException("Greška u CategorylDAOImpl", ex);
            }
            finally
            {
                MySQLUtil.CloseQuietly(conn);
            }
        }

        public bool DeleteCategory(int categoryId)
        {
            MySqlConnection conn = null;
            MySqlCommand cmd;
            bool result = false;

            try
            {
                conn = MySQLUtil.GetMySQLConnection();
                cmd = conn.CreateCommand();
                cmd.CommandText = DELETE;
                cmd.Parameters.AddWithValue("@Id", categoryId);
                result = cmd.ExecuteNonQuery() == 1;
            }
            catch (Exception ex)
            {
                throw new DataAccessException("Greška u CategorylDAOImpl", ex);
            }
            finally
            {
                MySQLUtil.CloseQuietly(conn);
            }

            return result;
        }
    }
}
