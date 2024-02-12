using System;
using System.Drawing;
using System.Windows.Forms;
using System.Drawing.Printing;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace WindowsFormsApp1
{
    public partial class Form1 : Form
    {
        private Articulo articulo = new Articulo();
        private List<Articulo>  listaVenta = new List<Articulo>();
        private string inputBuffer = string.Empty; // Variable para almacenar la entrada acumulada del escaner
        private decimal total = 0.0m;

        public Form1()
        {
            InitializeComponent();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            /*
            PrinterSettings.StringCollection impresoras = PrinterSettings.InstalledPrinters;

            // Imprime la lista de impresoras instaladas
            Console.WriteLine("Impresoras instaladas:");
            string nombreImpresora = "XP-58 (copy 2)";
            foreach (string impresora in impresoras)
            {
                // Verificar si la cadena comienza con "px-58"
                bool comienzaConPx58 = impresora.StartsWith("XP-58");

                // Mostrar el resultado
                if (comienzaConPx58)
                {
                    nombreImpresora = impresora;
                    break;
                    //MessageBox.Show($"Impresora seleccionada: \"{nombreImpresora }\" ", "Alerta", MessageBoxButtons.OK);
                }
            }
            */

            // Crear el diálogo de impresora
            PrintDialog printDialog = new PrintDialog();
            string nombreImpresora = "XP-58";

            // Mostrar el diálogo de impresora
            if (printDialog.ShowDialog() == DialogResult.OK)
            {
                // Obtener la impresora seleccionada
                PrinterSettings printerSettings = printDialog.PrinterSettings;
                nombreImpresora = printerSettings.PrinterName;

                // Realizar acciones con la impresora seleccionada
                Console.WriteLine("Impresora seleccionada: " + nombreImpresora);
            }

            // Puedes seleccionar la impresora por su nombre


            // Configura la impresora seleccionada
            PrinterSettings ps = new PrinterSettings();
            ps.PrinterName = nombreImpresora;

            PrintDocument pd = new PrintDocument();
            pd.PrintPage += new PrintPageEventHandler(ImprimirTicket);
            pd.PrinterSettings = ps;
            pd.Print();
            ((Control)sender).Parent.Focus();
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
                                     "UDS   PRECIO \t \t SUBTOTAL\n" +
                                     "-----------------------------------------------\n";
                                     // "Rollo de malla hexagonal 45mx1.2m calib\n" +
                                     // "Desarmador dieléctrico de cruz 1/4' x 4', Truper\n" +
                                     // "10  X  $1175 \t \t $11750\n" +
                                     

            foreach (Articulo item in listaVenta)
            {
                // item.Nombre, item.Cantidad, item.Precio, item.Importe
                contenidoTicket += item.Nombre + "\n";
                contenidoTicket += item.Cantidad.ToString() + "  X  $" + item.Precio.ToString() + " \t \t $" +  item.Importe.ToString() + "\n";
            }

            contenidoTicket += "-----------------------------------------------\n" +

                                     "Total: $" + total.ToString() + "\n\n\n\n\n\n" +

                                     "Gracias por su compra\n\n\n" +
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

        private void Buscar_Click(object sender, EventArgs e)
        {
            DAO archivo = new DAO();
            Dictionary<string, string> response = archivo.Show(textBox1.Text);
            articulo.Fill(response);
            ((Control)sender).Parent.Focus();
        }
        private void AgregarATabla_Click(object sender, EventArgs e)
        {
            if(
                textBox2.Text != ""
                && textBox3.Text != ""
                && textBox4.Text != ""
            )
            {
                AgregaArticuloATabla();
                textBox2.Text = string.Empty;
                textBox3.Text = "1";
                textBox4.Text = string.Empty;
            }
            ((Control)sender).Parent.Focus();
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            articulo.Nombre = textBox2.Text;
        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {
            if (decimal.TryParse(textBox3.Text, out _))
            {
                articulo.Cantidad = int.Parse(textBox3.Text);
            }
        }

        private void textBox3_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Permite los dígitos y la tecla de retroceso
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void textBox4_TextChanged(object sender, EventArgs e)
        {
            if (decimal.TryParse(textBox4.Text, out _))
            {
                articulo.Precio = decimal.Parse(textBox4.Text);
            }
        }

        private void textBox4_TextChanged(object sender, KeyPressEventArgs e)
        {
            // Permite los dígitos, el punto decimal y la tecla de retroceso
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && (e.KeyChar != '.'))
            {
                e.Handled = true;
            }

            // Permite solo un punto decimal
            if ((e.KeyChar == '.') && ((sender as TextBox).Text.IndexOf('.') > -1))
            {
                e.Handled = true;
            }
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
                
                if (textBox2.Text.Length > 0)
                {
                    AgregaArticuloATabla();
                    textBox3.Text = "1";
                }
                // Procesar la cadena acumulada al presionar Enter
                DAO DB_archivo = new DAO();
                Dictionary<string, string>  fila = DB_archivo.Show(inputBuffer);
                if(fila == null)
                {
                    return;
                }
                
                articulo.Fill(fila);
                textBox2.Text = articulo.Nombre;
                textBox3.Text = articulo.Cantidad.ToString();
                textBox4.Text = articulo.Precio.ToString();

                // Limpiar el búfer después de procesar
                inputBuffer = string.Empty;
                textBox1.Text = "";
            }
        }

        private void AgregaArticuloATabla()
        {
            listaVenta.Add((Articulo)articulo.Clone());
            //dataGridView1.Rows.Add(articulo.Nombre, articulo.Cantidad, articulo.Precio, articulo.Importe);
            RedibujaTabla();
        }

        private void RedibujaTabla()
        {
            total = 0;
            dataGridView1.Rows.Clear();
            foreach (Articulo item in listaVenta)
            {
                dataGridView1.Rows.Add(item.Nombre, item.Cantidad, item.Precio, item.Importe);
                total += item.Importe;
                // dataGridView1.Rows.Add("Nombre", "Cantidad", "Precio", "Importe");
            }

            lbl_total.Text = total.ToString();

            // Verificar si hay al menos una fila en el DataGridView
            if (dataGridView1.Rows.Count > 0)
            {
                // Seleccionar la última fila
                int lastIndex = dataGridView1.Rows.Count - 1;
                // Asegurarse de que la última fila esté visible
                dataGridView1.FirstDisplayedScrollingRowIndex = lastIndex;

                // Seleccionar la última fila
                dataGridView1.Rows[lastIndex].Selected = true;
            }

            CalculaCambio();
        }
        private void Borrar_Click(object sender, EventArgs e)
        {
            dataGridView1.Rows.Clear();
            listaVenta.Clear();
            articulo.FillEmpty();
            textBox2.Text = "";
            textBox3.Text = "1";
            textBox4.Text = "";
            total = 0.0m;
            lbl_total.Text = "0.00";
            CalculaCambio();
            ((Control)sender).Parent.Focus();
        }


        private void Pago_txt_TextChanged(object sender, EventArgs e)
        {
            CalculaCambio();
        }

        private void CalculaCambio()
        {
            if (decimal.TryParse(Pago_txt.Text, out _))
            {
                decimal pago = decimal.Parse(Pago_txt.Text);
                decimal cambio = pago - total;
                CambioLabel.Text = cambio.ToString();
            }
        }

        private void Pago_txt_KeyDown(object sender, KeyPressEventArgs e)
        {
            // Permite los dígitos, el punto decimal y la tecla de retroceso
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && (e.KeyChar != '.'))
            {
                e.Handled = true;
            }

            // Permite solo un punto decimal
            if ((e.KeyChar == '.') && ((sender as TextBox).Text.IndexOf('.') > -1))
            {
                e.Handled = true;
            }
        }

        private void dataGridView1_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            // Verificar si hay al menos una fila seleccionada
            if (dataGridView1.SelectedRows.Count > 0)
            {
                // Obtener la fila seleccionada
                DataGridViewRow selectedRow = dataGridView1.SelectedRows[0];

                DialogResult resultado = MessageBox.Show("¿Está seguro de que desea eliminar el ARTICULO seleccionado?", "Confirmar Eliminación", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                // Verificar la respuesta del usuario
                if (resultado == DialogResult.Yes)
                {
                    // Eliminar la fila de la tabla
                    dataGridView1.Rows.Remove(selectedRow);
                    listaVenta.RemoveAt(e.RowIndex);
                }
            }
            else
            {
                // Informar al usuario que no hay una fila seleccionada
                // MessageBox.Show("Seleccione una fila para eliminar.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            string ruta = "C:\\base de datos\\";
            try
            {
                // Verificar si el directorio existe
                if (!Directory.Exists(ruta))
                {
                    // Crear el directorio si no existe
                    Directory.CreateDirectory(ruta);
                    Console.WriteLine($"Directorio creado en: {ruta}");
                }
                Process.Start("explorer.exe", ruta);
            }
            catch (Exception ex)
            {
                // Manejar excepciones, si es necesario
                MessageBox.Show($"No se pudo crear el directorio: \"{ruta}\"", "Error", MessageBoxButtons.OK);
            }
            ((Control)sender).Parent.Focus();
        }

        private void button6_Click(object sender, EventArgs e)
        {
            DAO archivo = new DAO();
            archivo.Leer();
            ((Control)sender).Parent.Focus();
        }

        private void textBox1_KeyDown(object sender, KeyEventArgs e)
        {
            // Verificar si la tecla presionada es Enter
            if (e.KeyCode == Keys.Enter)
            {
                
                DAO archivo = new DAO();
                Dictionary<string, string> response = archivo.Show(textBox1.Text);
                articulo.Fill(response);
                
                ((Control)sender).Parent.Focus();
            }
        }
    }
}
