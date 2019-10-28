﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Security.Permissions;
namespace FileListenerTest
{
    class Program
    {
        static String file = "F:\\usuarios\\alumno\\Documentos\\Visual Studio 2008\\Projects\\FileListenerTest\\FileListenerTest\\bin\\Debug\\salida.txt";
        static Registro R;
        static void Main(string[] args)
        {   
            Console.WriteLine("Proyecto Ansilta: Registros del Archivo Salida");
            R = new Registro(file);
            while (true)
            {
                R.Actualizar(); //Detecta cambios en el archivo
                System.Threading.Thread.Sleep(15000);
                R.backup = false;
            }
        }
    }
}
