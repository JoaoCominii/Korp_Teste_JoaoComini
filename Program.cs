using Korp_Teste_JoaoComini.Services;

Console.WriteLine("=== Sistema Korp Test - Modo Manual CRUD ===\n");
Console.WriteLine("Escolha uma opcao:");
Console.WriteLine("1. Listar todos produtos");
Console.WriteLine("2. Cadastrar novo produto (Codigo, Descricao, Saldo)");
Console.WriteLine("3. Obter produto por Codigo");
Console.WriteLine("4. Atualizar produto por Codigo");
Console.WriteLine("5. Remover produto por Codigo");
Console.WriteLine("6. Listar todas notas fiscais");
Console.WriteLine("7. Cadastrar nova nota fiscal");
Console.WriteLine("8. Imprimir nota fiscal");
Console.WriteLine("9. Sair");
Console.Write("\nOpcao: ");

var estoque = new EstoqueService();
var faturamento = new FaturamentoService();
faturamento.EstoqueService = estoque;

int opcao;
while (true)
{
    if (!int.TryParse(Console.ReadLine(), out opcao))
    {
        Console.WriteLine("Opcao invalida! Digite um numero de 1 a 9.");
        continue;
    }

    switch (opcao)
    {
        case 1:
            // Listar todos produtos
            Console.WriteLine("\n--- Produtos Cadastrados ---");
            var prods = estoque.ObterCodigos();
            if (prods.Count == 0)
            {
                Console.WriteLine("Nenhum produto cadastrado.");
            }
            else
            {
                foreach (var cod in prods)
                {
                    var info = estoque.ObterProduto(cod);
                    if (info.HasValue)
                    {
                        var (desc, sal) = info.Value;
                        Console.WriteLine(" - Codigo: " + cod + ", Descricao: " + desc + ", Saldo: " + sal + " unidades");
                    }
                }
            }
            break;

        case 2:
            // Cadastrar novo produto
            Console.WriteLine("\n--- Cadastrar Novo Produto ---");
            Console.Write("Digite o Codigo do produto: ");
            string codNovo = Console.ReadLine();
            Console.Write("Digite a Descricao (nome): ");
            string descNova = Console.ReadLine();
            Console.Write("Digite o Saldo/quantidade em estoque: ");
            if (int.TryParse(Console.ReadLine(), out int qtyNovo) && qtyNovo >= 0)
            {
                try
                {
                    estoque.CadastrarProduto(codNovo, descNova, qtyNovo);
                    Console.WriteLine("Produto cadastrado com sucesso!");
                }
                catch (ArgumentException ex)
                {
                    Console.WriteLine("Erro: " + ex.Message);
                }
            }
            else
            {
                Console.WriteLine("Quantidade invalida!");
            }
            break;

        case 3:
            // Obter produto por Codigo
            Console.WriteLine("\n--- Obter Produto por Codigo ---");
            Console.Write("Digite o Codigo do produto: ");
            string codBusca = Console.ReadLine();
            var res = estoque.ObterProduto(codBusca);
            if (res.HasValue)
            {
                var (desc, sal) = res.Value;
                Console.WriteLine("Produto encontrado - Codigo: " + codBusca);
                Console.WriteLine("  Descricao: " + desc);
                Console.WriteLine("  Saldo: " + sal + " unidades");
            }
            else
            {
                Console.WriteLine("Produto nao encontrado com codigo: " + codBusca);
            }
            break;

        case 4:
            // Atualizar produto por Codigo
            Console.WriteLine("\n--- Atualizar Produto por Codigo ---");
            Console.Write("Digite o Codigo do produto a atualizar: ");
            string codAtu = Console.ReadLine();
            Console.Write("Digite a nova Descricao (ou pressione Enter para manter): ");
            string novaDesc = Console.ReadLine();
            Console.Write("Digite o novo Saldo (ou digite -1 para manter o atual): ");
            if (int.TryParse(Console.ReadLine(), out int novoSal))
            {
                try
                {
                    string descAplicar = string.IsNullOrEmpty(novaDesc) ? null : novaDesc;
                    estoque.AtualizarProduto(codAtu, descAplicar, novoSal >= 0 ? novoSal : (int?)null);
                    Console.WriteLine("Produto atualizado com sucesso!");
                }
                catch (KeyNotFoundException ex)
                {
                    Console.WriteLine("Erro: " + ex.Message);
                }
            }
            break;

        case 5:
            // Remover produto por Codigo
            Console.WriteLine("\n--- Remover Produto por Codigo ---");
            Console.Write("Digite o Codigo do produto a remover: ");
            string codRem = Console.ReadLine();
            try
            {
                estoque.RemoverProduto(codRem);
                Console.WriteLine("Produto removido com sucesso!");
            }
            catch (KeyNotFoundException ex)
            {
                Console.WriteLine("Erro: " + ex.Message);
            }
            break;

        case 6:
            // Listar todas notas fiscais
            Console.WriteLine("\n--- Notas Fiscais Cadastradas ---");
            var notas = faturamento.ListarNotasFiscais();
            if (notas.Count == 0)
            {
                Console.WriteLine("Nenhuma nota fiscal cadastrada.");
            }
            else
            {
                Console.WriteLine("\nLista de notas fiscais:");
                for (int i = 0; i < notas.Count; i++)
                {
                    Console.WriteLine((i + 1) + ". Status: " + notas[i].Status + ", Produtos=" + notas[i].Produtos.Count);
                }
                Console.Write("Digite o numero da nota para detalhes (1-" + notas.Count + ") ou 0 para voltar: ");
                if (int.TryParse(Console.ReadLine(), out int numSel) && numSel > 0 && numSel <= notas.Count)
                {
                    var nota = notas[numSel - 1];
                    Console.WriteLine("\nDetalhes da nota:");
                    Console.WriteLine("  Status: " + nota.Status);
                    Console.WriteLine("  Produtos: " + nota.Produtos.Count);
                    Console.WriteLine("  Saldo Total: " + nota.SaldoTotal);
                }
            }
            break;

        case 7:
            // Cadastrar nova nota fiscal
            Console.WriteLine("\n--- Cadastrar Nova Nota Fiscal ---");
            Console.WriteLine("Inclua os produtos (usando seus Codigo) que estarao na nota:");
            var listaProd = new List<(string codigo, int quantidade)>();

            while (true)
            {
                Console.WriteLine("\nProduto e quantidade (ou 'fim' para terminar):");
                Console.Write("Codigo do produto: ");
                string prodCod = Console.ReadLine();

                if (prodCod.ToLower() == "fim")
                    break;

                Console.Write("Quantidade: ");
                if (int.TryParse(Console.ReadLine(), out int qtd) && qtd > 0)
                {
                    if (estoque.ObterProduto(prodCod).HasValue)
                    {
                        listaProd.Add((prodCod, qtd));
                        Console.WriteLine("Produto adicionado! Pressione Enter para adicionar outro ou 'fim' para terminar.");
                        Console.ReadLine();
                    }
                    else
                    {
                        Console.WriteLine("Codigo de produto invalido! Tente novamente.");
                    }
                }
                else
                {
                    Console.WriteLine("Quantidade invalida!");
                }
            }

            if (listaProd.Count > 0)
            {
                var novaNota = faturamento.CadastrarNotafiscal(listaProd);
                Console.WriteLine("\nNota fiscal criada: " + novaNota.Status);
                Console.WriteLine("Produtos incluidos: " + novaNota.Produtos.Count);
                Console.WriteLine("Saldo total: " + novaNota.SaldoTotal);
            }
            else
            {
                Console.WriteLine("Nenhum produto adicionado. Nota nao cadastrada.");
            }
            break;

        case 8:
            // Imprimir nota fiscal
            Console.WriteLine("\n--- Imprimir Nota Fiscal ---");
            Console.WriteLine("Notas disponiveis:");
            var todasNotas = faturamento.ListarNotasFiscais();
            if (todasNotas.Count == 0)
            {
                Console.WriteLine("Nenhuma nota fiscal para imprimir.");
                break;
            }

            Console.WriteLine("Digite o numero da nota para imprimir (1-" + todasNotas.Count + "):");
            if (int.TryParse(Console.ReadLine(), out int numNota) && numNota > 0 && numNota <= todasNotas.Count)
            {
                var notaSel = todasNotas[numNota - 1];
                Console.WriteLine("\nImprimindo nota: Status=" + notaSel.Status);

                bool resultado = faturamento.ImprimirNotafiscal(notaSel.Status, estoque);

                if (resultado)
                {
                    Console.WriteLine(" Nota impressa com sucesso!");
                    Console.WriteLine(" Status alterado para: Fechada");
                    Console.WriteLine(" Indicador de processamento: CONCLUIDO");

                    Console.WriteLine("\nSaldos em estoque apos impressao:");
                    var prodsAt = estoque.ObterCodigos();
                    foreach (var p in prodsAt)
                    {
                        var inf = estoque.ObterProduto(p);
                        if (inf.HasValue)
                        {
                            var (d, s) = inf.Value;
                            Console.WriteLine(" - Codigo: " + p + ", Descricao: " + d + ", Saldo: " + s + " unidades");
                        }
                    }
                }
                else
                {
                    Console.WriteLine(" Falha ao imprimir. Verifique se a nota esta 'Aberta'.");
                }
            }
            else
            {
                Console.WriteLine(" Numero de nota invalido!");
            }
            break;

        case 9:
            // Sair
            Console.WriteLine("\nEncerrando o sistema. ate mais!");
            return;

        default:
            Console.WriteLine("Opcao invalida! Escolha de 1 a 9.");
            break;
    }

    Console.WriteLine("\n" + new string('-', 40));
    Console.WriteLine("\nEscolha uma opcao:");
    Console.WriteLine("1. Listar todos produtos");
    Console.WriteLine("2. Cadastrar novo produto (Codigo, Descricao, Saldo)");
    Console.WriteLine("3. Obter produto por Codigo");
    Console.WriteLine("4. Atualizar produto por Codigo");
    Console.WriteLine("5. Remover produto por Codigo");
    Console.WriteLine("6. Listar todas notas fiscais");
    Console.WriteLine("7. Cadastrar nova nota fiscal");
    Console.WriteLine("8. Imprimir nota fiscal");
    Console.WriteLine("9. Sair");
    Console.Write("\nOpcao: ");
}