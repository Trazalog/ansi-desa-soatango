using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Security.Permissions;
namespace FileListenerTest
{
    class Program
    {
        static String file = System.Configuration.ConfigurationSettings.AppSettings["salida"];
        static int tiempo_espera = Int16.Parse(System.Configuration.ConfigurationSettings.AppSettings["tiempo_espera"]);
        static Registro R;

        static void Main(string[] args)
        {
            Console.WriteLine("Proyecto Ansilta: Registros del Archivo Salida");
            R = new Registro(file);
            while (true)
            {
                R.Actualizar(); //Detecta cambios en el archivo
                System.Threading.Thread.Sleep(tiempo_espera);
                R.backup = false;
            }
        }
    }
}
