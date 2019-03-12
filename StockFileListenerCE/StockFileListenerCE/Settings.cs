using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Specialized;
using System.IO;
using System.Xml;
using System.Reflection;

namespace StockFileListener
{
    public class Settings
    {
        private static NameValueCollection m_settings;

       
        public Settings()
        {
            // Get the path of the settings file.
            string targetFolder = Path.GetDirectoryName(Assembly.GetExecutingAssembly().GetModules()[0].FullyQualifiedName);
            string m_settingsPath = Path.Combine(targetFolder, "settings.xml");

            if (!File.Exists(m_settingsPath))
                throw new FileNotFoundException(
                                  m_settingsPath + " could not be found.");

            System.Xml.XmlDocument xdoc = new XmlDocument();
            xdoc.Load(m_settingsPath);
            XmlElement root = xdoc.DocumentElement;
            System.Xml.XmlNodeList nodeList = root.ChildNodes.Item(0).ChildNodes;

            // Add settings to the NameValueCollection.
            m_settings = new NameValueCollection();
            m_settings.Add("id_lectora", nodeList.Item(0).Attributes["value"].Value);
            m_settings.Add("url_server", nodeList.Item(1).Attributes["value"].Value);
            m_settings.Add("nombre_archivo",nodeList.Item(2).Attributes["value"].Value);
            m_settings.Add("tiempo_espera",nodeList.Item(3).Attributes["value"].Value);
        }

        public static String getIdLectora() {
            return m_settings.Get("id_lectora");
        }

        public static int getTiempoEspera()
        {
            return Int32.Parse(m_settings.Get("tiempo_espera"));
        }

        public static String getNombreArchivo()
        {
            return m_settings.Get("nombre_archivo");
        }

        public static String getURL()
        {
            return m_settings.Get("url_server");
        }

    }
}
