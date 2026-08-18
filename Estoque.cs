using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Korp_Teste_JoaoComini.Services
{
    /// <summary>
    /// Serviço de Estoque - controla produtos e saldos (quantidade em estoque)
    /// </summary>
    public class EstoqueService
    {
        // Dicionário: produto -> saldo/quantidade em estoque
        // O "saldo" aqui representa a quantidade disponível de produtos
        private readonly Dictionary<string, int> _saldos;

        /// <summary>
        /// Construtor - inicializa o dicionário de saldos
        /// </summary>
        public EstoqueService()
        {
            _saldos = new Dictionary<string, int>();
        }

        /// <summary>
        /// Cadastra um novo produto com saldo inicial (quantidade em estoque)
        /// </summary>
        /// <param name="produto">Nome/descrição do produto</param>
        /// <param name="saldo">Quantidade inicial em estoque (saldo disponível)</param>
        /// <exception cref="ArgumentException">Lançado se produto já existir</exception>
        public void CadastrarProduto(string produto, int saldo)
        {
            if (string.IsNullOrWhiteSpace(produto))
                throw new ArgumentException("Nome do produto não pode ser vazio", nameof(produto));

            if (_saldos.ContainsKey(produto))
                throw new ArgumentException($"Produto '{produto}' já cadastrado", nameof(produto));

            _saldos[produto] = saldo;
            Console.WriteLine($"[Estoque] Produto '{produto}' cadastrado com saldo de {saldo} unidades em estoque");
        }

        /// <summary>
        /// Obtém o saldo/quantidade de um produto
        /// </summary>
        /// <param name="produto">Nome do produto</param>
        /// <return>Saldo/quantidade em estoque, ou -1 se não existir</return>
        public int ObterSaldo(string produto)
        {
            if (_saldos.TryGetValue(produto, out int saldo))
                return saldo;

            Console.WriteLine($"[Estoque] Aviso: Saldo do produto '{produto}' não encontrado");
            return -1;
        }

        /// <summary>
        /// Atualiza o saldo do produto
        /// </summary>
        /// <param name="produto">Nome do produto</param>
        /// <param name="novoSaldo">Novo saldo</param>
        /// <exception cref="KeyNotFoundException">Lançado se produto não existir</exception>
        public void AtualizarSaldo(string produto, int novoSaldo)
        {
            if (!_saldos.ContainsKey(produto))
                throw new KeyNotFoundException($"Produto '{produto}' não encontrado no estoque");

            _saldos[produto] = novoSaldo;
            Console.WriteLine($"[Estoque] Saldo do produto '{produto}' atualizado para {novoSaldo}");
        }

        /// <summary>
        /// Remove quantidade do estoque (venda/saída) - versao idempotente
        /// Operação sobre o saldo/quantidade. Se tentar remover mais do que existe, 
        /// reduz apenas ao disponível. Operações repetidas não causam estoque negativo.
        /// </summary>
        /// <param name="produto">Nome do produto</param>
        /// <param name="quantidade">Quantidade a remover do saldo</param>
        /// <return>Quantidade realmente removida do saldo</return>
        public int RemoverDoEstoqueIdempotente(string produto, int quantidade)
        {
            if (!_saldos.ContainsKey(produto))
            {
                Console.WriteLine($"[Estoque] Aviso: Produto '{produto}' não existe - operação idempotente ignorada");
                return 0;
            }

            int disponivel = _saldos[produto];
            int removida = Math.Min(quantidade, disponivel);

            if (removida < quantidade)
            {
                Console.WriteLine($"[Estoque] Aviso: Estoque insuficiente para '{produto}'. " +
                    $"Solicitado: {quantidade}, Disponível: {disponivel}, Removido: {removida}");
            }

            _saldos[produto] -= removida;
            Console.WriteLine($"[Estoque] Idempotente: Removido {removida} do saldo de '{produto}'. Restante: {_saldos[produto]}");
            return removida;
        }

        /// <summary>
        /// Adiciona produto de forma idempotente - se já existir, soma ao saldo existente
        /// Garante que chamadas múltiplas não perdem dados do saldo/quantidade
        /// </summary>
        /// <param name="produto">Nome do produto</param>
        /// <param name="quantidade">Quantidade a adicionar ao saldo</param>
        public void CadastrarProdutoIdempotente(string produto, int quantidade)
        {
            if (string.IsNullOrWhiteSpace(produto))
                throw new ArgumentException("Nome do produto não pode ser vazio", nameof(produto));

            if (_saldos.ContainsKey(produto))
            {
                _saldos[produto] += quantidade;
                Console.WriteLine($"[Estoque] Idempotente:Produto '{produto}' já existia. Novo saldo: {_saldos[produto]}");
            }
            else
            {
                _saldos[produto] = quantidade;
                Console.WriteLine($"[Estoque] Idempotente:Produto '{produto}' cadastrado com saldo de {quantidade} unidades em estoque");
            }
        }

        /// <summary>
        /// Obtém lista com todos os nomes de produtos cadastrados
        /// </summary>
        /// <returns>Lista de nomes de produtos</returns>
        public List<string> ObterProdutos()
        {
            return _saldos.Keys.ToList();
        }

        /// <summary>
        /// Verifica se produto existe no estoque
        /// </summary>
        /// <param name="produto">Nome do produto</param>
        /// <returns>true se existe, false caso contrário</returns>
        public bool ProdutoExists(string produto)
        {
            return _saldos.ContainsKey(produto);
        }

        /// <summary>
        /// Testa concorrência - simula múltiplas operações paralelas no estoque
        /// Usa 'lock' para thread safety 
        /// </summary>
        /// <param name="operations">Operações a serem executadas concurrently</param>
        /// <returns>Resultados de cada operação</returns>
        public List<(string produto, int removido, bool sucesso)> TestarConcorrencia(List<(string produto, int quantidade)> operations)
        {
            var results = new List<(string produto, int removido, bool sucesso)>();
            
            // 'lock' garante exclusão mútua - apenas uma thread acessa o dicionário por vez
            lock (_saldos)
            {
                foreach (var op in operations)
                {
                    int removida = RemoverDoEstoqueIdempotente(op.produto, op.quantidade);
                    results.Add((op.produto, removida, removida > 0));
                }
            }
            
            return results;
        }
    }
}