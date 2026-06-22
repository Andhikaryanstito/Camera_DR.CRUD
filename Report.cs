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

                if (dtMahasiswa.Columns.Contains("NamaProdi"))
                    dtMahasiswa.Columns["NamaProdi"].ColumnName = "KodeProdi";
                if (dtMahasiswa.Columns.Contains("TanggalDaftar"))
                    dtMahasiswa.Columns["TanggalDaftar"].ColumnName = "TanggalLahir";

                if (!dtMahasiswa.Columns.Contains("NIM"))
                    dtMahasiswa.Columns.Add("NIM", typeof(string));
                if (!dtMahasiswa.Columns.Contains("FotoPath"))
                    dtMahasiswa.Columns.Add("FotoPath", typeof(string));

                dtMahasiswa.TableName = "Mahasiswa";

                // listMahasiswa di modul mengacu pada objek Crystal Report kamu
                CrystalReport1 listMahasiswa = new CrystalReport1();

                listMahasiswa.Database.Tables[0].SetDataSource(dtMahasiswa);
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