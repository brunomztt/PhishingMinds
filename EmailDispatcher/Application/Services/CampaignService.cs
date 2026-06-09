using EmailDispatcher.Infrastructure.Email;
using EmailDispatcher.Infrastructure.Repositories;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic; 
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using EmailDispatcher.Application.Services.Utils;

namespace EmailDispatcher.Application.Services
{
    public class CampaignService
    {
        private readonly CampaignRepository _repo;
        private readonly EmailSender _emailSender;
        private readonly IConfiguration _configuration;
        private readonly ILogger<CampaignService> _logger;

        private static readonly string[] FakeSenders =
        {
            "Microsoft",
            "Equipe de TI",
            "Office 365",
            "Portal Corporativo",
            "Central de Senhas",
            "Administrador de Sistemas",
            "Seguranca da Informacao",
            "Suporte Tecnico",
            "Microsoft Security",
            "Office Support"
        };

        public CampaignService(
            CampaignRepository repo,
            EmailSender emailSender,
            IConfiguration configuration,
            ILogger<CampaignService> logger)
        {
            _repo = repo;
            _emailSender = emailSender;
            _configuration = configuration;
            _logger = logger;
        }

        public async Task ProcessarCampanhas()
        {
            var campanhas = await _repo.BuscarCampanhasPendentes();

            if (!campanhas.Any())
            {
                _logger.LogInformation(
                    "Nenhuma campanha pendente."
                );
                return;
            }

            var cred = await _repo.GetMailCredential();

            foreach (var campanha in campanhas)
            {
                _logger.LogInformation(
                    $"Processando campanha {campanha.IdCampaign}"
                );

                var usuarios =
                    await _repo.BuscarUsuarios(
                        campanha.IdCampaign
                    );

                if (!usuarios.Any())
                {
                    _logger.LogWarning(
                        $"Campanha {campanha.IdCampaign} sem usuários."
                    );

                    continue;
                }

                int enviados = 0;

                var baseUrl =
                    _configuration["TrackingBaseUrl"]
                    ?? "https://localhost:5164";

                // Fetch template parameter values for this campaign template
                var parametros = await _repo.BuscarParametrosCampanha(campanha.IdTemplateEmpresa);

                foreach (var user in usuarios)
                {
                    _logger.LogInformation(
                        $"Enviando para {user.Email} | Target={user.IdTarget}"
                    );

                    try
                    {
                        var trackingLink =
                            $"{baseUrl}/track?idTarget={user.IdTarget}";

                        var openTracking =
                            $"{baseUrl}/track/open?idTarget={user.IdTarget}";

                        var body = campanha.BodyMail;
                        var subjectRaw = campanha.Subject;

                        // Replace custom template parameters (like Banco, Valor, etc.)
                        foreach (var param in parametros)
                        {
                            if (param.Name.Equals("Nome", StringComparison.OrdinalIgnoreCase) || 
                                param.Name.Equals("Link", StringComparison.OrdinalIgnoreCase))
                            {
                                continue;
                            }
                            body = body.Replace($"{{{{{param.Name}}}}}", param.Value);
                            subjectRaw = subjectRaw.Replace($"{{{{{param.Name}}}}}", param.Value);
                        }

                        // Replace dynamic user-specific placeholders
                        body = body
                            .Replace("{{Nome}}", user.Nome)
                            .Replace("{{Link}}", trackingLink);

                        body += $@"
<img src='{openTracking}'
     width='1'
     height='1'
     style='display:none;' />";

                        string subject =
                            HomoglyphGenerator.Transform(
                                subjectRaw
                            );

                        string senderName =
                            HomoglyphGenerator.Transform(
                                FakeSenders[
                                    Random.Shared.Next(
                                        FakeSenders.Length
                                    )
                                ]
                            );

                        _logger.LogInformation(
                            $"Remetente escolhido: {senderName}"
                        );

                        _logger.LogInformation(
                            $"Assunto original: {campanha.Subject}"
                        );

                        _logger.LogInformation(
                            $"Assunto enviado: {subject}"
                        );

                        await _emailSender.Send(
                            user.Email,
                            subject,
                            body,
                            cred.Mail,
                            senderName,
                            cred.Senha
                        );

                        await _repo.MarcarComoEnviado(
                            user.IdTarget
                        );

                        enviados++;

                        _logger.LogInformation(
                            $"Email enviado para {user.Email}"
                        );
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(
                            ex,
                            $"Erro ao enviar para {user.Email}"
                        );
                    }
                }

                if (enviados > 0)
                {
                    await _repo.MarcarCampanhaComoProcessada(
                        campanha.IdCampaign
                    );

                    _logger.LogInformation(
                        $"Campanha {campanha.IdCampaign} processada."
                    );
                }
                else
                {
                    _logger.LogWarning(
                        $"Nenhum email enviado na campanha {campanha.IdCampaign}"
                    );
                }
            }
        }
    }
}