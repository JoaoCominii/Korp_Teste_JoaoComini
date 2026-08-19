# Korp_Teste_JoaoComini

## Sistema de Emissão de Notas Fiscais - Angular + ASP.NET Web API + SQLite

**Status**: Build com **0 erros**

Desenvolvido em C# / .NET 10 com frontend Angular, seguindo arquitetura de microsserviços com persistência em banco de dados SQLite.

---

## Funcionalidades

- **CRUD de Produtos** - Cadastrar, listar, editar e remover produtos por Código
- **Gestão de Notas Fiscais** - Criar notas com múltiplos produtos, status Aberta/Fechada
- **Impressão de Notas** - Altera status para Fechada e atualiza saldos em estoque (idempotente)
- **Dashboard de Estoque** - Visão geral com totais e alertas de estoque baixo
- **Frontend Angular** - Interface responsiva com sidebar, rotas e modais

## Arquitetura

```
┌─────────────────┐     HTTP/REST     ┌─────────────────────┐
│                 │ ◄──────────────► │                     │
│  Angular (SPA)  │                   │  ASP.NET Web API   │
│  localhost:4200 │                   │  localhost:5000     │
│                 │                   │                     │
└─────────────────┘                   └─────────┬───────────┘
                                                │
                                    ┌───────────┴───────────┐
                                    │                       │
                              ┌─────┴─────┐          ┌──────┴──────┐
                              │  Estoque  │          │ Faturamento │
                              │  Service  │          │   Service   │
                              └─────┬─────┘          └──────┬──────┘
                                    │                       │
                                    └───────────┬───────────┘
                                                │
                                        ┌───────┴───────┐
                                        │  SQLite (EF)  │
                                        │  KorpTeste.db │
                                        └───────────────┘
```

### Serviços Backend

| Serviço | Controller | Responsabilidade |
|---------|-----------|------------------|
| **Estoque** | `ProdutosController` | CRUD de produtos, controle de saldos |
| **Faturamento** | `NotasFiscaisController` | Notas fiscais, impressão, validação |

## Tecnologias

### Backend (C#)
- **ASP.NET Web API** - Framework principal
- **Entity Framework Core** - ORM para SQLite
- **Swagger/OpenAPI** - Documentação da API
- **CORS** - Configurado para Angular

### Frontend (Angular)
- **Angular 20** - Framework SPA
- **RxJS** - Gerenciamento de estado com BehaviorSubject
- **HttpClient** - Comunicação com a API REST
- **SCSS** - Estilização com variáveis CSS

## Como Executar

### Pré-requisitos
- .NET 10.0 SDK ou superior
- Node.js 18+ e npm

### Backend (porta 5000)
```powershell
cd C:\Users\jujuc\Korp_Teste_JoaoComini
dotnet run
```

API disponível em: `http://localhost:5000`
Swagger: `http://localhost:5000/swagger`

### Frontend (porta 4200)
```powershell
cd korp-frontend
npx ng serve
```

Aplicação disponível em: `http://localhost:4200`

## Endpoints da API

### Produtos (`/api/produtos`)
| Método | Rota | Descrição |
|--------|------|-----------|
| GET | `/api/produtos` | Listar todos os produtos |
| GET | `/api/produtos/{codigo}` | Obter produto por código |
| POST | `/api/produtos` | Cadastrar novo produto |
| PUT | `/api/produtos/{codigo}` | Atualizar produto |
| DELETE | `/api/produtos/{codigo}` | Remover produto |

### Notas Fiscais (`/api/notasfiscais`)
| Método | Rota | Descrição |
|--------|------|-----------|
| GET | `/api/notasfiscais` | Listar todas as notas |
| GET | `/api/notasfiscais/{numero}` | Obter nota por número |
| POST | `/api/notasfiscais` | Criar nova nota fiscal |
| POST | `/api/notasfiscais/{numero}/imprimir` | Imprimir nota (Aberta → Fechada) |

## Frontend - Páginas

| Rota | Componente | Descrição |
|------|------------|-----------|
| `/produtos` | `ProdutoList` | CRUD de produtos com grid de cards |
| `/notas-fiscais` | `NotaFiscal` | Criar e gerenciar notas fiscais |
| `/estoque` | `Estoque` | Dashboard com estatísticas de estoque |

## Banco de Dados

- Arquivo **KorpTeste.db** criado automaticamente
- Tabelas: `Produto` (chave: `Codigo`) e `NotaFiscal` (chave: `Numero`)
- Dados persistem entre execuções

## Detalhamento Técnico

### Ciclos de vida Angular
- **ngOnInit** - Inicialização de componentes, subscriptions e carregamento de dados
- **ngOnDestroy** - Cancelamento de subscriptions para evitar memory leaks

### RxJS
- **BehaviorSubject** - Gerenciamento de estado reativo (produtos$, notasFiscais$, error$)
- **Observable subscription** - Componentes se inscrevem nos observables do serviço
- **tap** - Efeitos colaterais nas requisições HTTP (reload automático)
- **catchError** - Tratamento de erros nas chamadas HTTP

### Tratamento de Erros no Backend
- **ArgumentException** - Produto já existe no cadastro
- **KeyNotFoundException** - Produto/nota não encontrado
- **Validação de status** - Nota só impressa se status "Aberta"
- **Idempotência** - Remoção de estoque respeita saldo disponível

### LINQ
- `_contexto.Produtos.Any()` - Verificação de existência
- `_contexto.Produtos.Select()` - Projeção de campos
- `_contexto.Produtos.FirstOrDefault()` - Busca por critério
- `_contexto.NotasFiscais.OrderByDescending()` - Ordenação
- `produtos.Sum()` - Agregação de valores

---

**Desenvolvido para teste técnico de estágio - Korp**
