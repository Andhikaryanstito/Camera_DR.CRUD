using System;
using CrystalDecisions.CrystalReports.Engine;

namespace CRTest
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                ReportDocument rpt = new ReportDocument();
                rpt.Load(@"d:\SEMESTER 4\PABD_14\CRUDMahasiswaADO\CrystalReport1.rpt");
                foreach (Table t in rpt.Database.Tables)
                {
                    Console.WriteLine("Table Name: " + t.Name);
                    foreach (DatabaseFieldDefinition f in t.Fields)
                    {
                        Console.WriteLine("  Field: " + f.Name);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }
        }
    }
}
