using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace CRUDMahasiswaADO
{
    public partial class Form1 : Form
    {
        // Menambahkan variabel koneksi dan alamat database
        private readonly SqlConnection conn;
        private readonly string connectionString = @"Data Source=LAPTOP-7SOCNODM\ANDHIKA1; Initial Catalog=DBAkademikADO; Integrated Security=True";

        public Form1()
        {
            InitializeComponent();
            // Inisialisasi koneksi
            conn = new SqlConnection(connectionString);
        }
    }
}