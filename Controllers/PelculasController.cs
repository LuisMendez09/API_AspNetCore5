using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using netCoreApi.DTOs.Cines;
using netCoreApi.DTOs.Generos;
using netCoreApi.DTOs.Peliculas;
using netCoreApi.Entidades;
using netCoreApi.Utilidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace netCoreApi.Controllers
{
    [ApiController]
    [Route("api/peliculas")]
    public class PelculasController : ControllerBase
    {
        private readonly ApplicationDbContext context;
        private readonly IMapper mapper;
        private readonly IAlmacenadorArchivos almacenadorArchivos;
        private readonly string contenedor = "pelicula";

        public PelculasController(ApplicationDbContext context,
            IMapper mapper,
            IAlmacenadorArchivos almacenadorArchivos)
        {
            this.context = context;
            this.mapper = mapper;
            this.almacenadorArchivos = almacenadorArchivos;
        }
        [HttpGet]
        public async Task<ActionResult<LandingPageDTO>> Get()
        {
            var top = 6;
            var hoy = DateTime.Today;

            var proximosEstrenos = await context.Peliculas
                .Where(x => x.FechaLanzamiento > hoy)
                .OrderBy(x => x.FechaLanzamiento)
                .Take(top)
                .ToListAsync();

            var enCines = await context.Peliculas
                .Where(x => x.Encines)
                .OrderBy(x => x.FechaLanzamiento)
                .Take(top)
                .ToListAsync();

            var resultado = new LandingPageDTO();
            resultado.ProximosEstrenos = mapper.Map<List<PeliculaDTO>>(proximosEstrenos);
            resultado.EnCines = mapper.Map<List<PeliculaDTO>>(enCines);

            return resultado;

        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<PeliculaDTO>> Get(int id)
        {
            var pelicula = await context.Peliculas
                .Include(x => x.PeliculasGeneros).ThenInclude(x => x.Genero)
                .Include(x => x.PeliculasActores).ThenInclude(x => x.Actor)
                .Include(x => x.PeliculasCines).ThenInclude(x => x.Cine)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (pelicula == null)
                return NotFound();

            var dto = mapper.Map<PeliculaDTO>(pelicula);
            dto.Actores = dto.Actores.OrderBy(x => x.Orden).ToList();
            return dto;

        }

        [HttpGet("filtrar")]
        public async Task<ActionResult<List<PeliculaDTO>>> Filtrar([FromQuery] PeliculaFiltrarDTO peliculaFiltrarDTO)
        {
            var peliculaQueryable = context.Peliculas.AsQueryable();
            if (!string.IsNullOrEmpty(peliculaFiltrarDTO.Titulo))
            {
                peliculaQueryable = peliculaQueryable.Where(x => x.Titulo.Contains(peliculaFiltrarDTO.Titulo));
            }

            if (peliculaFiltrarDTO.EnCines)
            {
                peliculaQueryable = peliculaQueryable.Where(x => x.Encines);
            }

            if (peliculaFiltrarDTO.ProximosEstrenos)
            {
                peliculaQueryable = peliculaQueryable.Where(x => x.FechaLanzamiento> DateTime.Today);
            }

            if(peliculaFiltrarDTO.GeneroId != 0)
            {
                peliculaQueryable = peliculaQueryable
                    .Where(x => x.PeliculasGeneros.Select(x => x.GeneroId)
                    .Contains(peliculaFiltrarDTO.GeneroId));
            }

            await HttpContext.InsertarParametrosPaginacionEnCabecera(peliculaQueryable);
            var peliculas = await peliculaQueryable.Paginar(peliculaFiltrarDTO.paginacionDTO).ToListAsync();
            return mapper.Map<List<PeliculaDTO>>(peliculas);

        }

        [HttpPost]
        public async Task<ActionResult<int>> Post([FromForm] PeliculaCreacionDTO peliculaCreacionDTO)
        {
            var pelicula = mapper.Map<Peliculas>(peliculaCreacionDTO);
            if (peliculaCreacionDTO.Poster != null)
            {
                pelicula.Poster =
                    await almacenadorArchivos.GuardarArchivo(contenedor, peliculaCreacionDTO.Poster);
            }

            EscribirOrdenActores(pelicula);

            context.Add(pelicula);
            await context.SaveChangesAsync();
            
            return pelicula.Id;
        }

        [HttpGet("PostGet")]
        public async Task<ActionResult<PeliculaPostGetDTO>> PostGet()
        {
            var cines = await context.Cines.ToListAsync();
            var generos = await context.Generos.ToListAsync();

            var cinesDTO = mapper.Map<List<CineDTO>>(cines);
            var generosDTO = mapper.Map<List<GeneroDTO>>(generos);

            return new PeliculaPostGetDTO() { Cines = cinesDTO, Generos = generosDTO };
        }

        [HttpGet("PutGet/{id:int}")]
        public async Task<ActionResult<PeliculaPutGetDTO>> PutGet(int id)
        {
            var peliculaActionResult = await Get(id);
            if(peliculaActionResult.Result is NotFoundResult) { return NotFound(); }

            var pelicula = peliculaActionResult.Value;

            var generosSeleccionadosId = pelicula.Generos.Select(x => x.Id).ToList();
            var generosNoSeleccionados = await context.Generos
                .Where(x => !generosSeleccionadosId.Contains(x.Id))
                .ToListAsync();

            var cinesSeleccionadosId = pelicula.Cines.Select(x => x.Id).ToList();
            var cinesNoSeleccionados = await context.Cines
                .Where(x => !cinesSeleccionadosId.Contains(x.Id))
                .ToListAsync();

            var generosNoSeleccionadosDTO = mapper.Map<List<GeneroDTO>>(generosNoSeleccionados);
            var cinesNoSeleccionadosDTO = mapper.Map<List<CineDTO>>(cinesNoSeleccionados);

            var respuesta = new PeliculaPutGetDTO();
            respuesta.pelicula = pelicula;
            respuesta.CinesSeleccionados = pelicula.Cines;
            respuesta.CinesNoSeleccionados = cinesNoSeleccionadosDTO;
            respuesta.GenerosSeleccionados = pelicula.Generos;
            respuesta.GenerosNoSeleccionados = generosNoSeleccionadosDTO;
            respuesta.Actores = pelicula.Actores;

            return respuesta;
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult> Put(int id,[FromForm] PeliculaCreacionDTO peliculaCreacionDTO)
        {
            var pelicula = await context.Peliculas
                .Include(x => x.PeliculasActores)
                .Include(x => x.PeliculasCines)
                .Include(x => x.PeliculasGeneros)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (pelicula == null)
                return NotFound();

            pelicula = mapper.Map(peliculaCreacionDTO, pelicula);//actualiza las propiedades diferentes

            if(peliculaCreacionDTO.Poster != null)
            {
                pelicula.Poster = await almacenadorArchivos
                    .editarArhivo(contenedor, peliculaCreacionDTO.Poster, pelicula.Poster);
            }

            EscribirOrdenActores(pelicula);

            await context.SaveChangesAsync();
            return NoContent();
        }
        [HttpDelete("{id:int}")]
        public async Task<ActionResult> Delete(int id)
        {
            var pelicula = await context.Peliculas.FirstOrDefaultAsync(x => x.Id == id);
            if (pelicula == null)
                return NotFound();

            context.Remove(pelicula);
            await context.SaveChangesAsync();

            await almacenadorArchivos.borrarArhivo(pelicula.Poster, contenedor);
            return NoContent();
        }

        private void EscribirOrdenActores(Peliculas peliculas)
        {
            if (peliculas.PeliculasActores != null)
            {
                for(int i = 0; i < peliculas.PeliculasActores.Count; i++)
                {
                    peliculas.PeliculasActores[i].Orden = i;
                }
            }
        }
    }
}
