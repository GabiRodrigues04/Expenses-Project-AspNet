using Microsoft.AspNetCore.Mvc;
using ProjetoEconomias.Models;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Dynamic;

namespace ProjetoEconomias.Controllers
{
    public class HomeController : Controller
    {

        private readonly IDbConnection _dbConnection;

        public HomeController(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

        public IActionResult Index(int?mes)
        {

            int mesSelecionado = mes ?? DateTime.Now.Month;

            var viewModel = new FinanceiroViewModel
            {
                Entradas = GetEntradas(mesSelecionado),
                Despesas = GetDespesas(mesSelecionado),
                Resumo = GetResumos(mesSelecionado),
                Notas = GetNotas(mesSelecionado),
                Meses = GetMeses()
            };

            return View(viewModel);
        }

        private List<Entrada> GetEntradas(int mesSelecionado) {

            var entradas = new List<Entrada>();

            string query = "SELECT * FROM Entradas WHERE MesID = @mesSelecionado";

            _dbConnection.Open();

            using (var command = _dbConnection.CreateCommand()) {
                command.CommandText = query;
                var parameter = command.CreateParameter();
                parameter.ParameterName = "@mesSelecionado";
                parameter.Value = mesSelecionado;

                command.Parameters.Add(parameter);
                using (var reader = command.ExecuteReader()) {

                    while (reader.Read()) {
                        entradas.Add(new Entrada{

                            EntradaID = Convert.ToInt32(reader["EntradaID"]),
                            MesID = Convert.ToInt32(reader["MesID"]),
                            DataEntrada = Convert.ToDateTime(reader["DataEntrada"]),
                            Descricao = reader["Descricao"].ToString(),
                            Valor = Convert.ToDecimal(reader["Valor"])

                        });
                    }
                }
            }
            _dbConnection.Close();
            return entradas;
        }

        private List<Despesa> GetDespesas(int mesSelecionado)
        {

            var despesas = new List<Despesa>();

            string query = "SELECT * FROM Despesas WHERE MesID = @mesSelecionado";

            _dbConnection.Open();

            using (var command = _dbConnection.CreateCommand())
            {
                command.CommandText = query;
                var parameter = command.CreateParameter();
                parameter.ParameterName = "@mesSelecionado";
                parameter.Value = mesSelecionado;

                command.Parameters.Add(parameter);
                using (var reader = command.ExecuteReader())
                {

                    while (reader.Read())
                    {
                        despesas.Add(new Despesa
                        {

                            DespesaID = Convert.ToInt32(reader["DespesaID"]),
                            MesID = Convert.ToInt32(reader["MesID"]),
                            DataDespesa = Convert.ToDateTime(reader["DataDespesa"]),
                            Descricao = reader["Descricao"].ToString(),
                            Valor = Convert.ToDecimal(reader["Valor"])

                        });
                    }
                }
            }
            _dbConnection.Close();
            return despesas;
        }

        private Resumo GetResumos(int mesSelecionado)
        {

            decimal entradasTotais = 0m;
            decimal despesasTotais = 0m;

            string queryEntradas = "SELECT SUM(Valor) FROM Entradas WHERE MesID = @mesSelecionado";
            using (var command = _dbConnection.CreateCommand())
            {
                command.CommandText = queryEntradas;
                var parameter = command.CreateParameter();
                parameter.ParameterName = "@mesSelecionado";
                parameter.Value = mesSelecionado;

                command.Parameters.Add(parameter);
                _dbConnection.Open();
                var result = command.ExecuteScalar();
                if (result != DBNull.Value)
                {
                    entradasTotais = (decimal)result;
                }
                _dbConnection.Close();
            }

            string queryDespesas = "SELECT SUM(Valor) FROM Despesas WHERE MesID = @mesSelecionado";
            using (var command = _dbConnection.CreateCommand())
            {
                command.CommandText = queryDespesas;
                var parameter = command.CreateParameter();
                parameter.ParameterName = "@mesSelecionado";
                parameter.Value = mesSelecionado;

                command.Parameters.Add(parameter);
                _dbConnection.Open();
                var result = command.ExecuteScalar();
                if (result != DBNull.Value)
                {
                    despesasTotais = (decimal)result;
                }
                _dbConnection.Close();
            }
            return new Resumo
            {
                MesID = mesSelecionado,
                EntradasTotais = entradasTotais,
                DespesasTotais = despesasTotais,
                EconomiaTotal = entradasTotais - despesasTotais
            };
        }

        private List<Nota> GetNotas(int mesSelecionado)
        {

            var notas = new List<Nota>();

            string query = "SELECT * FROM Notas WHERE MesID = @mesSelecionado";

            _dbConnection.Open();

            using (var command = _dbConnection.CreateCommand())
            {
                command.CommandText = query;
                var parameter = command.CreateParameter();
                parameter.ParameterName = "@mesSelecionado";
                parameter.Value = mesSelecionado;

                command.Parameters.Add(parameter);
                using (var reader = command.ExecuteReader())
                {

                    while (reader.Read())
                    {
                        notas.Add(new Nota
                        {

                            NotaID = Convert.ToInt32(reader["NotaID"]),
                            MesID = Convert.ToInt32(reader["MesID"]),
                            TextoNota = reader["TextoNota"].ToString(),
           

                        });
                    }
                }
            }
            _dbConnection.Close();
            return notas;
        }

        private List<Mes> GetMeses()
        {
            var meses = new List<Mes>();
            string query = "SELECT * FROM Meses";
            _dbConnection.Open();
            using (var command = _dbConnection.CreateCommand())
            {
                command.CommandText = query;
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        meses.Add(new Mes
                        {
                            MesID = Convert.ToInt32(reader["MesID"]),
                            NomeMes = reader["NomeMes"].ToString(),
                            NomeInteiro = reader["NomeInteiro"].ToString()

                        });
                    }
                }
            }
            _dbConnection.Close();
            return meses;
        }

