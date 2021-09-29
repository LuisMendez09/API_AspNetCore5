using AutoMapper;
using Microsoft.AspNetCore.Identity;
using netCoreApi.DTOs.Actores;
using netCoreApi.DTOs.AutenticacionUsuarios;
using netCoreApi.DTOs.Cines;
using netCoreApi.DTOs.Generos;
using netCoreApi.DTOs.Peliculas;
using netCoreApi.Entidades;
using NetTopologySuite.Geometries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace netCoreApi.Utilidades
{
    public class AutoMapperProfiles:Profile
    {
        public AutoMapperProfiles(GeometryFactory geometryFactory)
        {
            CreateMap<Generos, GeneroDTO>().ReverseMap();
            CreateMap<GeneroCreacionDTO, Generos>();
            CreateMap<Actores, ActorDTO>().ReverseMap();
            CreateMap<ActorCreacionDTO, Actores>()
                .ForMember(x=>x.Foto,options => options.Ignore());
            CreateMap<CineCreacionDTO, Cines>()
                .ForMember(x => x.Ubicacion, x => x.MapFrom(dto =>
                     geometryFactory.CreatePoint(new Coordinate(dto.Longitud,dto.Latitud))));
            
            CreateMap<Cines, CineDTO>()
                .ForMember(x => x.Latitud, dto => dto.MapFrom(campo => campo.Ubicacion.Y))
                .ForMember(x => x.Longitud, dto => dto.MapFrom(campo => campo.Ubicacion.X));

            CreateMap<PeliculaCreacionDTO, Peliculas>()
                .ForMember(x => x.Poster, opciones => opciones.Ignore())
                .ForMember(x => x.PeliculasGeneros, 
                opciones => opciones.MapFrom(MappePeliculasgeneros))
                .ForMember(x => x.PeliculasCines, 
                opciones => opciones.MapFrom(MappePeliculasCines))
                .ForMember(x => x.PeliculasActores, 
                opciones => opciones.MapFrom(MappePeliculasActores));

            CreateMap<Peliculas, PeliculaDTO>()
                .ForMember(x => x.Generos, options => options.MapFrom(MapearPeliculaGeneros))
                .ForMember(x => x.Actores, options => options.MapFrom(MapearPeliculaActores))
                .ForMember(x => x.Cines, options => options.MapFrom(MapearPeliculaCines));

            CreateMap<IdentityUser, UsuarioDTO>();
        }

        private List<GeneroDTO> MapearPeliculaGeneros(Peliculas pelicula,PeliculaDTO peliculaDTO)
        {
            var resultado = new List<GeneroDTO>();

            if (pelicula.PeliculasGeneros != null) 
            {
                foreach (var genero in pelicula.PeliculasGeneros)
                {
                    resultado.Add(new GeneroDTO()
                    { Id = genero.GeneroId, Nombre = genero.Genero.Nombre });
                }
            }

            return resultado;
        }
        private List<PeliculaActorDTO> MapearPeliculaActores(Peliculas pelicula, PeliculaDTO peliculaDTO)
        {
            var resultado = new List<PeliculaActorDTO>();

            if (pelicula.PeliculasActores != null)
            {
                foreach (var actor in pelicula.PeliculasActores)
                {
                    resultado.Add(new PeliculaActorDTO()
                    {
                        Id = actor.ActorId,
                        Nombre = actor.Actor.Nombre,
                        Foto = actor.Actor.Foto,
                        Orden = actor.Orden,
                        Personaje = actor.Personaje
                    });
                }
            }
            return resultado;
        }

        private List<CineDTO> MapearPeliculaCines(Peliculas pelicula, PeliculaDTO peliculaDTO)
        {
            var resultado = new List<CineDTO>();

            if (pelicula.PeliculasCines != null)
            {
                foreach (var cine in pelicula.PeliculasCines)
                {
                    resultado.Add(new CineDTO()
                    {
                        Id = cine.CineId,
                        Nombre = cine.Cine.Nombre,
                        Latitud = cine.Cine.Ubicacion.Y,
                        Longitud = cine.Cine.Ubicacion.X
                    });
                }
            }

            return resultado;
        }

        private List<PeliculasActores> MappePeliculasActores(PeliculaCreacionDTO peliculaCreacionDTO
            , Peliculas peliculas)
        {
            var resultado = new List<PeliculasActores>();
            if (peliculaCreacionDTO.Actores == null) { return resultado; }

            foreach (var actor in peliculaCreacionDTO.Actores)
            {
                resultado.Add(new PeliculasActores() 
                            { ActorId = actor.Id, Personaje = actor.Personaje });
            }

            return resultado;
        }

        private List<PeliculasGeneros> MappePeliculasgeneros(PeliculaCreacionDTO peliculaCreacionDTO
            ,Peliculas peliculas)
        {
            var resultado = new List<PeliculasGeneros>();
            if(peliculaCreacionDTO.GenerosIds == null) { return resultado; }

            foreach(var id in peliculaCreacionDTO.GenerosIds)
            {
                resultado.Add(new PeliculasGeneros() { GeneroId = id });
            }

            return resultado;

        }

        private List<PeliculasCines> MappePeliculasCines(PeliculaCreacionDTO peliculaCreacionDTO
            , Peliculas peliculas)
        {
            var resultado = new List<PeliculasCines>();
            if (peliculaCreacionDTO.CinesIds == null) { return resultado; }

            foreach (var id in peliculaCreacionDTO.CinesIds)
            {
                resultado.Add(new PeliculasCines() { CineId = id });
            }

            return resultado;

        }
    }
}
