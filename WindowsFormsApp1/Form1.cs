using System;
using System.Drawing;
using System.Windows.Forms;
using System.Drawing.Printing;
using System.Collections.Generic;

namespace WindowsFormsApp1
{
    public partial class Form1 : Form
    {
        private Dictionary<string, string>  currentArticulo = null;
        private List<Dictionary<string, string>>  listaVenta = new List<Dictionary<string, string>>();
        private string inputBuffer = string.Empty; // Variable para almacenar la entrada acumulada del escaner

        public Form1()
        {
            InitializeComponent();
        }

        private void button2_Click222(object sender, EventArgs e)
        {
            DAO archivo = new DAO();
            archivo.Leer();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            PrinterSettings.StringCollection impresoras = PrinterSettings.InstalledPrinters;

            // Imprime la lista de impresoras instaladas
            Console.WriteLine("Impresoras instaladas:");
            foreach (string impresora in impresoras)
            {
                Console.WriteLine(impresora);
            }

            // Puedes seleccionar la impresora por su nombre
            string nombreImpresora = "XP-58";

            // Configura la impresora seleccionada
            PrinterSettings ps = new PrinterSettings();
            ps.PrinterName = nombreImpresora;

            PrintDocument pd = new PrintDocument();
            pd.PrintPage += new PrintPageEventHandler(ImprimirTicket);
            pd.PrinterSettings = ps;
            pd.Print();
        }

        private void ImprimirTicket(object sender, PrintPageEventArgs e)
        {
            string cabera = "========================================\n\n\n\n\n\n\n\n" +
                            "== FERRETERIA ALONDRA ==\n" +
                            "AV RECTOR HIDALGO #1329\n"+
                            "             COL CENTRO\n"+
                            "       LAZARO CARDENAS\n"+
                            "         MICHOACAN";
            // string ciudad = "COL CENTRO LAZARO CARDENAS\n"+
            //                "                  MICHOACAN";
            DateTime fechaEntrada = DateTime.Now;
            Console.WriteLine(fechaEntrada);
            // Contenido del ticket
            string contenidoTicket = "                 ***VENTA***\n\n" +
                                     "Cantidad de articulos: 10\n\n\n" +
                                     "UDS   PRECIO \t \t TOTAL\n" +
                                     "-----------------------------------------------\n" +
                                     // "Rollo de malla hexagonal 45mx1.2m calib\n" +
                                     "Desarmador dieléctrico de cruz 1/4' x 4', Truper\n" +
                                     "10  X  $1175 \t \t $11750\n" +
                                     "-----------------------------------------------\n" +
                                     
                                     "Total: $.00\n\n\n\n\n\n" +
                                     
                                     "Gracias por su compra\n\n\n"+
                                     fechaEntrada +
                                     "\n\n\n\n\n.\n\n\n" +
                                     "========================================\n";

            // Configuración de la fuente y posición de impresión
            Font fuente = new Font("Arial", 9);
            float y = 10.0F;
            float x = 10.0F;

            // Imprime el contenido del ticket
            e.Graphics.DrawString(cabera, fuente, Brushes.Black, x, y);
             y += 150;
            // fuente = new Font("Arial", 7);
            // e.Graphics.DrawString(ciudad, fuente, Brushes.Black, x, y);
            fuente = new Font("Arial", 7);
            y += 80;
            e.Graphics.DrawString(contenidoTicket, fuente, Brushes.Black, x, y);
        }

        private void button4_Click(object sender, EventArgs e)
        {
            DAO archivo = new DAO();
            currentArticulo = archivo.Show(textBox1.Text);
            if(currentArticulo != null)
            {
                currentArticulo["Cantidad"] = "0";
                textBox2.Text = currentArticulo["Descripcion"];
                textBox4.Text = currentArticulo["Precio"];
            }
                
        }

        private void button1_Click(object sender, EventArgs e)
        {
            AgregaArticulo();
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            SetCurrentArticulo("Descripcion", textBox2.Text);
            
        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {
            SetCurrentArticulo("Cantidad", textBox3.Text);
        }

        private void textBox4_TextChanged(object sender, EventArgs e)
        {
            SetCurrentArticulo("Precio", textBox4.Text);
        }

        private void refreshCaja()
        {
            listaVenta.ForEach(delegate (Dictionary<string, string> articulo) {
                string[] row = { articulo["Descripcion"], articulo["Cantidad"], articulo["Precio"] };
                dataGridView1.Rows.Add(row); 
            });
        }

        private void SetCurrentArticulo(string attr, string value)
        {
            if (currentArticulo == null)
            {
                currentArticulo = new Dictionary<string, string>();
                currentArticulo.Add("Cantidad", "1");
                currentArticulo.Add("Descripcion", "");
                currentArticulo.Add("Precio", "0");
            }
            currentArticulo[attr] = value;
        }

        private void Form1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == '\t')
            {
                // Vaciar el búfer al inicio del escaneo
                inputBuffer = string.Empty;
                
            }
            // Almacenar los caracteres hasta recibir Enter
            else if (e.KeyChar != '\r') // '\r' representa la tecla "Enter"
            {
                inputBuffer += e.KeyChar;
            }
            else
            {
                // Procesar la cadena acumulada al presionar Enter
                DAO archivo = new DAO();
                currentArticulo = archivo.Show(textBox1.Text);
                if (textBox2.Text != "")
                {
                    AgregaArticulo();
                    currentArticulo = null;
                }

                if (currentArticulo != null)
                {
                    currentArticulo["Cantidad"] = "1";
                    textBox2.Text = currentArticulo["Descripcion"];
                    textBox4.Text = currentArticulo["Precio"];
                }

                // Limpiar el búfer después de procesar
                inputBuffer = string.Empty;
                textBox1.Text = string.Empty;
            }
        }

        private void AgregaArticulo()
        {
            if (currentArticulo != null)
            {
                if (currentArticulo["Cantidad"] == ""
                    || currentArticulo["Precio"] == ""
                    || currentArticulo["Descripcion"] == ""
                )
                {
                    Console.WriteLine("Algo vacio");
                    return;
                }

                listaVenta.Add(currentArticulo);
            }

            refreshCaja();
        }
    }
}
