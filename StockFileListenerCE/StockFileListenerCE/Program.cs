using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Security.Permissions;
using StockFileListener;

namespace FileListenerTest
{
    class Program
    {
        static int tiempo_espera;
        static String idLectora;
        static Registro R;

        static void Main(string[] args)
        {
            ConfigurarApp();
            Console.WriteLine("Proyecto Ansilta: Registros del Archivo Salida");
            Data.setIdLectora(idLectora);
            R = new Registro();
            
            while (true)
            {
                R.Actualizar(); //Detecta cambios en el archivo
                R.backup = false;
                System.Threading.Thread.Sleep(tiempo_espera);
                //Console.Clear();
                Console.WriteLine("Esperando Cambios..."); 
            }
        }

        static void ConfigurarApp(){
            new Settings();
            tiempo_espera = Settings.getTiempoEspera();
            idLectora = Settings.getIdLectora();
            Console.WriteLine("LEC: " + idLectora+" TE: "+tiempo_espera);
        }
    }
}
