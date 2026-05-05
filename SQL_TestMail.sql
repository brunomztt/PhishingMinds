-- =========================
-- LIMPEZA
-- =========================
SET FOREIGN_KEY_CHECKS = 0;

TRUNCATE TABLE PhishingCampaignTarget;
TRUNCATE TABLE PhishingCampaign;
TRUNCATE TABLE PhishingTemplateEmpresa;
TRUNCATE TABLE PhishingTemplate;
TRUNCATE TABLE Pessoa;
TRUNCATE TABLE Setor;
TRUNCATE TABLE Cargo;
TRUNCATE TABLE Empresa;
TRUNCATE TABLE Plano;

SET FOREIGN_KEY_CHECKS = 1;

-- =========================
-- BASE
-- =========================

INSERT INTO Plano VALUES (1, 'Teste', 'Plano teste', 30, 0, 100, 10);

INSERT INTO Empresa 
VALUES (1, 1, 'Empresa Teste', 'Bruno', 'teste@empresa.com', '00000000000100', NOW(), NOW(), NULL, 1);

INSERT INTO Cargo VALUES (1, 'Funcionário');

INSERT INTO Setor VALUES (1, 1, 'TI', NULL);

-- =========================
-- USUÁRIOS (MULTIPLOS)
-- =========================
INSERT INTO Pessoa 
(IdUser, IdEmpresa, IdSetor, IdCargo, Nome, Email, Senha, Ativo, Dt_Cadastro)
VALUES
(1, 1, 1, 1, 'Bruno Mazetto', 'bruno.maz3tto@gmail.com', '123', 1, NOW()),
(2, 1, 1, 1, 'Helen Bonato', 'helenbonato02@gmail.com', '123', 1, NOW()),
(3, 1, 1, 1, 'Leticia Fabri', 'leticiafabri10@gmail.com', '123', 1, NOW());

-- =========================
-- TEMPLATE
-- =========================
INSERT INTO PhishingTemplate (IdTemplate, NomeTemplate, Subject, BodyMail, Categoria, NivelDificuldade)
VALUES (
   1,
   'Template IFood',
   'Clube IFood Beneficios',
   '<html><style>@media (max-width:480px){body{font-size:14px;}h2{font-size:18px;}a{display:block;width:100%;text-align:center;}}</style><body style="margin:0;background:#fff;font-family:'Segoe UI',Arial,sans-serif;"><div style="background:#E50914;padding:24px;text-align:center;color:white;"><h1 style="margin:0;font-size:28px;font-weight:700;">iFood</h1><p style="margin:0;font-size:14px;opacity:0.9;">Chegou o Clube iFood 🍔✨</p></div><div style="max-width:640px;margin:0 auto;padding:32px;"><h2 style="margin:0;font-weight:600;color:#E50914;">Novo Plano de Benefícios</h2><p style="margin-top:16px;line-height:1.6;color:#333;">Olá {{Nome}},</p><p style="line-height:1.6;color:#333;">Chegou a nova parceria com o IFood 🤝 
Agora você pode aproveitar ainda mais suas refeições com o <strong>Clube iFood</strong>.</p><ul style="line-height:1.6;margin-left:20px;color:#333;"><li>Vale alimentação e refeição</li><li>Frete grátis em restaurantes parceiros</li><li>30% de desconto em produtos do mercado</li></ul><div style="text-align:center;margin:32px 0;"><a href="#" style="background:#E50914;color:white;padding:14px 26px;border-radius:24px;text-decoration:none;font-weight:600;">Ativar Clube</a></div><p style="font-size:13px;color:#777;text-align:center;"><a href="https://contrate-beneficios.ifood.com.br/ativacao-ifood-beneficios">Clique aqui</a> para visualizar todos os benefícios.</p><hr style="border:none;border-top:1px solid #eee;margin:32px 0;"><p style="font-size:12px;color:#777;text-align:center;">Esta é uma mensagem automática enviada pelo sistema iFood. Por favor, não responda a este e-mail.</p><p style="font-size:12px;color:#777;text-align:center;">© 2026 iFood - Osasco, São Paulo, Brasil. Todos os direitos reservados.</p><p style="font-size:12px;color:#777;text-align:center;">Para suporte, acesse a <a href="https://www.ifood.com.br/ajuda" style="color:#E50914;text-decoration:none;">Central de Ajuda</a> no aplicativo.</p></div></body></html>',
   'FakeMail',
   5
);



-- TEMPLATE EMPRESA
INSERT INTO PhishingTemplateEmpresa (IdTemplateEmpresa, IdEmpresa, IdTemplate, NomePersonalizado)
VALUES (1, 1, 1, 'Template IFood');


-- =========================
-- CAMPANHA
-- =========================
INSERT INTO PhishingCampaign 
VALUES (1, 1, 1, NULL, 'Campanha IFood', NOW() - INTERVAL 1 MINUTE, 'PENDENTE');

-- =========================
-- TARGETS (MULTIPLOS)
-- =========================
INSERT INTO PhishingCampaignTarget 
(IdTarget, IdCampaign, IdUser, MailSent, Dt_Register)
VALUES
(1, 1, 1, 0, NOW()),
(2, 1, 2, 0, NOW()),
(3, 1, 3, 0, NOW());
