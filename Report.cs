using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace CRUDMahasiswaADO
{
    public partial class Report : Form
    {
        // Tambahkan object DAL agar bisa dipanggil di konstruktor
        DAL dbLogic = new DAL();

        public string prodi { get; set; }
        public DateTime tglmasuk { get; set; }

        // 16. Ubah Konstruktor Report sesuai modul
        public Report(string Prodi, DateTime TglMasuk)
        {
            InitializeComponent();

            prodi = Prodi;
            tglmasuk = TglMasuk;

            try
            {
                DataTable dtMahasiswa = dbLogic.getDataRekap(prodi, tglmasuk);
                
                // 1. Samakan nama tabel dengan nama Object Data Source di report Anda
                dtMahasiswa.TableName = "CRUDMahasiswaADO_Data";

                // listMahasiswa di modul mengacu pada objek Crystal Report kamu
                CrystalReport1 listMahasiswa = new CrystalReport1();

                // 2. JURUS PAMUNGKAS: Paksa semua tabel di report untuk nyambung ke dtMahasiswa
                for (int i = 0; i < listMahasiswa.Database.Tables.Count; i++)
                {
                    listMahasiswa.Database.Tables[i].SetDataSource(dtMahasiswa);
                }

                crystalReportViewer1.ReportSource = listMahasiswa;
                crystalReportViewer1.Refresh();
            }
            catch (Exception ex)
            {
                //simpanLog(ex.Message);
                MessageBox.Show("Gagal load data: " + ex.Message);
            }
        }
    }
}