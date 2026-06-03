DROP DATABASE IF EXISTS phishing_minds;
CREATE DATABASE phishing_minds
CHARACTER SET utf8mb4
COLLATE utf8mb4_unicode_ci;

USE phishing_minds;


-- =========================
-- PLANO
-- =========================
CREATE TABLE Plano (
    IdPlano INT AUTO_INCREMENT PRIMARY KEY,
    Nm_Plano VARCHAR(100) NOT NULL,
    Desc_Plano TEXT,
    Temp_Plano INT,
    Value_Plano DECIMAL(10,2),
    MaxUsers INT,
    MaxCampaigns INT
) ENGINE=InnoDB;


-- =========================
-- EMPRESA
-- =========================
CREATE TABLE Empresa (
    IdEmpresa INT AUTO_INCREMENT PRIMARY KEY,
    IdPlano INT,

    Nm_Empresa VARCHAR(200) NOT NULL,
    Nm_Dono VARCHAR(200),
    Mail VARCHAR(200) UNIQUE,
    CNPJ VARCHAR(20),

    Dt_Cadastro DATETIME DEFAULT CURRENT_TIMESTAMP,
    Dt_Contratacao DATETIME,
    Dt_FimContrato DATETIME,

    Ativo TINYINT(1) DEFAULT 1,

    Senha VARCHAR(255),
    Foto VARCHAR(500),

    FOREIGN KEY (IdPlano)
        REFERENCES Plano(IdPlano)
        ON DELETE SET NULL
) ENGINE=InnoDB;



-- =========================
-- CARGO
-- =========================
CREATE TABLE Cargo (
    IdCargo INT AUTO_INCREMENT PRIMARY KEY,
    Nm_Cargo VARCHAR(100) NOT NULL
) ENGINE=InnoDB;



-- =========================
-- SETOR
-- =========================
CREATE TABLE Setor (
    IdSetor INT AUTO_INCREMENT PRIMARY KEY,

    IdEmpresa INT NOT NULL,

    Nm_Setor VARCHAR(100) NOT NULL,

    IdGestor INT NULL,

    FOREIGN KEY(IdEmpresa)
        REFERENCES Empresa(IdEmpresa)
        ON DELETE CASCADE
) ENGINE=InnoDB;



-- =========================
-- PESSOA
-- =========================
CREATE TABLE Pessoa (
    IdUser INT AUTO_INCREMENT PRIMARY KEY,

    IdEmpresa INT NOT NULL,
    IdSetor INT,
    IdCargo INT,

    Nome VARCHAR(200) NOT NULL,
    Email VARCHAR(200) NOT NULL UNIQUE,

    Senha VARCHAR(255),

    Ativo TINYINT(1) DEFAULT 1,

    Dt_Cadastro DATETIME DEFAULT CURRENT_TIMESTAMP,

    UltimoLogin DATETIME,

    PhishingScore INT DEFAULT 0,


    FOREIGN KEY(IdEmpresa)
        REFERENCES Empresa(IdEmpresa)
        ON DELETE CASCADE,


    FOREIGN KEY(IdSetor)
        REFERENCES Setor(IdSetor)
        ON DELETE SET NULL,


    FOREIGN KEY(IdCargo)
        REFERENCES Cargo(IdCargo)
        ON DELETE SET NULL


) ENGINE=InnoDB;



-- agora cria gestor
ALTER TABLE Setor
ADD CONSTRAINT fk_setor_gestor
FOREIGN KEY(IdGestor)
REFERENCES Pessoa(IdUser)
ON DELETE SET NULL;



-- =========================
-- TEMPLATE
-- =========================
CREATE TABLE PhishingTemplate (

    IdTemplate INT AUTO_INCREMENT PRIMARY KEY,

    NomeTemplate VARCHAR(200) NOT NULL,

    Subject VARCHAR(300),

    BodyMail TEXT,

    Categoria VARCHAR(100),

    NivelDificuldade INT

) ENGINE=InnoDB;




-- =========================
-- PARAMETROS TEMPLATE
-- =========================
CREATE TABLE TemplateParameter (

    IdParameter INT AUTO_INCREMENT PRIMARY KEY,

    IdTemplate INT NOT NULL,

    ParameterName VARCHAR(100),

    ExampleValue VARCHAR(200),


    FOREIGN KEY(IdTemplate)
        REFERENCES PhishingTemplate(IdTemplate)
        ON DELETE CASCADE

) ENGINE=InnoDB;




