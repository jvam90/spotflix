# Relatório de Verificação de Rúbricas - Projeto Spotflix

**Data:** 22 de junho de 2026  
**Projeto:** Spotflix - Plataforma de Streaming de Música

---

## 1. Desenvolvimento de Sistemas Web e Utilização de Arquiteturas em Camadas

### ✅ Camada de Apresentação
**Status:** Demonstrou o item de rubrica

A camada de apresentação foi desenvolvida através de Controllers ASP.NET Core:
- `AuthController.cs` - Gerenciar registros, login, confirmação de email, redefinição de senha
- `AccountController.cs` - Gerenciar dados do perfil do usuário
- `TransactionsController.cs` - Autorizar e listar transações
- `SearchController.cs` - Buscar bandas e músicas
- `FavoritesController.cs` - Gerenciar músicas e bandas favoritas
- Adicionalmente: `AlbumsController`, `BandsController`, `CardsController`, `PlansController`, `SubscriptionsController`, `SongsController`, `UsersController`

**Detalhes técnicos:**
- Controllers utilizam atributos `[ApiController]`, `[Route]`, `[Authorize]`
- Métodos usam `async/await` para operações assíncronas
- Retornam tipos específicos (`ActionResult<T>`, `IActionResult`)

---

### ✅ Camada de Serviços
**Status:** Demonstrou o item de rubrica

A camada de serviços foi desenvolvida na pasta `/Services`:
- `TokenService.cs` - Geração, rotação e revogação de tokens JWT
- `CurrentUserService.cs` - Obter usuário autenticado no contexto
- `IEmailSender` e `LoggingEmailSender.cs` - Serviço de envio de emails
- `AuthorizationService.cs` - Autorização e validação de transações
- `TransactionNotifier.cs` - Notificação de transações autorizadas

**Detalhes técnicos:**
- Implementam interfaces definidas (`ITokenService`, `ICurrentUserService`, `IEmailSender`, etc.)
- Injetam `AppDbContext` para acessar dados
- Registrados via Dependency Injection no `Program.cs` (`.AddScoped`, `.AddSingleton`)

---

### ✅ Camada de Negócios
**Status:** Demonstrou o item de rubrica

A lógica de negócios está implementada nos serviços e models:

**Exemplos:**
1. **TokenService** - Implementa lógica de:
   - Geração de Access Token com Claims (Sub, Email, Roles)
   - Rotação segura de Refresh Tokens
   - Revogação de tokens

2. **AuthorizationService** - Implementa regras de negócio para transações:
   - Validação se cartão está ativo
   - Verificação de limite disponível
   - Detecção de transações duplicadas na janela de 2 minutos
   - Limite de 3 transações por janela de tempo
   - Registro de violações

3. **Models de Negócio** em `/Models`:
   - `ApplicationUser` - Usuário com identidade
   - `Transaction` - Transações com status e violações
   - `Card` - Cartões com limite disponível
   - `Plan` e `Subscription` - Gestão de planos
   - `FavoriteSong` e `FavoriteBand` - Favoritos do usuário

---

### ✅ Camada de Acesso a Dados
**Status:** Demonstrou o item de rubrica

A camada de acesso a dados foi desenvolvida com Entity Framework:

**DbContext:** `AppDbContext.cs`
```csharp
public class AppDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, Guid>
{
    public DbSet<RefreshToken> RefreshTokens { get; set; }
    public DbSet<Band> Bands { get; set; }
    public DbSet<Album> Albums { get; set; }
    public DbSet<Song> Songs { get; set; }
    public DbSet<Plan> Plans { get; set; }
    public DbSet<Subscription> Subscriptions { get; set; }
    public DbSet<Card> Cards { get; set; }
    public DbSet<Transaction> Transactions { get; set; }
    public DbSet<Notification> Notifications { get; set; }
    public DbSet<FavoriteSong> FavoriteSongs { get; set; }
    public DbSet<FavoriteBand> FavoriteBands { get; set; }
}
```

**Características:**
- Herda de `IdentityDbContext` para suportar ASP.NET Core Identity
- Integração com PostgreSQL via `UseNpgsql`
- Configurações de relacionamentos e índices em `OnModelCreating`

---

## 2. Projetar Aplicativos Web com ASP.NET MVC e Web API

### ✅ Módulo de Cadastro e Login
**Status:** Demonstrou o item de rubrica

**Arquivo:** `Controllers/AuthController.cs`