        [HttpPost]
        public IActionResult CreateEntrada(Entrada entrada)
        {
            string query = "INSERT INTO Entradas (MesID, DataEntrada, Descricao, Valor) VALUES (@MesID, @DataEntrada, @Descricao, @Valor)";

            _dbConnection.Open();
            using (var command = _dbConnection.CreateCommand())
            {
                command.CommandText = query;
                command.Parameters.Add(new SqlParameter("@MesID", entrada.MesID));
                command.Parameters.Add(new SqlParameter("@DataEntrada", DateTime.Now));
                command.Parameters.Add(new SqlParameter("@Descricao", entrada.Descricao ?? (object)DBNull.Value));
                command.Parameters.Add(new SqlParameter("@Valor", entrada.Valor));

                command.ExecuteNonQuery();
            }
            _dbConnection.Close();

            return RedirectToAction("Index", new { mes = entrada.MesID });
        }

        [HttpPost]
        public IActionResult CreateDespesa(Despesa despesa)
        {
            string query = "INSERT INTO Despesas (MesID, DataDespesa, Descricao, Valor) VALUES (@MesID, @DataDespesa, @Descricao, @Valor)";

            _dbConnection.Open();
            using (var command = _dbConnection.CreateCommand())
            {
                command.CommandText = query;
                command.Parameters.Add(new SqlParameter("@MesID", despesa.MesID));
                command.Parameters.Add(new SqlParameter("@DataDespesa", DateTime.Now));
                command.Parameters.Add(new SqlParameter("@Descricao", despesa.Descricao ?? (object)DBNull.Value));
                command.Parameters.Add(new SqlParameter("@Valor", despesa.Valor));

                command.ExecuteNonQuery();
            }
            _dbConnection.Close();

            return RedirectToAction("Index", new { mes = despesa.MesID });
        }

        [HttpPost]
        public IActionResult DeleteEntrada(int entradaID, int mesID)
        {
            string query = "DELETE FROM Entradas WHERE EntradaID = @EntradaID";

            _dbConnection.Open();
            using (var command = _dbConnection.CreateCommand())
            {
                command.CommandText = query;
                command.Parameters.Add(new SqlParameter("@EntradaID", entradaID));

                command.ExecuteNonQuery();
            }
            _dbConnection.Close();

            return RedirectToAction("Index", new { mes = mesID });
        }

        [HttpPost]
        public IActionResult DeleteDespesa(int despesaID, int mesID)
        {
            string query = "DELETE FROM Despesas WHERE DespesaID = @DespesaID";

            _dbConnection.Open();
            using (var command = _dbConnection.CreateCommand())
            {
                command.CommandText = query;
                command.Parameters.Add(new SqlParameter("@DespesaID", despesaID));

                command.ExecuteNonQuery();
            }
            _dbConnection.Close();

            return RedirectToAction("Index", new { mes = mesID });
        }

        [HttpPost]
        public IActionResult CreateNota(Nota nota){

            string query = "INSERT INTO Notas (MesID, TextoNota) values (@MesID, @TextoNota)";

            _dbConnection.Open();
            using (var command = _dbConnection.CreateCommand())
            {
                command.CommandText = query;
                command.Parameters.Add(new SqlParameter("@MesID", nota.MesID));
                command.Parameters.Add(new SqlParameter("@TextoNota", nota.TextoNota));

                command.ExecuteNonQuery();
            }
            _dbConnection.Close();

            return RedirectToAction("Index", new { mes = nota.MesID });

        }

        [HttpPost]
        public IActionResult UpdateNota(Nota nota, int mesID)
        {
            string query = "UPDATE Notas SET TextoNota = @TextoNota WHERE MesID = @MesID";

            _dbConnection.Open();
            using (var command = _dbConnection.CreateCommand())
            {
                command.CommandText = query;
                command.Parameters.Add(new SqlParameter("@TextoNota", nota.TextoNota));
                command.Parameters.Add(new SqlParameter("@MesID", mesID));

                command.ExecuteNonQuery();
            }
            _dbConnection.Close();

            return RedirectToAction("Index", new { mes = mesID });
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }


}
