# Seed de Dados Completo - Spotflix

## Visão Geral

O seed de dados foi expandido para Popular o banco de dados com informações realistas e completas, permitindo que a aplicação seja iniciada com dados prontos para uso e testes.

**Status:** ✅ Implementado  
**Data:** 22 de junho de 2026

## Dados Seados

### 1. Papéis (Roles)
Criados automaticamente no startup:
- `admin` - Administrador
- `user` - Usuário comum

### 2. Usuários (3 usuários de teste + 1 admin)

#### Usuário Admin
- **Email:** `admin@spotflix.com` (configurável via `appsettings.json`)
- **Senha:** Configurável via `Seed:AdminPassword`
- **Role:** Admin
- **Status:** Email confirmado

#### Usuários de Teste
| Email | Nome | Senha | Role |
|-------|------|-------|------|
| user1@spotflix.com | João Silva | Senha@123 | User |
| user2@spotflix.com | Maria Santos | Senha@123 | User |
| user3@spotflix.com | Pedro Oliveira | Senha@123 | User |

**Status:** Todos com email confirmado (prontos para login)

### 3. Planos (3 planos)

| Nome | Preço | Período | Descrição |
|------|-------|---------|-----------|
| Gratuito | R$ 0,00 | Mensal | Acesso limitado com anúncios |
| Premium | R$ 19,90 | Mensal | Sem anúncios, qualidade alta, sem limite |
| Family | R$ 34,90 | Mensal | Até 6 perfis, Premium completo |

Todos os planos estão ativos (`IsActive = true`)

### 4. Catálogo de Música

#### Bandas (6 bandas clássicas)
1. **The Beatles** (Rock, 1960)
2. **Pink Floyd** (Rock Progressivo, 1965)
3. **Queen** (Rock, 1970)
4. **David Bowie** (Rock, 1967)
5. **Led Zeppelin** (Hard Rock, 1968)
6. **The Rolling Stones** (Rock, 1962)

#### Álbuns (9 álbuns clássicos)

**The Beatles:**
- Abbey Road (1969) - 4 músicas
- The White Album (1968) - 4 músicas

**Pink Floyd:**
- The Dark Side of the Moon (1973) - 4 músicas
- Wish You Were Here (1975) - 4 músicas

**Queen:**
- A Night at the Opera (1975) - 4 músicas
- News of the World (1977) - 4 músicas

**David Bowie:**
- The Rise and Fall of Ziggy Stardust (1972) - 4 músicas

**Led Zeppelin:**
- Led Zeppelin IV (1971) - 4 músicas

**The Rolling Stones:**
- Satisfaction (1965) - 4 músicas

#### Músicas (36 músicas clássicas)

**Abbey Road (The Beatles):**
- Come Together (259s)
- Something (183s)
- Maxwell's Silver Hammer (207s)
- Oh! Darling (210s)

**The White Album (The Beatles):**
- Back in the U.S.S.R. (230s)
- Dear Prudence (241s)
- Glass Onion (246s)
- Ob-La-Di, Ob-La-Da (171s)

**The Dark Side of the Moon (Pink Floyd):**
- Speak to Me (90s)
- Breathe (163s)
- On the Run (345s)
- Time (408s)

**Wish You Were Here (Pink Floyd):**
- Shine On You Crazy Diamond (745s)
- Welcome to the Machine (360s)
- Have a Cigar (330s)
- Wish You Were Here (295s)

**A Night at the Opera (Queen):**
- Death on Two Legs (356s)
- Lazing on a Sunday Afternoon (282s)
- I'm in Love with My Car (199s)
- Bohemian Rhapsody (354s)

**News of the World (Queen):**
- We Will Rock You (125s)
- Another One Bites the Dust (215s)
- Sheer Heart Attack (199s)
- All Dead, All Dead (227s)

**The Rise and Fall of Ziggy Stardust (David Bowie):**
- Five Years (300s)
- Soul Love (310s)
- Moonage Daydream (215s)
- Starman (260s)

**Led Zeppelin IV:**
- Black Dog (296s)
- Rock and Roll (236s)
- The Battle of Evermore (367s)
- Stairway to Heaven (482s)

