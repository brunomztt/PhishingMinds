namespace PhishingMinds.Server.Class
{
    public class PhishingCampaignTarget
    {
        public int IdTarget { get; set; }
        public int IdCampaign { get; set; }
        public int IdUser { get; set; }

        public bool MailSent { get; set; }
        public bool MailOpened { get; set; }
        public bool LinkClicked { get; set; }
        public bool CredentialsSubmitted { get; set; }
        public bool Reported { get; set; }

        public DateTime Dt_Register { get; set; }

        // helper
        public string? NomeUsuario { get; set; }
        public string? NomeCampanha { get; set; }
    }
}