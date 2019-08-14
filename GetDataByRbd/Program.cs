using HtmlAgilityPack;
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;

namespace GetDataByRbd {
    class Program {
        static void Main() {
            // URL a cargar.
            const int rbd = 25001;
            var url = @"http://www.mime.mineduc.cl/mime-web/mvc/mime/ficha?rbd=" + rbd;

            // Obtener el HTML;
            var response = GetHttpResponse(url);

            // Instanciar el HTML.
            var htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(response);

            // Obtener la tabla con los datos del colegio.
            var tables = htmlDoc.DocumentNode.SelectNodes("//table[contains(@class, 'tabla_form')]");
            var table = tables.First(t => t.InnerHtml.Contains("Sostenedor"));

            // Estructura que almacena los datos a encontrar.
            var school = new Colegio();

            // Por cada fila...
            foreach (var row in table.SelectNodes("tr")) {
                var cells = row.SelectNodes("td");

                // Por cada celda...
                for (var i = 0; i < cells.Count; i += 2) {
                    var cell = cells[i];

                    // Se identifica la celda que contiene el nombre del dato, para luego almacenarlo.
                    if (cell.InnerHtml.Contains(":")) {
                        var text = FormatData(cell.InnerText);
                        var nextText = FormatData(cells[i + 1].InnerText);

                        switch (text) {
                            case "Dirección:":
                                school.Direccion = nextText;
                                break;
                            case "Mapa:":
                                school.Mapa = nextText;
                                break;
                            case "Comuna:":
                                school.Comuna = nextText;
                                break;
                            case "Teléfono:":
                                school.Telefono = nextText;
                                break;
                            case "E-mail contacto:":
                                school.Correo = nextText;
                                break;
                            case "Página web:":
                                school.PaginaWeb = nextText;
                                break;
                            case "Director(a):":
                                school.Director = nextText;
                                break;
                            case "Sostenedor:":
                                school.Sostenedor = nextText;
                                break;
                            default:
                                break;
                        }
                    }
                }
            }

            var csvText = 
                "RBD;Dirección;Mapa;Comuna;Teléfono;Página Web;E-mail contacto;Director;Sostenedor\n" +
                string.Format(
                    "{0};{1};{2};{3};{4};{5};{6};{7};{8}",
                    rbd,
                    school.Direccion,
                    school.Mapa,
                    school.Comuna,
                    school.Telefono,
                    school.PaginaWeb,
                    school.Correo,
                    school.Director,
                    school.Sostenedor
                );
            File.WriteAllText("Datos Colegios.csv", csvText, Encoding.UTF8);
            Console.WriteLine(csvText);

            Console.WriteLine("\nPress any key to exit...");
            Console.ReadKey();
        }

        public static string GetHttpResponse(string url) {
            try {
                var request = (HttpWebRequest)WebRequest.Create(url);
                request.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/76.0.3809.100 Safari/537.36";
                var response = (HttpWebResponse)request.GetResponse();

                if (response.StatusCode == HttpStatusCode.OK) {
                    var responseStream = response.GetResponseStream();
                    StreamReader streamReader = null;

                    if (response.CharacterSet == null) {
                        streamReader = new StreamReader(responseStream);
                    }
                    else {
                        streamReader = new StreamReader(
                            responseStream, Encoding.GetEncoding(response.CharacterSet));
                    }

                    var data = streamReader.ReadToEnd();

                    response.Close();
                    streamReader.Close();

                    return data;
                }
            }
            catch (Exception) { }

            return null;
        }

        public static string FormatData(string data) {
            const string pattern = @"^\s+(.*)\s+$";
            var regex = new Regex(pattern);
            var match = regex.Match(data);


            if (match.Success) {
                var result = match.Groups[1].Value;
                result = result.Replace("\r\n", "").Replace("\r", "").Replace("\n", "");
                return result;
            }

            return data;
        }
    }
}