**Funcionalidades implementadas:**
- `[HttpPost("register")]` - Registro de novo usuário com validação de senha
- `[HttpPost("confirm-email")]` - Confirmação de email com token
- `[HttpPost("login")]` - Login com email/senha, retorna JWT
- `[HttpPost("refresh")]` - Rotação de refresh token
- `[HttpPost("logout")]` - Revogação de token
- `[HttpPost("forgot-password")]` - Solicitação de redefinição de senha
- `[HttpPost("reset-password")]` - Redefinição de senha com token

**Segurança:**
- Validação de força de senha (8+ caracteres, dígito, maiúscula, minúscula)
- Email confirmado obrigatório
- Bloqueio de conta após 5 tentativas falhas por 15 minutos
- Uso de JWT com expiração configurável

---

### ✅ Módulo de Transação
**Status:** Demonstrou o item de rubrica

**Arquivo:** `Controllers/TransactionsController.cs`

**Funcionalidades implementadas:**
- `[HttpPost("authorize")]` - Autorizar transação de pagamento
  - Valida propriedade do cartão
  - Valida limite disponível
  - Aplica regras de negócio (frequência, duplicação)
  - Retorna 200 (autorizado) ou 402 (recusado)

- `[HttpGet("/api/cards/{cardId}/transactions")]` - Listar histórico de transações
  - Paginação implícita
  - Ordenação por data descrescente

**Regras de negócio aplicadas:**
- Limite de 3 transações em 2 minutos
- Detecção de transações duplicadas (mesmo comerciante e valor)
- Validação de atividade do cartão

---

### ✅ Módulo de Busca de Música
**Status:** Demonstrou o item de rubrica

**Arquivo:** `Controllers/SearchController.cs`

**Funcionalidades implementadas:**
- `[HttpGet]` - Buscar bandas e músicas por termo
  - Parâmetro `q` (termo de busca, mínimo 2 caracteres)
  - Parâmetro `limit` (máximo 50 resultados)
  - Retorna bandas e músicas em uma única resposta

**Performance:**
- Usa `EF.Functions.ILike` para busca case-insensitive
- Implementa índices GIN trigram no PostgreSQL
  - `entity.HasIndex(b => b.Name).HasMethod("gin").HasOperators("gin_trgm_ops")`
  - Permite busca eficiente com padrões `%termo%`

**Acesso:**
- `[AllowAnonymous]` - Disponível sem autenticação

---

### ✅ Módulo de Favoritar Música
**Status:** Demonstrou o item de rubrica

**Arquivo:** `Controllers/FavoritesController.cs`

**Funcionalidades implementadas:**

**Músicas:**
- `[HttpGet("songs")]` - Listar músicas favoritas do usuário
- `[HttpPut("songs/{songId}")]` - Adicionar música aos favoritos
- `[HttpDelete("songs/{songId}")]` - Remover música dos favoritos

**Bandas:**
- `[HttpGet("bands")]` - Listar bandas favoritas do usuário
- `[HttpPut("bands/{bandId}")]` - Adicionar banda aos favoritos
- `[HttpDelete("bands/{bandId}")]` - Remover banda dos favoritos

**Funcionalidades:**
- Validação de propriedade (apenas usuário logado acessa seus favoritos)
- Verificação de existência do item antes de adicionar
- Prevenção de duplicatas
- Ordenação por data mais recente

---

## 3. Implementar o Acesso a Dados Utilizando o Entity Framework

### ✅ Modelo de Acesso Utilizando EF
**Status:** Demonstrou o item de rubrica

**Arquivo:** `Data/AppDbContext.cs`

**Características:**
- Herança de `IdentityDbContext<ApplicationUser, ApplicationRole, Guid>`
- Definição de DbSets para 11 entidades principais
- Configurações de mapeamento em `OnModelCreating`:
  - Definição de chaves primárias
  - Relacionamentos um-para-muitos (Band→Albums→Songs)
  - Relacionamentos muitos-para-muitos (User→FavoriteSongs)
  - Comportamento de exclusão em cascata
  - Índices para performance

**Banco de dados:**
- PostgreSQL com extensão `pg_trgm` para busca trigram

---

### ✅ Migrações para Criar o Banco de Dados
**Status:** Demonstrou o item de rubrica

**Arquivo:** `Migrations/` (7 migrações)

**Histórico de migrações:**
1. `20260609135332_InitialIdentitySchema` - Schema do ASP.NET Identity
2. `20260609140429_AddCatalog` - Entidades de catálogo (Bands, Albums, Songs)
3. `20260609141007_AddBilling` - Entidades de billing (Plans, Subscriptions)
4. `20260609141619_AddPayments` - Entidades de pagamento (Cards, Transactions)
5. `20260609141821_AddNotifications` - Entidades de notificação
6. `20260609142300_AddFavorites` - Entidades de favoritos
7. `20260609142937_AddSearchTrigramIndexes` - Índices trigram para busca

