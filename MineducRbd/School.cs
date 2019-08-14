using System.ComponentModel;

namespace MineducRbd {
    public class School {
        [DisplayName("RBD")]
        public int Rbd { get; set; }

        [DisplayName("Dirección")]
        public string Direccion { get; set; }

        public string Mapa { get; set; }
        public string Comuna { get; set; }

        [DisplayName("Teléfono")]
        public string Telefono { get; set; }

        [DisplayName("Página Web")]
        public string PaginaWeb { get; set; }

        public string Correo { get; set; }
        public string Director { get; set; }
        public string Sostenedor { get; set; }
        public string Estado { get; set; }
    }
}
