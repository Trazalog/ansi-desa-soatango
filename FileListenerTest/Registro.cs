using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Reflection;
using System.Net;
using Newtonsoft.Json;

namespace FileListenerTest
{
    public class Registro
    {
        String file;
        StringBuilder sb;
        List<String> registros = new List<String>();
        int cantidad;
        public Boolean backup = false;
        static String url = System.Configuration.ConfigurationSettings.AppSettings["url_server"];
        static String archivo_salida = System.Configuration.ConfigurationSettings.AppSettings["archivo_salida"];
        static String registro_temp = System.Configuration.ConfigurationSettings.AppSettings["registros_temp"];

        public Registro(String file)
        {
            this.file = file;
            cantidad = LeerArchivo(archivo_salida, FileShare.Write);
           // Mostrar_Registros();
 
        }

        public void GuardarTemporales(String registro)
        {

            string targetFolder = Path.GetDirectoryName(Assembly.GetExecutingAssembly().GetModules()[0].FullyQualifiedName);

            string targetFileName = Path.Combine(targetFolder, registro_temp);

            var fs = new FileStream(targetFileName, FileMode.Append, FileAccess.Write, FileShare.Read);

            using (StreamWriter sw = new StreamWriter(fs))
            {

                sw.WriteLine(registro);

                sw.Flush();

                sw.Close();

            }

        }

        public void LimpiarRegistrosTemp() {

            string targetFolder = Path.GetDirectoryName(Assembly.GetExecutingAssembly().GetModules()[0].FullyQualifiedName);

            string targetFileName = Path.Combine(targetFolder, registro_temp);

            if(File.Exists(targetFileName))File.Delete(targetFileName);
        }

        public Boolean SendToApi(String data)
        {
            //if (new Random().Next(100) % 2 == 0) return false;
            data = JsonConvert.SerializeObject(new Data(data));
            Console.WriteLine(data);
            // create a request HttpWebRequest 
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.KeepAlive = false;
            request.ProtocolVersion = HttpVersion.Version10;
            request.Method = "POST"; // turn our request string into a byte stream 
            byte[] postBytes = Encoding.UTF8.GetBytes(data); // this is important - make sure you specify type this way 
            request.ContentType = "application/json; charset=UTF-8";
            request.Accept = "application/json";
            request.ContentLength = postBytes.Length;

            try{
                Stream requestStream = request.GetRequestStream(); // now send it request
                requestStream.Write(postBytes, 0, postBytes.Length);
                requestStream.Close(); // grab te response and print it out to the console along with the status code 
                return true;
            }
            catch (WebException ex){
                Console.WriteLine(ex.Message.ToString());
                return false;
            }
        }


        public void ReenviarRegistrosTemporales(){

            if (!ExisteArchivo(registro_temp) || LeerArchivo(registro_temp, FileShare.ReadWrite) == 0) return;
            LimpiarRegistrosTemp();
            foreach (String s in registros)
            {
                if (!backup && SendToApi(s))
                {
                    Console.WriteLine("R): " + s + " >> Reenviado");
                }
                else
                {
                    Console.WriteLine("R): " + s + "  >> Fallo");
                    GuardarTemporales(s);
                    backup = true;
                }
            }
        }

        public bool Actualizar() {
            ReenviarRegistrosTemporales();
            int aux = LeerArchivo(archivo_salida, FileShare.Write);
            if (this.cantidad != aux)
            {
                    
                registros.RemoveRange(0, this.cantidad);
                foreach (String s in registros)
                {
                    if (!backup && SendToApi(s))
                    {
                        Console.WriteLine("R): " + s + " >> Enviado");
                    }
                    else
                    {
                        Console.WriteLine("R): " + s + "  >> Fallo");
                        this.GuardarTemporales(s);
                        backup = true;
                        
                    }
                }
                this.cantidad = aux;
                return true;
            }
            else {
                return false;
            }
        }
     
        public int LeerArchivo(String archivo,FileShare fso) {

            registros.Clear();

            try
            {

                string targetFolder = Path.GetDirectoryName(Assembly.GetExecutingAssembly().GetModules()[0].FullyQualifiedName);

                string targetFileName = Path.Combine(targetFolder, archivo);

                if (!File.Exists(targetFileName)) return 0;

                var fs = new FileStream(targetFileName, FileMode.Open, FileAccess.Read, fso);

                using (StreamReader sr = new StreamReader(fs))
                {
                    String line;

                    while ((line = sr.ReadLine()) != null)
                    {
                        if (line.Length != 0) registros.Add(line);
                    }

                    sr.Close();
                }

            }catch(Exception E){
            
            }
            return registros.Count;

        }

        public void Mostrar_Registros() {
            Console.WriteLine("******************************");
            foreach (String s in registros) {
                Console.WriteLine(s);
            }
        }

        public bool ExisteArchivo(String nombre) {

            string targetFolder = Path.GetDirectoryName(Assembly.GetExecutingAssembly().GetModules()[0].FullyQualifiedName);

            string targetFileName = Path.Combine(targetFolder, nombre);

            return File.Exists(targetFileName);
        }

        //public void getToApi() {

        //    System.Net.WebClient client = new System.Net.WebClient();

        //    client.Headers.Add("content-type", "application/json");//set your header here, you can add multiple headers

        //    //string s = Encoding.ASCII.GetString(client.UploadData("http://localhost/restful/index.php/api/example/users?format=json", "get", Encoding.Default.GetBytes("{\"EmailId\": \"admin@admin.com\",\"Password\": \"pass#123\"}")));

        //    string s = Encoding.ASCII.GetString(client.UploadData("http://localhost/restful/index.php/api/example/users?format=json", "GET", Encoding.Default.GetBytes("{\"EmailId\": \"admin@admin.com\",\"Password\": \"pass#123\"}")));

        //    Console.WriteLine(s);

        //}

    }
    
}
