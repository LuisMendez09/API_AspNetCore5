using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace netCoreApi.Entidades.Repositorios
{

    public class RepositorioEnMomoria:Irepositorio
    {
        private List<Generos> _generos;

        public RepositorioEnMomoria(ILogger<RepositorioEnMomoria> logger)
        {
            _generos = new List<Generos>()
            {
                new Generos(){Id=1,nombre="Accion"},
                new Generos(){Id=2,nombre="Comedia"}
            };

            _guid = Guid.NewGuid();
        }

        public Guid _guid;

        public List<Generos> ObtenerTodosLosGeneros()
        {
            return _generos;
        }

        public async Task<Generos> ObtenerPorId(int id)
        {
            await Task.Delay(1);
            return _generos.FirstOrDefault(x => x.Id == id);
        }

        public Guid ObtenerGuid()
        {
            return _guid;
        }

        public void CrearGenero(Generos genero)
        {
            genero.Id = _generos.Count + 1;
            _generos.Add(genero);
        }
    }
}
