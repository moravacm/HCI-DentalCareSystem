using DentalCareSystem.Data.DAO.Exceptions;
using DentalCareSystem.Data.DTO;
using MySql.Data.MySqlClient;

namespace DentalCareSystem.Data.DAO.MySQL
{
    public class MaterialDAOImpl : IMaterial
    {
        private static readonly string SELECT = @"
            SELECT m.*, k.idKategorije, k.Naziv AS NazivKategorije
            FROM MATERIJAL m
            JOIN KATEGORIJA k ON m.idKategorije = k.idKategorije
            ORDER BY m.Naziv";

        private static readonly string INSERT = @"
            INSERT INTO MATERIJAL 
            (idKategorije, Naziv, JedinicaMjere, CijenaPoJedinici, MinimalnaZaliha, TrenutnaZaliha) 
            VALUES 
            (@idKategorije, @Naziv, @JedinicaMjere, @CijenaPoJedinici, @MinimalnaZaliha, @TrenutnaZaliha)";

        private static readonly string DELETE = "DELETE FROM MATERIJAL WHERE idMaterijala = @id";

        private static readonly string UPDATE = @"
            UPDATE MATERIJAL SET 
                idKategorije = @idKategorije,
                Naziv = @Naziv,
                JedinicaMjere = @JedinicaMjere,
                CijenaPoJedinici = @CijenaPoJedinici,
                MinimalnaZaliha = @MinimalnaZaliha,
                TrenutnaZaliha = @TrenutnaZaliha
            WHERE idMaterijala = @id";

        public List<Material> GetMaterials()
        {
            List<Material> result = new List<Material>();
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
                    Material material = new Material()
                    {
                        MaterialId = reader.GetInt32("idMaterijala"),
                        CategoryId = reader.GetInt32("idKategorije"),
                        Category = new Category
                        {
                            Id = reader.GetInt32("idKategorije"),
                            Name = reader.GetString("NazivKategorije")
                        },
                        Name = reader.GetString("Naziv"),
                        Unit = reader.GetString("JedinicaMjere"),
                        PricePerUnit = reader.GetDecimal("CijenaPoJedinici"),
                        MinimumStock = reader.GetDecimal("MinimalnaZaliha"),
                        CurrentStock = reader.GetDecimal("TrenutnaZaliha")
                    };
                    result.Add(material);
                }
            }
            catch (Exception ex)
            {
                throw new DataAccessException("Greška u MaterialDAOImpl", ex);
            }
            finally
            {
                MySQLUtil.CloseQuietly(reader, conn);
            }

            return result;
        }

        public bool AddMaterial(Material material)
        {
            bool result = false;
            MySqlConnection conn = null;
            MySqlCommand cmd;

            try
            {
                conn = MySQLUtil.GetMySQLConnection();
                cmd = conn.CreateCommand();
                cmd.CommandText = INSERT;

                cmd.Parameters.AddWithValue("@idKategorije", material.CategoryId);
                cmd.Parameters.AddWithValue("@Naziv", material.Name);
                cmd.Parameters.AddWithValue("@JedinicaMjere", material.Unit);
                cmd.Parameters.AddWithValue("@CijenaPoJedinici", material.PricePerUnit);
                cmd.Parameters.AddWithValue("@MinimalnaZaliha", material.MinimumStock);
                cmd.Parameters.AddWithValue("@TrenutnaZaliha", material.CurrentStock);

                result = cmd.ExecuteNonQuery() == 1;
            }
            catch (Exception ex)
            {
                throw new DataAccessException("Greška u MaterialDAOImpl", ex);
            }
            finally
            {
                MySQLUtil.CloseQuietly(conn);
            }

            return result;
        }

        public bool DeleteMaterial(int materialId)
        {
            bool result = false;
            MySqlConnection conn = null;
            MySqlCommand cmd;

            try
            {
                conn = MySQLUtil.GetMySQLConnection();
                cmd = conn.CreateCommand();
                cmd.CommandText = DELETE;
                cmd.Parameters.AddWithValue("@id", materialId);

                result = cmd.ExecuteNonQuery() == 1;
            }
            catch (Exception ex)
            {
                throw new DataAccessException("Greška u MaterialDAOImpl", ex);
            }
            finally
            {
                MySQLUtil.CloseQuietly(conn);
            }

            return result;
        }

        public void UpdateMaterial(Material material)
        {
            MySqlConnection conn = null;
            MySqlCommand cmd;

            try
            {
                conn = MySQLUtil.GetMySQLConnection();
                cmd = conn.CreateCommand();
                cmd.CommandText = UPDATE;

                cmd.Parameters.AddWithValue("@idKategorije", material.CategoryId);
                cmd.Parameters.AddWithValue("@Naziv", material.Name);
                cmd.Parameters.AddWithValue("@JedinicaMjere", material.Unit);
                cmd.Parameters.AddWithValue("@CijenaPoJedinici", material.PricePerUnit);
                cmd.Parameters.AddWithValue("@MinimalnaZaliha", material.MinimumStock);
                cmd.Parameters.AddWithValue("@TrenutnaZaliha", material.CurrentStock);
                cmd.Parameters.AddWithValue("@id", material.MaterialId);

                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                throw new DataAccessException("Greška u MaterialDAOImpl", ex);
            }
            finally
            {
                MySQLUtil.CloseQuietly(conn);
            }
        }
    }
}
