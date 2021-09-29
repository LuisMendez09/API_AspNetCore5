using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using netCoreApi.DTOs;
using netCoreApi.DTOs.AutenticacionUsuarios;
using netCoreApi.Utilidades;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace netCoreApi.Controllers
{
    [Route("api/cuentas")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "EsAdmin")]
    public class CuentasController : ControllerBase
    {
        private readonly UserManager<IdentityUser> userManager;
        private readonly SignInManager<IdentityUser> signInManager;
        private readonly ApplicationDbContext context;
        private readonly IMapper mapper;
        private readonly IConfiguration configuration;

        public CuentasController(UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager,
            ApplicationDbContext context,
            IMapper mapper,
            IConfiguration configuration)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.context = context;
            this.mapper = mapper;
            this.configuration = configuration;
        }

        [HttpGet("listadousuarios")]
        public async Task<ActionResult<List<UsuarioDTO>>> ListadoUsuarios([FromQuery] PaginacionDTO paginacionDTO)
        {
            var querable = context.Users.AsQueryable();
            await HttpContext.InsertarParametrosPaginacionEnCabecera(querable);
            var usuarios = await querable.OrderBy(x => x.Email).Paginar(paginacionDTO).ToListAsync();

            return mapper.Map<List<UsuarioDTO>>(usuarios);
        }

        [HttpPost("haceradmin")]
        public async Task<ActionResult> HacerAdmin([FromBody]string adminId)
        {
            var usuario = await userManager.FindByIdAsync(adminId);
            await userManager.AddClaimAsync(usuario, new Claim ( "role", "admin" ));
            return NoContent();
        }

        [HttpPost("removeradmin")]
        public async Task<ActionResult> RemoverrAdmin([FromBody] string adminId)
        {
            var usuario = await userManager.FindByIdAsync(adminId);
            await userManager.RemoveClaimAsync(usuario, new Claim("role", "admin"));
            return NoContent();
        }

        [HttpPost("crear")]
        [AllowAnonymous]
        public async Task<ActionResult<RespuestaAutentucacion>> Crear([FromBody] CredencialesUsuario credenciales)
        {
            var usuario = new IdentityUser { UserName = credenciales.Email, Email = credenciales.Email };
            var resultado = await userManager.CreateAsync(usuario, credenciales.Password);

            if (resultado.Succeeded)
            {
                return await ConstruirToken(credenciales);
            }
            else
            {
                return BadRequest(resultado.Errors);
            }
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<ActionResult<RespuestaAutentucacion>> Login([FromBody] CredencialesUsuario credenciales)
        {
            var resultado = await signInManager.PasswordSignInAsync(credenciales.Email, credenciales.Password,
                isPersistent: false, lockoutOnFailure: false);

            if (resultado.Succeeded)
            {
                return await ConstruirToken(credenciales);
            }
            else
            {
                return BadRequest("Login incorrecto");
            }
        }


        private async Task<RespuestaAutentucacion> ConstruirToken(CredencialesUsuario credenciales)
        {
            var claims = new List<Claim>()
            {
                new Claim("email",credenciales.Email)
            };

            var usuario = await userManager.FindByEmailAsync(credenciales.Email);
            var claimsDB = await userManager.GetClaimsAsync(usuario);
            claims.AddRange(claimsDB);

            var llave = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["llavejwt"]));
            var creds = new SigningCredentials(llave, SecurityAlgorithms.HmacSha256);

            var expitacion = DateTime.UtcNow.AddYears(1);

            var token = new JwtSecurityToken(issuer: null, audience: null, claims: claims
                , expires: expitacion, signingCredentials: creds);

            return new RespuestaAutentucacion()
            {
                Token = new JwtSecurityTokenHandler().WriteToken(token),
                Expiracion = expitacion
            };
        }


    }
}
