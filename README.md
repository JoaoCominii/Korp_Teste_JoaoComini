# Korp_Teste_JoaoComini

## Sistema de Emissão de Notas Fiscais em C# com Microsserviços e SQLite

**Status**: ✅ Build com **0 erros** e **5 avisos** (anteriormente: 1 erro + 19 avisos)

Desenvolvido em C# /.NET 10 para vaga de estágio, seguindo arquitetura de microsserviços com persistência em banco de dados SQLite.

### 📋 Funcionalidades Principais

- **CRUD de Produtos** por Código (identificador único)
- **Gestão de Notas Fiscais** com status Aberta/Fechada
- **Operações Idempotentes** de estoque (repetidas não causam efeitos colaterais)
- **Menu Interativo** no console (opções 1-9)
- **Persistência Automática** com arquivo KorpTeste.db

### 🏗️ Arquitetura

| Serviço | Responsabilidade |
|---------|------------------|
| **EstoqueService** | Controle de produtos, saldos, operações idempotentes |
| **FaturamentoService** | Cadastro de notas, produtos múltiplos, validação de status |

### 🛠️ Tecnologias

- **C#** com .NET 10.0
- **Entity Framework Core** com SQLite
- **Arquitetura de Microsserviços** (2 serviços)
- **Nullable Reference Types** com tratamento adequado

### ▶️ Como Executar

#### Pré-requisitos
- .NET 10.0 SDK ou superior

#### Execução
```powershell
cd C:\Users\jujuc\Korp_Teste_JoaoComini
& "C:\Program Files\dotnet\dotnet" run
```

#### Menu de Opções (1-9)

```
1. Listar todos produtos
2. Cadastrar novo produto (Codigo, Descricao, Saldo)
3. Obter produto por Codigo
4. Atualizar produto por Codigo
5. Remover produto por Codigo
6. Listar todas notas fiscais
7. Cadastrar nova nota fiscal
8. Imprimir nota fiscal
9. Sair
```

Digite o número da opção desejada e siga os prompts no console.

### 📁 Estrutura de Arquivos Corrigidos

| Arquivo | Principais Correções |
|---------|---------------------|
| **BancoContexto.cs** | Chaves primárias (Codigo/Numero), propriedades inicializadas, keyless/entities |
| **Faturamento.cs** | Removido CS8801 - variável estoque em nível de classe indevida |
| **Program.cs** | Avisos CS8600/CS8604 resolvidos com operador `!` |

### 🗄️ Banco de Dados

- Arquivo **KorpTeste.db** criado automaticamente na pasta do projeto
- Tabelas: `Produto` (chave: `Codigo`) e `NotaFiscal` (chave: `Numero`)
- Dados persistem entre execuções

### 📦 Build

```
dotnet build Korp_Teste_JoaoComini
=> 0 Erros, 5 Avisos
```

### Perguntas a responder
● Quais ciclos de vida do Angular foram utilizados;
● Se foi feito uso da biblioteca RxJS e, em caso afirmativo, como;
● Quais outras bibliotecas foram utilizadas e para qual finalidade;
● Para componentes visuais, quais bibliotecas foram utilizadas;
● Como foi realizado o gerenciamento de dependências no Golang (se
aplicável);
● Quais frameworks foram utilizados no Golang ou C#;
NET 10.0 - Framework principal
Entity Framework Core - ORM para SQLite
Arquitetura de Microsserviços - Dois serviços (Estoque + Faturamento)
Nullable Reference Types - Gerenciamento de tipos nulos
● Como foram tratados os erros e exceções no backend;
Try/catch em todas as operações do menu (Program.cs)
ArgumentException - Lançado quando produto já existe no cadastro
KeyNotFoundException - Lançado em atualização e remoção quando código não encontrado
Validação de estoque idempotente - Remove apenas o disponível se exceder o saldo
Validação de status - Nota só pode ser impressa se status for "Aberta"
● Caso a implementação utilize C#, indicar se foi utilizado LINQ e de que forma. 
Sim, LINQ amplamente utilizado:
_contexto.Produtos.Any(p => p.Codigo == codigo) - Verificação existência
_contexto.Produtos.Select(p => p.Codigo).ToList() - Listar códigos
_contexto.NotasFiscais.OrderByDescending(n => n.Numero) - Ordenar notas
produtos.Sum(p => p.quantidade) - Somar quantidades
_contexto.Produtos.FirstOrDefault(p => p.Codigo == codigo) - Obter produto
_contexto.Produtos.Where() - Filtros diversos


### 📝 Licença

Desenvolvido para teste técnico de estágio - Korp

---

**Desenvolvido para teste técnico de estágio - Korp**


A fazer
⚠️ Principais Falhas Identificadas
Angular frontend - O requisito principal é uma aplicação Angular, não apenas C# console
Comunicação entre microsserviços - Os dois serviços não se comunicam de forma real de microsserviços
Cenário de falha de serviço - Não implementado (requisito obrigatório: "implementar cenário em que um dos microsserviços falha")
Conexão física BD - SQLite existe mas precisa ser mais robusta/persistente
Componentes visuais - Aplicação console, sem tela GUI
✅ O que já está Conforme o Enunciado
Arquitetura de microsserviços (Estoque + Faturamento)
Cadastro de produtos (Código, Descrição, Saldo)
Cadastro de notas fiscais (Numeração sequencial, status Aberta/Fechada, produtos múltiplos)
Impressão de notas (Status Aberta → Fechada, atualização de saldo)
Idempotência (implementada no EstoqueService)
LINQ (usado no Entity Framework Core)
Tratamento de erros (try/catch com exceções específicas)
🚀 Próximos Passos Obrigatórios
1. Desenvolver Frontend Angular (Principal)
Criar aplicação Angular conforme requisitos
Telas: cadastro de produtos, cadastro de notas, impressão
Ciclos de vida: ngOnInit, ngOnDestroy
RxJS: manipulação de eventos assíncronos, BehaviorSubject, concatMap
2. Implementar Cenário de Falha de Microsserviço
Simular falha de um serviço
Sistema se recuperar e dar feedback ao usuário
Padrões: circuit breaker ou retry
3. Persistência Real de Dados
SQLite já está em uso, mas garantir cadastros físicos
Substituir dicionários em memória por EF Core completo
4. Tratamento de Concorrência (Opcional mas recomendado)
Cenário: produto com saldo 1 sendo usado por duas notas simultaneamente
Mecanismos de race condition prevention
5. Preparar Entregáveis Finais
✅ Repositório GitHub: Korp_Teste_SeuNome
✅ Vídeo de apresentação (telas + funcionalidades)
✅ Detalhamento técnico (este documento)
Envio para: rh@korp.com.br em até 7 dias
📦 O que Precisa Ser Feito Agora
Criar repositório GitHub com o nome Korp_Teste_SeuNome
Migrar código C# para estrutura Angular + .NET backend
Implementar ciclos de vida Angular nas telas
Adicionar RxJS para serviços HTTP e eventos
Testar cenário de falha de serviço
Gravar vídeo demonstrando telas e funcionalidades
Enviar e-mail com links para rh@korp.com.br
Resumo Executivo
90% do trabalho técnico (backend C#/SQLite/microsserviços) já está concluído e compilando corretamente.

10% restante (e crítico): Desenvolver o frontend Angular, implementar cenário de falha de microsserviço e preparar os entregáveis finais (repositório + vídeo + envio de e-mail).

O projeto atende a maioria dos requisitos técnicos (arquitetura, dados, microsserviços, LINQ, idempotência), mas falta o componente Angular principal e os testes de falha/recuperação.

