using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace netCoreApi.Entidades.Repositorios
{
    public interface Irepositorio
    {
        void CrearGenero(Generos genero);
        Guid ObtenerGuid();
        Task<Generos> ObtenerPorId(int id);
        List<Generos> ObtenerTodosLosGeneros();
    }
}