**Características:**
- Cada migração é auto-contida e reversível
- Criadas de forma incremental conforme funcionalidades eram adicionadas
- Aplicadas automaticamente no startup via `DbSeeder.SeedAsync`

---

### ❌ Padrão Repository para Operações de Acesso a Dados
**Status:** Não demonstrou o item de rubrica

**Observação:**
- Não foi implementado um padrão Repository explícito
- Os Controllers e Services acessam o `DbContext` diretamente
- Uso de `_db.Entities.Where(...).ToListAsync()` ao invés de `_repository.GetAsync(...)`

**Exemplo do padrão atual:**
```csharp
// Controllers/SearchController.cs
var bands = await _db.Bands.AsNoTracking()
    .Where(b => EF.Functions.ILike(b.Name, term))
    .OrderBy(b => b.Name)
    .Take(limit)
    .ToListAsync(ct);
```

**O que seria esperado (padrão Repository):**
```csharp
var bands = await _bandRepository.SearchByNameAsync(term, limit, ct);
```

---

### ✅ Injeção de Dependência para Acesso aos Dados
**Status:** Demonstrou o item de rubrica

**Arquivo:** `Program.cs`

**Configuração:**
```csharp
// DbContext
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("Default")));

// Services com escopo de requisição
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();
builder.Services.AddScoped<IPaymentAuthorizationService, AuthorizationService>();
builder.Services.AddScoped<ITransactionNotifier, TransactionNotifier>();

// Singleton para accessadores
builder.Services.AddSingleton<IEmailSender, LoggingEmailSender>();
builder.Services.AddHttpContextAccessor();
```

**Padrão utilizado:**
- Interfaces segregadas (`ITokenService`, `ICurrentUserService`, etc.)
- Escopo apropriado (`.AddScoped` para DbContext, `.AddSingleton` para utilitários)
- Injeção nos construtores dos Controllers e Services

**Exemplo:**
```csharp
public class AuthorizationService : IPaymentAuthorizationService
{
    private readonly AppDbContext _db;
    private readonly ITransactionNotifier _notifier;
    private readonly ILogger<AuthorizationService> _logger;

    public AuthorizationService(AppDbContext db, ITransactionNotifier notifier, ILogger<AuthorizationService> logger)
    {
        _db = db;
        _notifier = notifier;
        _logger = logger;
    }
}
```

---

## 4. Disponibilizar Aplicativos Web no Microsoft Azure

### ❌ Demonstrou compreender o Microsoft Azure
### ❌ Demonstrou compreender os serviços de armazenamento de dados do Microsoft Azure
### ❌ Demonstrou compreender o serviço de SQL do Microsoft Azure
### ❌ Demonstrou compreender o serviço de aplicativos web do Microsoft Azure

**Status:** Não demonstrou os itens de rubrica

**Observação:** O projeto não foi implantado no Azure. Não há configurações, scripts de deployment ou documentação sobre Azure.

---

## Serviços Azure Recomendados para Implantação do Spotflix

### 1. **Azure App Service** (Serviço de Aplicativos Web)
**Por que:** Hospedar a aplicação ASP.NET Core  
**Detalhes:**
- Plano `B1` (Basic) para desenvolvimento
- Plano `S1` (Standard) para produção
- Suporta ASP.NET Core runtime nativo
- Auto-scaling baseado em carga
- Integração com GitHub Actions/Azure DevOps para CI/CD

---

### 2. **Azure Database for PostgreSQL**
**Por que:** Banco de dados relacional (aplicação já usa PostgreSQL)  
**Detalhes:**
- Single Server ou Flexible Server
- Backups automáticos (7-35 dias)
- Replicação geográfica para alta disponibilidade
- Extensão `pg_trgm` já suportada
- SSL/TLS para comunicação segura
- Firewall com regras por IP

---

### 3. **Azure Storage Account (Blob Storage)**
**Por que:** Armazenar assets de mídia (capas de álbuns, fotos de bandas)  
**Detalhes:**
- Containers blob para organizar arquivos
- CDN integrado para entrega rápida globalmente
- Lifecycle policies para arquivar dados antigos
- Custos reduzidos com camada "Cool" para acesso infrequente

---

### 4. **Azure Key Vault**
**Por que:** Gerenciar secrets e credenciais de forma segura  
**Detalhes:**
- Armazenar `JWT:Key` (chave de assinatura JWT)
- Armazenar `ConnectionString` do banco
- Armazenar credenciais de APIs externas
- Rotação automática de secrets
- Auditoria de acessos

---

