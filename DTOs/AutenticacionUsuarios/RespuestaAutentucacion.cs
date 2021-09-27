using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace netCoreApi.DTOs.AutenticacionUsuarios
{
    public class RespuestaAutentucacion
    {
        public string Token { get; set; }
        public DateTime Expiracion { get; set; }
    }
}
