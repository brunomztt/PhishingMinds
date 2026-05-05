using System;
using System.Collections.Generic;
using System.Text;

namespace EmailDispatcher.Domain.Entities
{
    public class MailCredential
    {
        public int Id { get; set; }
        public string Mail { get; set; }
        public string Senha { get; set; }
    }
}
