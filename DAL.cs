using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;
using System.Net; // Ditambahkan untuk fungsi IP Address
using System.Net.Sockets; // Ditambahkan untuk fungsi IP Address

namespace CRUDMahasiswaADO
{
    public class DAL
    {
        // ========================================================
        // LANGKAH 19: KONFIGURASI IP DINAMIS UNTUK DEPLOY APLIKASI
        // ========================================================

        // 19.a Tambahkan fungsi pencari IP Address lokal otomatis
        public static string GetLocalIPAddress()
        {
            string localIP = string.Empty;
            try
            {
                var host = Dns.GetHostEntry(Dns.GetHostName());
                foreach (var ip in host.AddressList)
                {
                    if (ip.AddressFamily == AddressFamily.InterNetwork)
                    {
                        localIP = ip.ToString();
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error getting local IP address: " + ex.Message);
            }
            return localIP;
        }

        // 19.b Ubah string connection memanggil IP Address dinamis
        public static string GetConnectionString()
        {
            // Menyisipkan IP secara dinamis namun tetap memanggil instance \ANDHIKA1 milikmu
            string connectionString = $"Data Source={GetLocalIPAddress()}\\ANDHIKA1; Initial Catalog=DBAkademikADO; User ID=sa; Password=Purworejo123";
            return connectionString;
        }

        SqlConnection conn = new SqlConnection(GetConnectionString());
        SqlDataAdapter da;
        DataTable dtMahasiswa;
        DataTable dtProdi;

        // ========================================================
        // KUMPULAN FUNGSI CRUD & LOGIC
        // ========================================================

        public int CountMhs()
        {
            if (conn.State == ConnectionState.Closed) conn.Open();

            SqlCommand cmd = new SqlCommand("sp_CountMahasiswa", conn);
            cmd.CommandType = CommandType.StoredProcedure;

            SqlParameter outputParam = new SqlParameter("@Total", SqlDbType.Int);
            outputParam.Direction = ParameterDirection.Output;

            cmd.Parameters.Add(outputParam);
            cmd.ExecuteNonQuery();

            return Convert.ToInt32(outputParam.Value);
        }

        public DataTable GetMhs()
        {
            if (conn.State == ConnectionState.Closed) conn.Open();

            SqlCommand cmd = new SqlCommand("sp_GetMahasiswa", conn);
            cmd.CommandType = CommandType.StoredProcedure;

            da = new SqlDataAdapter(cmd);
            dtMahasiswa = new DataTable();
            da.Fill(dtMahasiswa);

            return dtMahasiswa;
        }

        public void InsertMhs(string nim, string nama, string alamat, string jenisKelamin, DateTime tanggalLahir, string kodeProdi, byte[] foto)
        {
            if (conn.State == ConnectionState.Closed) conn.Open();
            SqlTransaction trans = conn.BeginTransaction();
            try
            {
                SqlCommand command = new SqlCommand("sp_InsertMahasiswa", conn);
                command.Transaction = trans;
                command.CommandType = CommandType.StoredProcedure;

                command.Parameters.AddWithValue("@pNIM", nim);
                command.Parameters.AddWithValue("@pNama", nama);
                command.Parameters.AddWithValue("@pAlamat", alamat);
                command.Parameters.AddWithValue("@pTanggalLahir", tanggalLahir);
                command.Parameters.AddWithValue("@pJenisKelamin", jenisKelamin);
                command.Parameters.AddWithValue("@pKodeProdi", kodeProdi);

                // Proteksi VarBinary agar SQL tidak salah mendeteksi teks kosong (NVarChar)
                SqlParameter picParam = new SqlParameter("@pFoto", SqlDbType.VarBinary);
                if (foto != null && foto.Length > 0)
                    picParam.Value = foto;
                else
                    picParam.Value = DBNull.Value;
                command.Parameters.Add(picParam);

                command.ExecuteNonQuery();
                trans.Commit();
            }
            catch (Exception ex)
            {
                trans.Rollback();
                throw new Exception(ex.Message);
            }
            finally
            {
                conn.Close();
            }
        }

        public void UpdateMhs(string nim, string nama, string alamat, string jenisKelamin, DateTime tanggalLahir, string kodeProdi, byte[] foto)
        {
            if (conn.State == ConnectionState.Closed) conn.Open();

            try
            {
                SqlCommand command = new SqlCommand("sp_UpdateMahasiswa", conn);
                command.CommandType = CommandType.StoredProcedure;

                command.Parameters.AddWithValue("@pNIM", nim);
                command.Parameters.AddWithValue("@pNama", nama);
                command.Parameters.AddWithValue("@pAlamat", alamat);
                command.Parameters.AddWithValue("@pJenisKelamin", jenisKelamin);
                command.Parameters.AddWithValue("@pTanggalLahir", tanggalLahir);
                command.Parameters.AddWithValue("@pKodeProdi", kodeProdi);

                // Proteksi VarBinary agar SQL tidak salah mendeteksi teks kosong (NVarChar)
                SqlParameter picParam = new SqlParameter("@pFoto", SqlDbType.VarBinary);
                if (foto != null && foto.Length > 0)
                    picParam.Value = foto;
                else
                    picParam.Value = DBNull.Value;
                command.Parameters.Add(picParam);

                command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public void DeleteMhs(string nim)
        {
            if (conn.State == ConnectionState.Closed) conn.Open();

            try
            {
                SqlCommand cmd = new SqlCommand("sp_DeleteMahasiswa", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@pNIM", nim);
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public void resetData()
        {
            if (conn.State == ConnectionState.Closed) conn.Open();

            string deleteQuery = "DELETE FROM mahasiswa;";
            SqlCommand cmdDelete = new SqlCommand(deleteQuery, conn);
            cmdDelete.ExecuteNonQuery();

            string insertQuery = @"
                INSERT INTO mahasiswa
                SELECT * FROM mahasiswa_backup;";
            SqlCommand cmdInsert = new SqlCommand(insertQuery, conn);
            cmdInsert.ExecuteNonQuery();
        }

        public void testInject(string nim)
        {
            if (conn.State == ConnectionState.Closed) conn.Open();

            string query = "Update mahasiswa set nama = 'HACKED' where NIM = " + nim;
            SqlCommand cmd = new SqlCommand(query, conn);
            cmd.ExecuteNonQuery();
        }

        public DataTable GetMhsByNIM(string nim)
        {
            if (conn.State == ConnectionState.Closed) conn.Open();

            SqlCommand cmd = new SqlCommand("sp_GetMahasiswaByNIM", conn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@pNIM", nim);

            da = new SqlDataAdapter(cmd);
            dtMahasiswa = new DataTable();
            da.Fill(dtMahasiswa);

            return dtMahasiswa;
        }

        public void InsertLog(string message)
        {
            if (conn.State == ConnectionState.Closed) conn.Open();

            SqlCommand cmd = new SqlCommand("sp_LogMessage", conn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@psn", message);

            cmd.ExecuteNonQuery();
        }

        public DataTable getProdi()
        {
            if (conn.State == ConnectionState.Closed) conn.Open();

            SqlCommand cmd = new SqlCommand("sp_GetProdi", conn);
            cmd.CommandType = CommandType.StoredProcedure;

            da = new SqlDataAdapter(cmd);
            dtProdi = new DataTable();
            da.Fill(dtProdi);

            return dtProdi;
        }

        public DataTable getDataRekap(string prodi, DateTime tanggalMasuk)
        {
            if (conn.State == ConnectionState.Closed) conn.Open();

            SqlCommand cmd = new SqlCommand("sp_Report", conn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@inProdi", prodi);
            cmd.Parameters.AddWithValue("@inTglMsuk", tanggalMasuk.Year.ToString());

            da = new SqlDataAdapter(cmd);
            dtMahasiswa = new DataTable();
            da.Fill(dtMahasiswa);

            return dtMahasiswa;
        }

        public DataTable getAllDataChart()
        {
            if (conn.State == ConnectionState.Closed) conn.Open();

            SqlCommand cmd = new SqlCommand("sp_DashBoard", conn);
            cmd.CommandType = CommandType.StoredProcedure;

            da = new SqlDataAdapter(cmd);
            dtMahasiswa = new DataTable();
            da.Fill(dtMahasiswa);

            return dtMahasiswa;
        }

        public DataTable getDataChartByTahun(DateTime thMasuk)
        {
            if (conn.State == ConnectionState.Closed) conn.Open();

            SqlCommand cmd = new SqlCommand("sp_DashBoardByTahun", conn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@inTglMsuk", thMasuk.Year);

            da = new SqlDataAdapter(cmd);
            dtMahasiswa = new DataTable();
            da.Fill(dtMahasiswa);

            return dtMahasiswa;
        }
    }
}