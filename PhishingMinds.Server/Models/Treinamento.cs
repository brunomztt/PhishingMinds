namespace PhishingMinds.Server.Class
{
    public class Treinamento
    {
        public int IdTreinamento { get; set; }
        public int IdUser { get; set; }
        public bool Aprovado { get; set; }
        public DateTime DtConclusao { get; set; }
    }
}