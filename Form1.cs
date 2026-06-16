using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace CRUDMahasiswaADO
{
    public partial class Form1 : Form
    {
        // Connection string
        private readonly string connectionString = @"Data Source=LAPTOP-7SOCNODM\ANDHIKA1; Initial Catalog=DBAkademikADO; Integrated Security=True";

        private BindingSource bindingSource = new BindingSource();
        private DataTable dtMahasiswa = new DataTable();

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            cmbJK.Items.Clear();
            cmbJK.Items.Add("L");
            cmbJK.Items.Add("P");

            dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridView1.MultiSelect = false;
            dataGridView1.ReadOnly = true;
            dataGridView1.AllowUserToAddRows = false;
            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            LoadData();
        }

        private void LoadData()
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("sp_GetMahasiswa", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {
                        dtMahasiswa = new DataTable();
                        da.Fill(dtMahasiswa);

                        bindingSource.DataSource = dtMahasiswa;
                        dataGridView1.DataSource = bindingSource;

                        BindControls();
                    }
                }
            }
            HitungTotal();
        }

        private void BindControls()
        {
            txtNIM.DataBindings.Clear();
            txtNama.DataBindings.Clear();
            cmbJK.DataBindings.Clear();
            dtpTanggalLahir.DataBindings.Clear();
            txtAlamat.DataBindings.Clear();
            txtKodeProdi.DataBindings.Clear();

            txtNIM.DataBindings.Add("Text", bindingSource, "NIM");
            txtNama.DataBindings.Add("Text", bindingSource, "Nama");
            cmbJK.DataBindings.Add("Text", bindingSource, "JenisKelamin");
            dtpTanggalLahir.DataBindings.Add("Value", bindingSource, "TanggalLahir");
            txtAlamat.DataBindings.Add("Text", bindingSource, "Alamat");
            txtKodeProdi.DataBindings.Add("Text", bindingSource, "KodeProdi");
        }

        // === SEMUA EVENT BUTTON SUDAH LENGKAP ===

        private void btnRekapData_Click(object sender, EventArgs e)
        {
            // Menampilkan form RekapMahasiswa
            RekapMahasiswa fm3 = new RekapMahasiswa();
            fm3.Show();
            this.Hide();
        }

        private void btnConnect_Click(object sender, EventArgs e)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    MessageBox.Show("Koneksi berhasil");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Koneksi gagal: " + ex.Message);
            }
        }

        private void btnLoad_Click(object sender, EventArgs e)
        {
            LoadData();
        }

        private void btnInsert_Click(object sender, EventArgs e)
        {
            SqlConnection conn = new SqlConnection(connectionString);
            conn.Open();
            SqlTransaction trans = conn.BeginTransaction();
            try
            {
                SqlCommand cmd = new SqlCommand("sp_InsertMahasiswa", conn, trans);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@NIM", txtNIM.Text);
                cmd.Parameters.AddWithValue("@Nama", txtNama.Text);
                cmd.Parameters.AddWithValue("@JenisKelamin", cmbJK.Text);
                cmd.Parameters.AddWithValue("@TanggalLahir", dtpTanggalLahir.Value.Date);
                cmd.Parameters.AddWithValue("@Alamat", txtAlamat.Text);
                cmd.Parameters.AddWithValue("@KodeProdi", txtKodeProdi.Text);
                cmd.Parameters.AddWithValue("@TanggalDaftar", DateTime.Now);
                cmd.ExecuteNonQuery();

                SqlCommand cmdLog = new SqlCommand(@"INSERT INTO LogAktivitas (aktivitas,waktu) VALUES (@aktivitas,GETDATE())", conn, trans);
                cmdLog.Parameters.AddWithValue("@aktivitas", "INSERT MAHASISWA : " + txtNIM.Text);
                cmdLog.ExecuteNonQuery();

                trans.Commit();
                MessageBox.Show("Data berhasil ditambahkan");
                LoadData();
            }
            catch (Exception ex)
            {
                trans.Rollback();
                MessageBox.Show(ex.Message);
            }
            finally
            {
                conn.Close();
            }
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            if (txtNIM.Text.Contains("'") || txtNIM.Text.Contains("-") || txtNIM.Text.Contains("="))
            {
                MessageBox.Show("Data ditolak! Karakter terlarang.", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("sp_UpdateMahasiswa", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@NIM", txtNIM.Text);
                        cmd.Parameters.AddWithValue("@Nama", txtNama.Text);
                        cmd.Parameters.AddWithValue("@JenisKelamin", cmbJK.Text);
                        cmd.Parameters.AddWithValue("@TanggalLahir", dtpTanggalLahir.Value.Date);
                        cmd.Parameters.AddWithValue("@Alamat", txtAlamat.Text);
                        cmd.Parameters.AddWithValue("@KodeProdi", txtKodeProdi.Text);

                        conn.Open();
                        cmd.ExecuteNonQuery();
                    }
                }
                MessageBox.Show("Data berhasil diupdate");
                LoadData();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("sp_DeleteMahasiswa", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add("@NIM", SqlDbType.Char, 11).Value = txtNIM.Text;
                        conn.Open();
                        if (cmd.ExecuteNonQuery() > 0) MessageBox.Show("Data berhasil dihapus");
                        else MessageBox.Show("Data tidak ditemukan");
                    }
                }
                LoadData();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnResetData_Click(object sender, EventArgs e)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    string query = "IF OBJECT_ID('dbo.Mahasiswa_Backup') IS NOT NULL BEGIN DELETE FROM dbo.Mahasiswa; INSERT INTO dbo.Mahasiswa SELECT * FROM dbo.Mahasiswa_Backup; END";
                    using (SqlCommand cmd = new SqlCommand(query, conn)) { cmd.ExecuteNonQuery(); }
                }
                MessageBox.Show("Data berhasil direset");
                LoadData();
            }
            catch (Exception ex) { MessageBox.Show("Reset gagal: " + ex.Message); }
        }

        private void HitungTotal()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("sp_CountMahasiswa", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        SqlParameter outputParam = new SqlParameter("@Total", SqlDbType.Int) { Direction = ParameterDirection.Output };
                        cmd.Parameters.Add(outputParam);
                        conn.Open();
                        cmd.ExecuteNonQuery();
                        lblTotal.Text = "Total Mahasiswa: " + outputParam.Value.ToString();
                    }
                }
            }
            catch { }
        }

        private void SimpanLog(string pesan) { /* Fungsi Log */ }
        private void btnTestInjection_Click(object sender, EventArgs e) { /* Logika Injection */ }
    }
}