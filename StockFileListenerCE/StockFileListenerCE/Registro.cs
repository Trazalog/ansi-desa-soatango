using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Reflection;
using System.Net;
using StockFileListener;

namespace FileListenerTest
{
    public class Registro
    {
        List<String> registros = new List<String>();
        List<String> reg_fails = new List<String>();
        int cantidad;
        public Boolean backup = false;
        static String url = Settings.getURL();
        static String archivo_salida = Settings.getNombreArchivo();
        static String registro_temp = "registros_temp.txt";

        public Registro()
        {
            Console.WriteLine("RestS: "+url);
            cantidad = LeerArchivo(archivo_salida, FileShare.Write);
        }

        public void GuardarTemporales()
        {
            Console.Write("Guardando Temporales...("+reg_fails.Count+")");
            string targetFolder = Path.GetDirectoryName(Assembly.GetExecutingAssembly().GetModules()[0].FullyQualifiedName);

            string targetFileName = Path.Combine(targetFolder, registro_temp);

            var fs = new FileStream(targetFileName, FileMode.Append, FileAccess.Write, FileShare.Read);

            using (StreamWriter sw = new StreamWriter(fs))
            {
                foreach(String s in reg_fails){
                  sw.WriteLine(s);
                }

                sw.Flush();

                sw.Close();

            }
         
            fs.Dispose();
            reg_fails.Clear();
            Console.WriteLine("OK");

        }

        public void LimpiarRegistrosTemp() {

           // Console.Write("Clear Temp...");

            string targetFolder = Path.GetDirectoryName(Assembly.GetExecutingAssembly().GetModules()[0].FullyQualifiedName);

            string targetFileName = Path.Combine(targetFolder, registro_temp);

            if (File.Exists(targetFileName)) { File.Delete(targetFileName); }
            else Console.WriteLine("Error: S/R");
        }

        public Boolean SendToApi(String data)
        {
            try
            {
                // Create a request using a URL that can receive a post.   
                WebRequest request = WebRequest.Create(url);
                // Set the Method property of the request to POST.  
                request.Method = "POST";
                // Create POST data and convert it to a byte array.  
                string postData = new Data(data).ToString();

                byte[] byteArray = Encoding.UTF8.GetBytes(postData);

                // Set the ContentType property of the WebRequest.  
                request.ContentType = "application/json; charset=UTF-8";
                ((HttpWebRequest)request).Accept = "application/json";
                // Set the ContentLength property of the WebRequest.  
                request.ContentLength = byteArray.Length;
                // Get the request stream.  
                Stream dataStream = request.GetRequestStream();
                // Write the data to the request stream.  
                dataStream.Write(byteArray, 0, byteArray.Length);
                // Close the Stream object.  
                dataStream.Close();
                // Get the response.  
                WebResponse response = request.GetResponse();
                //Responce Status
                int cod = (int)((HttpWebResponse)response).StatusCode;
                // Display the status.  
                //Console.WriteLine(((HttpWebResponse)response).StatusDescription);
                // Get the stream containing content returned by the server.  
                dataStream = response.GetResponseStream();
                // Open the stream using a StreamReader for easy access.  
                StreamReader reader = new StreamReader(dataStream);
                // Read the content.  
                string responseFromServer = reader.ReadToEnd();
                // Display the content.  
                //Console.WriteLine(responseFromServer);
               
                // Clean up the streams.  
                reader.Close();
                dataStream.Close();
                response.Close();

                return cod < 300;
            }
            catch (Exception E) {
                Console.WriteLine(E.Message.ToString());
                return false;
            }
        }

        public void ReenviarRegistrosTemporales(){

            Console.Write("Reenvio Temporales...");
            
            if (LeerArchivo(registro_temp, FileShare.Read) == 0) { Console.WriteLine("S/R"); return; }


            Console.WriteLine("(" + registros.Count + ")");

            LimpiarRegistrosTemp();


            //INICIALIZAR LISTA DE ENVIOS FALLIDOS
            reg_fails.Clear();

            foreach (String s in registros)
            {
                if (!backup && SendToApi(s))
                {
                    Console.WriteLine("R): " + s + " >> Reenviado");
                }
                else
                {
                    Console.WriteLine("R): " + s + "  >> Fallo");
                    reg_fails.Add(s);
                    backup = true;
                }
            }
            if (reg_fails.Count > 0) GuardarTemporales();
        }

        public bool Actualizar() {

            //ENVIAR ARCHIVOS PENDIENTES
            ReenviarRegistrosTemporales();

            int aux = LeerArchivo(archivo_salida, FileShare.ReadWrite);
            if (this.cantidad >= aux) return false;
            {
                if (registros.Count > 0 && cantidad < registros.Count) registros.RemoveRange(0, this.cantidad); 
                else {
                    Console.WriteLine("Error");
                    return false;
                }

                //INICIALIZAR LISTA DE ENVIOS FALLIDOS
                reg_fails.Clear();

                foreach (String s in registros)
                {
                    if (!backup && SendToApi(s))
                    {
                        Console.WriteLine("R): " + s + " >> Enviado");
                    }
                    else
                    {
                        Console.WriteLine("R): " + s + "  >> Fallo");
                        reg_fails.Add(s);
                        backup = true;
                    }
                }
                if (reg_fails.Count > 0) GuardarTemporales();
                this.cantidad = aux;
                return true;
            }
            
        }
     
        public int LeerArchivo(String archivo,FileShare fso) {
        //    Console.Write("Read "+archivo+"...");
            registros.Clear();

            try
            {
                string targetFolder = Path.GetDirectoryName(Assembly.GetExecutingAssembly().GetModules()[0].FullyQualifiedName);

                string targetFileName = Path.Combine(targetFolder, archivo);

                if (!File.Exists(targetFileName))
                {
                    Console.WriteLine("not exist");
                    if (archivo.CompareTo(archivo_salida) == 0) this.cantidad=0 ; return 0;
                }

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
               Console.WriteLine("Error:" + E.Message.ToString());
               this.cantidad = 0;
               return 0;
            }

            //Console.WriteLine("("+registros.Count+")");
            return registros.Count;

        }

        public void Mostrar_Registros() {
            Console.WriteLine("******** REGISTROS SALIDA *************");
            foreach (String s in registros) {
                Console.WriteLine(s);
            }
        }

        public bool ExisteArchivo(String nombre) {

            string targetFolder = Path.GetDirectoryName(Assembly.GetExecutingAssembly().GetModules()[0].FullyQualifiedName);

            string targetFileName = Path.Combine(targetFolder, nombre);

            return File.Exists(targetFileName);
        }

    }
    
}
