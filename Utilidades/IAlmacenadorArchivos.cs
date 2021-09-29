using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace netCoreApi.Utilidades
{
    public interface IAlmacenadorArchivos
    {
        Task borrarArhivo(string ruta, string contenedor);
        Task<string> editarArhivo(string contenedor, IFormFile archivo, string ruta);
        Task<string> GuardarArchivo(string contenedor, IFormFile archivo);
    }
}