namespace PhishingMinds.Server.Class
{
    public class PhishingTemplateEmpresa
    {
        public int IdTemplateEmpresa { get; set; }
        public int IdEmpresa { get; set; }
        public int IdTemplate { get; set; }
        public string NomePersonalizado { get; set; }

        // helper
        public string? NomeTemplate { get; set; }
        public string? Nm_Empresa { get; set; }
    }
}