using Korp_Teste_JoaoComini.Services;

Console.WriteLine("=== Sistema Korp Test - Modo Manual ===\n");
Console.WriteLine("Escolha uma opção:");
Console.WriteLine("1. Listar todos produtos");
Console.WriteLine("2. Cadastrar novo produto");
Console.WriteLine("3. Listar todas notas fiscais");
Console.WriteLine("4. Cadastrar nova nota fiscal");
Console.WriteLine("5. Imprimir nota fiscal");
Console.WriteLine("6. Sair");
Console.Write("\nOpção: ");

var estoque = new EstoqueService();
var faturamento = new FaturamentoService();
faturamento.EstoqueService = estoque;

int opcao;
while (true)
{
    if (!int.TryParse(Console.ReadLine(), out opcao))
    {
        Console.WriteLine("Opção inválida! Digite um número de 1 a 6.");
        continue;
    }

    switch (opcao)
    {
        case 1:
            // Listar todos produtos
            Console.WriteLine("\n--- Produtos Cadastrados ---");
            var produtos = estoque.ObterProdutos();
            if (produtos.Count == 0)
            {
                Console.WriteLine("Nenhum produto cadastrado.");
            }
            else
            {
                foreach (var produto in produtos)
                {
                    Console.WriteLine(" - " + produto + ": Saldo=" + estoque.ObterSaldo(produto) + " unidades");
                }
            }
            break;

        case 2:
            // Cadastrar novo produto
            Console.WriteLine("\n--- Cadastrar Novo Produto ---");
            Console.Write("Nome do produto: ");
            string nomeProduto = Console.ReadLine();
            Console.Write("Saldo/quantidade em estoque: ");
            if (int.TryParse(Console.ReadLine(), out int qty))
            {
                estoque.CadastrarProduto(nomeProduto, qty);
                Console.WriteLine("Produto cadastrado com sucesso! Saldo atual: " + estoque.ObterSaldo(nomeProduto) + " unidades");
            }
            else
            {
                Console.WriteLine("Quantidade inválida!");
            }
            break;

        case 3:
            // Listar todas notas fiscais
            Console.WriteLine("\n--- Notas Fiscais Cadastradas ---");
            var notas = faturamento.ListarNotasFiscais();
            if (notas.Count == 0)
            {
                Console.WriteLine("Nenhuma nota fiscal cadastrada.");
            }
            else
            {
                foreach (var nota in notas)
                {
                    Console.WriteLine(" - " + nota.Numeração + ": Status=" + nota.Status + ", Produtos=" + nota.Produtos.Count + ", Total=" + nota.SaldoTotal);
                }
            }
            break;

        case 4:
            // Cadastrar nova nota fiscal
            Console.WriteLine("\n--- Cadastrar Nova Nota Fiscal ---");
            Console.WriteLine("Inclua os produtos que estarão na nota:");
            var listaProdutos = new List<(string, int)>();
            
            while (true)
            {
                Console.WriteLine("\nProduto e quantidade (ou 'fim' para terminar):");
                Console.Write("Produto: ");
                string prod = Console.ReadLine();
                
                if (prod.ToLower() == "fim")
                    break;
                
                Console.Write("Quantidade: ");
                if (int.TryParse(Console.ReadLine(), out int qtd) && qtd > 0)
                {
                    listaProdutos.Add((prod, qtd));
                    Console.WriteLine("Produto adicionado! Pressione Enter para adicionar outro ou 'fim' para terminar.");
                    Console.ReadLine(); // aguarda enter
                }
                else
                {
                    Console.WriteLine("Quantidade inválida!");
                }
            }

            if (listaProdutos.Count > 0)
            {
                var novaNota = faturamento.CadastrarNotafiscal(listaProdutos);
                Console.WriteLine("\nNota fiscal criada: " + novaNota.Numeração);
                Console.WriteLine("Status: " + novaNota.Status);
                Console.WriteLine("Produtos inclusos: " + novaNota.Produtos.Count);
                Console.WriteLine("Saldo total: " + novaNota.SaldoTotal);
            }
            else
            {
                Console.WriteLine("Nenhum produto adicionado. Nota não cadastrada.");
            }
            break;

        case 5:
            // Imprimir nota fiscal
            Console.WriteLine("\n--- Imprimir Nota Fiscal ---");
            Console.WriteLine("Notas disponíveis:");
            var todasNotas = faturamento.ListarNotasFiscais();
            if (todasNotas.Count == 0)
            {
                Console.WriteLine("Nenhuma nota fiscal para imprimir.");
                break;
            }
            
            for (int i = 0; i < todasNotas.Count; i++)
            {
                Console.WriteLine((i + 1) + ". " + todasNotas[i].Numeração + " - Status: " + todasNotas[i].Status);
            }
            
            Console.Write("\nNúmero da nota para imprimir: ");
            if (int.TryParse(Console.ReadLine(), out int numNota) && numNota > 0 && numNota <= todasNotas.Count)
            {
                var notaSelecionada = todasNotas[numNota - 1];
                Console.WriteLine("\nImprimindo nota: " + notaSelecionada.Numeração);
                
                bool resultado = faturamento.ImprimirNotafiscal(notaSelecionada.Numeração, estoque);
                
                if (resultado)
                {
                    Console.WriteLine("✓ Nota impressa com sucesso!");
                    Console.WriteLine("Status alterado para: Fechada");
                    Console.WriteLine("Indicador de processamento: CONCLUÍDO");
                    
                    // Mostrar saldos atualizados
                    Console.WriteLine("\nSaldos em estoque após impressão:");
                    var produtosAtualizados = estoque.ObterProdutos();
                    foreach (var p in produtosAtualizados)
                    {
                        Console.WriteLine(" - " + p + ": " + estoque.ObterSaldo(p) + " unidades");
                    }
                }
                else
                {
                    Console.WriteLine("✗ Falha ao imprimir. Verifique se a nota está 'Aberta'.");
                }
            }
            else
            {
                Console.WriteLine("Número de nota inválido!");
            }
            break;

        case 6:
            // Sair
            Console.WriteLine("\nEncerrando o sistema. Até mais!");
            return;

        default:
            Console.WriteLine("Opção inválida! Escolha de 1 a 6.");
            break;
    }
    
    Console.WriteLine("\n" + new string('-', 40));
    Console.WriteLine("\nEscolha uma opção:");
    Console.WriteLine("1. Listar todos produtos");
    Console.WriteLine("2. Cadastrar novo produto");
    Console.WriteLine("3. Listar todas notas fiscais");
    Console.WriteLine("4. Cadastrar nova nota fiscal");
    Console.WriteLine("5. Imprimir nota fiscal");
    Console.WriteLine("6. Sair");
    Console.Write("\nOpção: ");
}