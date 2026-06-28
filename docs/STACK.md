# 🛠️ Stack Tecnológico - Spotflix

Documento que lista todas as tecnologias utilizadas no frontend e backend do projeto Spotflix.

---

## 📱 Frontend

### Tecnologia Principal
- **Framework:** Angular 21.2.0
- **Linguagem:** TypeScript 5.9.2
- **Gerenciador de Pacotes:** npm 10.9.2

### Dependências Principais

| Pacote | Versão | Descrição |
|--------|--------|-----------|
| `@angular/core` | ^21.2.0 | Core do Angular - funcionalidades centrais |
| `@angular/common` | ^21.2.0 | Componentes e diretivas comuns |
| `@angular/forms` | ^21.2.0 | Formulários e validação |
| `@angular/router` | ^21.2.0 | Roteamento de aplicação |
| `@angular/platform-browser` | ^21.2.0 | Plataforma de navegador |
| `@angular/compiler` | ^21.2.0 | Compilador Angular |
| `@angular/build` | ^21.2.12 | Build tools |
| `@angular/cli` | ^21.2.12 | CLI para desenvolvimento |
| `@angular/compiler-cli` | ^21.2.0 | Compiler CLI |
| `rxjs` | ~7.8.0 | Programação reativa |
| `tslib` | ^2.3.0 | Utilitários do TypeScript |

### DevDependencies (Desenvolvimento)

| Pacote | Versão | Descrição |
|--------|--------|-----------|
| `typescript` | ~5.9.2 | Superset de JavaScript com tipagem estática |
| `prettier` | ^3.8.1 | Formatador de código |
| `vitest` | ^4.0.8 | Framework de testes (compatível com Vite) |
| `jsdom` | ^28.0.0 | Implementação do DOM para testes |

### Scripts Disponíveis

```bash
npm start          # Inicia servidor de desenvolvimento (ng serve)
npm run build      # Build de produção
npm run watch      # Build em modo watch
npm test           # Executa testes
npm run ng         # Acesso direto ao Angular CLI
```

### Arquitetura Frontend
- **Single Page Application (SPA)**
- **Componentes reutilizáveis**
- **Reatividade com RxJS**
- **Roteamento cliente-lado**

---

## 🔧 Backend

### Tecnologia Principal
- **Framework:** ASP.NET Core 10.0
- **Linguagem:** C# 12.0
- **Plataforma:** .NET 10.0
- **Banco de Dados:** PostgreSQL

### Dependências Principais

| Pacote | Versão | Descrição |
|--------|--------|-----------|
| `Microsoft.AspNetCore.Authentication.JwtBearer` | 10.0.8 | Autenticação com JWT (JSON Web Token) |
| `Microsoft.AspNetCore.Identity.EntityFrameworkCore` | 10.0.8 | Identidade e autenticação de usuários |
| `Microsoft.EntityFrameworkCore.Design` | 10.0.8 | ORM para acesso a dados |
| `Microsoft.AspNetCore.OpenApi` | 10.0.7 | Suporte OpenAPI 3.0 |
| `Npgsql.EntityFrameworkCore.PostgreSQL` | 10.0.2 | Provider do Entity Framework para PostgreSQL |
| `Swashbuckle.AspNetCore` | 10.2.1 | Swagger UI para documentação de API |

### Configurações

**Framework Target:** .NET 10.0  
**Nullable:** Habilitado (strict null checking)  
**Implicit Usings:** Habilitado  
**User Secrets:** Configurado (ID: 15a3d836-e401-45c0-8dc2-862f3066920e)

### Arquitetura Backend
- **RESTful API**
- **Autenticação JWT**
- **Entity Framework Core (ORM)**
- **Documentação com Swagger/OpenAPI**
- **Gerenciamento de identidade integrado**

### Diretório Estrutura
```
src/
├── Spotflix.Api/          # Projeto principal da API
│   ├── Controllers/       # Controllers REST
│   ├── Models/           # Modelos de dados
│   ├── Services/         # Lógica de negócio
│   ├── Data/             # DbContext e migrações
│   └── Spotflix.Api.csproj
└── spotflix-web/          # Projeto Angular
    ├── src/
    ├── package.json
    └── angular.json
```

---

## 🔗 Integração Frontend-Backend

### Comunicação
- **Protocolo:** HTTP/HTTPS (REST)
- **Formato:** JSON
- **Autenticação:** JWT Token (Bearer Token)

### Headers Padrão
```
Authorization: Bearer {token}
Content-Type: application/json
```

### Fluxo de Autenticação
1. User faz login via Frontend (Angular)
2. Backend (ASP.NET) valida credenciais no banco PostgreSQL
3. Se válido, retorna JWT token
4. Frontend armazena token
5. Frontend envia token em cada requisição autenticada
6. Backend valida token nas requisições

---

## 🗄️ Banco de Dados

### PostgreSQL
- **Versão:** 10.x (conforme suporte do Npgsql)
- **ORM:** Entity Framework Core 10.0.2
- **Tabelas Principais:**
  - Users (Usuários)
  - Cards (Cartões de crédito)
  - Subscriptions (Assinaturas)
  - Transactions (Transações)
  - Songs (Músicas)
  - Bands (Bandas)
  - Albums (Álbuns)
  - Favorites (Favoritos)

