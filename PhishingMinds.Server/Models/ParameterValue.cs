namespace PhishingMinds.Server.Class
{
    public class ParameterValue
    {
        public int IdParameterValue { get; set; }
        public int IdParameter { get; set; }
        public int IdTemplateEmpresa { get; set; }
        public string ParameterValueText { get; set; }

        // helper
        public string? ParameterName { get; set; }
    }
}