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

                // listMahasiswa di modul mengacu pada objek Crystal Report kamu
                CrystalReport1 listMahasiswa = new CrystalReport1();

                listMahasiswa.SetDataSource(dtMahasiswa);
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