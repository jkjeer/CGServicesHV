using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    class Program
    {
        static void Main(string[] args)
        {
            ServiceReference1.RackTagServiceClient myClient = new ServiceReference1.RackTagServiceClient();
            var tags = new DataTable();
            tags = myClient.GetRackTagsDataTable("TWCG", 7319829, "unit", null, false);
            DataTable dt = new DataTable();
            dt = tags;
            Console.WriteLine(" EmpID".PadRight(10)
                              + "FirstName".PadRight(10)
                              + "LastName".PadRight(10));
            Console.WriteLine("---------------------------------------");
            for (int i = 1; i < dt.Rows.Count; i++)
            {
                Console.WriteLine(dt.Rows[i][0].ToString().PadRight(10) +
                                  dt.Rows[i][1].ToString().PadRight(10) +
                                  dt.Rows[i][2].ToString().PadRight(10));
            }
            Console.WriteLine(dt.Rows.Count.ToString());
            Console.ReadKey();
        }
    }
}
