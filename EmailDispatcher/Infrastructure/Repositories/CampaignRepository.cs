using EmailDispatcher.Domain.Entities;
using EmailDispatcher.Infrastructure.Data;
using MySql.Data.MySqlClient;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace EmailDispatcher.Infrastructure.Repositories
{
    public class CampaignRepository
    {
        private readonly MySqlConnectionFactory _factory;

        public CampaignRepository(MySqlConnectionFactory factory)
        {
            _factory = factory;
        }

        // Buscar campanhas prontas para disparo
        public virtual async Task<List<Campaign>> BuscarCampanhasPendentes()
        {
            var campanhas = new List<Campaign>();

            using var conn = _factory.CreateConnection();
            await conn.OpenAsync();

            var cmd = new MySqlCommand(@"
                SELECT 
                    pc.IdCampaign,
                    pc.IdTemplateEmpresa,
                    pt.Subject,
                    pt.BodyMail,
                    pc.NomeCampanha
                FROM PhishingCampaign pc
                JOIN PhishingTemplateEmpresa pte 
                    ON pte.IdTemplateEmpresa = pc.IdTemplateEmpresa
                JOIN PhishingTemplate pt 
                    ON pt.IdTemplate = pte.IdTemplate
                WHERE pc.Status = 'AGENDADO'
                AND pc.Dt_Disparo <= NOW()
            ", conn);

            using var reader = await cmd.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                campanhas.Add(new Campaign
                {
                    IdCampaign = reader.GetInt32("IdCampaign"),
                    IdTemplateEmpresa = reader.GetInt32("IdTemplateEmpresa"),
                    Subject = reader.GetString("Subject"),
                    BodyMail = reader.GetString("BodyMail"),
                    NomeCampanha = reader.GetString("NomeCampanha")
                });
            }

            return campanhas;
        }

        // Buscar parâmetros customizados do template da empresa
        public virtual async Task<List<(string Name, string Value)>> BuscarParametrosCampanha(int idTemplateEmpresa)
        {
            var paramsList = new List<(string Name, string Value)>();

            using var conn = _factory.CreateConnection();
            await conn.OpenAsync();

            var cmd = new MySqlCommand(@"
                SELECT 
                    tp.ParameterName,
                    COALESCE(pv.ParameterValue, tp.ExampleValue) AS ParameterValue
                FROM TemplateParameter tp
                LEFT JOIN ParameterValue pv 
                    ON tp.IdParameter = pv.IdParameter 
                    AND pv.IdTemplateEmpresa = @IdTemplateEmpresa
                WHERE tp.IdTemplate = (
                    SELECT IdTemplate 
                    FROM PhishingTemplateEmpresa 
                    WHERE IdTemplateEmpresa = @IdTemplateEmpresa
                )
            ", conn);

            cmd.Parameters.AddWithValue("@IdTemplateEmpresa", idTemplateEmpresa);

            using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                paramsList.Add((
                    reader.GetString("ParameterName"),
                    reader.GetString("ParameterValue")
                ));
            }

            return paramsList;
        }

        // Buscar usuários da campanha
        public virtual async Task<List<User>> BuscarUsuarios(int idCampaign)
        {
            var usuarios = new List<User>();

            using var conn = _factory.CreateConnection();
            await conn.OpenAsync();

            var cmd = new MySqlCommand(@"
                SELECT 
                    t.IdTarget,
                    p.Nome,
                    p.Email
                FROM PhishingCampaignTarget t
                JOIN Pessoa p ON p.IdUser = t.IdUser
                WHERE t.IdCampaign = @IdCampaign
                AND t.MailSent = 0
            ", conn);

            cmd.Parameters.AddWithValue("@IdCampaign", idCampaign);

            using var reader = await cmd.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                usuarios.Add(new User
                {
                    IdTarget = reader.GetInt32("IdTarget"),
                    Nome = reader.GetString("Nome"),
                    Email = reader.GetString("Email")
                });
            }

            return usuarios;
        }

        // Marcar email como enviado
        public virtual async Task MarcarComoEnviado(int idTarget)
        {
            using var conn = _factory.CreateConnection();
            await conn.OpenAsync();

            var cmd = new MySqlCommand(@"
                UPDATE PhishingCampaignTarget
                SET MailSent = 1,
                    Dt_Register = NOW()
                WHERE IdTarget = @IdTarget
            ", conn);

            cmd.Parameters.AddWithValue("@IdTarget", idTarget);

            await cmd.ExecuteNonQueryAsync();
        }

        // Marcar campanha como finalizada
        public virtual async Task MarcarCampanhaComoProcessada(int idCampaign)
        {
            using var conn = _factory.CreateConnection();
            await conn.OpenAsync();

            var cmd = new MySqlCommand(@"
                UPDATE PhishingCampaign
                SET Status = 'PROCESSADA'
                WHERE IdCampaign = @IdCampaign
            ", conn);

            cmd.Parameters.AddWithValue("@IdCampaign", idCampaign);

            await cmd.ExecuteNonQueryAsync();
        }

        // Buscar credencial de envio
        public virtual async Task<MailCredential> GetMailCredential()
        {
            using var conn = _factory.CreateConnection();
            await conn.OpenAsync();

            var cmd = new MySqlCommand(@"
                SELECT Id_EmailCredentials, Mail, Senha
                FROM MailCredentials
                LIMIT 1
            ", conn);

            using var reader = await cmd.ExecuteReaderAsync();

            if (await reader.ReadAsync())
            {
                return new MailCredential
                {
                    Id = reader.GetInt32("Id_EmailCredentials"),
                    Mail = reader.GetString("Mail"),
                    Senha = reader.GetString("Senha")
                };
            }

            return null;
        }
    }
}