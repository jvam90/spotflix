# 🚀 Guia de Início Rápido - Spotflix

## Pré-requisitos

- .NET 10.0 SDK
- PostgreSQL 15+
- Node.js 18+ (para o frontend Angular)
- Git
- smtp4dev (servidor SMTP local para desenvolvimento — ver seção 3)

## 1. Configurar Banco de Dados

### PostgreSQL (Windows)

```bash
# Iniciar PostgreSQL (se não estiver rodando)
# Já está configurado como serviço por padrão

# Criar banco de dados (opcional - migrations criam automaticamente)
psql -U postgres -c "CREATE DATABASE spotflix_db;"
```

### Configurar Connection String

**Arquivo:** `src/Spotflix.Api/appsettings.Development.json`

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

## 2. Rodar a Aplicação Backend

```bash
cd src/Spotflix.Api

# Restaurar dependências
dotnet restore

# Aplicar migrações e seed de dados
dotnet ef database update
# Ou rodar direto (migration é automática):
dotnet run
```

**Saída esperada:**
```
info: Microsoft.Hosting.Lifetime[14]
      Now listening on: https://localhost:7001
```

## 3. Configurar Envio de Email (smtp4dev)

O Spotflix exige confirmação de email no cadastro. Em desenvolvimento, usamos o **smtp4dev** — um servidor SMTP local que captura os emails sem enviá-los de verdade, exibindo-os numa interface web.

### Instalação (uma vez)

```bash
dotnet tool install -g Rnwood.Smtp4dev
```

### Iniciar antes de rodar a API

```bash
smtp4dev
```

O smtp4dev sobe automaticamente em:
- **SMTP:** `localhost:25` (a API envia para cá)
- **Interface web:** `http://localhost:5000` (onde você lê os emails)

> **Dica:** Deixe o smtp4dev rodando em segundo plano enquanto desenvolve. Pode minimizar o terminal.

### Fluxo de confirmação de conta

1. Cadastrar novo usuário via `POST /api/auth/register`
2. Abrir `http://localhost:5000` — o email de confirmação aparece na caixa de entrada
3. Copiar o `userId` e o `token` exibidos no corpo do email
4. Usar `POST /api/auth/confirm-email` com os valores copiados

## 4. Rodar o Frontend (Opcional)

```bash
cd src/spotflix-web

# Instalar dependências
npm install

# Iniciar servidor de desenvolvimento
npm start
```

**Acesso:** http://localhost:4200

## 5. Logins de Teste Disponíveis

### Usuários de Teste
```
Email: user1@spotflix.com
Senha: Senha@123

Email: user2@spotflix.com
Senha: Senha@123

Email: user3@spotflix.com
Senha: Senha@123
```

### Usuário Admin
```
Email: admin@spotflix.com
Senha: Admin@123456
```

## 6. Explorar a API

### Via Swagger (Recomendado)

Acesse: **https://localhost:7001/swagger/index.html**

### Endpoints Principais

#### 🔐 Autenticação
```bash
# Register
POST /api/auth/register
Body: {
  "email": "novo@email.com",
  "password": "Senha@123",
  "fullName": "Novo Usuário"
}
# → Após o cadastro, acesse http://localhost:5000 para ler o email
#   com o userId e token de confirmação.

# Confirmar email (obrigatório para fazer login)
POST /api/auth/confirm-email
Body: {
  "userId": "...",
  "token": "..."
}

# Login
POST /api/auth/login
Body: {
  "email": "user1@spotflix.com",
  "password": "Senha@123"
}

# Logout
POST /api/auth/logout
Header: Authorization: Bearer {token}
Body: {
  "refreshToken": "{refresh_token}"
}
```

#### 🎵 Busca de Música
```bash
# Buscar
GET /api/search?q=beatles&limit=10
```

**Resposta:**
```json
{
  "bands": [
    {
      "id": "...",
      "name": "The Beatles",
      "genre": "Rock",
      "formedYear": 1960,
      "albumCount": 2
    }
  ],
  "songs": [
    {
      "id": "...",
      "title": "Come Together",
      "durationSeconds": 259,
      "albumId": "...",
      "albumTitle": "Abbey Road",
      "bandId": "...",
      "bandName": "The Beatles"
    }
  ]
}
```

#### ⭐ Favoritos
```bash
# Listar músicas favoritas
GET /api/favorites/songs
Header: Authorization: Bearer {token}

# Adicionar música aos favoritos
PUT /api/favorites/songs/{songId}
Header: Authorization: Bearer {token}

# Remover música dos favoritos
DELETE /api/favorites/songs/{songId}
Header: Authorization: Bearer {token}

# Listar bandas favoritas
GET /api/favorites/bands
Header: Authorization: Bearer {token}

# Adicionar banda aos favoritos
PUT /api/favorites/bands/{bandId}
Header: Authorization: Bearer {token}
```

#### 💳 Transações
```bash
# Autorizar transação
POST /api/transactions/authorize
Header: Authorization: Bearer {token}
Body: {
  "cardId": "...",
  "merchant": "Spotify",
  "amount": 19.90
}

# Ver histórico de transações
GET /api/cards/{cardId}/transactions
Header: Authorization: Bearer {token}
```

