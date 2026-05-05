using Dapper;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using PhishingMinds.Server.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

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

        public class ResetPasswordRequest
        {
            public string Email { get; set; }
            public string NewPassword { get; set; }
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
                // else
                // {
                //     return Unauthorized("Credenciais inválidas.");
                // }
            }

            // if (user != null && user.Senha != request.Password)
            // {
            //     return Unauthorized("Senha inválida.");
            // }

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


        //senha reset
        [HttpPost("reset-password")]
        public IActionResult ResetPassword([FromBody] ResetPasswordRequest request)
        {
            try
            {
                using var db = _dbFactory.CreateConnection();

                var user = db.QueryFirstOrDefault<PhishingMinds.Server.Class.Empresa>(
                    "SELECT * FROM empresa WHERE Mail = @Email",
                    new { Email = request.Email }
                );

                if (user == null)
                {
                    return NotFound("Usuário não encontrado.");
                }

                // atualiza senha (SEM criptografia)
                db.Execute(
                    "UPDATE empresa SET Senha = @Senha WHERE Mail = @Email",
                    new { Senha = request.NewPassword, Email = request.Email }
                );

                return Ok("Senha atualizada com sucesso.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost("register")]
        public IActionResult Register([FromBody] PhishingMinds.Server.Class.Empresa request)
        {
            try
            {
                using var db = _dbFactory.CreateConnection();

                // Check if email already exists
                var existingUser = db.QueryFirstOrDefault<PhishingMinds.Server.Class.Empresa>(
                    "SELECT * FROM empresa WHERE Mail = @Email",
                    new { Email = request.Mail }
                );

                if (existingUser != null)
                {
                    return BadRequest("Este e-mail já está em uso.");
                }

                // Set default fields
                request.Dt_Cadastro = DateTime.UtcNow;
                request.Dt_Contratacao = DateTime.UtcNow;
                request.Dt_FimContrato = DateTime.UtcNow.AddYears(1);
                request.Ativo = true;
                
                // If IdPlano is not set, default to 1
                if (request.IdPlano == 0)
                {
                    request.IdPlano = 1;
                }

                var sql = @"
                    INSERT INTO empresa (IdPlano, Nm_Empresa, Nm_Dono, Mail, Senha, CNPJ, Dt_Cadastro, Dt_Contratacao, Dt_FimContrato, Ativo)
                    VALUES (@IdPlano, @Nm_Empresa, @Nm_Dono, @Mail, @Senha, @CNPJ, @Dt_Cadastro, @Dt_Contratacao, @Dt_FimContrato, @Ativo)";

                db.Execute(sql, request);

                return Ok(new { message = "Cadastro realizado com sucesso." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

    }
}
