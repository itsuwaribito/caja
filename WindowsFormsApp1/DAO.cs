using System;
using System.IO;
using OfficeOpenXml;
using System.Collections.Generic;
using System.Dynamic;
using Newtonsoft.Json;

namespace WindowsFormsApp1
{
    class DAO
    {
        
        private string  baseURL = "C:\\base de datos\\Catalogo Truper 2024.xlsx";
        private string  baseDB = "C:\\base de datos\\DB.json";

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
            // Verificar si el archivo existe
            if (!File.Exists(baseURL))
            {
                Console.WriteLine("El archivo Excel no existe.");
                return;
            }
            // Crear una nueva instancia de ExcelPackage
            using (var package = new ExcelPackage(new FileInfo(baseURL)))
            {
                // Obtener la hoja de trabajo (worksheet) por su nombre
                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                ExcelWorksheet worksheet = package.Workbook.Worksheets["cat"];

                // Obtener el número de filas y columnas en la hoja de trabajo
                int rowCount = worksheet.Dimension.Rows;
                int colCount = worksheet.Dimension.Columns;

                // Crear una lista para almacenar los datos
                var data = new System.Collections.Generic.List<ExpandoObject>();
                Console.WriteLine("Filas del Excel: " + rowCount);
                // Iterar a través de las filas y columnas y almacenar los datos en la lista
                for (int row = 2; row <= rowCount; row++)
                {
                    if (worksheet.Cells[row, 1].Text == "")
                    {
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

                // Escribir la cadena JSON en un archivo
                File.WriteAllText(baseDB, jsonString);

                Console.WriteLine($"El contenido del archivo Excel se ha guardado en un archivo JSON en: {baseDB}");
            }
        }
    }
}
