using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using netCoreApi.Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace netCoreApi
{
    public class ApplicationDbContext : IdentityDbContext
    {
        //configuracion el sistema de usuario de ASP.Net core
        /**
         * -para utilizar el sistema de usuario se tiene que instalar el paquete 
         * -Identity.EntityFrameworkCore desde el administrador de paquetes NuGet
         * -Luego en la clase AppilicaionDbContext tiene que eredar de la clase IdentityDbContext
         * -Despues se realiza la migracion y actualizacion de las tablas de la base de datos con
         * los comandos Add-Migration {SistemaDeUsuarios}-> nombre de la migracion y Update-Database
         * -una vez echa la migracion se tiene que configurar los servicios en la clase startup
         */

        public ApplicationDbContext(DbContextOptions options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<PeliculasActores>().HasKey(x => new { x.ActorId,x.PeliculaId});
            modelBuilder.Entity<PeliculasGeneros>().HasKey(x => new { x.GeneroId, x.PeliculaId });
            modelBuilder.Entity<PeliculasCines>().HasKey(x => new { x.CineId, x.PeliculaId });

            base.OnModelCreating(modelBuilder);
        }

        public DbSet<Generos> Generos { get; set; }
        public DbSet<Actores> Actores { get; set; }
        public DbSet<Cines> Cines { get; set; }
        public DbSet<Peliculas> Peliculas { get; set; }
        public DbSet<PeliculasActores> PeliculasActores { get; set; }
        public DbSet<PeliculasCines> PeliculasCines { get; set; }
        public DbSet<PeliculasGeneros> PeliculasGeneros { get; set; }
        public DbSet<Rating> Ratings { get; set; }
    }
}
