namespace PhishingMinds.Server.Class
{
    public class Empresa
    {
        int IdPlano { get; set; }
        string Nm_Empresa { get; set; }
        string Nm_Dono { get; set; }
        string Mail { get; set; }
        string CNPJ { get; set; }
        string Dt_Cadastro { get; set; }
        string Dt_Contratacao { get; set; }
        string Dt_FimContrato { get; set; }
        bool Ativo { get; set; }
    }
}
