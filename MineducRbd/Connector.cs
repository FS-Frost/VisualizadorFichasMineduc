using HtmlAgilityPack;
using System.Linq;

namespace MineducRbd {
    public static class Connector {
        public static School GetSchoolData(int rbd) {
            // Estructura que almacena los datos a encontrar.
            var school = new School() {
                Rbd = rbd,
                Estado = "Activo"
            };

            var url = @"http://www.mime.mineduc.cl/mime-web/mvc/mime/ficha?rbd=" + rbd;

            // Obtener el HTML;
            var response = Utils.GetHttpResponse(url);

            if (!response.Contains("Sostenedor")) {
                school.Estado = "Sin registro o cerrado";
                return school;
            }

            // Instanciar el HTML.
            var htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(response);

            // Obtener la tabla con los datos del colegio.
            var tables = htmlDoc.DocumentNode.SelectNodes("//table[contains(@class, 'tabla_form')]");
            var table = tables.First(t => t.InnerHtml.Contains("Sostenedor"));

            // Por cada fila...
            foreach (var row in table.SelectNodes("tr")) {
                var cells = row.SelectNodes("td");

                // Por cada celda...
                for (var i = 0; i < cells.Count; i += 2) {
                    var cell = cells[i];

                    // Se identifica la celda que contiene el nombre del dato, para luego almacenarlo.
                    if (cell.InnerHtml.Contains(":")) {
                        var text = Utils.FormatData(cell.InnerText);
                        var nextText = Utils.FormatData(cells[i + 1].InnerText);

                        switch (text) {
                            case "Dirección:":
                                school.Direccion = CheckCellValue(nextText);
                                break;
                            case "Mapa:":
                                school.Mapa = CheckCellValue(nextText);
                                break;
                            case "Comuna:":
                                school.Comuna = CheckCellValue(nextText);
                                break;
                            case "Teléfono:":
                                school.Telefono = CheckCellValue(nextText);
                                break;
                            case "E-mail contacto:":
                                school.Correo = CheckCellValue(nextText);
                                break;
                            case "Página web:":
                                school.PaginaWeb = CheckCellValue(nextText);
                                break;
                            case "Director(a):":
                                school.Director = CheckCellValue(nextText);
                                break;
                            case "Sostenedor:":
                                school.Sostenedor = CheckCellValue(nextText);
                                break;
                            default:
                                break;
                        }
                    }
                }
            }

            return school;
        }

        private static string CheckCellValue(string text) {
            return (text == string.Empty) ? "Sin información." : text;
        }
    }
}