**Satisfaction (The Rolling Stones):**
- Tell Me (219s)
- (I Can't Get No) Satisfaction (264s)
- Grown Up Wrong (172s)
- Down Home Girl (223s)

### 5. Cartões de Crédito (6 cartões)

Cada um dos 3 usuários de teste recebe 2 cartões:

**Usuário 1 (João Silva):**
- Visa (Last4: 1234) - Limite: R$ 5.000,00
- Mastercard (Last4: 5678) - Limite: R$ 3.000,00

**Usuário 2 (Maria Santos):**
- Visa (Last4: 1234) - Limite: R$ 5.000,00
- Mastercard (Last4: 5678) - Limite: R$ 3.000,00

**Usuário 3 (Pedro Oliveira):**
- Visa (Last4: 1234) - Limite: R$ 5.000,00
- Mastercard (Last4: 5678) - Limite: R$ 3.000,00

Todos os cartões estão **ativos** e com limites disponíveis.

### 6. Transações (10 transações de exemplo)

**Para os primeiros 2 usuários:**
- 5 transações cada
- Comerciantes: Spotify, Apple Music, YouTube Music, Deezer, Tidal, Amazon Music
- Valores: R$ 10 a R$ 110 (aleatórios)
- Datas: Últimos 5 dias
- Status: Todas autorizadas

**Exemplo:**
```
- Spotify: R$ 45,50 (há 5 dias)
- Apple Music: R$ 67,20 (há 4 dias)
- YouTube Music: R$ 23,80 (há 3 dias)
- Deezer: R$ 89,10 (há 2 dias)
- Tidal: R$ 55,30 (há 1 dia)
```

### 7. Assinaturas (3 subscriptions)

| Usuário | Plano | Status | Período |
|---------|-------|--------|---------|
| João Silva (user1) | Premium | Ativa | Há 1 mês até +1 mês |
| Maria Santos (user2) | Family | Ativa | Há 2 meses até +1 mês |
| Pedro Oliveira (user3) | Premium | Expirada | Há 1 mês até -1 dia |

### 8. Favoritos (8 favoritos de músicas + 3 de bandas)

**João Silva (user1):**
- Músicas: 3 músicas favoritas
- Bandas: The Beatles

**Maria Santos (user2):**
- Músicas: 2 músicas favoritas
- Bandas: Pink Floyd, Queen

**Pedro Oliveira (user3):**
- Músicas: 2 músicas favoritas

## Fluxo de Seeding

O seed é executado automaticamente no startup da aplicação no método `DbSeeder.SeedAsync()`:

```csharp
// Program.cs
await DbSeeder.SeedAsync(app.Services);
```

### Ordem de Execução

1. **Aplicar migrações** - Cria o schema do banco
2. **Seed de papéis** - Cria roles (admin, user)
3. **Seed de admin** - Cria usuário administrador (se configurado)
4. **Seed de usuários teste** - Cria 3 usuários de teste
5. **Seed de planos** - Cria 3 planos de assinatura
6. **Seed de catálogo** - Cria bandas, álbuns e músicas
7. **Seed de cartões** - Cria cartões para usuários
8. **Seed de transações** - Cria histórico de transações
9. **Seed de assinaturas** - Cria subscriptions dos usuários
10. **Seed de favoritos** - Cria favoritos dos usuários

### Idempotência

Cada função de seed verifica se os dados já existem antes de inserir:

```csharp
if (await db.Plans.AnyAsync())
    return; // Não insere novamente
```

Isso garante que:
- ✅ Pode executar `dotnet ef database update` múltiplas vezes sem duplicar
- ✅ Dados existentes não são sobrescrito
- ✅ Seguro para ambientes de desenvolvimento

## Configuração

### Credenciais do Admin (appsettings.json)

```json
{
  "Seed": {
    "AdminEmail": "admin@spotflix.com",
    "AdminPassword": "SuaSenhaForte@123"
  }
}
```

Se não configurado, o seed pula a criação do usuário admin.

### Variáveis de Ambiente (Opcional)

```bash
# Windows
set Seed__AdminEmail=admin@spotflix.com
set Seed__AdminPassword=SuaSenhaForte@123

# Linux/Mac
export Seed__AdminEmail=admin@spotflix.com
export Seed__AdminPassword=SuaSenhaForte@123
```

## Como Usar

### 1. Primeira Execução

```bash
cd src/Spotflix.Api
dotnet run
```

Automaticamente:
- Cria o banco de dados
- Aplica migrações
- Popula com dados de teste

### 2. Explorar os Dados

**Login com usuário de teste:**
```
Email: user1@spotflix.com
Senha: Senha@123
```

**Ou com admin:**
```
Email: admin@spotflix.com
Senha: SuaSenhaForte@123
```

### 3. Testes com os Dados

**Buscar músicas:**
```bash
GET /api/search?q=beatles&limit=10
```

**Listar favoritos:**
```bash
GET /api/favorites/songs
GET /api/favorites/bands
```

**Ver histórico de transações:**
```bash
GET /api/cards/{cardId}/transactions
```

**Autorizar transação:**
```bash
POST /api/transactions/authorize
Body: {
  "cardId": "...",
  "merchant": "Spotify",
  "amount": 19.90
}
```

## Dados Estatísticos

| Tipo | Quantidade |
|------|-----------|
| Usuários | 4 (1 admin + 3 teste) |
| Planos | 3 |
| Bandas | 6 |
| Álbuns | 9 |
| Músicas | 36 |
| Cartões | 6 |
| Transações | 10 |
| Subscriptions | 3 |
| Favoritos (Músicas) | 8 |
| Favoritos (Bandas) | 3 |

**Total de registros:** ~80+ registros para começar

## Notas Importantes

1. **Não remova o seed em produção** - Apenas use em desenvolvimento
2. **Customize os dados** - Edite `DbSeeder.cs` para seus dados específicos
3. **Senhas seguras** - Altere `Seed:AdminPassword` antes de deployar
4. **Email simulado** - O LoggingEmailSender apenas loga, não envia realmente
5. **Dados aleatórios** - Transações têm valores aleatórios

## Próximas Melhorias

- [ ] Adicionar mais de 36 músicas
- [ ] Criar seeds por ambiente (Dev/Test/Staging)
- [ ] Adicionar seed para imagens/covers de álbuns
- [ ] Implementar seed factories com Bogus
- [ ] Adicionar histórico de transações recusadas

---

**Documentação criada em:** 22 de junho de 2026  
**Status:** ✅ Pronto para uso
