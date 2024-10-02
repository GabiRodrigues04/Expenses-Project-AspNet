namespace ProjetoEconomias.Models
{
    public class Entrada
    {

            public int EntradaID { get; set; } 
            public int MesID { get; set; }
            public DateTime DataEntrada { get; set; }
            public string Descricao { get; set; }
            public decimal Valor { get; set; }

        }

    }
