using EmailDispatcher.Domain.Entities;
using EmailDispatcher.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
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

        private static int GetInt32(DbDataReader reader, string columnName)
        {
            return reader.GetInt32(reader.GetOrdinal(columnName));
        }

        private static string GetString(DbDataReader reader, string columnName)
        {
            return reader.GetString(reader.GetOrdinal(columnName));
        }

        private static void AddParameter(DbCommand cmd, string name, object value)
        {
            var p = cmd.CreateParameter();
            p.ParameterName = name;
            p.Value = value ?? DBNull.Value;
            cmd.Parameters.Add(p);
        }

        // Buscar campanhas prontas para disparo
        public virtual async Task<List<Campaign>> BuscarCampanhasPendentes()
        {
            var campanhas = new List<Campaign>();

            using var conn = _factory.CreateConnection();
            await conn.OpenAsync();

            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"
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
            ";

            using var reader = await cmd.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                campanhas.Add(new Campaign
                {
                    IdCampaign = GetInt32(reader, "IdCampaign"),
                    IdTemplateEmpresa = GetInt32(reader, "IdTemplateEmpresa"),
                    Subject = GetString(reader, "Subject"),
                    BodyMail = GetString(reader, "BodyMail"),
                    NomeCampanha = GetString(reader, "NomeCampanha")
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

            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"
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
            ";

            AddParameter(cmd, "@IdTemplateEmpresa", idTemplateEmpresa);

            using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                paramsList.Add((
                    GetString(reader, "ParameterName"),
                    GetString(reader, "ParameterValue")
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

            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"
                SELECT 
                    t.IdTarget,
                    p.Nome,
                    p.Email
                FROM PhishingCampaignTarget t
                JOIN Pessoa p ON p.IdUser = t.IdUser
                WHERE t.IdCampaign = @IdCampaign
                AND t.MailSent = 0
            ";

            AddParameter(cmd, "@IdCampaign", idCampaign);

            using var reader = await cmd.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                usuarios.Add(new User
                {
                    IdTarget = GetInt32(reader, "IdTarget"),
                    Nome = GetString(reader, "Nome"),
                    Email = GetString(reader, "Email")
                });
            }

            return usuarios;
        }

        // Marcar email como enviado
        public virtual async Task MarcarComoEnviado(int idTarget)
        {
            using var conn = _factory.CreateConnection();
            await conn.OpenAsync();

            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"
                UPDATE PhishingCampaignTarget
                SET MailSent = 1,
                    Dt_Register = NOW()
                WHERE IdTarget = @IdTarget
            ";

            AddParameter(cmd, "@IdTarget", idTarget);

            await cmd.ExecuteNonQueryAsync();
        }

        // Marcar campanha como finalizada
        public virtual async Task MarcarCampanhaComoProcessada(int idCampaign)
        {
            using var conn = _factory.CreateConnection();
            await conn.OpenAsync();

            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"
                UPDATE PhishingCampaign
                SET Status = 'PROCESSADA'
                WHERE IdCampaign = @IdCampaign
            ";

            AddParameter(cmd, "@IdCampaign", idCampaign);

            await cmd.ExecuteNonQueryAsync();
        }

        // Buscar credencial de envio
        public virtual async Task<MailCredential> GetMailCredential()
        {
            using var conn = _factory.CreateConnection();
            await conn.OpenAsync();

            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"
                SELECT Id_EmailCredentials, Mail, Senha
                FROM MailCredentials
                LIMIT 1
            ";

            using var reader = await cmd.ExecuteReaderAsync();

            if (await reader.ReadAsync())
            {
                return new MailCredential
                {
                    Id = GetInt32(reader, "Id_EmailCredentials"),
                    Mail = GetString(reader, "Mail"),
                    Senha = GetString(reader, "Senha")
                };
            }

            return null;
        }
    }
}