### 5. **Azure Application Insights**
**Por que:** Monitoramento, logging e diagnóstico em tempo real  
**Detalhes:**
- Coleta automática de métricas (CPU, memória, requisições)
- Rastreamento distribuído (Distributed Tracing)
- Alertas para erros e performance
- Análise de experiência do usuário
- Integração com Application Performance Monitoring (APM)

---

### 6. **Azure DevOps Pipelines ou GitHub Actions**
**Por que:** Integração contínua e entrega contínua (CI/CD)  
**Detalhes:**
- Build automático ao fazer push
- Testes automatizados
- Deploy automático para App Service
- Ambientes staging e produção
- Rollback automático em case de falha

---

### 7. **Azure Front Door (Opcional)**
**Por que:** CDN e balanceador de carga global  
**Detalhes:**
- Entrega de conteúdo acelerada globalmente
- Roteamento inteligente de tráfego
- Proteção contra DDoS
- Failover automático entre regiões

---

### 8. **Azure Monitor (Opcional)**
**Por que:** Alertas e notificações proativas  
**Detalhes:**
- Regras de alerta (CPU > 80%, erros > taxa limite)
- Webhooks para integração com Slack/Teams
- Painéis customizados (Dashboards)

---

## Arquitetura de Implantação Recomendada

```
┌─────────────────────────────────────────────────────────────┐
│                    Azure DevOps / GitHub Actions             │
│                    (CI/CD Pipeline)                          │
└────────────────────────────┬────────────────────────────────┘
                             │
                    ┌────────▼──────────┐
                    │  Build & Test     │
                    └────────┬──────────┘
                             │
                    ┌────────▼──────────┐
                    │  Deploy to        │
                    │  App Service      │
                    └────────┬──────────┘
                             │
        ┌────────────────────┼────────────────────┐
        │                    │                    │
   ┌────▼─────┐    ┌────────▼────────┐    ┌─────▼──────┐
   │ Azure App │    │ Azure Database  │    │ Blob       │
   │ Service   │◄─►│ for PostgreSQL  │    │ Storage    │
   │(ASP.NET)  │    │                 │    │(Assets)    │
   └──────────┘    └─────────────────┘    └───────────┘
        │
        ├─ Application Insights (Monitoramento)
        ├─ Azure Key Vault (Secrets)
        ├─ Azure Front Door (CDN global)
        └─ Azure Monitor (Alertas)
```

---

## Resumo Executivo

| Rubrica | Item | Status |
|---------|------|--------|
| 1. Arquitetura em Camadas | Camada de Apresentação | ✅ Demonstrou |
| | Camada de Serviços | ✅ Demonstrou |
| | Camada de Negócios | ✅ Demonstrou |
| | Camada de Acesso a Dados | ✅ Demonstrou |
| 2. ASP.NET MVC e Web API | Módulo Cadastro/Login | ✅ Demonstrou |
| | Módulo Transação | ✅ Demonstrou |
| | Módulo Busca | ✅ Demonstrou |
| | Módulo Favoritos | ✅ Demonstrou |
| 3. Entity Framework | Modelo de Acesso EF | ✅ Demonstrou |
| | Migrações | ✅ Demonstrou |
| | Padrão Repository | ❌ Não Demonstrou |
| | Injeção de Dependência | ✅ Demonstrou |
| 4. Microsoft Azure | Compreensão Azure | ❌ Não Demonstrou |
| | Armazenamento Dados Azure | ❌ Não Demonstrou |
| | SQL Azure | ❌ Não Demonstrou |
| | App Service Azure | ❌ Não Demonstrou |

**Pontuação:** 11 de 16 itens demonstrados (68,75%)

---

## Observações Gerais

### Pontos Fortes
1. ✅ Arquitetura bem organizada em camadas
2. ✅ Uso correto de ASP.NET Core Identity
3. ✅ Implementação robusta de JWT com refresh tokens
4. ✅ Boas práticas de segurança (validação, autorização, hash de senhas)
5. ✅ Performance: índices trigram para busca eficiente
6. ✅ Migrações bem documentadas
7. ✅ Frontend Angular estruturado com separação de concerns
8. ✅ Injeção de dependência corretamente configurada

### Melhorias Sugeridas
1. **Padrão Repository:** Implementar um genérico `IRepository<T>` para padronizar acesso a dados
2. **Azure Deployment:** Adicionar workflows GitHub Actions para CI/CD
3. **Documentação Azure:** Criar guia de deployment no Azure
4. **Testes Unitários:** Adicionar testes para serviços (TokenService, AuthorizationService)
5. **Logging Estruturado:** Usar Serilog em vez de ILogger básico
6. **Error Handling:** Implementar middleware global mais robusto

---

**Relatório gerado em:** 22 de junho de 2026  
**Avaliador:** Claude Code Assistant
