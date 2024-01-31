using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsFormsApp1
{
    class Articulo
    {
        public string Nombre { get; set; }
        public decimal Precio { get; set; }
        public int Cantidad { get; set; } = 1;

        public decimal Importe => Precio * Cantidad;

        public void FillEmpty()
        {
            Nombre = string.Empty;
            Precio = 0;
            Cantidad = 1;
        }
        public void Fill(Dictionary<string, string>  data)
        {
            if (data == null)
            {
                FillEmpty();
                return;
            }

            Nombre = data["Descripcion"];
            Precio = decimal.Parse(data["Precio"]);
            Cantidad = 1;
        }

        public object Clone()
        {
            return new Articulo
            {
                Nombre = this.Nombre,
                Precio = this.Precio,
                Cantidad = this.Cantidad
            };
        }
    }
}