-- =========================
-- TEMPLATE EMPRESA
-- =========================
CREATE TABLE PhishingTemplateEmpresa (

    IdTemplateEmpresa INT AUTO_INCREMENT PRIMARY KEY,

    IdEmpresa INT NOT NULL,

    IdTemplate INT NOT NULL,

    NomePersonalizado VARCHAR(200),


    FOREIGN KEY(IdEmpresa)
        REFERENCES Empresa(IdEmpresa)
        ON DELETE CASCADE,


    FOREIGN KEY(IdTemplate)
        REFERENCES PhishingTemplate(IdTemplate)
        ON DELETE CASCADE


) ENGINE=InnoDB;




-- =========================
-- VALOR PARAMETRO
-- =========================
CREATE TABLE ParameterValue (

    IdParameterValue INT AUTO_INCREMENT PRIMARY KEY,

    IdParameter INT NOT NULL,

    IdTemplateEmpresa INT NOT NULL,

    ParameterValue VARCHAR(200),


    FOREIGN KEY(IdParameter)
        REFERENCES TemplateParameter(IdParameter)
        ON DELETE CASCADE,


    FOREIGN KEY(IdTemplateEmpresa)
        REFERENCES PhishingTemplateEmpresa(IdTemplateEmpresa)
        ON DELETE CASCADE


) ENGINE=InnoDB;




-- =========================
-- CAMPANHA
-- =========================
CREATE TABLE PhishingCampaign (

    IdCampaign INT AUTO_INCREMENT PRIMARY KEY,

    IdEmpresa INT NOT NULL,

    IdTemplateEmpresa INT NOT NULL,

    IdSetor INT NULL,

    NomeCampanha VARCHAR(200),

    Dt_Disparo DATETIME,

    Status VARCHAR(50),



    FOREIGN KEY(IdEmpresa)
        REFERENCES Empresa(IdEmpresa)
        ON DELETE CASCADE,


    FOREIGN KEY(IdTemplateEmpresa)
        REFERENCES PhishingTemplateEmpresa(IdTemplateEmpresa)
        ON DELETE CASCADE,


    FOREIGN KEY(IdSetor)
        REFERENCES Setor(IdSetor)
        ON DELETE SET NULL


) ENGINE=InnoDB;





-- =========================
-- TARGET CAMPANHA
-- =========================
CREATE TABLE PhishingCampaignTarget (

    IdTarget INT AUTO_INCREMENT PRIMARY KEY,

    IdCampaign INT NOT NULL,

    IdUser INT NOT NULL,


    MailSent TINYINT(1) DEFAULT 0,

    MailOpened TINYINT(1) DEFAULT 0,

    LinkClicked TINYINT(1) DEFAULT 0,

    CredentialsSubmitted TINYINT(1) DEFAULT 0,

    Reported TINYINT(1) DEFAULT 0,


    Dt_Register DATETIME DEFAULT CURRENT_TIMESTAMP,



    FOREIGN KEY(IdCampaign)
        REFERENCES PhishingCampaign(IdCampaign)
        ON DELETE CASCADE,


    FOREIGN KEY(IdUser)
        REFERENCES Pessoa(IdUser)
        ON DELETE CASCADE,


    UNIQUE(IdCampaign, IdUser)


) ENGINE=InnoDB;





-- =========================
-- LANDING PAGE
-- =========================
CREATE TABLE LandingPage (

    IdLandingPage INT AUTO_INCREMENT PRIMARY KEY,

    Nome VARCHAR(200),

    HtmlPage TEXT,

    Categoria VARCHAR(100)

) ENGINE=InnoDB;





-- =========================
-- CREDENCIAIS EMAIL DISPARO
-- =========================
CREATE TABLE MailCredentials (

    Id_EmailCredentials INT AUTO_INCREMENT PRIMARY KEY,

    Mail VARCHAR(200),

    Senha VARCHAR(255)

) ENGINE=InnoDB;




-- =========================
-- INDICES
-- =========================

CREATE INDEX idx_empresa_nome 
ON Empresa(Nm_Empresa);


CREATE INDEX idx_pessoa_empresa 
ON Pessoa(IdEmpresa);


CREATE INDEX idx_pessoa_setor 
ON Pessoa(IdSetor);


CREATE INDEX idx_campaign_empresa 
ON PhishingCampaign(IdEmpresa);


CREATE INDEX idx_target_campaign 
ON PhishingCampaignTarget(IdCampaign);


CREATE INDEX idx_target_user 
ON PhishingCampaignTarget(IdUser);