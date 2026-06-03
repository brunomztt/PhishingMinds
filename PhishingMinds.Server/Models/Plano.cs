namespace PhishingMinds.Server.Class
{
    public class Plano
    {
        public int Id_Plano { get; set; }

        public string Nm_Plano { get; set; } = string.Empty;

        public string Desc_Plano { get; set; } = string.Empty;

        public string Temp_Plano { get; set; } = string.Empty;

        public string Value_Plano { get; set; } = string.Empty;

        public int MaxUsers { get; set; }

        public int MaxCampaigns { get; set; }
    }
}
