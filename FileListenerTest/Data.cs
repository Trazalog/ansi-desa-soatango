using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FileListenerTest
{
    class Data
    {
        public String codigo;
        public int cantidad;
        public String tipo;
        public int orden_corte;
        public Data(String data) {
            String[] aux = data.Split(',');
            this.codigo = aux[0];
            this.cantidad = Int16.Parse(aux[1]);
            this.tipo = aux[2];
            this.orden_corte = Int32.Parse(aux[3]);
        }
    }
}
