# Spotflix

Plataforma de streaming de música com catálogo real de MP3, gerenciamento de assinaturas e autenticação JWT.

---

## Stack

| Camada | Tecnologia |
|--------|-----------|
| Frontend | Angular 21 + TypeScript 5.9, RxJS 7.8 |
| Backend | ASP.NET Core 10 / C# 12, JWT Bearer |
| Banco | PostgreSQL + EF Core 10 (Npgsql) |
| Email (dev) | smtp4dev (SMTP local) |
| Documentação API | Swagger (Swashbuckle 10) |

---

## Como rodar

### Pré-requisitos

- .NET 10 SDK
- Node.js 18+ e npm
- PostgreSQL 12+
- smtp4dev — `dotnet tool install -g Rnwood.Smtp4dev`

### 1. Banco de dados

Crie o banco (as migrations são aplicadas automaticamente na inicialização):

```bash
psql -U postgres -c "CREATE DATABASE spotflix_db;"
```

Configure a connection string em `src/Spotflix.Api/appsettings.Development.json`:

```json
{
  "ConnectionStrings": {
    "Default": "Host=localhost;Port=5432;Database=spotflix_db;Username=postgres;Password=sua_senha"
  },
  "Seed": {
    "AdminEmail": "admin@spotflix.com",
    "AdminPassword": "Admin@123456"
  }
}
```

### 2. Backend

```bash
cd src/Spotflix.Api
dotnet run
```

A API sobe em `https://localhost:7001`. Na primeira execução, as migrations são aplicadas e o banco é populado automaticamente com usuários, planos e o catálogo de MP3s.

### 3. Email (smtp4dev)

O cadastro de novos usuários exige confirmação de e-mail. Em desenvolvimento, use o smtp4dev para capturar os e-mails localmente:

```bash
smtp4dev   # SMTP em localhost:25 | UI em http://localhost:5000
```

Fluxo de confirmação:
1. `POST /api/auth/register` com email e senha
2. Abrir `http://localhost:5000` → copiar `userId` e `token` do e-mail recebido
3. `POST /api/auth/confirm-email` com os valores copiados

### 4. Frontend

```bash
cd src/spotflix-web
npm install
npm start   # http://localhost:4200
```

O frontend faz proxy automático para `https://localhost:7001`.

---

## Usuários de teste

Todos já têm e-mail confirmado e estão prontos para login.

| Email | Senha | Role | Assinatura |
|-------|-------|------|-----------|
| admin@spotflix.com | Admin@123456 | admin | — |
| user1@spotflix.com | Senha@123 | user | Premium (ativa) |
| user2@spotflix.com | Senha@123 | user | Family (ativa) |
| user3@spotflix.com | Senha@123 | user | Premium (expirada) |

Cada usuário de teste possui 2 cartões cadastrados (Visa •••• 1234 e Mastercard •••• 5678).

---

## Planos

| Plano | Preço | Descrição |
|-------|-------|-----------|
| Gratuito | R$ 0,00/mês | Acesso básico |
| Premium | R$ 19,90/mês | Sem anúncios, qualidade alta |
| Family | R$ 34,90/mês | Até 6 perfis, Premium completo |

Regras de upgrade/downgrade: é possível migrar para um plano superior (o plano atual é cancelado e o novo é ativado); migração para plano inferior é bloqueada.

---

## Catálogo de músicas

Gerado automaticamente a partir dos arquivos MP3 em `Mp3/` (raiz do projeto). Na primeira execução, o seeder lê os metadados ID3 via TagLibSharp e armazena os bytes de áudio no banco (`bytea`). As músicas são reproduzidas diretamente pelo browser via `GET /api/songs/{id}/stream`.

---

## Principais endpoints

```
POST /api/auth/register          Cadastro
POST /api/auth/confirm-email     Confirmação de e-mail
POST /api/auth/login             Login → retorna accessToken + refreshToken

GET  /api/search?q=termo         Busca bandas e músicas
GET  /api/bands                  Lista bandas
GET  /api/bands/{id}             Detalhe da banda com álbuns
GET  /api/songs/{id}/stream      Stream de áudio (range requests)

GET  /api/favorites/songs        Músicas favoritas
PUT  /api/favorites/songs/{id}   Adicionar favorito
DELETE /api/favorites/songs/{id} Remover favorito

GET  /api/plans                  Listar planos
GET  /api/subscriptions/me       Assinatura ativa do usuário
POST /api/subscriptions          Assinar plano
POST /api/subscriptions/cancel   Cancelar assinatura

GET  /api/cards                  Meus cartões
POST /api/cards                  Adicionar cartão
```

Documentação interativa completa em `https://localhost:7001/swagger`.

---

## Estrutura do projeto

```
spotflix-master/
├── Mp3/                          Arquivos MP3 (fonte do catálogo)
└── src/
    ├── Spotflix.Api/             Backend ASP.NET Core
    │   ├── Controllers/          Endpoints REST
    │   ├── Data/
    │   │   ├── Migrations/       Migrações EF Core
    │   │   ├── Repositories/     Padrão Repository
    │   │   └── DbSeeder.cs       Seed automático
    │   ├── Models/               Entidades
    │   ├── Dtos/                 Contratos de API
    │   ├── Services/             Lógica de negócio
    │   └── Program.cs            Startup e DI
    └── spotflix-web/             Frontend Angular
        └── src/app/
            ├── core/             Auth, player, API services
            ├── features/         Páginas (home, search, plans…)
            └── shared/           Componentes reutilizáveis
```

---

## Troubleshooting

**Conexão recusada na porta 5432**
Verifique se o PostgreSQL está em execução (`services.msc` no Windows).

**Banco não existe**
As migrations criam o banco automaticamente. Se o erro persistir: `dotnet ef database drop && dotnet ef database update` em `src/Spotflix.Api`.

**E-mail não aparece no smtp4dev**
Confirme que o smtp4dev está rodando (`smtp4dev` no terminal) e que a porta 25 não está bloqueada por antivírus. Alternativa: use a porta 2525 (`smtp4dev --smtpport 2525` e atualize `appsettings.Development.json`).

**Catálogo vazio após reinício**
Se o banco já tinha dados sem áudio (seed antigo), o seeder detecta a condição e reinsere tudo a partir dos MP3s. Certifique-se de que a pasta `Mp3/` contém os arquivos.
