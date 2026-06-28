# Spotflix Web

Frontend Angular do Spotflix — streaming de música (bandas, álbuns e playlists).
Consome a API REST em `src/Spotflix.Api` (ASP.NET Core / JWT).

## Stack

- **Angular 21** (standalone components, **zoneless**, signals, novo control flow `@if`/`@for`)
- Roteamento com lazy-loading por feature
- `HttpClient` + interceptor de autenticação (Bearer + refresh token automático)
- SCSS com tema escuro inspirado no Spotify
- Sem dependências de UI externas

## Funcionalidades cobertas (espelham os 8 requisitos da API)

| Requisito | Onde |
|-----------|------|
| Conta / cadastro / confirmação de e-mail / recuperação de senha | `features/auth/*` |
| Login (JWT + refresh) | `features/auth/login`, `core/auth/*` |
| Catálogo (bandas → álbuns → faixas) | `features/home`, `band-detail`, `album-detail` |
| Busca (bandas e músicas) | `features/search` |
| Assinatura de planos | `features/plans` |
| Transações / cartões | `features/account` |
| Favoritos (músicas e bandas) | `features/favorites`, `shared/song-row` |
| Notificações (in-app/toasts) | `core/notify`, `shared/toast` |
| Administração (catálogo, planos, usuários) | `features/admin/*` |

Há também um **player simulado** (barra inferior): o catálogo guarda apenas metadados
(sem arquivos de áudio), então a reprodução avança um playhead virtual.

## Pré-requisitos

- Node.js 20+ e npm
- A API rodando em `http://localhost:5286` (perfil `http` em `Spotflix.Api`)

> O `proxy.conf.json` encaminha `/api/*` para `http://localhost:5286`, evitando CORS em
> desenvolvimento. Se a API rodar em outra porta, ajuste o `target` lá.

## Como rodar

```bash
cd src/spotflix-web
npm install
npm start          # ng serve com proxy → http://localhost:4200
```

Em outro terminal, suba a API:

```bash
cd src/Spotflix.Api
dotnet run          # http://localhost:5286
```

## Build de produção

```bash
npm run build      # saída em dist/spotflix-web
```

## Notas de autenticação

- O token JWT e o refresh token ficam em `localStorage` (`sfx.access` / `sfx.refresh`).
- O interceptor anexa o Bearer e, em `401`, tenta um único refresh compartilhado antes
  de redirecionar ao login.
- Papéis são lidos do claim `.../identity/claims/role`; rotas de administração exigem `Admin`.
- A confirmação de e-mail, em desenvolvimento, usa o `userId`+`token` que a API registra no
  log (o `LoggingEmailSender` não envia e-mails reais). Cole-os em **/auth/confirm-email**.

## Estrutura

```
src/app/
  core/          # serviços, modelos, auth (service/interceptor/guards), player, utils
  layout/shell/  # casca autenticada (sidebar, topbar, player bar)
  shared/        # componentes reutilizáveis (song-row, media-card, toasts, player-bar)
  features/      # páginas lazy: auth, home, band/album detail, search, favorites,
                 # plans, account, admin (catalog/plans/users)
```
