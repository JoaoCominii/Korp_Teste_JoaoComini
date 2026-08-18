using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Korp_Teste_JoaoComini.Services
{
    /// <summary>
    /// Serviço de Estoque - controla produtos e saldos (quantidade em estoque)
    /// Utiliza "Código" como identificador único para CRUD completo
    /// </summary>
    public class EstoqueService
    {
        // Dicionário: Código -> (Descrição, Saldo/quantidade em estoque)
        // O "Código" serve como chave única para identificação, apagamento e atualização
        private readonly Dictionary<string, (string descricao, int saldo)> _produtos;

        /// <summary>
        /// Construtor - inicializa o dicionário de produtos
        /// </summary>
        public EstoqueService()
        {
            _produtos = new Dictionary<string, (string, int)>();
        }

        /// <summary>
        /// Cadastra um novo produto com código, descrição e saldo
        /// Requisito: Campos obrigatórios são Código, Descrição e Saldo
        /// </summary>
        /// <param name="codigo">Código identificador do produto (chave única)</param>
        /// <param name="descricao">Descrição/nome do produto</param>
        /// <param name="saldo">Quantidade em estoque</param>
        /// <exception cref="ArgumentException">Lançado se código já existir</exception>
        public void CadastrarProduto(string codigo, string descricao, int saldo)
        {
            if (string.IsNullOrWhiteSpace(codigo))
                throw new ArgumentException("Código do produto não pode ser vazio", nameof(codigo));

            if (string.IsNullOrWhiteSpace(descricao))
                throw new ArgumentException("Descrição do produto não pode ser vazia", nameof(descricao));

            if (_produtos.ContainsKey(codigo))
                throw new ArgumentException($"Produto com código '{codigo}' já cadastrado", nameof(codigo));

            _produtos[codigo] = (descricao, saldo);
            Console.WriteLine($"[Estoque] Produto cadastrado - Código: {codigo}, Descrição: {descricao}, Saldo: {saldo} unidades");
        }

        /// <summary>
        /// Lista todos os produtos cadastrados
        /// </summary>
        /// <return>Lista de códigos dos produtos</return>
        public List<string> ListarProdutos()
        {
            return new List<string>(_produtos.Keys);
        }

        /// <summary>
        /// Obtém descrição e saldo de um produto pelo Código
        /// </summary>
        /// <param name="codigo">Código do produto</param>
        /// <return>Tuple (descrição, saldo) ou null se não existir</return>
        public (string descricao, int saldo)? ObterProduto(string codigo)
        {
            if (_produtos.TryGetValue(codigo, out var produto))
                return produto;

            Console.WriteLine($"[Estoque] Aviso: Produto com código '{codigo}' não encontrado");
            return null;
        }

        /// <summary>
        /// Atualiza a descrição e/ou saldo de um produto existente
        /// </summary>
        /// <param name="codigo">Código do produto a ser atualizado</param>
        /// <param name="novaDescricao">Nova descrição (ou null para manter a atual)</param>
        /// <param name="novoSaldo">Novo saldo (ou -1 para manter o atual)</param>
        /// <exception cref="KeyNotFoundException">Lançado se código não existir</exception>
        public void AtualizarProduto(string codigo, string? novaDescricao, int? novoSaldo)
        {
            if (!_produtos.ContainsKey(codigo))
                throw new KeyNotFoundException($"Produto com código '{codigo}' não encontrado");

            var atual = _produtos[codigo];
            string descricao = string.IsNullOrEmpty(novaDescricao) ? atual.descricao : novaDescricao;
            int saldo = novoSaldo.HasValue && novoSaldo.Value >= 0 ? novoSaldo.Value : atual.saldo;

            _produtos[codigo] = (descricao, saldo);
            Console.WriteLine($"[Estoque] Produto atualizado - Código: {codigo}, Nova Descrição: {descricao}, Novo Saldo: {saldo} unidades");
        }

        /// <summary>
        /// Remove/Apaga um produto pelo Código
        /// </summary>
        /// <param name="codigo">Código do produto a ser removido</param>
        /// <exception cref="KeyNotFoundException">Lançado se código não existir</exception>
        public void RemoverProduto(string codigo)
        {
            if (!_produtos.Remove(codigo))
                throw new KeyNotFoundException($"Produto com código '{codigo}' não encontrado para remoção");

            Console.WriteLine($"[Estoque] Produto removido - Código: {codigo}");
        }

        /// <summary>
        /// Remove quantidade do estoque (venda/saída) - versao idempotente
        /// Se tentar remover mais do que existe, reduz apenas ao disponível
        /// Operações repetidas não causam estoque negativo (efeito colateral indesejado)
        /// </summary>
        /// <param name="codigo">Código do produto</param>
        /// <param name="quantidade">Quantidade a remover</param>
        /// <return>Quantidade realmente removida</return>
        public int RemoverDoEstoqueIdempotente(string codigo, int quantidade)
        {
            if (!_produtos.ContainsKey(codigo))
            {
                Console.WriteLine($"[Estoque] Aviso: Produto com código '{codigo}' não existe - operação idempotente ignorada");
                return 0;
            }

            int disponivel = _produtos[codigo].saldo;
            int removida = Math.Min(quantidade, disponivel);

            if (removida < quantidade)
            {
                Console.WriteLine($"[Estoque] Aviso: Estoque insuficiente para código '{codigo}'. " +
                    $"Solicitado: {quantidade}, Disponível: {disponivel}, Removido: {removida}");
            }

            _produtos[codigo] = (_produtos[codigo].descricao, _produtos[codigo].saldo - removida);
            Console.WriteLine($"[Estoque] Idempotente: Removido {removida} do código '{codigo}'. Restante: {_produtos[codigo].saldo}");
            return removida;
        }

        /// <summary>
        /// Adiciona produto de forma idempotente - se já existir, soma à descrição e quantidade existente
        /// </summary>
        /// <param name="codigo">Código do produto</param>
        /// <param name="quantidade">Quantidade a adicionar</param>
        public void CadastrarProdutoIdempotente(string codigo, string descricao, int quantidade)
        {
            if (string.IsNullOrWhiteSpace(codigo))
                throw new ArgumentException("Código do produto não pode ser vazio", nameof(codigo));

            if (string.IsNullOrWhiteSpace(descricao))
                throw new ArgumentException("Descrição do produto não pode ser vazia", nameof(descricao));

            if (_produtos.ContainsKey(codigo))
            {
                _produtos[codigo] = (_produtos[codigo].descricao + " + " + descricao, _produtos[codigo].saldo + quantidade);
                Console.WriteLine($"[Estoque] Idempotente:Produto código '{codigo}' já existia. Nova descrição: {_produtos[codigo].descricao}, Nova quantidade: {_produtos[codigo].saldo}");
            }
            else
            {
                _produtos[codigo] = (descricao, quantidade);
                Console.WriteLine($"[Estoque] Idempotente:Produto código '{codigo}' cadastrado com {quantidade} unidades");
            }
        }

        /// <summary>
        /// Obtém lista com todos os códigos de produtos cadastrados
        /// </summary>
        /// <return>Lista de códigos</return>
        public IReadOnlyList<string> ObterCodigos()
        {
            return _produtos.Keys.ToList().AsReadOnly();
        }
    }
}