#### 📊 Planos e Assinaturas
```bash
# Listar planos
GET /api/plans

# Ver minhas assinaturas
GET /api/subscriptions/me
Header: Authorization: Bearer {token}
```

## 7. Dados Disponíveis para Teste

### Bandas (já seadas)
- The Beatles
- Pink Floyd
- Queen
- David Bowie
- Led Zeppelin
- The Rolling Stones

### Cartões de Crédito
Cada usuário de teste já tem 2 cartões:
- Visa (Last4: 1234)
- Mastercard (Last4: 5678)

### Transações de Exemplo
Já existem 10 transações no histórico dos primeiros 2 usuários.

## 8. Usar via Postman/Insomnia

### 1. Criar Collection
- Arquivo: `Spotflix.Api.http` (já existe no projeto)

### 2. Configurar Environment
```json
{
  "baseUrl": "https://localhost:7001",
  "token": "",
  "refreshToken": ""
}
```

### 3. Primeiro Request (Login)
```
POST {{baseUrl}}/api/auth/login
Content-Type: application/json

{
  "email": "user1@spotflix.com",
  "password": "Senha@123"
}
```

Copie o `accessToken` para usar nos próximos requests.

## 9. Estrutura de Diretórios

```
spotflix/
├── src/
│   ├── Spotflix.Api/              # Backend ASP.NET Core
│   │   ├── Controllers/           # Endpoints
│   │   ├── Services/              # Lógica de negócios
│   │   │   ├── SmtpEmailSender.cs # Envio de email (smtp4dev em dev)
│   │   │   └── LoggingEmailSender.cs # Stub de log (não usado por padrão)
│   │   ├── Configuration/
│   │   │   └── SmtpSettings.cs    # Configuração SMTP
│   │   ├── Data/
│   │   │   ├── Repositories/      # Padrão Repository
│   │   │   └── Migrations/        # Migrações EF
│   │   ├── Models/                # Entidades de negócio
│   │   ├── Dtos/                  # DTOs para requests/responses
│   │   └── Program.cs             # Configuração startup
│   │
│   └── spotflix-web/              # Frontend Angular
│       ├── src/
│       │   ├── app/
│       │   │   ├── core/          # Services compartilhados
│       │   │   ├── features/      # Módulos de features
│       │   │   └── shared/        # Componentes reutilizáveis
│       │   └── index.html
│       └── package.json
│
├── REPOSITORY_PATTERN.md          # Documentação do padrão
├── SEED_DATA.md                   # Documentação do seed
└── README.md
```

## 10. Troubleshooting

### Erro: "Connection to localhost:5432 refused"
```bash
# Verificar se PostgreSQL está rodando
# Windows: Services > PostgreSQL

# Linux:
sudo systemctl start postgresql

# Mac:
brew services start postgresql
```

### Erro: "FATAL: database 'spotflix_db' does not exist"
```bash
# As migrações criam o banco automaticamente
# Se não funcionar:
dotnet ef database drop
dotnet ef database update
```

### Erro: "No such table or column"
```bash
# Aplicar migrações pendentes
dotnet ef database update
```

### Porta 7001 já em uso
```bash
# Alternar porta no launchSettings.json
# Ou usar variável de ambiente:
dotnet run --urls "https://localhost:5001"
```

### Email não aparece no smtp4dev

```bash
# Verificar se o smtp4dev está rodando
smtp4dev

# A interface web fica em http://localhost:5000
# A porta SMTP é a 25 — verifique se não está bloqueada por antivírus/firewall

# Se a porta 25 estiver bloqueada, altere em appsettings.Development.json:
# "Smtp": { "Port": 2525 }
# e reinicie o smtp4dev na porta alternativa:
smtp4dev --smtpport 2525
```

### Erro ao instalar smtp4dev
```bash
# Verificar se o .NET SDK está instalado
dotnet --version

# Atualizar ferramentas globais
dotnet tool update -g Rnwood.Smtp4dev

# Listar ferramentas instaladas
dotnet tool list -g
```

## 11. Próximos Passos

1. ✅ Explorar endpoints no Swagger
2. ✅ Fazer login com usuário de teste
3. ✅ Buscar músicas (Beatles)
4. ✅ Adicionar favoritos
5. ✅ Autorizar uma transação
6. ✅ Ver histórico de transações
7. ✅ Criar nova conta
8. ✅ Confirmar email via smtp4dev (`http://localhost:5000`)
9. ✅ Assinar um plano

## 📚 Documentação Completa

- [REPOSITORY_PATTERN.md](REPOSITORY_PATTERN.md) - Padrão Repository
- [SEED_DATA.md](SEED_DATA.md) - Dados Seados
- [RUBRICA_AVALIACAOO.md](RUBRICA_AVALIACAOO.md) - Verificação de Rubrica
- [README.md](README.md) - Visão Geral do Projeto

## 🆘 Suporte

Para problemas ou dúvidas:
1. Verifique os logs da aplicação
2. Consulte a documentação correspondente
3. Verifique o Swagger da API

---

**Última atualização:** 27 de junho de 2026
