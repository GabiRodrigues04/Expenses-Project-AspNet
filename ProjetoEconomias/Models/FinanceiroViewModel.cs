namespace ProjetoEconomias.Models
{
    public class FinanceiroViewModel
    {

        public List<Entrada> Entradas { get; set; }
        public List<Despesa> Despesas { get; set; }
        public List<Mes> Meses { get; set; }
        public Resumo Resumo { get; set; }
        public List<Nota> Notas {get; set; }

    }
}



