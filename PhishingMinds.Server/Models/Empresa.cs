namespace PhishingMinds.Server.Class
{
    public class Empresa
    {
        public int IdEmpresa { get; set; }
        public int IdPlano { get; set; }

        public string Nm_Empresa { get; set; }
        public string Nm_Dono { get; set; }
        public string Mail { get; set; }
        public string CNPJ { get; set; }

        public DateTime Dt_Cadastro { get; set; }
        public DateTime Dt_Contratacao { get; set; }
        public DateTime Dt_FimContrato { get; set; }

        public bool Ativo { get; set; }
    }
}