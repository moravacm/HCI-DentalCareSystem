using DentalCareSystem.Data.DAO.Exceptions;
using DentalCareSystem.Data.DTO;
using MySql.Data.MySqlClient;
using static DentalCareSystem.Forms.Windows.Modals.ExaminationModal;

namespace DentalCareSystem.Data.DAO.MySQL
{
    public class ExaminationDAOImpl : IExamination
    {
        private static readonly string SELECT_ALL = @"
            SELECT p.*, t.DatumIVrijeme, t.JMBGPacijenta, t.JMBGStomatologa 
            FROM PREGLED p 
            INNER JOIN TERMIN t ON p.idTermina = t.idTermina";

        private static readonly string SELECT_BY_ID = @"
            SELECT p.*, t.DatumIVrijeme, t.JMBGPacijenta, t.JMBGStomatologa 
            FROM PREGLED p 
            INNER JOIN TERMIN t ON p.idTermina = t.idTermina 
            WHERE p.idPregleda = @idPregleda";

        private static readonly string SELECT_BY_PATIENT = @"
            SELECT p.*, t.DatumIVrijeme, t.JMBGPacijenta, t.JMBGStomatologa 
            FROM PREGLED p 
            INNER JOIN TERMIN t ON p.idTermina = t.idTermina 
            WHERE t.JMBGPacijenta = @JMBGPacijenta";

        private static readonly string SELECT_BY_DENTIST = @"
            SELECT p.*, t.DatumIVrijeme, t.JMBGPacijenta, t.JMBGStomatologa 
            FROM PREGLED p 
            INNER JOIN TERMIN t ON p.idTermina = t.idTermina 
            WHERE t.JMBGStomatologa = @JMBGStomatologa";

        private static readonly string SELECT_BY_DATE = @"
            SELECT p.*, t.DatumIVrijeme, t.JMBGPacijenta, t.JMBGStomatologa 
            FROM PREGLED p 
            INNER JOIN TERMIN t ON p.idTermina = t.idTermina 
            WHERE DATE(t.DatumIVrijeme) = @Datum";

        private static readonly string INSERT = @"
            INSERT INTO PREGLED (Dijagnoza, idTermina, JMBGMedicinskogTehnicara) 
            VALUES (@Dijagnoza, @idTermina, @JMBGMedicinskogTehnicara);
            SELECT LAST_INSERT_ID();";

        private static readonly string UPDATE = @"
            UPDATE PREGLED 
            SET Dijagnoza = @Dijagnoza, idTermina = @idTermina, 
                JMBGMedicinskogTehnicara = @JMBGMedicinskogTehnicara 
            WHERE idPregleda = @idPregleda";

        private static readonly string DELETE = "DELETE FROM PREGLED WHERE idPregleda = @idPregleda";

        private static readonly string SELECT_TREATMENTS_FOR_EXAMINATION = @"
            SELECT ts.idTretmana, t.Naziv, ts.Cijena 
            FROM TRETMAN_STAVKA ts 
            INNER JOIN TRETMAN t ON ts.idTretmana = t.idTretmana 
            WHERE ts.idPregleda = @idPregleda";

        private static readonly string SELECT_MATERIALS_FOR_EXAMINATION = @"
            SELECT ms.idMaterijala, m.Naziv, ms.Kolicina, m.JedinicaMjere, m.CijenaPoJedinici 
            FROM MATERIJAL_STAVKA ms 
            INNER JOIN MATERIJAL m ON ms.idMaterijala = m.idMaterijala 
            WHERE ms.idPregleda = @idPregleda";

