using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using netCoreApi.Controllers;
using netCoreApi.Entidades.Repositorios;
using netCoreApi.Filtros;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace netCoreApi
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer();
            services.AddResponseCaching();//servicio para activar el cache de la aplicacion
            //services.AddTransient<Irepositorio,RepositorioEnMomoria>();
            services.AddScoped<Irepositorio, RepositorioEnMomoria>();
            services.AddScoped<WeatherForecastController>();
            //services.AddSingleton<Irepositorio, RepositorioEnMomoria>();
            services.AddTransient<MiFiltroDeAccion>();
            services.AddControllers(options =>
            {
                options.Filters.Add(typeof(FiltroDeExepcion));
            });
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "netCoreApi", Version = "v1" });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env,ILogger<Startup> logger)
        {
            app.Use(async (context,next) =>
            {
                using(var swapStream = new MemoryStream())
                {
                    var respuestaoriginal = context.Response.Body;
                    context.Response.Body = swapStream;

                    await next.Invoke();

                    swapStream.Seek(0, SeekOrigin.Begin);
                    string respuesta = new StreamReader(swapStream).ReadToEnd();
                    swapStream.Seek(0, SeekOrigin.Begin);

                    await swapStream.CopyToAsync(respuestaoriginal);
                    context.Response.Body = respuestaoriginal;

                    logger.LogInformation(respuesta);
                }
            });

            app.Map("/mapa1", (app) =>
            {
                app.Run(async context =>
                {
                    await context.Response.WriteAsync("esto interseptando el pipeline");
                });
            });
            

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "netCoreApi v1"));
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseResponseCaching();//activa la respuesta del cache

            app.UseAuthentication();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
