using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FileListenerTest
{
    class Data
    {
        public static String idLec;
        public String idLectora;
        public String codBarra;
        public int cantidad;
        public String tipoMovimiento;
        public int ordenCorte;
        public Data(String data) {
            String[] aux = data.Split(',');
            this.idLectora = idLec;
            this.codBarra = aux[0];
            this.cantidad = Int16.Parse(aux[1]);
            this.tipoMovimiento = aux[2];
            this.ordenCorte = Int32.Parse(aux[3]);
        }
        public static void setIdLectora(String id){
            idLec = id;
        }
    }
}
