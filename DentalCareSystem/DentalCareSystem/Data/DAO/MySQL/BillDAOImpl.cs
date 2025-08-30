using DentalCareSystem.Data.DAO.Exceptions;
using DentalCareSystem.Data.DTO;
using MySql.Data.MySqlClient;
using System.Data;

namespace DentalCareSystem.Data.DAO.MySQL
{
    public class BillDAOImpl : IBill
    {
        private static readonly string SELECT = "SELECT IdRacuna, Iznos, DatumIzdavanja, JMBGPacijenta FROM `RACUN` ORDER BY DatumIzdavanja";

        public int CreateBill(int orderId)
        {
            MySqlConnection conn = null;
            MySqlCommand cmd;
            try
            {
                conn = MySQLUtil.GetMySQLConnection();
                cmd = conn.CreateCommand(); cmd = new MySqlCommand("kreirajRacun", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("@idNarudzba", MySqlDbType.Int32).Value = orderId;
                cmd.Parameters.Add("@racunId", MySqlDbType.Int32).Direction = ParameterDirection.Output;
                cmd.ExecuteNonQuery();
                int billId = Convert.ToInt32(cmd.Parameters["@računId"].Value);
                return billId;
            }
            catch (Exception ex)
            {
                throw new DataAccessException("Exception in BillDAOImpl", ex);
            }

            finally
            {
                MySQLUtil.CloseQuietly(conn);
            }
        }

        public List<Bill> GetBills()
        {
            List<Bill> result = new List<Bill>();
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
                    result.Add(new Bill()
                    {
                        BillId = reader.GetInt32(0),
                        TotalPrice = reader.GetDouble(1),
                        DateTime = reader.GetDateTime(2),
                        PatientJMBG = reader.GetString(3)
                    });
                }
            }
            catch (Exception ex)
            {
                throw new DataAccessException("Exception in BillDAOImpl", ex);
            }
            finally
            {
                MySQLUtil.CloseQuietly(reader, conn);
            }
            return result;
        }

        public DetailedBill GetBill(int billId)
        {
            MySqlDataReader reader = null;
            MySqlConnection conn = null;
            DetailedBill detailedBill = null;

            try
            {
                conn = MySQLUtil.GetMySQLConnection();

                string sql = @"
            SELECT
                r.idRacuna,
                r.Iznos,
                r.DatumIzdavanja,
                t.idTretmana,
                t.Naziv AS NazivTretmana,
                ts.Cijena AS CijenaTretmana,
                COALESCE(mt.Ime, '') AS ImeMT,
                COALESCE(mt.Prezime, '') AS PrezimeMT,
                p.Ime AS ImeP,
                p.Prezime AS PrezimeP
            FROM RACUN r
            JOIN PREGLED pr ON r.idPregleda = pr.idPregleda
            JOIN PACIJENT p ON r.JMBGPacijenta = p.JMBG
            JOIN ZAPOSLENI mt ON r.JMBGMedicinskogTehnicara = mt.JMBG
            JOIN TRETMAN_STAVKA ts ON pr.idPregleda = ts.idPregleda
            JOIN TRETMAN t ON ts.idTretmana = t.idTretmana
            WHERE r.idRacuna = @idRacuna";

                using (MySqlCommand cmd = new MySqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@idRacuna", billId);
                    reader = cmd.ExecuteReader();
                    List<TreatmentItem> items = new List<TreatmentItem>();

                    if (reader.HasRows)
                    {
                        reader.Read();

                        detailedBill = new DetailedBill()
                        {
                            BillId = reader.GetInt32("idRacuna"),
                            TotalPrice = reader.GetDouble("Iznos"),
                            DateTime = reader.GetDateTime("DatumIzdavanja"),
                            PatientFirstName = reader.GetString("ImeP"),
                            PatientLastName = reader.GetString("PrezimeP"),
                            MedichalTechnicianFirstName = reader.GetString("ImeMT"),
                            MedichalTechnicianLastName = reader.GetString("PrezimeMT"),
                            Items = items
                        };

                        items.Add(new TreatmentItem()
                        {
                            TreatmentId = reader.GetInt32("idTretmana"),
                            TreatmentName = reader.GetString("NazivTretmana"),
                            Price = reader.GetDecimal("CijenaTretmana")
                        });

                        while (reader.Read())
                        {
                            items.Add(new TreatmentItem()
                            {
                                TreatmentId = reader.GetInt32("idTretmana"),
                                TreatmentName = reader.GetString("NazivTretmana"),
                                Price = reader.GetDecimal("CijenaTretmana")
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new DataAccessException("Exception in GetBill", ex);
            }
            finally
            {
                MySQLUtil.CloseQuietly(reader, conn);
            }

            return detailedBill;
        }

    }
}
