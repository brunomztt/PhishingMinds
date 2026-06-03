USE phishing_minds;

SET FOREIGN_KEY_CHECKS = 0;


TRUNCATE TABLE PhishingCampaignTarget;
TRUNCATE TABLE PhishingCampaign;

TRUNCATE TABLE ParameterValue;
TRUNCATE TABLE TemplateParameter;

TRUNCATE TABLE PhishingTemplateEmpresa;
TRUNCATE TABLE PhishingTemplate;

TRUNCATE TABLE LandingPage;
TRUNCATE TABLE MailCredentials;

TRUNCATE TABLE Pessoa;
TRUNCATE TABLE Setor;
TRUNCATE TABLE Cargo;

TRUNCATE TABLE Empresa;
TRUNCATE TABLE Plano;


SET FOREIGN_KEY_CHECKS = 1;



-- =========================
-- PLANO
-- =========================

INSERT INTO Plano
(Nm_Plano, Desc_Plano, Temp_Plano, Value_Plano, MaxUsers, MaxCampaigns)
VALUES
(
'Starter',
'Plano inicial',
12,
99.90,
50,
5
),
(
'Enterprise',
'Plano completo',
12,
999.90,
1000,
999
);



-- =========================
-- EMPRESA
-- =========================

INSERT INTO Empresa
(
IdPlano,
Nm_Empresa,
Nm_Dono,
Mail,
CNPJ,
Dt_Cadastro,
Dt_Contratacao,
Dt_FimContrato,
Ativo,
Senha
)
VALUES
(
2,
'Phishing Minds Corp',
'Bruno Mazetto',
'admin@phishingminds.com',
'12345678000100',
NOW(),
NOW(),
DATE_ADD(NOW(), INTERVAL 1 YEAR),
1,
'1'
);



-- =========================
-- CARGOS
-- =========================

INSERT INTO Cargo (Nm_Cargo)
VALUES
('Analista'),
('Gerente'),
('Diretor'),
('Financeiro'),
('TI');



-- =========================
-- SETORES
-- =========================

INSERT INTO Setor
(
IdEmpresa,
Nm_Setor
)
VALUES
(1,'Financeiro'),
(1,'TI'),
(1,'RH'),
(1,'Comercial');




-- =========================
-- PESSOAS
-- =========================

INSERT INTO Pessoa
(
IdEmpresa,
IdSetor,
IdCargo,
Nome,
Email,
Senha,
Ativo,
Dt_Cadastro
)
VALUES

(1,2,5,
'Bruno Mazetto',
'bruno@empresa.com',
'1',
1,
NOW()),


(1,3,1,
'Leticia Fabri',
'leticia@empresa.com',
'1',
1,
NOW()),


(1,4,1,
'Helen Kenway',
'helen@empresa.com',
'1',
1,
NOW()),


(1,1,4,
'Rafael Emo',
'rafael@empresa.com',
'1',
1,
NOW()),


(1,1,2,
'Marco Agronomo',
'marco@empresa.com',
'1',
1,
NOW());



-- =========================
-- GESTOR SETOR
-- =========================

UPDATE Setor
SET IdGestor = 1
WHERE IdSetor = 2;




-- =========================
-- TEMPLATE
-- =========================

INSERT INTO PhishingTemplate
(
NomeTemplate,
Subject,
BodyMail,
Categoria,
NivelDificuldade
)
VALUES
(
'Template IFood',
'Clube IFood Beneficios',

'<html>
<body>

<h1>iFood</h1>

<p>
Olá {{Nome}},
</p>

<p>
Chegou o Clube iFood.
Acesse aqui:
{{Link}}
</p>

</body>
</html>',

'FakeMail',
5
);



-- =========================
-- PARAMETROS
-- =========================

INSERT INTO TemplateParameter
(
IdTemplate,
ParameterName,
ExampleValue
)
VALUES
(1,'Nome','Bruno'),
(1,'Link','https://fake-login.com');




-- =========================
-- TEMPLATE EMPRESA
-- =========================

INSERT INTO PhishingTemplateEmpresa
(
IdEmpresa,
IdTemplate,
NomePersonalizado
)
VALUES
(
1,
1,
'Template Teste Empresa'
);




-- =========================
-- PARAMETER VALUE
-- =========================

INSERT INTO ParameterValue
(
IdParameter,
IdTemplateEmpresa,
ParameterValue
)
VALUES
(
1,
1,
'Bruno Mazetto'
),
(
2,
1,
'https://fake-login.com'
);





-- =========================
-- CAMPANHA
-- =========================

INSERT INTO PhishingCampaign
(
IdEmpresa,
IdTemplateEmpresa,
IdSetor,
NomeCampanha,
Dt_Disparo,
Status
)
VALUES
(
1,
1,
NULL,
'Campanha Teste',
NOW(),
'PENDENTE'
);




-- =========================
-- TARGETS
-- =========================

INSERT INTO PhishingCampaignTarget
(
IdCampaign,
IdUser,
MailSent,
MailOpened,
LinkClicked,
CredentialsSubmitted,
Reported,
Dt_Register
)
VALUES

(1,1,0,0,0,0,0,NOW()),
(1,2,0,0,0,0,0,NOW()),
(1,3,0,0,0,0,0,NOW()),
(1,4,0,0,0,0,0,NOW()),
(1,5,0,0,0,0,0,NOW());




-- =========================
-- EMAIL SMTP
-- =========================

INSERT INTO MailCredentials
(
Mail,
Senha
)
VALUES
(
'security@phishingminds.com',
'1'
);