using EmailDispatcher.Infrastructure.Email;
using EmailDispatcher.Infrastructure.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EmailDispatcher.Application.Services
{
    public class CampaignService
    {
        private readonly CampaignRepository _repo;
        private readonly EmailSender _emailSender;
        private readonly IConfiguration _configuration;
        private readonly ILogger<CampaignService> _logger;

        public CampaignService(CampaignRepository repo, EmailSender emailSender, IConfiguration configuration, ILogger<CampaignService> logger)
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
                _logger.LogInformation("Nenhuma campanha pendente.");
                return;
            }

            var cred = await _repo.GetMailCredential();

            foreach (var campanha in campanhas)
            {
                _logger.LogInformation($"Processando campanha {campanha.IdCampaign}");

                var usuarios = await _repo.BuscarUsuarios(campanha.IdCampaign);

                if (!usuarios.Any())
                {
                    _logger.LogWarning($"Campanha {campanha.IdCampaign} sem usuários.");
                    continue; // 🔴 NÃO marca como processada
                }

                int enviados = 0;

                var baseUrl = _configuration["TrackingBaseUrl"] ?? "https://localhost:7193";

                foreach (var user in usuarios)
                {
                    try
                    {
                        var trackingLink = $"{baseUrl}/track?idTarget={user.IdTarget}";
                        var body = campanha.BodyMail
                            .Replace("{{Nome}}", user.Nome)
                            .Replace("{{Link}}", trackingLink);

                        await _emailSender.Send(
                            user.Email,
                            campanha.Subject,
                            body,
                            cred.Mail,
                            campanha.NomeCampanha,
                            cred.Senha
                        );

                        await _repo.MarcarComoEnviado(user.IdTarget);

                        enviados++;

                        _logger.LogInformation($"Email enviado para {user.Email}");
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, $"Erro ao enviar para {user.Email}");
                    }
                }

                // 🔥 SÓ marca se realmente enviou
                if (enviados > 0)
                {
                    await _repo.MarcarCampanhaComoProcessada(campanha.IdCampaign);
                }
                else
                {
                    _logger.LogWarning($"Nenhum email enviado na campanha {campanha.IdCampaign}");
                }
            }
        }
    }
}
