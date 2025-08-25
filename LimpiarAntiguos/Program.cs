using System;
using System.IO;
using System.Collections.Generic;

namespace LimpiarAntiguos
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("");
            Console.WriteLine("******************************¡Bienvenido!******************************");
            Console.WriteLine("");
            //Ruta de la carpeta a limpiar.
            string folder = @"C:\NombreCarpetaArchivo-dot-ini\";
            Console.WriteLine("");
            int dias; //Determinamos los días de antigüedad. Si un archivo cumple más de ese tiempo, será eliminado.
            string valorDias; //Valor extraido del documento "config.ini".

            try
            {
                var listaArchivos = new List<string>(); //Arreglo donde se almacenarán los ficheros con su ruta y nombre
                int i = 0; //Contador de archivos que cumplan con los criterios de búsqueda

                // Leer el archivo de texto con las rutas de las carpetas
                string[] carpetas = File.ReadAllLines(Path.Combine(folder, "config.ini")); //Archivo donde se almacenarán las rutas de archivos a eliminar

                if (carpetas.Length > 0 && carpetas[0].StartsWith("dias="))
                {
                    valorDias = carpetas[0].Split('=')[1];
                    if (!int.TryParse(valorDias, out _) || int.Parse(valorDias)< 0) //El comando "out _" nos sirve para descartar un valor temporal.
                    {
                        Console.WriteLine("Error: El valor de 'dias' en config.ini no es válido. Favor, ingrese un número entero positivo o cero.");
                        Console.WriteLine("Presione cualquier tecla para salir...");
                        Console.ReadKey();
                        return;
                    }
                    else
                    {
                        dias = int.Parse(valorDias);
                        if (dias == 0)
                        {
                            Console.WriteLine($"Advertencia!!!\n\nTodos los archivos de las carpetas listadas en la configuración serán eliminados permanentemente.\n");
                        }
                        else
                        {
                            Console.WriteLine($"Advertencia!!!\n\nLos archivos con una fecha de modificación superior a {dias} días, tomando como referencia la fecha de modificación del archivo más reciente en cada carpeta, serán eliminados permanentemente.\n");
                        }
                    }
                }
                else
                {
                    Console.WriteLine($"No se encontró la configuración de 'dias'.");
                    Console.WriteLine("Presione cualquier tecla para salir...");
                    Console.ReadKey();
                    return;
                }

                foreach (string carpeta in carpetas)
                {
                    // Verificar si el directorio existe
                    if (!Directory.Exists(carpeta))
                    {
                        if (carpeta.StartsWith("dias=", StringComparison.OrdinalIgnoreCase) || string.IsNullOrWhiteSpace(carpeta))
                        {
                            continue;
                        }
                        else
                        {
                            Console.WriteLine($"La carpeta {carpeta} no existe.");
                            continue;
                        }
                    }

                    // Arreglo donde daremos la instrucción para localizar archivos hasta en subcarpetas
                    string[] archivos = Directory.GetFiles(carpeta, "*.*", SearchOption.AllDirectories);

                    DateTime fechaMasReciente = DateTime.MinValue.Date; //Obtenemos la fecha actual

                    // Iterar sobre todos los archivos para encontrar la fecha de modificación más reciente
                    foreach (string archivo in archivos)
                    {
                        DateTime fechaModificado = File.GetLastWriteTime(archivo).Date; //Obtenemos la fecha de modificación del archivo

                        if (fechaModificado > fechaMasReciente)
                        {
                            fechaMasReciente = fechaModificado; //Actualiza la fecha más reciente de modificación
                        }
                    }

                    DateTime fechaLimite = fechaMasReciente.AddDays(-dias); //Establece la fecha límite hacia atrás según la variable 'dias'

                    // Iterar sobre los archivos nuevamente para identificar aquellos dentro del rango de fecha límite
                    foreach (string archivo in archivos)
                    {
                        DateTime fechaModificado = File.GetLastWriteTime(archivo).Date; // Obtén la fecha de modificación del archivo

                        // Calcula la diferencia en días entre la fecha límite y la fecha de modificación del archivo
                        int diferenciaDias = (fechaLimite - fechaModificado).Days;

                        // Si la diferencia en días es igual a cero, significa que la fecha de modificación está dentro del rango
                        if (diferenciaDias >= 0)  //if (diferenciaDias >= 0) //Para borrar archivos más antiguos de esa fecha
                        {
                            listaArchivos.Add(archivo); // Agrega el nombre del archivo a la lista
                            i++;
                        }
                    }
                }
                if (listaArchivos.Count > 0)
                {
                    Console.WriteLine("Los siguientes archivos serán eliminados: ");
                    foreach (string archivo in listaArchivos)
                    {
                        Console.WriteLine(archivo);
                    }
                    Console.WriteLine($"Total archivos: {i}"); //El símbolo de dólar al principio nos sirve para poder visualizar el contenido de otras variables por medio de llaves.
                    Console.WriteLine("");
                    int j = 0;
                    foreach (string archivo in listaArchivos)
                    {
                        File.Delete(archivo);
                        Console.WriteLine($"Archivo eliminado: {archivo}");
                        j++;
                    }
                    Console.WriteLine("");
                    Console.WriteLine($"¡Limpieza completada! Archivos eliminados: {j}");
                }
                else
                {
                    Console.WriteLine("No hay archivos para eliminar.");
                }

            }
            catch (Exception ex)
            {

                Console.WriteLine($"Ocurrió un error: {ex.Message}");
            }
            Console.WriteLine("");
            //Comentar las siguientes dos lineas para evitar pausas. Solo si fuera necesario.
            Console.Write("Presione cualquier tecla para salir...");
            Console.ReadKey();
        }
    }
}