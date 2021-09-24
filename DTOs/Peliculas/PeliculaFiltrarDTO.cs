using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace netCoreApi.DTOs.Peliculas
{
    public class PeliculaFiltrarDTO
    {
        public int Pagina { get; set; }
        public int RecordsPorPagina { get; set; }
        public PaginacionDTO paginacionDTO
        {
            get {
                return new PaginacionDTO(){Pagina = Pagina,RecordsPorPagina = RecordsPorPagina};
            }
        }
        public string Titulo { get; set; }
        public int GeneroId { get; set; }
        public bool EnCines { get; set; }
        public bool ProximosEstrenos { get; set; }
    }
}
