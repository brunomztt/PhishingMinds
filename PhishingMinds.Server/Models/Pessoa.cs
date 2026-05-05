namespace PhishingMinds.Server.Class
{
    public class Pessoa
    {
        public int IdUser { get; set; }
        public int IdEmpresa { get; set; }
        public int? IdSetor { get; set; }
        public int? IdCargo { get; set; }
        public int IdGestor { get; set; }
        public string Nome { get; set; }
        public string Email { get; set; }
        public string Senha { get; set; }
        public bool Ativo { get; set; }
        public string Dt_cadastro { get; set; }
        public string UltimoLogin { get; set; }
        public int PhishingScore { get; set; }

        //helper properties (join)
        public string? Nm_Setor { get; set; }
        public string? Nm_Cargo { get; set; }
        public string? Nm_Gestor { get; set; }
    }
}
