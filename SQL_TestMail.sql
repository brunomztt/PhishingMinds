-- DESATIVA FK
SET FOREIGN_KEY_CHECKS = 0;

-- LIMPA NA ORDEM CORRETA
TRUNCATE TABLE PhishingCampaignTarget;
TRUNCATE TABLE PhishingCampaign;
TRUNCATE TABLE PhishingTemplateEmpresa;
TRUNCATE TABLE PhishingTemplate;
TRUNCATE TABLE Pessoa;
TRUNCATE TABLE Setor;
TRUNCATE TABLE Cargo;
TRUNCATE TABLE Empresa;
TRUNCATE TABLE Plano;

-- REATIVA FK
SET FOREIGN_KEY_CHECKS = 1;

-- =========================
-- INSERTS
-- =========================

-- PLANO
INSERT INTO Plano (IdPlano, Nm_Plano, Desc_Plano, Temp_Plano, Value_Plano, MaxUsers, MaxCampaigns)
VALUES (1, 'Teste', 'Plano teste', 30, 0, 100, 10);

-- EMPRESA
INSERT INTO Empresa (IdEmpresa, IdPlano, Nm_Empresa, Nm_Dono, Mail, CNPJ, Dt_Cadastro, Ativo)
VALUES (1, 1, 'Empresa Teste', 'Bruno', 'teste@empresa.com', '00000000000100', NOW(), 1);

-- CARGO
INSERT INTO Cargo (IdCargo, Nm_Cargo)
VALUES (1, 'Funcionário');

-- SETOR
INSERT INTO Setor (IdSetor, IdEmpresa, Nm_Setor)
VALUES (1, 1, 'TI');

-- PESSOA (DESTINATÁRIO)
INSERT INTO Pessoa (IdUser, IdEmpresa, IdSetor, IdCargo, Nome, Email, Senha, Ativo, Dt_Cadastro)
VALUES (1, 1, 1, 1, 'Bruno Mazetto', 'Bruno.maz3tto@gmail.com', '123', 1, NOW());

-- TEMPLATE
INSERT INTO PhishingTemplate (IdTemplate, NomeTemplate, Subject, BodyMail, Categoria, NivelDificuldade)
VALUES (
    1,
    'Teste Email',
    'Teste do Sistema 🚀',
    '<h1>Olá {{Nome}}</h1><p>Se você recebeu isso, o sistema está funcionando PFVR FUNCIONA</p>',
    'Teste',
    1
);

-- TEMPLATE EMPRESA
INSERT INTO PhishingTemplateEmpresa (IdTemplateEmpresa, IdEmpresa, IdTemplate, NomePersonalizado)
VALUES (1, 1, 1, 'Template Teste Empresa');

-- CAMPANHA
INSERT INTO PhishingCampaign (IdCampaign, IdEmpresa, IdTemplateEmpresa, NomeCampanha, Dt_Disparo, Status)
VALUES (1, 1, 1, 'Campanha Teste', NOW(), 'PENDENTE');

-- TARGET
INSERT INTO PhishingCampaignTarget (IdTarget, IdCampaign, IdUser, MailSent, Dt_Register)
VALUES (1, 1, 1, 0, NOW());