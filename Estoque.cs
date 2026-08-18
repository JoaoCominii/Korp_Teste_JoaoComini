using Microsoft.EntityFrameworkCore;
using Korp_Teste_JoaoComini.Data;

namespace Korp_Teste_JoaoComini.Services
{
    public class EstoqueService
    {
        private readonly BancoContexto _contexto;

        public EstoqueService()
        {
            _contexto = new BancoContexto();
            // Garante que o banco e as tabelas existem ao criar o serviço
            _contexto.Database.EnsureCreated();
        }

        // Cadastra produto salvando no banco
        public void CadastrarProduto(string codigo, string descricao, int saldo)
        {
            // Verifica se já existe
            if (_contexto.Produtos.Any(p => p.Codigo == codigo))
                throw new ArgumentException($"Produto com código '{codigo}' já cadastrado");

            var produto = new Produto { Codigo = codigo, Descricao = descricao, Saldo = saldo };
            _contexto.Produtos.Add(produto);
            _contexto.SaveChanges();
            Console.WriteLine($"[Estoque] Produto cadastrado - Código: {codigo}, Descrição: {descricao}, Saldo: {saldo}");
        }

        // Listar todos os códigos
        public List<string> ObterCodigos()
        {
            return _contexto.Produtos.Select(p => p.Codigo).ToList();
        }

        // Obter produto por Código
        public (string descricao, int saldo)? ObterProduto(string codigo)
        {
            var prod = _contexto.Produtos.FirstOrDefault(p => p.Codigo == codigo);
            if (prod == null)
            {
                Console.WriteLine($"[Estoque] Aviso: Produto com código '{codigo}' não encontrado");
                return null;
            }
            return (prod.Descricao, prod.Saldo);
        }

        // Atualizar produto por Código
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

        // Remover produto por Código
        public void RemoverProduto(string codigo)
        {
            var prod = _contexto.Produtos.FirstOrDefault(p => p.Codigo == codigo);
            if (prod == null)
                throw new KeyNotFoundException($"Produto com código '{codigo}' não encontrado para remoção");

            _contexto.Produtos.Remove(prod);
            _contexto.SaveChanges();
            Console.WriteLine($"[Estoque] Produto removido - Código: {codigo}");
        }

        // Remover do estoque (idempotente - agora salva no banco)
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