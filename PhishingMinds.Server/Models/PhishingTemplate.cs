namespace PhishingMinds.Server.Class
{
    public class PhishingTemplate
    {
        public int IdTemplate { get; set; }
        public string NomeTemplate { get; set; }
        public string Subject { get; set; }
        public string BodyMail { get; set; }
        public string Categoria { get; set; }
        public int NivelDificuldade { get; set; }
    }
}