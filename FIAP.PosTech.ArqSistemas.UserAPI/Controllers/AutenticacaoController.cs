using FIAP.PosTech.ArqSistemas.UserAPI.DTOs;
using FIAP.PosTech.ArqSistemas.UserAPI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.Json;

namespace FIAP.PosTech.ArqSistemas.CloudGames.Api.Controllers
{
    public class AutenticacaoController : ControllerBase 
    {

        private readonly IUsuarioService _autenticacaoService;
        private readonly ILogger<AutenticacaoController> _logger;
        private readonly IConfiguration _configuration;

        public AutenticacaoController(IUsuarioService
            autenticacaoService, ILogger<AutenticacaoController> logger, IConfiguration configuration)
        {
            _autenticacaoService = autenticacaoService;
            _logger = logger;
            _configuration = configuration;
        }


        [HttpPost("Login")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public IActionResult Login([FromBody] LoginUsuarioDto login)
        {
            _logger.LogInformation($"Iniciando autenticação do usuário: {login.Email} senha: {login.Senha}");

            var usuario = _autenticacaoService.Autenticar(login.Email, login.Senha);

            if (usuario != null)
            {
                _logger.LogInformation($"Usuário autenticado com sucesso: {JsonSerializer.Serialize(usuario)}");

                //var token = GenerateToken(login.Email, usuario.Administrador ? "Admin" : "User");
                var token = GenerateToken(login.Email, "Admin");
                return Ok(new { token });
            }
            else
            {
                _logger.LogInformation($"Usuário não autenticado");
                return Unauthorized();
            }
        }

        private string GenerateToken(string email, string role)
        {
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, email),
                new Claim(ClaimTypes.Role, role),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"], 
                claims: claims,
                expires: DateTime.Now.AddMinutes(30),
                signingCredentials: creds
                );

            return new JwtSecurityTokenHandler().WriteToken(token); 
        }
    }
}
