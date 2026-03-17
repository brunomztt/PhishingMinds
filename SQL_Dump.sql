-- PLANOS
INSERT INTO Plano (Nm_Plano, Desc_Plano, Temp_Plano, Value_Plano, MaxUsers, MaxCampaigns)
VALUES
('Starter','Plano inicial',12,99.90,50,5),
('Enterprise','Plano completo',12,999.90,1000,999);

-- EMPRESA
INSERT INTO Empresa
(IdPlano,Nm_Empresa,Nm_Dono,Mail,CNPJ,Dt_Cadastro,Dt_Contratacao,Dt_FimContrato,Ativo)
VALUES
(2,'Phishing Minds Corp','Bruno Mazetto','admin@phishingminds.com','12345678000100',NOW(),NOW(),DATE_ADD(NOW(),INTERVAL 1 YEAR),1);

-- CARGOS
INSERT INTO Cargo (Nm_Cargo)
VALUES
('Analista'),
('Gerente'),
('Diretor'),
('Financeiro'),
('TI');

-- SETORES
INSERT INTO Setor (IdEmpresa,Nm_Setor)
VALUES
(1,'Financeiro'),
(1,'TI'),
(1,'RH'),
(1,'Comercial');

-- PESSOAS FAKE
INSERT INTO Pessoa
(IdEmpresa,IdSetor,IdCargo,Nome,Email,Senha,Ativo,Dt_Cadastro)
VALUES
(1,2,5,'Bruno Mazetto','bruno@empresa.com','123',1,NOW()),
(1,3,1,'Leticia Fabri','leticia@empresa.com','123',1,NOW()),
(1,4,1,'Helen Kenway','helen@empresa.com','123',1,NOW()),
(1,1,4,'Rafael Emo','rafael@empresa.com','123',1,NOW()),
(1,1,2,'Marco Agronomo','marco@empresa.com','123',1,NOW());

-- TEMPLATE PHISHING
INSERT INTO PhishingTemplate
(NomeTemplate,Subject,BodyMail,Categoria,NivelDificuldade)
VALUES
(
'Reset Senha Microsoft',
'Atualização de senha obrigatória',
'Olá {{Nome}}, detectamos atividade suspeita. Clique aqui {{Link}}',
'Credential',
3
);

-- PARAMETROS
INSERT INTO TemplateParameter (IdTemplate,ParameterName,ExampleValue)
VALUES
(1,'Nome','Bruno'),
(1,'Link','https://fake-login.com');

-- TEMPLATE EMPRESA
INSERT INTO PhishingTemplateEmpresa
(IdEmpresa,IdTemplate,NomePersonalizado)
VALUES
(1,1,'Template Reset Empresa');

-- CAMPANHA
INSERT INTO PhishingCampaign
(IdEmpresa,IdTemplateEmpresa,IdSetor,NomeCampanha,Dt_Disparo,Status)
VALUES
(1,1,NULL,'Campanha Teste Geral',NOW(),'AGENDADO');

-- EMAIL CREDENTIAL
INSERT INTO MailCredentials
(Mail,Senha)
VALUES
('security@phishingminds.com','123456');