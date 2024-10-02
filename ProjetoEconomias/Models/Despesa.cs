namespace ProjetoEconomias.Models
{
    public class Despesa
    {

            public int DespesaID { get; set; }
            public int MesID { get; set; } 
            public DateTime DataDespesa { get; set; } 
            public string Descricao { get; set; } 
            public decimal Valor { get; set; } 

    }
}
