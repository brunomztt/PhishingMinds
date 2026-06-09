namespace PhishingMinds.Server.Class
{
    public class ParameterValueEntry
    {
        public int IdParameterValue { get; set; }
        public int IdParameter { get; set; }
        public int IdTemplateEmpresa { get; set; }
        public string ParameterValue { get; set; }

        // helper
        public string? ParameterName { get; set; }
    }
}