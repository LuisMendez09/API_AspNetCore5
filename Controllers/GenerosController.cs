using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Logging;
using netCoreApi.Entidades;
using netCoreApi.Entidades.Repositorios;
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
        private readonly Irepositorio reposittorio;
        private readonly WeatherForecastController weatherForecastController;
        private readonly ILogger<GenerosController> logger;

        public GenerosController(Irepositorio reposittorio,
            WeatherForecastController weatherForecastController,
            ILogger<GenerosController> logger)
        {
            this.reposittorio = reposittorio;
            this.weatherForecastController = weatherForecastController;
            this.logger = logger;
        }

        [HttpGet]//api/generos
        [HttpGet ("listado")]//api/generos/listado
        [HttpGet("/listadogeneros")]// /listadogeneros
        //[ResponseCache(Duration = 60)]
        [ServiceFilter(typeof(MiFiltroDeAccion))]
        public ActionResult<List<Generos>> Get()
        {
            logger.LogInformation("vamos a mostrar los generos");
            return reposittorio.ObtenerTodosLosGeneros();
        }

        [HttpGet("guid")]// api/generos/guid
        public ActionResult<Guid> GetGuid()
        {

            return Ok(new
            {
                GUID_GenerosController = reposittorio.ObtenerGuid(),
                GUID_WeatherForecastController = weatherForecastController.ObtenerGuidWeatherForecastController()
            });
        }

        //pasar varias variables=> [accion ("{variable}/{otra varaible}")]
        //valor por defecto => nombre='valor por defecto'
        //tipo de datos especifico=> variable:tipo de dato        
        [HttpGet ("{id:int}")]//api/generos/1/accion
        public async Task<ActionResult<Generos>> Get(int id,[FromHeader]string nombre)
        {
            logger.LogDebug($"obteniendo un genero por id {id}");
            var genero = await reposittorio.ObtenerPorId(id);
            if (genero == null)
            {
                throw new ApplicationException($"el genero de ID {id} no se encontro");
                logger.LogWarning($"no pudimos encontrar el genero de id {id}");
                return NotFound();
            }

            return genero;
        }

        [HttpPost]
        public ActionResult Post([FromBody] Generos genero)
        {
            reposittorio.CrearGenero(genero);
            return NoContent();
        }

        [HttpPut]
        public ActionResult Put()
        {
            return NoContent();
        }

        [HttpDelete]
        public ActionResult Delete()
        {
            return NoContent();
        }
    }
}
