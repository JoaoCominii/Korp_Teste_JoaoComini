using Microsoft.EntityFrameworkCore;
using Korp_Teste_JoaoComini.Data;

namespace Korp_Teste_JoaoComini.Services
{
    public class EstoqueService
    {
        private readonly BancoContexto _contexto;

        public EstoqueService(BancoContexto contexto)
        {
            _contexto = contexto;
        }

        public void CadastrarProduto(string codigo, string descricao, int saldo)
        {
            if (_contexto.Produtos.Any(p => p.Codigo == codigo))
                throw new ArgumentException($"Produto com código '{codigo}' já cadastrado");

            var produto = new Produto { Codigo = codigo, Descricao = descricao, Saldo = saldo };
            _contexto.Produtos.Add(produto);
            _contexto.SaveChanges();
            Console.WriteLine($"[Estoque] Produto cadastrado - Código: {codigo}, Descrição: {descricao}, Saldo: {saldo}");
        }

        public List<string> ObterCodigos()
        {
            return _contexto.Produtos.Select(p => p.Codigo).ToList();
        }

        public (string descricao, int saldo)? ObterProduto(string codigo)
        {
            var prod = _contexto.Produtos.FirstOrDefault(p => p.Codigo == codigo);
            if (prod == null)
                return null;
            return (prod.Descricao, prod.Saldo);
        }

        public void AtualizarProduto(string codigo, string? novaDescricao, int? novoSaldo)
        {
            var prod = _contexto.Produtos.FirstOrDefault(p => p.Codigo == codigo);
            if (prod == null)
                throw new KeyNotFoundException($"Produto com código '{codigo}' não encontrado");

            if (!string.IsNullOrEmpty(novaDescricao))
                prod.Descricao = novaDescricao;

            if (novoSaldo.HasValue && novoSaldo.Value >= 0)
                prod.Saldo = novoSaldo.Value;

            _contexto.SaveChanges();
            Console.WriteLine($"[Estoque] Produto atualizado - Código: {codigo}");
        }

        public void RemoverProduto(string codigo)
        {
            var prod = _contexto.Produtos.FirstOrDefault(p => p.Codigo == codigo);
            if (prod == null)
                throw new KeyNotFoundException($"Produto com código '{codigo}' não encontrado para remoção");

            _contexto.Produtos.Remove(prod);
            _contexto.SaveChanges();
            Console.WriteLine($"[Estoque] Produto removido - Código: {codigo}");
        }

        public int RemoverDoEstoqueIdempotente(string codigo, int quantidade)
        {
            if (!_contexto.Produtos.Any(p => p.Codigo == codigo))
            {
                Console.WriteLine($"[Estoque] Aviso: Produto com código '{codigo}' não existe");
                return 0;
            }

            var prod = _contexto.Produtos.First(p => p.Codigo == codigo);
            int disponivel = prod.Saldo;
            int removida = Math.Min(quantidade, disponivel);

            if (removida < quantidade)
                Console.WriteLine($"[Estoque] Aviso: Estoque insuficiente para código '{codigo}'.");

            prod.Saldo -= removida;
            _contexto.SaveChanges();
            Console.WriteLine($"[Estoque] Idempotente: Removido {removida} do código '{codigo}'. Restante: {prod.Saldo}");
            return removida;
        }
    }
}