        public List<Examination> GetExaminations()
        {
            List<Examination> result = new List<Examination>();
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
                    result.Add(MapReaderToExamination(reader));
                }
            }
            catch (Exception ex)
            {
                throw new DataAccessException("Exception in ExaminationDAOImpl", ex);
            }
            finally
            {
                MySQLUtil.CloseQuietly(reader, conn);
            }
            return result;
        }

        public Examination GetExaminationById(int examinationId)
        {
            Examination examination = null;
            MySqlConnection conn = null;
            MySqlCommand cmd;
            MySqlDataReader reader = null;

            try
            {
                conn = MySQLUtil.GetMySQLConnection();
                cmd = conn.CreateCommand();
                cmd.CommandText = SELECT_BY_ID;
                cmd.Parameters.AddWithValue("@idPregleda", examinationId);
                reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    examination = MapReaderToExamination(reader);
                }
            }
            catch (Exception ex)
            {
                throw new DataAccessException("Exception in ExaminationDAOImpl", ex);
            }
            finally
            {
                MySQLUtil.CloseQuietly(reader, conn);
            }
            return examination;
        }

        public List<Examination> GetExaminationsByPatient(string patientJMBG)
        {
            List<Examination> result = new List<Examination>();
            MySqlConnection conn = null;
            MySqlCommand cmd;
            MySqlDataReader reader = null;

            try
            {
                conn = MySQLUtil.GetMySQLConnection();
                cmd = conn.CreateCommand();
                cmd.CommandText = SELECT_BY_PATIENT;
                cmd.Parameters.AddWithValue("@JMBGPacijenta", patientJMBG);
                reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    result.Add(MapReaderToExamination(reader));
                }
            }
            catch (Exception ex)
            {
                throw new DataAccessException("Exception in ExaminationDAOImpl", ex);
            }
            finally
            {
                MySQLUtil.CloseQuietly(reader, conn);
            }
            return result;
        }

        public List<Examination> GetExaminationsByDentist(string dentistJMBG)
        {
            List<Examination> result = new List<Examination>();
            MySqlConnection conn = null;
            MySqlCommand cmd;
            MySqlDataReader reader = null;

            try
            {
                conn = MySQLUtil.GetMySQLConnection();
                cmd = conn.CreateCommand();
                cmd.CommandText = SELECT_BY_DENTIST;
                cmd.Parameters.AddWithValue("@JMBGStomatologa", dentistJMBG);
                reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    result.Add(MapReaderToExamination(reader));
                }
            }
            catch (Exception ex)
            {
                throw new DataAccessException("Exception in ExaminationDAOImpl", ex);
            }
            finally
            {
                MySQLUtil.CloseQuietly(reader, conn);
            }
            return result;
        }

        public List<Examination> GetExaminationsByDate(DateTime date)
        {
            List<Examination> result = new List<Examination>();
            MySqlConnection conn = null;
            MySqlCommand cmd;
            MySqlDataReader reader = null;

            try
            {
                conn = MySQLUtil.GetMySQLConnection();
                cmd = conn.CreateCommand();
                cmd.CommandText = SELECT_BY_DATE;
                cmd.Parameters.AddWithValue("@Datum", date.Date);
                reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    result.Add(MapReaderToExamination(reader));
                }
            }
            catch (Exception ex)
            {
                throw new DataAccessException("Exception in ExaminationDAOImpl", ex);
            }
            finally
            {
                MySQLUtil.CloseQuietly(reader, conn);
            }
            return result;
        }

        public List<MaterialItem> GetMaterialsForExamination(int examinationId)
        {
            List<MaterialItem> result = new List<MaterialItem>();
            MySqlConnection conn = null;
            MySqlCommand cmd;
            MySqlDataReader reader = null;

            try
            {
                conn = MySQLUtil.GetMySQLConnection();
                cmd = conn.CreateCommand();
                cmd.CommandText = SELECT_MATERIALS_FOR_EXAMINATION;
                cmd.Parameters.AddWithValue("@idPregleda", examinationId);
                reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    result.Add(new MaterialItem
                    {
                        MaterialId = reader.GetInt32("idMaterijala"),
                        MaterialName = reader.GetString("Naziv"),
                        Quantity = reader.GetDecimal("Kolicina"),
                        UnitOfMeasure = reader.GetString("JedinicaMjere"),
                        PricePerUnit = reader.GetDecimal("CijenaPoJedinici"),
                        ExaminationId = examinationId
                    });
                }
            }
            catch (Exception ex)
            {
                throw new DataAccessException("Exception in ExaminationDAOImpl", ex);
            }
            finally
            {
                MySQLUtil.CloseQuietly(reader, conn);
            }
            return result;
        }

        public int AddExamination(Examination examination)
        {
            int newId = 0;
            MySqlConnection conn = null;
            MySqlCommand cmd;

            try
            {
                conn = MySQLUtil.GetMySQLConnection();
                cmd = conn.CreateCommand();
                cmd.CommandText = INSERT;

                cmd.Parameters.AddWithValue("@Dijagnoza", examination.Diagnosis);
                cmd.Parameters.AddWithValue("@idTermina", examination.AppointmentId);
                cmd.Parameters.AddWithValue("@JMBGMedicinskogTehnicara", examination.MedicalTechnicianJMBG);

                newId = Convert.ToInt32(cmd.ExecuteScalar());
            }
            catch (MySqlException mysqlEx) 
            {
                string errorMsg = $"MySQL Error {mysqlEx.Number}: {mysqlEx.Message}";
                if (mysqlEx.Number == 1062) errorMsg += " (DUPLIKAT)";
                if (mysqlEx.Number == 1452) errorMsg += " (NEPOSTOJEĆI STRANI KLJUČ)";

                throw new DataAccessException(errorMsg, mysqlEx);
            }
            catch (Exception ex)
            {
                throw new DataAccessException("Exception in ExaminationDAOImpl", ex);
            }
            finally
            {
                MySQLUtil.CloseQuietly(conn);
            }
            return newId;
        }

        public void UpdateExamination(Examination examination)
        {
            MySqlConnection conn = null;
            MySqlCommand cmd;

            try
            {
                conn = MySQLUtil.GetMySQLConnection();
                cmd = conn.CreateCommand();
                cmd.CommandText = UPDATE;

                cmd.Parameters.AddWithValue("@Dijagnoza", examination.Diagnosis);
                cmd.Parameters.AddWithValue("@idTermina", examination.AppointmentId);
                cmd.Parameters.AddWithValue("@JMBGMedicinskogTehnicara", examination.MedicalTechnicianJMBG);
                cmd.Parameters.AddWithValue("@idPregleda", examination.ExaminationId);

                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                throw new DataAccessException("Exception in ExaminationDAOImpl", ex);
            }
            finally
            {
                MySQLUtil.CloseQuietly(conn);
            }
        }

        public bool DeleteExamination(int examinationId)
        {
            bool result = false;
            MySqlConnection conn = null;
            MySqlCommand cmd;

            try
            {
                conn = MySQLUtil.GetMySQLConnection();
                cmd = conn.CreateCommand();
                cmd.CommandText = DELETE;
                cmd.Parameters.AddWithValue("@idPregleda", examinationId);

                result = cmd.ExecuteNonQuery() == 1;
            }
            catch (Exception ex)
            {
                throw new DataAccessException("Exception in ExaminationDAOImpl", ex);
            }
            finally
            {
                MySQLUtil.CloseQuietly(conn);
            }
            return result;
        }

        private Examination MapReaderToExamination(MySqlDataReader reader)
        {
            return new Examination
            {
                ExaminationId = reader.GetInt32("idPregleda"),
                Diagnosis = reader.GetString("Dijagnoza"),
                AppointmentId = reader.GetInt32("idTermina"),
                MedicalTechnicianJMBG = reader.GetString("JMBGMedicinskogTehnicara"),
                ExaminationDate = reader.GetDateTime("DatumIVrijeme"),
                PatientJMBG = reader.GetString("JMBGPacijenta"),
                DentistJMBG = reader.GetString("JMBGStomatologa")
            };
        }

        public bool AddTreatmentToExamination(int examinationId, int treatmentId, decimal price)
        {
            MySqlConnection conn = null;
            MySqlCommand cmd;

            try
            {
                conn = MySQLUtil.GetMySQLConnection();
                cmd = conn.CreateCommand();
                cmd.CommandText = "INSERT INTO TRETMAN_STAVKA (idTretmana, idPregleda, Cijena) VALUES (@idTretmana, @idPregleda, @Cijena)";

                cmd.Parameters.AddWithValue("@idTretmana", treatmentId);
                cmd.Parameters.AddWithValue("@idPregleda", examinationId);
                cmd.Parameters.AddWithValue("@Cijena", price);

                return cmd.ExecuteNonQuery() == 1;
            }
            catch (Exception ex)
            {
                throw new DataAccessException("Exception in ExaminationDAOImpl", ex);
            }
            finally
            {
                MySQLUtil.CloseQuietly(conn);
            }
        }

        public bool RemoveTreatmentFromExamination(int examinationId, int treatmentId)
        {
            MySqlConnection conn = null;
            MySqlCommand cmd;

            try
            {
                conn = MySQLUtil.GetMySQLConnection();
                cmd = conn.CreateCommand();
                cmd.CommandText = "DELETE FROM TRETMAN_STAVKA WHERE idTretmana = @idTretmana AND idPregleda = @idPregleda";

                cmd.Parameters.AddWithValue("@idTretmana", treatmentId);
                cmd.Parameters.AddWithValue("@idPregleda", examinationId);

                return cmd.ExecuteNonQuery() == 1;
            }
            catch (Exception ex)
            {
                throw new DataAccessException("Exception in ExaminationDAOImpl", ex);
            }
            finally
            {
                MySQLUtil.CloseQuietly(conn);
            }
        }

        public bool AddMaterialToExamination(int examinationId, int materialId, decimal quantity, out string errorMessage)
        {
            MySqlConnection conn = null;
            errorMessage = string.Empty;

            try
            {
                conn = MySQLUtil.GetMySQLConnection();

                using (MySqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "dodaj_materijal_za_pregled";
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@p_idPregleda", examinationId);
                    cmd.Parameters.AddWithValue("@p_idMaterijala", materialId);
                    cmd.Parameters.AddWithValue("@p_Kolicina", quantity);

                    cmd.ExecuteNonQuery();
                }

                using (MySqlCommand checkCmd = conn.CreateCommand())
                {
                    checkCmd.CommandText = @"
                SELECT poruka 
                FROM upozorenja 
                WHERE idMaterijala = @matId 
                ORDER BY datum DESC 
                LIMIT 1";

                    checkCmd.Parameters.AddWithValue("@matId", materialId);

                    var result = checkCmd.ExecuteScalar();
                    if (result != null)
                    {
                        errorMessage = result.ToString();
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                errorMessage = $"Greška: {ex.Message}";
                return false;
            }
            finally
            {
                MySQLUtil.CloseQuietly(conn);
            }
        }

        /* public bool AddMaterialToExamination(int examinationId, int materialId, decimal quantity)
         {
             MySqlConnection conn = null;
             MySqlCommand cmd;

             try
             {
                 conn = MySQLUtil.GetMySQLConnection();
                 cmd = conn.CreateCommand();
                 cmd.CommandText = "dodaj_materijal_za_pregled";
                 cmd.CommandType = System.Data.CommandType.StoredProcedure;

                 cmd.Parameters.AddWithValue("@p_idPregleda", examinationId);
                 cmd.Parameters.AddWithValue("@p_idMaterijala", materialId);
                 cmd.Parameters.AddWithValue("@p_Kolicina", quantity);

                 Console.WriteLine($"Pozivam proceduru: idPregleda={examinationId}, idMaterijala={materialId}, Kolicina={quantity}");

                 // ExecuteNonQuery vraća broj affected redova
                 int rowsAffected = cmd.ExecuteNonQuery();
                 Console.WriteLine($"Procedura je izmijenila {rowsAffected} redova");

                 // INSERT i UPDATE unutar procedure bi trebali affect-ovati barem 1 red
                 return rowsAffected >= 1;
             }
             catch (MySqlException mysqlEx)
             {
                 Console.WriteLine($"MySQL greška {mysqlEx.Number}: {mysqlEx.Message}");
                 throw new DataAccessException($"MySQL Error: {mysqlEx.Message}", mysqlEx);
             }
             catch (Exception ex)
             {
                 Console.WriteLine($"Greška pri dodavanju materijala: {ex.Message}");
                 throw new DataAccessException("Exception in ExaminationDAOImpl", ex);
             }
             finally
             {
                 MySQLUtil.CloseQuietly(conn);
             }
         }*/

        public bool RemoveMaterialFromExamination(int examinationId, int materialId)
        {
            MySqlConnection conn = null;
            MySqlCommand cmd;

            try
            {
                conn = MySQLUtil.GetMySQLConnection();
                cmd = conn.CreateCommand();
                cmd.CommandText = "DELETE FROM MATERIJAL_STAVKA WHERE idMaterijala = @idMaterijala AND idPregleda = @idPregleda";

                cmd.Parameters.AddWithValue("@idMaterijala", materialId);
                cmd.Parameters.AddWithValue("@idPregleda", examinationId);

                return cmd.ExecuteNonQuery() == 1;
            }
            catch (Exception ex)
            {
                throw new DataAccessException("Exception in ExaminationDAOImpl", ex);
            }
            finally
            {
                MySQLUtil.CloseQuietly(conn);
            }
        }

        public List<TreatmentItem> GetTreatmentItemsForExamination(int examinationId)
        {
            List<TreatmentItem> result = new List<TreatmentItem>();
            MySqlConnection conn = null;
            MySqlCommand cmd;
            MySqlDataReader reader = null;

            try
            {
                conn = MySQLUtil.GetMySQLConnection();
                cmd = conn.CreateCommand();
                cmd.CommandText = SELECT_TREATMENTS_FOR_EXAMINATION;
                cmd.Parameters.AddWithValue("@idPregleda", examinationId);
                reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    result.Add(new TreatmentItem
                    {
                        TreatmentId = reader.GetInt32("idTretmana"),
                        TreatmentName = reader.GetString("Naziv"),
                        Price = reader.GetDecimal("Cijena"),
                        ExaminationId = examinationId
                    });
                }
            }
            catch (Exception ex)
            {
                throw new DataAccessException("Exception in ExaminationDAOImpl", ex);
            }
            finally
            {
                MySQLUtil.CloseQuietly(reader, conn);
            }
            return result;
        }

        public bool RemoveAllTreatmentsFromExamination(int examinationId)
        {
            MySqlConnection conn = null;
            MySqlCommand cmd;

            try
            {
                conn = MySQLUtil.GetMySQLConnection();
                cmd = conn.CreateCommand();
                cmd.CommandText = "DELETE FROM TRETMAN_STAVKA WHERE idPregleda = @idPregleda";
                cmd.Parameters.AddWithValue("@idPregleda", examinationId);

                return cmd.ExecuteNonQuery() >= 0; 
            }
            catch (Exception ex)
            {
                throw new DataAccessException("Exception in ExaminationDAOImpl", ex);
            }
            finally
            {
                MySQLUtil.CloseQuietly(conn);
            }
        }

        public bool RemoveAllMaterialsFromExamination(int examinationId)
        {
            MySqlConnection conn = null;
            MySqlCommand cmd;

            try
            {
                conn = MySQLUtil.GetMySQLConnection();
                cmd = conn.CreateCommand();
                cmd.CommandText = "DELETE FROM MATERIJAL_STAVKA WHERE idPregleda = @idPregleda";
                cmd.Parameters.AddWithValue("@idPregleda", examinationId);

                return cmd.ExecuteNonQuery() >= 0; 
            }
            catch (Exception ex)
            {
                throw new DataAccessException("Exception in ExaminationDAOImpl", ex);
            }
            finally
            {
                MySQLUtil.CloseQuietly(conn);
            }
        }


        public List<Treatment> GetTreatmentsForExamination(int examinationId)
        {
            List<Treatment> result = new List<Treatment>();
            MySqlConnection conn = null;
            MySqlCommand cmd;
            MySqlDataReader reader = null;

            try
            {
                conn = MySQLUtil.GetMySQLConnection();
                cmd = conn.CreateCommand();
                cmd.CommandText = @"
                    SELECT t.*, ts.Cijena 
                    FROM TRETMAN t 
                    INNER JOIN TRETMAN_STAVKA ts ON t.idTretmana = ts.idTretmana 
                    WHERE ts.idPregleda = @idPregleda";
                cmd.Parameters.AddWithValue("@idPregleda", examinationId);
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
                throw new DataAccessException("Exception in ExaminationDAOImpl", ex);
            }
            finally
            {
                MySQLUtil.CloseQuietly(reader, conn);
            }
            return result;
        }


        public List<MaterialUsage> GetMaterialUsagesForExamination(int examinationId)
        {
            List<MaterialUsage> result = new List<MaterialUsage>();
            MySqlConnection conn = null;
            MySqlCommand cmd;
            MySqlDataReader reader = null;

            try
            {
                conn = MySQLUtil.GetMySQLConnection();
                cmd = conn.CreateCommand();
                cmd.CommandText = @"
                    SELECT m.idMaterijala, m.Naziv, ms.Kolicina 
                    FROM MATERIJAL m 
                    INNER JOIN MATERIJAL_STAVKA ms ON m.idMaterijala = ms.idMaterijala 
                    WHERE ms.idPregleda = @idPregleda";
                cmd.Parameters.AddWithValue("@idPregleda", examinationId);
                reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    result.Add(new MaterialUsage
                    {
                        idMaterijala = reader.GetInt32("idMaterijala"),
                        Naziv = reader.GetString("Naziv"),
                        Kolicina = reader.GetDecimal("Kolicina")
                    });
                }
            }
            catch (Exception ex)
            {
                throw new DataAccessException("Exception in ExaminationDAOImpl", ex);
            }
            finally
            {
                MySQLUtil.CloseQuietly(reader, conn);
            }
            return result;
        }


        public decimal CalculateTotalForExamination(int examinationId)
        {
            decimal total = 0;
            MySqlConnection conn = null;
            MySqlCommand cmd;
            MySqlDataReader reader = null;

            try
            {
                conn = MySQLUtil.GetMySQLConnection();
                cmd = conn.CreateCommand();
                cmd.CommandText = @"
                    SELECT 
                        (SELECT IFNULL(SUM(ts.Cijena), 0) FROM TRETMAN_STAVKA ts WHERE ts.idPregleda = @idPregleda) +
                        (SELECT IFNULL(SUM(ms.Kolicina * m.CijenaPoJedinici), 0) 
                         FROM MATERIJAL_STAVKA ms 
                         INNER JOIN MATERIJAL m ON ms.idMaterijala = m.idMaterijala 
                         WHERE ms.idPregleda = @idPregleda) AS Total";
                cmd.Parameters.AddWithValue("@idPregleda", examinationId);

                var result = cmd.ExecuteScalar();
                if (result != null && result != DBNull.Value)
                {
                    total = Convert.ToDecimal(result);
                }
            }
            catch (Exception ex)
            {
                throw new DataAccessException("Exception in ExaminationDAOImpl", ex);
            }
            finally
            {
                MySQLUtil.CloseQuietly(conn);
            }
            return total;
        }

        public bool HasExaminationForAppointment(int appointmentId)
        {
            MySqlConnection conn = null;
            MySqlCommand cmd;

            try
            {
                conn = MySQLUtil.GetMySQLConnection();
                cmd = conn.CreateCommand();
                cmd.CommandText = "SELECT COUNT(*) FROM PREGLED WHERE idTermina = @idTermina";
                cmd.Parameters.AddWithValue("@idTermina", appointmentId);

                var count = Convert.ToInt32(cmd.ExecuteScalar());
                return count > 0;
            }
            catch (Exception ex)
            {
                throw new DataAccessException("Exception in ExaminationDAOImpl", ex);
            }
            finally
            {
                MySQLUtil.CloseQuietly(conn);
            }
        }
    }
}
