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

        private string HashPassword(string password)
        {
            using var sha256 = System.Security.Cryptography.SHA256.Create();
            var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            return BitConverter.ToString(bytes).Replace("-", "").ToLower();
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginRequest request)
        {
            try
            {
                using var db = _dbFactory.CreateConnection();

                // 1. Tentar Empresa
                var sqlEmpresa = "SELECT * FROM Empresa WHERE Mail = @Email";
                var empresa = db.QueryFirstOrDefault<PhishingMinds.Server.Class.Empresa>(
                    sqlEmpresa,
                    new { Email = request.Email });

                if (empresa != null && empresa.Senha == request.Password)
                {
                    var tokenHandler = new JwtSecurityTokenHandler();
                    var jwtKey = _config["Jwt:Key"] ?? "PhishingMindsSuperSecretKey12345!@#";
                    var key = Encoding.ASCII.GetBytes(jwtKey);

                    var tokenDescriptor = new SecurityTokenDescriptor
                    {
                        Subject = new ClaimsIdentity(new[]
                        {
                            new Claim(ClaimTypes.NameIdentifier, empresa.IdEmpresa.ToString()),
                            new Claim(ClaimTypes.Email, empresa.Mail),
                            new Claim("IdEmpresa", empresa.IdEmpresa.ToString())
                        }),
                        Expires = DateTime.UtcNow.AddHours(8),
                        SigningCredentials = new SigningCredentials(
                            new SymmetricSecurityKey(key),
                            SecurityAlgorithms.HmacSha256Signature)
                    };

                    var token = tokenHandler.CreateToken(tokenDescriptor);

                    return Ok(new
                    {
                        Token = tokenHandler.WriteToken(token),
                        User = new
                        {
                            idEmpresa = empresa.IdEmpresa,
                            idPlano = empresa.IdPlano,
                            nome = empresa.Nm_Dono,
                            email = empresa.Mail,
                            isEmpresa = true
                        }
                    });
                }

                // 2. Tentar Pessoa
                var sqlPessoa = "SELECT * FROM Pessoa WHERE Email = @Email";
                var pessoa = db.QueryFirstOrDefault<PhishingMinds.Server.Class.Pessoa>(
                    sqlPessoa,
                    new { Email = request.Email });

                if (pessoa != null)
                {
                    var hashedInput = HashPassword(request.Password);

                    if (pessoa.Senha == request.Password ||
                        pessoa.Senha == hashedInput)
                    {
                        // Atualiza último login
                        db.Execute(
                            "UPDATE Pessoa SET UltimoLogin = @Now WHERE IdUser = @IdUser",
                            new
                            {
                                Now = DateTime.Now,
                                IdUser = pessoa.IdUser
                            });

                        var tokenHandler = new JwtSecurityTokenHandler();
                        var jwtKey = _config["Jwt:Key"] ?? "PhishingMindsSuperSecretKey12345!@#";
                        var key = Encoding.ASCII.GetBytes(jwtKey);

                        var tokenDescriptor = new SecurityTokenDescriptor
                        {
                            Subject = new ClaimsIdentity(new[]
                            {
                                new Claim(ClaimTypes.NameIdentifier, pessoa.IdUser.ToString()),
                                new Claim(ClaimTypes.Email, pessoa.Email),
                                new Claim("IdEmpresa", pessoa.IdEmpresa.ToString())
                            }),
                            Expires = DateTime.UtcNow.AddHours(8),
                            SigningCredentials = new SigningCredentials(
                                new SymmetricSecurityKey(key),
                                SecurityAlgorithms.HmacSha256Signature)
                        };

                        var idPlano = db.ExecuteScalar<int>(
                            "SELECT IdPlano FROM Empresa WHERE IdEmpresa = @IdEmpresa",
                            new { IdEmpresa = pessoa.IdEmpresa });

                        var token = tokenHandler.CreateToken(tokenDescriptor);

                        return Ok(new
                        {
                            Token = tokenHandler.WriteToken(token),
                            User = new
                            {
                                idEmpresa = pessoa.IdEmpresa,
                                idPlano = idPlano,
                                nome = pessoa.Nome,
                                email = pessoa.Email,
                                isPessoa = true
                            }
                        });
                    }
                }

                // 3. Fallback DEV
                if (request.Email.Trim().ToLower() == "admin@phishingminds.com")
                {
                    if (request.Password == "123456" ||
                        request.Password == "123")
                    {
                        var tokenHandler = new JwtSecurityTokenHandler();
                        var jwtKey = _config["Jwt:Key"] ?? "PhishingMindsSuperSecretKey12345!@#";
                        var key = Encoding.ASCII.GetBytes(jwtKey);

                        var tokenDescriptor = new SecurityTokenDescriptor
                        {
                            Subject = new ClaimsIdentity(new[]
                            {
                                new Claim(ClaimTypes.NameIdentifier, "1"),
                                new Claim(ClaimTypes.Email, request.Email),
                                new Claim("IdEmpresa", "1")
                            }),
                            Expires = DateTime.UtcNow.AddHours(8),
                            SigningCredentials = new SigningCredentials(
                                new SymmetricSecurityKey(key),
                                SecurityAlgorithms.HmacSha256Signature)
                        };

                        var token = tokenHandler.CreateToken(tokenDescriptor);

                        return Ok(new
                        {
                            Token = tokenHandler.WriteToken(token),
                            User = new
                            {
                                idEmpresa = 1,
                                email = request.Email,
                                isEmpresa = true
                            }
                        });
                    }
                }

                return Unauthorized("Credenciais inválidas.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("ERRO LOGIN:");
                Console.WriteLine(ex.ToString());

                return StatusCode(500, ex.Message);
            }
        }


        //senha reset
        [HttpPost("reset-password")]
        public IActionResult ResetPassword([FromBody] ResetPasswordRequest request)
        {
            try
            {
                using var db = _dbFactory.CreateConnection();

                // 1. Tentar Empresa
                var empresa = db.QueryFirstOrDefault<PhishingMinds.Server.Class.Empresa>(
                    "SELECT * FROM Empresa WHERE Mail = @Email",
                    new { Email = request.Email }
                );

                if (empresa != null)
                {
                    db.Execute(
                        "UPDATE Empresa SET Senha = @Senha WHERE Mail = @Email",
                        new { Senha = request.NewPassword, Email = request.Email }
                    );
                    return Ok("Senha de Administrador atualizada com sucesso.");
                }

                // 2. Tentar Pessoa
                var pessoa = db.QueryFirstOrDefault<PhishingMinds.Server.Class.Pessoa>(
                    "SELECT * FROM Pessoa WHERE Email = @Email",
                    new { Email = request.Email }
                );

                if (pessoa != null)
                {
                    var hashedNewPassword = HashPassword(request.NewPassword);
                    db.Execute(
                        "UPDATE Pessoa SET Senha = @Senha WHERE Email = @Email",
                        new { Senha = hashedNewPassword, Email = request.Email }
                    );
                    return Ok("Senha de Colaborador atualizada com sucesso.");
                }

                return NotFound("Usuário não encontrado.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.ToString);
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
                    "SELECT * FROM Empresa WHERE Mail = @Email",
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
                    INSERT INTO Empresa (IdPlano, Nm_Empresa, Nm_Dono, Mail, Senha, CNPJ, Dt_Cadastro, Dt_Contratacao, Dt_FimContrato, Ativo)
                    VALUES (@IdPlano, @Nm_Empresa, @Nm_Dono, @Mail, @Senha, @CNPJ, @Dt_Cadastro, @Dt_Contratacao, @Dt_FimContrato, @Ativo)";

                db.Execute(sql, request);

                return Ok(new { message = "Cadastro realizado com sucesso." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.ToString);
            }
        }

    }
}
