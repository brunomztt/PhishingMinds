namespace PhishingMinds.Server.Class
{
    public class TemplateParameter
    {
        public int IdParameter { get; set; }
        public int IdTemplate { get; set; }
        public string ParameterName { get; set; }
        public string ExampleValue { get; set; }

        // helper
        public string? NomeTemplate { get; set; }
    }
}