using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Logging;
using netCoreApi.Entidades;
using netCoreApi.Filtros;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace netCoreApi.Controllers
{
    [Route("api/generos")]//endpoint
    [ApiController]
    //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class GenerosController:ControllerBase
    {
        private readonly ILogger<GenerosController> logger;

        public GenerosController(ILogger<GenerosController> logger)
        {
            this.logger = logger;
        }

        [HttpGet]//api/generos
        public ActionResult<List<Generos>> Get()
        {
            return new List<Generos> { new Generos { Id=1,nombre="Comedia"} };
        }

        //pasar varias variables=> [accion ("{variable}/{otra varaible}")]
        //valor por defecto => nombre='valor por defecto'
        //tipo de datos especifico=> variable:tipo de dato        
        [HttpGet ("{id:int}")]//api/generos/1/accion
        public async Task<ActionResult<Generos>> Get(int id)
        {
            throw new NotImplementedException();
        }

        [HttpPost]
        public ActionResult Post([FromBody] Generos genero)
        {
            throw new NotImplementedException();
        }

        [HttpPut]
        public ActionResult Put()
        {
            throw new NotImplementedException();
        }

        [HttpDelete]
        public ActionResult Delete()
        {
            throw new NotImplementedException();
        }
    }
}
