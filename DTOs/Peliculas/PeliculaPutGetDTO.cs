using netCoreApi.DTOs.Cines;
using netCoreApi.DTOs.Generos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace netCoreApi.DTOs.Peliculas
{
    public class PeliculaPutGetDTO
    {
        public PeliculaDTO pelicula { get; set; }
        public List<GeneroDTO> GenerosSeleccionados { get; set; }
        public List<GeneroDTO> GenerosNoSeleccionados { get; set; }
        public List<CineDTO> CinesSeleccionados  { get; set; }
        public List<CineDTO> CinesNoSeleccionados { get; set; }
        public List<PeliculaActorDTO> Actores { get; set; }
    }
}
