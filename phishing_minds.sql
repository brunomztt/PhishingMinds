CREATE DATABASE phishing_minds;
USE phishing_minds;

-- =========================
-- TABELA PLANO
-- =========================
CREATE TABLE Plano (
    IdPlano INT AUTO_INCREMENT PRIMARY KEY,
    Nm_Plano VARCHAR(100),
    Desc_Plano TEXT,
    Temp_Plano INT,
    Value_Plano DECIMAL(10,2),
    MaxUsers INT,
    MaxCampaigns INT
);

-- =========================
-- TABELA EMPRESA
-- =========================
CREATE TABLE Empresa (
    IdEmpresa INT AUTO_INCREMENT PRIMARY KEY,
    IdPlano INT,
    Nm_Empresa VARCHAR(200),
    Nm_Dono VARCHAR(200),
    Mail VARCHAR(200),
    CNPJ VARCHAR(20),
    Dt_Cadastro DATETIME,
    Dt_Contratacao DATETIME,
    Dt_FimContrato DATETIME,
    Ativo BOOLEAN,
    FOREIGN KEY (IdPlano) REFERENCES Plano(IdPlano)
);

-- =========================
-- TABELA CARGO
-- =========================
CREATE TABLE Cargo (
    IdCargo INT AUTO_INCREMENT PRIMARY KEY,
    Nm_Cargo VARCHAR(100)
);

-- =========================
-- TABELA SETOR
-- =========================
CREATE TABLE Setor (
    IdSetor INT AUTO_INCREMENT PRIMARY KEY,
    IdEmpresa INT,
    Nm_Setor VARCHAR(100),
    IdGestor INT NULL,
    FOREIGN KEY (IdEmpresa) REFERENCES Empresa(IdEmpresa)
);

-- =========================
-- TABELA PESSOA
-- =========================
CREATE TABLE Pessoa (
    IdUser INT AUTO_INCREMENT PRIMARY KEY,
    IdEmpresa INT,
    IdSetor INT,
    IdCargo INT,
    Nome VARCHAR(200),
    Email VARCHAR(200),
    Senha VARCHAR(200),
    Ativo BOOLEAN,
    Dt_Cadastro DATETIME,
    UltimoLogin DATETIME,
    PhishingScore INT DEFAULT 0,
    FOREIGN KEY (IdEmpresa) REFERENCES Empresa(IdEmpresa),
    FOREIGN KEY (IdSetor) REFERENCES Setor(IdSetor),
    FOREIGN KEY (IdCargo) REFERENCES Cargo(IdCargo)
);

-- agora que Pessoa existe, adicionamos FK do gestor
ALTER TABLE Setor
ADD CONSTRAINT fk_setor_gestor
FOREIGN KEY (IdGestor) REFERENCES Pessoa(IdUser);

-- =========================
-- TABELA TEMPLATE
-- =========================
CREATE TABLE PhishingTemplate (
    IdTemplate INT AUTO_INCREMENT PRIMARY KEY,
    NomeTemplate VARCHAR(200),
    Subject VARCHAR(300),
    BodyMail TEXT,
    Categoria VARCHAR(100),
    NivelDificuldade INT
);

-- =========================
-- PARAMETROS DO TEMPLATE
-- =========================
CREATE TABLE TemplateParameter (
    IdParameter INT AUTO_INCREMENT PRIMARY KEY,
    IdTemplate INT,
    ParameterName VARCHAR(100),
    ExampleValue VARCHAR(200),
    FOREIGN KEY (IdTemplate) REFERENCES PhishingTemplate(IdTemplate)
);

-- =========================
-- TEMPLATE CUSTOM EMPRESA
-- =========================
CREATE TABLE PhishingTemplateEmpresa (
    IdTemplateEmpresa INT AUTO_INCREMENT PRIMARY KEY,
    IdEmpresa INT,
    IdTemplate INT,
    NomePersonalizado VARCHAR(200),
    FOREIGN KEY (IdEmpresa) REFERENCES Empresa(IdEmpresa),
    FOREIGN KEY (IdTemplate) REFERENCES PhishingTemplate(IdTemplate)
);

-- =========================
-- VALORES DOS PARAMETROS
-- =========================
CREATE TABLE ParameterValue (
    IdParameterValue INT AUTO_INCREMENT PRIMARY KEY,
    IdParameter INT,
    IdTemplateEmpresa INT,
    ParameterValue VARCHAR(200),
    FOREIGN KEY (IdParameter) REFERENCES TemplateParameter(IdParameter),
    FOREIGN KEY (IdTemplateEmpresa) REFERENCES PhishingTemplateEmpresa(IdTemplateEmpresa)
);

-- =========================
-- CAMPANHAS
-- =========================
CREATE TABLE PhishingCampaign (
    IdCampaign INT AUTO_INCREMENT PRIMARY KEY,
    IdEmpresa INT,
    IdTemplateEmpresa INT,
    IdSetor INT NULL,
    NomeCampanha VARCHAR(200),
    Dt_Disparo DATETIME,
    Status VARCHAR(50),
    FOREIGN KEY (IdEmpresa) REFERENCES Empresa(IdEmpresa),
    FOREIGN KEY (IdTemplateEmpresa) REFERENCES PhishingTemplateEmpresa(IdTemplateEmpresa),
    FOREIGN KEY (IdSetor) REFERENCES Setor(IdSetor)
);

-- =========================
-- RESULTADO DO USUARIO
-- =========================
CREATE TABLE PhishingCampaignTarget (
    IdTarget INT AUTO_INCREMENT PRIMARY KEY,
    IdCampaign INT,
    IdUser INT,
    MailSent BOOLEAN,
    MailOpened BOOLEAN,
    LinkClicked BOOLEAN,
    CredentialsSubmitted BOOLEAN,
    Reported BOOLEAN,
    Dt_Register DATETIME,
    FOREIGN KEY (IdCampaign) REFERENCES PhishingCampaign(IdCampaign),
    FOREIGN KEY (IdUser) REFERENCES Pessoa(IdUser)
);

-- =========================
-- LANDING PAGE FAKE
-- =========================
CREATE TABLE LandingPage (
    IdLandingPage INT AUTO_INCREMENT PRIMARY KEY,
    Nome VARCHAR(200),
    HtmlPage TEXT,
    Categoria VARCHAR(100)
);

-- =========================
-- EMAIL PARA DISPARO
-- =========================
CREATE TABLE MailCredentials (
    Id_EmailCredentials INT AUTO_INCREMENT PRIMARY KEY,
    Mail VARCHAR(200),
    Senha VARCHAR(200)
);