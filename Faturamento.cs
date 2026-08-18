using System;
using System.Collections.Generic;

namespace Korp_Teste_JoaoComini.Services
{
    /// <summary>
    /// Nota Fiscal - representa uma nota com numeração, status e produtos
    /// Campos: Numeração sequencial, Status: Aberta ou Fechada
    /// </summary>
    public class NotaFiscal
    {
        /// <summary>
        /// Numeração da nota fiscal - SERVE COMO IDENTIFICADOR ÚNICO
        /// Requisito: Numeração sequencial (ex: NF-2024-001, NF-2024-002)
        /// </summary>
        public string Numeração { get; set; }

        /// <summary>
        /// Status da nota: "Aberta" ou "Fechada"
        /// </summary>
        public string Status { get; set; }

        /// <summary>
        /// Lista de produtos inclusos na nota com suas quantidades
        /// </summary>
        public List<(string produto, int quantidade)> Produtos { get; set; }

        /// <summary>
        /// Saldo total da nota (soma das quantidades dos produtos)
        /// </summary>
        public int SaldoTotal { get; set; } // int para consistência com estoque

        /// <summary>
        /// Construtor padrão - cria nota com numeração automática e status "Aberta" 
        /// </summary>
        public NotaFiscal()
        {
            Status = "Aberta"; // Status inicial 
            Produtos = new List<(string, int)>();
            SaldoTotal = 0;
        }

        /// <summary>
        /// Construtor com numeração
        /// </summary>
        /// <param name="numeração">Numeração sequencial da nota (ex: NF-2024-001)</param>
        public NotaFiscal(string numeração)
        {
            Numeração = numeração;
            Status = "Aberta"; // Status inicial 
            Produtos = new List<(string, int)>();
            SaldoTotal = 0;
        }

        /// <summary>
        /// Adicionar produto à nota com quantidade
        /// </summary>
        /// <param name="produto">Nome do produto</param>
        /// <param name="quantidade">Quantidade da nota</param>
        public void AdicionarProduto(string produto, int quantidade)
        {
            if (string.IsNullOrWhiteSpace(produto))
                throw new ArgumentException("Nome do produto não pode ser vazio", nameof(produto));

            if (quantidade <= 0)
                throw new ArgumentException("Quantidade deve ser maior que zero", nameof(quantidade));

            Produtos.Add((produto, quantidade));
            SaldoTotal += quantidade; // Soma das quantidades = saldo total
            Console.WriteLine($"[Faturamento] Produto '{produto}' adicionado à nota {Numeração} com {quantidade} unidades");
        }

        /// <summary>
        /// Verificar se nota pode ser impressa (deve estar Aberta)
        /// </summary>
        /// <return>true se puder imprimir (status Aberta), false caso contrário</return>
        public bool PodeImprimir()
        {
            return Status == "Aberta";
        }
    }

/// <summary>
        /// Serviço de Faturamento - gestão de notas fiscais
        /// Requisitos: numeração sequencial, status Aberta/Fechada, produtos com quantidades,
        /// imprimir atualiza estoque e muda status para Fechada
        /// Numeração serve como identificador (sem campo ID separado)
        /// </summary>
        public class FaturamentoService
        {
            /// <summary>
            /// Referência ao serviço de estoque para atualização de saldos durante impressão
            /// Injetada via propriedade para permitir teste e flexibilidade
            /// </summary>
            public EstoqueService EstoqueService { get; set; }
        // Lista de notas fiscais cadastradas
        private readonly List<NotaFiscal> _notasFiscais;

        // Contador para numeração sequencial automática
        private int _sequencial;

        /// <summary>
        /// Construtor - inicializa lista e contador sequencial
        /// </summary>
        public FaturamentoService()
        {
            _notasFiscais = new List<NotaFiscal>();
            _sequencial = 1; // Começa do 1
        }

