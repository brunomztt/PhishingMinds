using System;
using System.Collections.Generic;
using System.Text;

namespace EmailDispatcher.Domain.Entities
{
    public class Campaign
    {
        public int IdCampaign { get; set; }
        public int IdTemplateEmpresa { get; set; }
        public string Subject { get; set; }
        public string BodyMail { get; set; }
        public string NomeCampanha { get; set; }

    }
}
