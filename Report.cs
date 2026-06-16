using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace CRUDMahasiswaADO
{
    public partial class Report : Form
    {
        static string connectionString = @"Data Source=LAPTOP-7SOCNODM\ANDHIKA1; Initial Catalog=DBAkademikADO; User ID=sa; Password=Purworejo123";

        SqlConnection conn = new SqlConnection(connectionString);
        SqlDataAdapter da;
        DataTable dtMahasiswa;

        public string prodi { get; set; }
        public DateTime tglmasuk { get; set; }

        public Report(string Prodi, DateTime TglMasuk)
        {
            InitializeComponent();

            prodi = Prodi;
            tglmasuk = TglMasuk;

            try
            {
                if (conn.State == ConnectionState.Closed)
                {
                    conn.Open();
                }

                SqlCommand cmd = new SqlCommand("sp_Report", conn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add("@inProdi", SqlDbType.VarChar, 50).Value = prodi;
                cmd.Parameters.Add("@inTglMsuk", SqlDbType.VarChar, 4).Value = tglmasuk.Year.ToString();

                da = new SqlDataAdapter(cmd);
                dtMahasiswa = new DataTable();

                // 1. Samakan nama tabel dengan nama Object Data Source di report Anda
                dtMahasiswa.TableName = "CRUDMahasiswaADO_Data";

                da.Fill(dtMahasiswa);
                conn.Close();

                CrystalReport1 myReport = new CrystalReport1();

                // 2. JURUS PAMUNGKAS: Paksa semua tabel di report untuk nyambung ke dtMahasiswa
                for (int i = 0; i < myReport.Database.Tables.Count; i++)
                {
                    myReport.Database.Tables[i].SetDataSource(dtMahasiswa);
                }

                crystalReportViewer1.ReportSource = myReport;
                crystalReportViewer1.Refresh();
            }
            catch (Exception ex)
            {
                string pesanError = "Gagal load data: " + ex.Message;

                if (ex.InnerException != null)
                {
                    pesanError += "\n\nDetail Asli: " + ex.InnerException.Message;
                }

                MessageBox.Show(pesanError, "Error Crystal Report", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}