### Migrações
Gerenciadas através do Entity Framework Core Design tools

---

## 🔐 Segurança

### Implementado
- ✅ JWT Bearer Authentication
- ✅ ASP.NET Core Identity
- ✅ Senha hasheada (via Identity)
- ✅ User Secrets para configurações sensíveis
- ✅ HTTPS/TLS

### Recomendações
- Utilizar HTTPS em produção
- Manter User Secrets seguros
- Validar entrada no backend
- CORS configurado corretamente

---

## 📊 Desenvolvimentos e Testes

### Frontend
- **Testes Unitários:** Vitest
- **JSDOM:** Simulação do DOM para testes
- **Formatação:** Prettier

### Backend
- **Testes:** MSTest/xUnit (não listado no .csproj, mas padrão .NET)
- **Documentação:** Swagger UI (Swashbuckle)

---

## 🚀 Deployment

### Frontend
- **Build:** `npm run build`
- **Output:** Arquivos estáticos (HTML, CSS, JS)
- **Hosting:** Pode ser feito em qualquer servidor web estático
- **Alternativa:** Servir via Express.js ou similar

### Backend
- **Build:** Publicação via .NET CLI
- **Runtime:** .NET 10.0 Runtime
- **Hosting:** IIS, Docker, Linux, etc
- **Database:** PostgreSQL (deve estar disponível)

---

## 📦 Versionamento

| Componente | Versão |
|-----------|--------|
| Node.js | 18+ (para npm 10.9.2) |
| .NET | 10.0 |
| PostgreSQL | 12+ |
| npm | 10.9.2 |
| Angular | 21.2.0 |
| TypeScript | 5.9.2 |

---

## 🔄 Fluxo de Desenvolvimento

```
┌─────────────────────────────────────┐
│ Desenvolvimento Local               │
├─────────────────────────────────────┤
│ Frontend: ng serve (porta 4200)     │
│ Backend: dotnet run (porta 5000)    │
│ Database: PostgreSQL                │
└─────────────────────────────────────┘
          ↓
┌─────────────────────────────────────┐
│ Testes                              │
├─────────────────────────────────────┤
│ Frontend: npm test (vitest)         │
│ Backend: dotnet test               │
└─────────────────────────────────────┘
          ↓
┌─────────────────────────────────────┐
│ Build para Produção                 │
├─────────────────────────────────────┤
│ Frontend: npm run build             │
│ Backend: dotnet publish             │
└─────────────────────────────────────┘
```

---

## 🎯 Comparativo de Tecnologias

### Frontend: Por que Angular?
- ✅ Framework completo (não precisa de muitas bibliotecas extras)
- ✅ TypeScript nativo
- ✅ Excelente para aplicações empresariais
- ✅ Comunidade ativa
- ✅ Roteamento e formulários embutidos

### Backend: Por que .NET?
- ✅ Performance excelente
- ✅ Segurança robusta
- ✅ Ecossistema maduro
- ✅ Entity Framework Core poderoso
- ✅ Suporte oficial da Microsoft

### Banco de Dados: Por que PostgreSQL?
- ✅ Open Source
- ✅ ACID completo
- ✅ JSON suporte nativo
- ✅ Escalável
- ✅ Free tier em muitos serviços cloud

---

## 📝 Arquivo de Configuração

### Frontend (.angular.json)
Arquivo de configuração do Angular que define:
- Diretórios de origem
- Configurações de build
- Proxies para API

### Backend (appsettings.json)
Arquivo de configuração do ASP.NET que contém:
- Connection strings
- Configurações de logging
- JWT settings
- CORS settings

---

## 🔗 Links Úteis

- [Angular Documentation](https://angular.io/docs)
- [ASP.NET Core Documentation](https://learn.microsoft.com/en-us/aspnet/core/)
- [Entity Framework Core](https://learn.microsoft.com/en-us/ef/core/)
- [PostgreSQL Documentation](https://www.postgresql.org/docs/)
- [RxJS Documentation](https://rxjs.dev/)
- [TypeScript Documentation](https://www.typescriptlang.org/docs/)

---

## 📌 Resumo da Stack

```
┌──────────────────────────────────────────────────────┐
│                   SPOTFLIX STACK                      │
├──────────────────────────────────────────────────────┤
│                                                       │
│  Frontend          Backend              Database     │
│  ─────────         ───────              ────────     │
│  Angular 21        .NET 10              PostgreSQL   │
│  TypeScript 5.9    C# 12                EF Core 10   │
│  RxJS 7.8          ASP.NET Core 10      Npgsql 10    │
│  npm 10.9          JWT Auth             SQL Queries  │
│  Vitest 4.0        Swagger 10.2                      │
│  Prettier 3.8                                        │
│                                                       │
└──────────────────────────────────────────────────────┘
```

---

**Última atualização:** 23 de junho de 2026  
**Versão:** 1.0  
**Status:** Completo

*Stack moderno, robusto e escalável para uma plataforma de streaming de música!* 🎵
