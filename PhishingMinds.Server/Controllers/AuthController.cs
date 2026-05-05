using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using PhishingMinds.Server.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Dapper;

namespace PhishingMinds.Server.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _config;
        private readonly DbConnectionFactory _dbFactory;

        public AuthController(IConfiguration config, DbConnectionFactory dbFactory)
        {
            _config = config;
            _dbFactory = dbFactory;
        }

        public class LoginRequest
        {
            public string Email { get; set; }
            public string Password { get; set; }
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginRequest request)
        {
            PhishingMinds.Server.Class.Empresa user = null;

            try
            {
                using var db = _dbFactory.CreateConnection();
                var sql = "SELECT * FROM empresa WHERE Mail = @Email";
                user = db.QueryFirstOrDefault<PhishingMinds.Server.Class.Empresa>(sql, new { Email = request.Email });
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }

            if (user == null)
            {
                // Fallback for dev purposes to bypass if DB is empty
                if (request.Email.Trim().ToLower() == "admin@phishingminds.com")
                {
                    user = new PhishingMinds.Server.Class.Empresa { IdEmpresa = 1, Nm_Dono = "Master Admin", Mail = request.Email };
                }
                else if (request.Email == "org@empresa.com")
                {
                    user = new PhishingMinds.Server.Class.Empresa { IdEmpresa = 2, Nm_Dono = "Org Admin", Mail = request.Email };
                }
                else
                {
                    return Unauthorized("Credenciais inválidas.");
                }
            }

            var tokenHandler = new JwtSecurityTokenHandler();
            var jwtKey = _config["Jwt:Key"] ?? "PhishingMindsSuperSecretKey12345!@#";
            var key = Encoding.ASCII.GetBytes(jwtKey);
            
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, user.IdEmpresa.ToString()),
                    new Claim(ClaimTypes.Email, user.Mail),
                    new Claim("IdEmpresa", user.IdEmpresa.ToString())
                }),
                Expires = DateTime.UtcNow.AddHours(8),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return Ok(new { 
                Token = tokenHandler.WriteToken(token),
                User = new { idEmpresa = user.IdEmpresa, nome = user.Nm_Dono, email = user.Mail }
            });
        }
    }
}
