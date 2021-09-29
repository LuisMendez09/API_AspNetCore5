using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace netCoreApi.Entidades
{
    public class PeliculasActores
    {
        public int PeliculaId { get; set; }
        public int ActorId { get; set; }
        public Peliculas Pelicula { get; set; }
        public Actores Actor { get; set; }
        [StringLength(maximumLength:100)]
        public string Personaje { get; set; }
        public int Orden { get; set; }

    }

}