        /// <summary>
        /// Cadastra uma nova nota fiscal com numeração sequencial
        /// A numeração sequencial será usada como identificador 
        /// </summary>
        /// <param name="produtos">Lista de tuplas (produto, quantidade) para incluir na nota</param>
        /// <returns>Nota fiscal criada com numeração sequencial</returns>
        public NotaFiscal CadastrarNotafiscal(List<(string produto, int quantidade)> produtos)
        {
            // Gera numeração sequencial
            string numeração = $"NF-{DateTime.Now.Year}-{_sequencial:D3}";
            _sequencial++;

            var nota = new NotaFiscal(numeração);

            // Adiciona todos os produtos à nota
            foreach (var (produto, quantidade) in produtos)
            {
                nota.AdicionarProduto(produto, quantidade);
            }

            // Status inicial é "Aberta" (padrão no construtor da NotaFiscal)
            _notasFiscais.Add(nota);

            Console.WriteLine($"[Faturamento] Nota fiscal cadastrada: {numeração}, Status: {nota.Status}, " +
                $"Produtos: {nota.Produtos.Count}, Saldo Total: {nota.SaldoTotal}");

            return nota;
        }

        /// <summary>
        /// Lista todas as notas fiscais
        /// </summary>
        /// <return>Lista de notas fiscais</return>
        public IReadOnlyList<NotaFiscal> ListarNotasFiscais()
        {
            return _notasFiscais.AsReadOnly();
        }

        /// <summary>
        /// Tenta imprimir uma nota fiscal
        /// Lógica: valida se está Aberta -> atualiza saldo do estoque -> muda status para Fechada
        /// A numeração da nota é usada como identificador para busca
        /// </summary>
        /// <param name="numeração">Numeração da nota a ser impressa</param>
        /// <param name="estoqueService">Referência ao serviço de estoque para atualizar saldos</param>
        /// <return>true se impressão realizada com sucesso, false se não puder imprimir</return>
        public bool ImprimirNotafiscal(string numeração, EstoqueService estoqueService)
        {
            // Busca a nota pela numeração
            var nota = _notasFiscais.Find(n => n.Numeração == numeração);
            if (nota == null)
            {
                Console.WriteLine($"[Faturamento] Erro: Nota fiscal '{numeração}' não encontrada");
                return false;
            }

            // Validação: só pode imprimir se status for "Aberta"
            if (!nota.PodeImprimir())
            {
                Console.WriteLine($"[Faturamento] Erro: Nota fiscal '{numeração}' não pode ser impressa. " +
                    $"Status atual: {nota.Status}. Apenas notas 'Abertas' podem ser impressas.");
                return false;
            }

            Console.WriteLine("[Faturamento] Iniciando processamento de impressão...");

            // Atualiza saldo dos produtos no estoque
            // para cada produto da nota, remove a quantidade do estoque
            foreach (var (produto, quantidade) in nota.Produtos)
            {
                // Remove a quantidade do estoque (usando o método idempotente)
                int removida = estoqueService.RemoverDoEstoqueIdempotente(produto, quantidade);

                Console.WriteLine($"[Faturamento] Nota '{numeração}': removido {removida} de '{produto}' do estoque");
            }

            // Após finalização, atualiza status para "Fechada"
            nota.Status = "Fechada";

            Console.WriteLine($"[Faturamento] Nota fiscal '{numeração}' FINALIZADA. Status alterado para: {nota.Status}");

            // Exibir indicador de processamento (simulado por console)
            Console.WriteLine("[Faturamento] Indicador de processamento: CONCLUÍDO");

            return true;
        }

        /// <summary>
        /// Obtém status de uma nota pela numeração
        /// </summary>
        /// <param name="numeração">Numeração da nota</param>
        /// <return>Status da nota</return>
        public string ObterStatus(string numeração)
        {
            var nota = _notasFiscais.Find(n => n.Numeração == numeração);
            return nota?.Status ?? "Não encontrada";
        }
    }
}