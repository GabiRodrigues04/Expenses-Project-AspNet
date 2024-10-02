namespace ProjetoEconomias.Models
{
    public class Resumo
    {

        public int ResumoID { get; set; }
        public int MesID { get; set; }
        public decimal EntradasTotais { get; set; }
        public decimal DespesasTotais { get; set; }
        public decimal EconomiaTotal { get; set; }

    }
}
