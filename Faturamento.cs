using Microsoft.EntityFrameworkCore;
using Korp_Teste_JoaoComini.Data;

namespace Korp_Teste_JoaoComini.Services
{
    public class FaturamentoService
    {
        private readonly BancoContexto _contexto;
        public EstoqueService EstoqueService { get; set; }

        public FaturamentoService()
        {
            _contexto = new BancoContexto();
            EstoqueService = new EstoqueService();
            // Garante que o banco existe
            _contexto.Database.EnsureCreated();
        }

        // Cadastra nota fiscal com produtos
        public void CadastrarNotafiscal(List<(string codigo, int quantidade)> produtos)
        {
            var nota = new NotaFiscal
            {
                Numero = $"NF-{DateTime.Now.Year}-{new Random().Next(1000, 9999)}",
                Status = "Aberta",
                Produtos = produtos,
                SaldoTotal = produtos.Sum(p => p.quantidade)
            };

            // Salva os produtos no estoque (se não existirem)
            foreach (var (codigo, qtd) in produtos)
            {
                if (!_contexto.Produtos.Any(p => p.Codigo == codigo))
                {
                    var prod = new Produto { Codigo = codigo, Descricao = $"Produto {codigo}", Saldo = qtd };
                    _contexto.Produtos.Add(prod);
                }
            }
            _contexto.SaveChanges();

            // Adiciona a nota
            _contexto.NotasFiscais.Add(nota);
            _contexto.SaveChanges();

            Console.WriteLine($"[Faturamento] Nota fiscal cadastrada: {nota.Numero}");
        }

        // Listar todas notas fiscais
        public List<NotaFiscal> ListarNotasFiscais()
        {
            return _contexto.NotasFiscais.ToList();
        }

        // Imprimir nota fiscal
        public bool ImprimirNotafiscal(string status, EstoqueService estoqueService)
        {
            // Pegar a última nota da lista para simplificar
            var ultimaNota = _contexto.NotasFiscais.OrderByDescending(n => n.Numero).FirstOrDefault();

            if (ultimaNota == null || ultimaNota.Status != "Aberta")
            {
                Console.WriteLine("[Faturamento] Erro: Nota não encontrada ou não está 'Aberta'.");
                return false;
            }

            // Processa cada produto da nota
            foreach (var (produto, quantidade) in ultimaNota.Produtos)
            {
                EstoqueService.RemoverDoEstoqueIdempotente(produto, quantidade);
            }

            // Atualiza status para Fechada
            ultimaNota.Status = "Fechada";
            _contexto.SaveChanges();

            Console.WriteLine("[Faturamento] Indicador de processamento: CONCLUÍDO");
            Console.WriteLine($"[Faturamento] Nota '{ultimaNota.Numero}' FINALIZADA. Status alterado para: Fechada");

            return true;
        }
    }
}