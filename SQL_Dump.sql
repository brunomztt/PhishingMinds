-- PLANOS
INSERT INTO Plano (Nm_Plano, Desc_Plano, Temp_Plano, Value_Plano, MaxUsers, MaxCampaigns)
VALUES
('Starter','Plano inicial',12,99.90,50,5),
('Enterprise','Plano completo',12,999.90,1000,999);

-- EMPRESA
INSERT INTO Empresa
(IdPlano,Nm_Empresa,Nm_Dono,Mail,CNPJ,Dt_Cadastro,Dt_Contratacao,Dt_FimContrato,Ativo, Senha)
VALUES
(1,'Phishing Minds Dev','Admin Dev','teste.admin@email.com','12345678000100',NOW(),NOW(),DATE_ADD(NOW(),INTERVAL 1 YEAR),1, '1'),
(2,'Teste Corp','Teste de Empresa','teste.empresa@email.com','98765432000100',NOW(),NOW(),DATE_ADD(NOW(),INTERVAL 1 YEAR),1, '1');


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
(2,'Financeiro'),
(2,'TI'),
(2,'RH'),
(2,'Comercial');

-- PESSOAS FAKE
INSERT INTO Pessoa
(IdEmpresa,IdSetor,IdCargo,Nome,Email,Senha,Ativo,Dt_Cadastro)
VALUES
(2,null,null,'Teste de User','teste.user@email.com','1',1,NOW()),
(2,2,5,'Bruno Mazetto','bruno@empresa.com','1',1,NOW()),
(2,3,1,'Leticia Fabri','leticia@empresa.com','1',1,NOW()),
(2,4,1,'Helen Kenway','helenbonato02@gmail.com','1',1,NOW()),
(2,1,4,'Rafael Emo','rafael@empresa.com','1',1,NOW()),
(2,1,2,'Marco Agronomo','marco@empresa.com','1',1,NOW());

-- TEMPLATE PHISHING
INSERT INTO PhishingTemplate
(NomeTemplate,Subject,BodyMail,Categoria,NivelDificuldade)
VALUES
(
'Reset Senha Microsoft',
'Atualizacao de senha obrigatoria',
'Ola {{Nome}}, detectamos atividade suspeita. Clique aqui {{Link}}',
'Credential',
3
),
(
'Alerta de Segurança Google',
'Novo login detectado no seu dispositivo',
'Ola {{Nome}}, um novo dispositivo acabou de fazer login na sua conta Google. Se nao foi voce, verifique a atividade imediatamente clicando aqui: {{Link}}',
'Credential',
4
);

-- PARAMETROS
INSERT INTO TemplateParameter (IdTemplate,ParameterName,ExampleValue)
VALUES
(1,'Nome','Bruno'),
(1,'Link','https://fake-login.com'),
(2,'Nome','Helen'),
(2,'Link','https://google-security-alert.com');


-- TEMPLATE EMPRESA
INSERT INTO PhishingTemplateEmpresa
(IdEmpresa,IdTemplate,NomePersonalizado)
VALUES
(2,1,'Template Reset Empresa');

-- CAMPANHA
INSERT INTO PhishingCampaign
(IdEmpresa,IdTemplateEmpresa,IdSetor,NomeCampanha,Dt_Disparo,Status)
VALUES
(2,1,NULL,'Campanha Teste Geral',NOW(),'AGENDADO');

-- EMAIL CREDENTIAL
INSERT INTO MailCredentials
(Mail,Senha)
VALUES
('security@phishingminds.com','123456');