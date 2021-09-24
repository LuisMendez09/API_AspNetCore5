using netCoreApi.DTOs.Cines;
using netCoreApi.DTOs.Generos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace netCoreApi.DTOs.Peliculas
{
    public class PeliculaPostGetDTO
    {
        public List<GeneroDTO> Generos { get; set; }
        public List<CineDTO> Cines { get; set; }

    }
}
