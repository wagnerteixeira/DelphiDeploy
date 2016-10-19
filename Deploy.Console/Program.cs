using Deploy.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Deploy.Console
{
    class Program
    {
        static void Main(string[] args)
        {
            string fileDpr = @"D:\Cartsys\NOTAS_2016_FULL\trunk\cartorio_notas.dpr";
            string dcuPath = @"D:\Cartsys\NOTAS_2016_FULL\trunk\";

            Service service = new Service(fileDpr, dcuPath);
            System.Console.WriteLine(service.CompileProject());
            System.Console.ReadKey();
        }
    }
}
