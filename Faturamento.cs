using Microsoft.EntityFrameworkCore;
using Korp_Teste_JoaoComini.Data;

namespace Korp_Teste_JoaoComini.Services
{
    public class FaturamentoService
    {
        private readonly BancoContexto _contexto;

        public FaturamentoService(BancoContexto contexto)
        {
            _contexto = contexto;
        }

        public void CadastrarNotafiscal(List<ProdutoNota> produtos)
        {
            var nota = new NotaFiscal
            {
                Numero = $"NF-{DateTime.Now.Year}-{new Random().Next(1000, 9999)}",
                Status = "Aberta",
                Produtos = produtos,
                SaldoTotal = produtos.Sum(p => p.Quantidade)
            };

            _contexto.NotasFiscais.Add(nota);
            _contexto.SaveChanges();

            Console.WriteLine($"[Faturamento] Nota fiscal cadastrada: {nota.Numero}");
        }

        public List<NotaFiscal> ListarNotasFiscais()
        {
            return _contexto.NotasFiscais.ToList();
        }

        public bool ImprimirNotafiscal(string numero, EstoqueService estoqueService)
        {
            var nota = _contexto.NotasFiscais.FirstOrDefault(n => n.Numero == numero);

            if (nota == null || nota.Status != "Aberta")
            {
                Console.WriteLine("[Faturamento] Erro: Nota não encontrada ou não está 'Aberta'.");
                return false;
            }

            if (nota.Produtos == null || nota.Produtos.Count == 0)
            {
                Console.WriteLine("[Faturamento] Erro: Nota não contém produtos.");
                return false;
            }

            Console.WriteLine($"[Faturamento] Imprimindo nota '{nota.Numero}' com {nota.Produtos.Count} produto(s)...");

            foreach (var pn in nota.Produtos)
            {
                Console.WriteLine($"[Faturamento] Removendo {pn.Quantidade}x do produto '{pn.Codigo}'...");
                int removido = estoqueService.RemoverDoEstoqueIdempotente(pn.Codigo, pn.Quantidade);
                Console.WriteLine($"[Faturamento] Removido {removido}x do produto '{pn.Codigo}'.");
            }

            nota.Status = "Fechada";
            _contexto.SaveChanges();

            Console.WriteLine($"[Faturamento] Nota '{nota.Numero}' FINALIZADA. Status alterado para: Fechada");

            return true;
        }
    }
}
