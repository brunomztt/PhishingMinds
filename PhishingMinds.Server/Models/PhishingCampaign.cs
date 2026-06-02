namespace PhishingMinds.Server.Class
{
    public class PhishingCampaign
    {
        public int IdCampaign { get; set; }
        public int IdEmpresa { get; set; }
        public int IdTemplateEmpresa { get; set; }
        public int? IdSetor { get; set; }
        public string? NomeCampanha { get; set; }
        public DateTime Dt_Disparo { get; set; }
        public string? Status { get; set; }

        public List<int> IdSetores { get; set; } = new List<int>();

        // helper
        public string? Nm_Empresa { get; set; }
        public string? NomeTemplate { get; set; }
        public string? Nm_Setor { get; set; }
    }
}