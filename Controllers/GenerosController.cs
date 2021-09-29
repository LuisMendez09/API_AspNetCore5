using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using netCoreApi.DTOs;
using netCoreApi.DTOs.Generos;
using netCoreApi.Entidades;
using netCoreApi.Filtros;
using netCoreApi.Utilidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace netCoreApi.Controllers
{
    [Route("api/generos")]//endpoint
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "EsAdmin")]
    public class GenerosController : ControllerBase
    {
        private readonly ILogger<GenerosController> logger;
        private readonly IMapper mapper;
        private readonly ApplicationDbContext context;

        public GenerosController(ILogger<GenerosController> logger,
            IMapper mapper,
            ApplicationDbContext context)
        {
            this.logger = logger;
            this.mapper = mapper;
            this.context = context;
        }

        [HttpGet]//api/generos
        public async Task<ActionResult<List<GeneroDTO>>> Get([FromQuery] PaginacionDTO paginacionDTO)
        {
            var queryable = context.Generos.AsQueryable();
            await HttpContext.InsertarParametrosPaginacionEnCabecera(queryable);
            var generos = await queryable.OrderBy(x => x.Nombre).Paginar(paginacionDTO).ToListAsync();
            return mapper.Map<List<GeneroDTO>>(generos);
        }

        [HttpGet("todos")]//api/generos
        [AllowAnonymous]
        public async Task<ActionResult<List<GeneroDTO>>> Todos([FromQuery] PaginacionDTO paginacionDTO)
        {
            var generos = await context.Generos.OrderBy(x=>x.Nombre).ToListAsync();
            return mapper.Map<List<GeneroDTO>>(generos);
        }

        //pasar varias variables=> [accion ("{variable}/{otra varaible}")]
        //valor por defecto => nombre='valor por defecto'
        //tipo de datos especifico=> variable:tipo de dato        
        [HttpGet ("{id:int}")]//api/generos/1
        public async Task<ActionResult<GeneroDTO>> Get(int id)
        {
            var genero = await context.Generos.FirstOrDefaultAsync(x => x.Id == id);
            if (genero == null)
                return NotFound();

            return mapper.Map<GeneroDTO>(genero);
        }

        [HttpPost]
        public async Task<ActionResult> Post([FromBody] GeneroCreacionDTO generoCreacionDTO)
        {
            var genero = mapper.Map<Generos>(generoCreacionDTO);
            context.Add(genero);
            await context.SaveChangesAsync();
            return NoContent();
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult> Put(int id,[FromBody] GeneroCreacionDTO generoCreacionDTO)
        {
            var genero = await context.Generos.FirstOrDefaultAsync(x => x.Id == id);

            if (genero == null)
                return NotFound();

            genero = mapper.Map(generoCreacionDTO, genero);//actualiza las propiedades diferentes
            await context.SaveChangesAsync();//actualiza en la base de datos
            return NoContent();
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult> Delete(int id)
        {
            var existe = await context.Generos.AnyAsync(x => x.Id == id);
            if (!existe)
                return NotFound();

            context.Remove(new Generos() { Id = id });
            await context.SaveChangesAsync();
            return NoContent();
        }
    }
}
