using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace CRUDMahasiswaADO
{
    public partial class RekapMahasiswa : Form
    {
        // 15. Tambahkan object untuk mendeklarasikan class DAL pada Form RekapMahasiswa
        DAL dbLogic = new DAL();

        // Connection string menggunakan credential database kamu
        static string connectionString = @"Data Source=LAPTOP-7SOCNODM\ANDHIKA1; Initial Catalog=DBAkademikADO; User ID=sa; Password=Purworejo123";

        SqlConnection conn = new SqlConnection(connectionString);
        SqlDataAdapter da;
        DataTable dtMahasiswa;
        DataTable dtProdi;

        public RekapMahasiswa()
        {
            InitializeComponent();
        }

        private void RekapMahasiswa_Load(object sender, EventArgs e)
        {
            dtpTanggalMasuk.Format = DateTimePickerFormat.Custom;
            dtpTanggalMasuk.CustomFormat = "yyyy";
            dtpTanggalMasuk.ShowUpDown = true;
            dtpTanggalMasuk.MinDate = new DateTime(2000, 1, 1);
            dtpTanggalMasuk.MaxDate = DateTime.Now;

            cmbProdi.DropDownStyle = ComboBoxStyle.DropDownList;

            btnCetak.Enabled = false;

            try
            {
                if (conn.State == ConnectionState.Closed)
                {
                    conn.Open();
                }

                SqlCommand cmd = new SqlCommand("select namaprodi from programstudi", conn);
                cmd.CommandType = CommandType.Text;
                dtProdi = new DataTable();
                da = new SqlDataAdapter(cmd);
                da.Fill(dtProdi);
                cmbProdi.DataSource = dtProdi;
                cmbProdi.DisplayMember = "namaprodi";
                cmbProdi.ValueMember = "namaprodi";
            }
            catch (Exception ex)
            {
                MessageBox.Show("Gagal load data: " + ex.Message);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                if (conn.State == ConnectionState.Closed)
                {
                    conn.Open();
                }

                SqlCommand cmd = new SqlCommand("sp_Report", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("@inProdi", SqlDbType.VarChar, 50).Value = cmbProdi.SelectedValue;
                cmd.Parameters.Add("@inTglMsuk", SqlDbType.VarChar, 4).Value = dtpTanggalMasuk.Value.Year.ToString();

                da = new SqlDataAdapter(cmd);

                dtMahasiswa = new DataTable();
                da.Fill(dtMahasiswa);

                dataGridView1.DataSource = dtMahasiswa;

                if (dtMahasiswa.Rows.Count > 0)
                {
                    btnCetak.Enabled = true;
                }
                else
                {
                    btnCetak.Enabled = false;
                    MessageBox.Show("Data tidak ditemukan");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Gagal load data: " + ex.Message);
            }
        }

        private void dtpTanggalMasuk_ValueChanged(object sender, EventArgs e)
        {
            // Method ini dibiarkan kosong agar desainer tidak error
        }

        // Tombol Cetak disesuaikan persis dengan modul menggunakan Form Report
        private void btnCetak_Click(object sender, EventArgs e)
        {
            Report frm2 = new Report(cmbProdi.SelectedValue.ToString(), dtpTanggalMasuk.Value);
            frm2.Show();
            this.Hide();
        }

        private void cmbProdi_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }
}