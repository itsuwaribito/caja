using System;
using System.IO;
using OfficeOpenXml;
using System.Collections.Generic;
using System.Dynamic;
using System.Diagnostics;
using Newtonsoft.Json;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
    class DAO
    {
        private string nombreArchivo = "Catalogo Truper.xlsx";
        private string baseURL = "C:\\base de datos";
        private string baseDB = "C:\\base de datos\\DB.json";

        public Dictionary<string,string> Show(string codigo)
        {
            // Console.WriteLine(codigo);
            try
            {
                // Leer el contenido del archivo JSON
                string jsonContent = File.ReadAllText(baseDB);

                // Deserializar la cadena JSON a una lista de diccionarios
                List<Dictionary<string, string>> dataList = JsonConvert.DeserializeObject<List<Dictionary<string, string>>>(jsonContent);

                foreach (var rowData in dataList)
                {
                    if(rowData["Codigo"] == codigo || rowData["Barras"] == codigo)
                    {
                        return rowData;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al leer el archivo JSON: {ex.Message}");
                return null;
            }
            return null;
        }

        public void Leer()
        {
            List<int> filas = new List<int>();
            // Verificar si el directorio existe
            if (!Directory.Exists(baseURL))
            {
                // Crear el directorio si no existe
                Directory.CreateDirectory(baseURL);
                Console.WriteLine($"Directorio creado en: {baseURL}");
            }

            // Verificar si el archivo existe
            
            else if (!File.Exists($"{baseURL}\\{nombreArchivo}"))
            {
                MessageBox.Show($"El archivo con nombre \"{nombreArchivo}\" no existe", "Error", MessageBoxButtons.OK);
                return;
            }
            // Crear una nueva instancia de ExcelPackage
            using (var package = new ExcelPackage(new FileInfo($"{baseURL}\\{nombreArchivo}")))
            {
                // Obtener la hoja de trabajo (worksheet) por su nombre
                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                ExcelWorksheet worksheet = package.Workbook.Worksheets["catalogo"];
                int rowCount, colCount;
                
                try
                {
                    // Obtener el número de filas y columnas en la hoja de trabajo
                    rowCount = worksheet.Dimension.Rows;
                    colCount = worksheet.Dimension.Columns;
                    
                }catch (Exception ex)
                {
                    MessageBox.Show("No se encontro la hoja con el nombre: \"catalogo\"\nO no se puede leer la informacion correctamente", "Error", MessageBoxButtons.OK);
                    return;
                }

                // Crear una lista para almacenar los datos
                var data = new System.Collections.Generic.List<ExpandoObject>();
                Console.WriteLine("Filas del Excel: " + rowCount);
                // Iterar a través de las filas y columnas y almacenar los datos en la lista
                for (int row = 2; row <= rowCount; row++)
                {
                    if (worksheet.Cells[row, 1].Text == "")
                    {
                        filas.Add(row);
                        continue;
                    }
                    //var rowData = new System.Collections.Generic.Dictionary<string, object>();
                    // Crear un objeto dinámico usando ExpandoObject

                    dynamic dynamicObject = new ExpandoObject();

                    dynamicObject.Codigo = worksheet.Cells[row, 1].Text;
                    dynamicObject.Descripcion = worksheet.Cells[row, 3].Text;
                    dynamicObject.Unidad = worksheet.Cells[row, 6].Text;
                    dynamicObject.Precio = worksheet.Cells[row, 11].Text;
                    dynamicObject.Marca = worksheet.Cells[row, 13].Text;
                    dynamicObject.Barras = worksheet.Cells[row, 20].Text;


                    data.Add(dynamicObject);
                }

                // Convertir la lista a una cadena JSON
                string jsonString = JsonConvert.SerializeObject(data);
                
                if(filas.Count > 0)
                {
                    string listado = string.Join(", ", filas);
                    MessageBox.Show($"Se omitieron las filas {listado}", "Alerta", MessageBoxButtons.OK);

                }
                // Escribir la cadena JSON en un archivo
                File.WriteAllText(baseDB, jsonString);
                MessageBox.Show("La base de datos ha sido actualizada", "Exito", MessageBoxButtons.OK);
                Console.WriteLine($"El contenido del archivo Excel se ha guardado en un archivo JSON en: {baseDB}");
            }
        }
    }
}
