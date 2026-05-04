using System;
using System.Collections.Generic;
using System.Text;

namespace EmailDispatcher.Domain.Entities
{
    public class User
    {
        public int IdTarget { get; set; }
        public string Nome { get; set; }
        public string Email { get; set; }
    }
}
