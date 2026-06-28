# 📋 Casos de Uso - Spotflix

Documento que descreve todos os casos de uso da plataforma de streaming de música Spotflix.

---

## 1. Autenticação e Gerenciamento de Conta

### UC-001: Criar Conta
**Descrição:** Um novo usuário se registra na plataforma fornecendo email e senha.

**Atores:** Visitante (usuário não autenticado)

**Pré-condições:**
- Usuário acessa a página de registro
- Email ainda não está registrado no sistema

**Fluxo Principal:**
1. Usuário clica em "Criar Conta"
2. Preenche formulário com email e senha
3. Digita senha com mínimo 8 caracteres
4. Clica em "Registrar"
5. Sistema envia email de confirmação
6. Usuário recebe notificação de sucesso

**Fluxo Alternativo (Erro):**
- Email já existe → Mostrar erro e sugerir recuperação de senha
- Senha fraca → Mostrar requisitos e pedir nova senha
- Email inválido → Mostrar erro de formato

**Pós-condições:**
- Conta criada com status "não confirmado"
- Email de confirmação enviado
- Usuário pode fazer login após confirmar email

---

### UC-002: Confirmar Email
**Descrição:** Usuário valida seu email clicando no link de confirmação recebido.

**Atores:** Visitante registrado

**Pré-condições:**
- Conta criada mas não confirmada
- Email de confirmação enviado

**Fluxo Principal:**
1. Usuário recebe email com link de confirmação
2. Clica no link
3. Sistema valida o token
4. Email é marcado como confirmado
5. Usuário vê mensagem de sucesso

**Fluxo Alternativo (Erro):**
- Link expirado → Oferecer reenvio de email
- Token inválido → Mostrar erro
- Email já confirmado → Redirecionar para login

**Pós-condições:**
- Email confirmado e ativo
- Usuário pode fazer login

---

### UC-003: Fazer Login
**Descrição:** Usuário registrado acessa a plataforma com email e senha.

**Atores:** Usuário registrado

**Pré-condições:**
- Conta criada e email confirmado
- Usuário não está logado

**Fluxo Principal:**
1. Usuário vai para página de login
2. Digita email e senha
3. Clica em "Entrar"
4. Sistema valida credenciais
5. Sessão criada e usuário é redirecionado para home

**Fluxo Alternativo (Erro):**
- Email não existe → Sugerir criação de conta
- Senha incorreta → Mostrar erro (após 3 tentativas, bloquear temporariamente)
- Múltiplas tentativas falhadas → Conta bloqueada por 15 minutos
- Email não confirmado → Oferecer reenvio de confirmação

**Pós-condições:**
- Usuário autenticado
- Sessão ativa
- Acesso a funcionalidades restritas liberado

---

### UC-004: Fazer Logout
**Descrição:** Usuário sai da plataforma encerrando sua sessão.

**Atores:** Usuário autenticado

**Pré-condições:**
- Usuário está logado

**Fluxo Principal:**
1. Usuário clica em "Sair" ou "Logout"
2. Sessão é encerrada
3. Usuário é redirecionado para página inicial
4. Dados sensíveis são apagados do navegador

**Pós-condições:**
- Sessão encerrada
- Usuário precisa fazer login novamente
- Dados de sessão limpos

---

### UC-005: Recuperar Senha
**Descrição:** Usuário que esqueceu sua senha solicita recuperação via email.

**Atores:** Visitante

**Pré-condições:**
- Conta existe
- Usuário não está logado

**Fluxo Principal:**
1. Usuário clica em "Esqueci a Senha"
2. Digita seu email
3. Sistema envia email com link de recuperação
4. Usuário clica no link
5. Define nova senha
6. Senha é atualizada
7. Usuário pode fazer login com nova senha

**Fluxo Alternativo (Erro):**
- Email não encontrado → Mostrar erro ou sugerir registro
- Link expirado → Oferecer nova solicitação
- Senhas não conferem → Pedir confirmação

**Pós-condições:**
- Senha resetada
- Usuário pode fazer login com nova senha

---

### UC-006: Alterar Senha
**Descrição:** Usuário autenticado muda sua senha de acesso.

**Atores:** Usuário autenticado

**Pré-condições:**
- Usuário está logado
- Acesso ao perfil/configurações

**Fluxo Principal:**
1. Usuário vai para "Configurações" ou "Meu Perfil"
2. Seleciona "Alterar Senha"
3. Digita senha atual
4. Digita nova senha (mínimo 8 caracteres)
5. Confirma nova senha
6. Clica em "Salvar"
7. Senha é atualizada com sucesso

**Fluxo Alternativo (Erro):**
- Senha atual incorreta → Mostrar erro
- Senha fraca → Mostrar requisitos
- Senhas não conferem → Pedir confirmação

**Pós-condições:**
- Senha alterada
- Sessão continua ativa

---

### UC-007: Visualizar Perfil
**Descrição:** Usuário visualiza seus dados de perfil (email, data de inscrição, etc).

**Atores:** Usuário autenticado

**Pré-condições:**
- Usuário está logado

**Fluxo Principal:**
1. Usuário clica em "Meu Perfil" ou "Configurações"
2. Sistema exibe dados da conta
3. Mostra email, data de inscrição, status de assinatura
4. Mostra plano atual

**Pós-condições:**
- Usuário pode visualizar informações de seu perfil

---

## 2. Busca e Descoberta de Músicas

### UC-008: Buscar Músicas por Termo
**Descrição:** Usuário busca músicas ou bandas usando um termo de busca.

**Atores:** Usuário autenticado

**Pré-condições:**
- Usuário está logado
- Catálogo de músicas disponível

**Fluxo Principal:**
1. Usuário acessa página de busca
2. Digita termo de busca (banda, música, álbum)
3. Clica em "Buscar" ou aguarda resultados em tempo real
4. Sistema retorna resultados relevantes
5. Resultados mostram músicas e bandas
6. Usuário clica em um resultado para ver detalhes

**Fluxo Alternativo:**
- Sem resultados → Mostrar mensagem "Nenhum resultado encontrado"
- Busca vazia → Mostrar sugestões ou catálogo popular

**Pós-condições:**
- Resultados exibidos ao usuário
- Usuário pode clicar em um resultado para detalhes

---

### UC-009: Visualizar Detalhes de Música
**Descrição:** Usuário vê informações detalhadas de uma música (nome, artista, duração, álbum).

**Atores:** Usuário autenticado

**Pré-condições:**
- Usuário buscou uma música
- Música existe no catálogo

**Fluxo Principal:**
1. Usuário clica em uma música
2. Sistema exibe:
   - Título
   - Artista/Banda
   - Duração
   - Álbum
   - Ano de lançamento
   - Opção de adicionar aos favoritos
   - Botão de reprodução

**Pós-condições:**
- Detalhes da música exibidos
- Usuário pode adicionar aos favoritos ou reproduzir

---

### UC-010: Visualizar Detalhes de Banda
**Descrição:** Usuário vê informações de uma banda (nome, ano de criação, álbuns, músicas).

**Atores:** Usuário autenticado

**Pré-condições:**
- Usuário buscou uma banda
- Banda existe no catálogo

**Fluxo Principal:**
1. Usuário clica em uma banda
2. Sistema exibe:
   - Nome da banda
   - Ano de criação
   - Gênero
   - Número de álbuns
   - Lista de álbuns
   - Opção de adicionar aos favoritos

**Pós-condições:**
- Detalhes da banda exibidos
- Usuário pode adicionar aos favoritos

---

### UC-011: Visualizar Catálogo
**Descrição:** Usuário explora o catálogo completo de música (recomendações, populares, novas).

**Atores:** Usuário autenticado

**Pré-condições:**
- Usuário está logado
- Catálogo disponível

**Fluxo Principal:**
1. Usuário acessa "Catálogo" ou "Explorar"
2. Sistema exibe:
   - Músicas mais populares
   - Bandas clássicas
   - Álbuns em destaque
3. Usuário pode navegar e clicar em itens
4. Visualiza mais detalhes ou adiciona aos favoritos

**Pós-condições:**
- Catálogo explorado
- Usuário pode descobrir novas músicas

---

## 3. Gerenciamento de Favoritos

### UC-012: Adicionar Música aos Favoritos
**Descrição:** Usuário marca uma música como favorita.

**Atores:** Usuário autenticado

**Pré-condições:**
- Usuário está logado
- Música visualizada ou encontrada

**Fluxo Principal:**
1. Usuário vê música no catálogo ou em busca
2. Clica no ícone de ❤️ (coração) ou "Adicionar aos Favoritos"
3. Sistema adiciona música à lista de favoritos
4. Ícone muda de aparência (cheio)
5. Mensagem de sucesso é exibida

**Fluxo Alternativo:**
- Música já é favorita → Remover dos favoritos
- Erro ao salvar → Mostrar mensagem de erro

**Pós-condições:**
- Música adicionada aos favoritos
- Música aparece na lista "Minhas Músicas Favoritas"

---

### UC-013: Remover Música dos Favoritos
**Descrição:** Usuário remove uma música de sua lista de favoritos.

**Atores:** Usuário autenticado

**Pré-condições:**
- Usuário está logado
- Música já é favorita

**Fluxo Principal:**
1. Usuário vê música na lista de favoritos
2. Clica no ícone ❤️ cheio
3. Sistema remove música dos favoritos
4. Ícone volta ao estado vazio
5. Mensagem de sucesso é exibida

**Pós-condições:**
- Música removida dos favoritos
- Música não aparece mais em "Minhas Músicas Favoritas"

---

### UC-014: Adicionar Banda aos Favoritos
**Descrição:** Usuário marca uma banda como favorita.

**Atores:** Usuário autenticado

**Pré-condições:**
- Usuário está logado
- Banda visualizada ou encontrada

**Fluxo Principal:**
1. Usuário vê banda no catálogo ou em busca
2. Clica no ícone de ❤️ ou "Adicionar aos Favoritos"
3. Sistema adiciona banda à lista de favoritos
4. Ícone muda de aparência
5. Mensagem de sucesso é exibida

**Pós-condições:**
- Banda adicionada aos favoritos
- Banda aparece em "Minhas Bandas Favoritas"

---

### UC-015: Remover Banda dos Favoritos
**Descrição:** Usuário remove uma banda de sua lista de favoritos.

**Atores:** Usuário autenticado

**Pré-condições:**
- Usuário está logado
- Banda já é favorita

**Fluxo Principal:**
1. Usuário vê banda na lista de favoritos
2. Clica no ícone ❤️ cheio
3. Sistema remove banda dos favoritos
4. Ícone volta ao estado vazio
5. Mensagem de sucesso é exibida

**Pós-condições:**
- Banda removida dos favoritos
- Banda não aparece mais em "Minhas Bandas Favoritas"

---

### UC-016: Visualizar Minhas Músicas Favoritas
**Descrição:** Usuário vê a lista de todas as suas músicas favoritas.

**Atores:** Usuário autenticado

**Pré-condições:**
- Usuário está logado
- Pelo menos uma música é favorita

**Fluxo Principal:**
1. Usuário clica em "Meus Favoritos" ou "Minhas Músicas"
2. Sistema exibe lista de todas as músicas marcadas como favoritas
3. Lista pode ser ordenada por nome ou data de adição
4. Usuário pode clicar em uma música para detalhes

**Fluxo Alternativo:**
- Sem favoritos → Mostrar mensagem "Nenhuma música favorita"

**Pós-condições:**
- Lista de favoritos exibida

---

### UC-017: Visualizar Minhas Bandas Favoritas
**Descrição:** Usuário vê a lista de todas as suas bandas favoritas.

**Atores:** Usuário autenticado

**Pré-condições:**
- Usuário está logado
- Pelo menos uma banda é favorita

**Fluxo Principal:**
1. Usuário clica em "Minhas Bandas Favoritas"
2. Sistema exibe lista de todas as bandas marcadas como favoritas
3. Usuário pode clicar em uma banda para ver suas músicas

**Fluxo Alternativo:**
- Sem favoritos → Mostrar mensagem "Nenhuma banda favorita"

**Pós-condições:**
- Lista de favoritos exibida

---

## 4. Gerenciamento de Cartões

### UC-018: Adicionar Cartão de Crédito
**Descrição:** Usuário cadastra um novo cartão de crédito na plataforma.

**Atores:** Usuário autenticado

**Pré-condições:**
- Usuário está logado
- Acesso à seção de "Pagamentos" ou "Meus Cartões"

**Fluxo Principal:**
1. Usuário clica em "Adicionar Cartão"
2. Preenche formulário com:
   - Número do cartão
   - Nome do titular
   - Data de validade
   - Código de segurança (CVV)
3. Clica em "Salvar"
4. Sistema valida dados do cartão
5. Cartão é criptografado e armazenado
6. Mensagem de sucesso é exibida

**Fluxo Alternativo (Erro):**
- Número inválido → Mostrar erro de validação
- Expirado → Mostrar erro
- CVV inválido → Mostrar erro

**Pós-condições:**
- Cartão cadastrado na conta do usuário
- Cartão está ativo por padrão
- Usuário pode usar para pagamento

---

### UC-019: Visualizar Meus Cartões
**Descrição:** Usuário vê a lista de cartões cadastrados em sua conta.

**Atores:** Usuário autenticado

**Pré-condições:**
- Usuário está logado
- Pelo menos um cartão cadastrado

**Fluxo Principal:**
1. Usuário acessa "Meus Cartões" em Configurações
2. Sistema exibe lista de cartões com:
   - Últimos 4 dígitos
   - Bandeira (Visa, Mastercard, etc)
   - Nome do titular
   - Data de validade
   - Status (Ativo/Inativo)
3. Usuário pode selecionar um cartão para mais opções

**Pós-condições:**
- Lista de cartões exibida

---

### UC-020: Ativar/Desativar Cartão
**Descrição:** Usuário ativa ou desativa um cartão cadastrado.

**Atores:** Usuário autenticado

**Pré-condições:**
- Usuário está logado
- Cartão cadastrado

**Fluxo Principal:**
1. Usuário acessa "Meus Cartões"
2. Encontra um cartão
3. Clica em "Desativar" ou "Ativar"
4. Sistema atualiza status do cartão
5. Mensagem de sucesso é exibida

**Regra de Negócio:**
- Não é possível desativar o cartão usado para pagamento ativo

**Pós-condições:**
- Status do cartão alterado
- Cartão inativo não pode ser usado para pagamento

---

### UC-021: Remover Cartão
**Descrição:** Usuário remove um cartão de sua conta.

**Atores:** Usuário autenticado

**Pré-condições:**
- Usuário está logado
- Cartão cadastrado

**Fluxo Principal:**
1. Usuário acessa "Meus Cartões"
2. Clica em "Remover" em um cartão
3. Sistema pede confirmação
4. Usuário confirma
5. Cartão é deletado
6. Mensagem de sucesso é exibida

**Fluxo Alternativo:**
- Cancelar remoção → Operação cancelada

**Regra de Negócio:**
- Não é possível remover o cartão ativo de uma assinatura em vigor

**Pós-condições:**
- Cartão removido
- Cartão não aparece mais na lista

---

## 5. Gerenciamento de Planos e Assinaturas

### UC-022: Visualizar Planos Disponíveis
**Descrição:** Usuário vê os planos de assinatura disponíveis com seus benefícios e preços.

**Atores:** Usuário autenticado

**Pré-condições:**
- Usuário está logado

**Fluxo Principal:**
1. Usuário acessa "Planos" ou "Assinatura"
2. Sistema exibe:
   - **Plano Gratuito** - R$ 0,00/mês
     - Acesso limitado com anúncios
   - **Plano Premium** - R$ 19,90/mês
     - Sem anúncios
     - Qualidade alta
     - Sem limite de streams
   - **Plano Family** - R$ 34,90/mês
     - Até 6 perfis
     - Sem anúncios
     - Qualidade alta
3. Cada plano mostra um botão "Escolher" ou "Assinar"

**Pós-condições:**
- Planos exibidos
- Usuário pode selecionar um plano

---

### UC-023: Escolher Plano
**Descrição:** Usuário seleciona um plano de assinatura.

**Atores:** Usuário autenticado

**Pré-condições:**
- Usuário está logado
- Visualizou planos disponíveis

**Fluxo Principal:**
1. Usuário clica em "Escolher Plano" ou "Assinar"
2. Sistema mostra resumo do plano
3. Pede para confirmar
4. Usuário confirma
5. Se for plano pago, redireciona para pagamento
6. Se for gratuito, ativa imediatamente

**Fluxo Alternativo:**
- Cancelar → Volta para página de planos

**Pós-condições:**
- Plano escolhido
- Se pago, vai para fluxo de pagamento
- Se gratuito, assinatura ativa

---

### UC-024: Visualizar Assinatura Atual
**Descrição:** Usuário vê informações sobre sua assinatura ativa.

**Atores:** Usuário autenticado

**Pré-condições:**
- Usuário está logado
- Usuário tem uma assinatura ativa

**Fluxo Principal:**
1. Usuário acessa "Minha Assinatura"
2. Sistema exibe:
   - Plano atual
   - Data de início
   - Data de renovação
   - Status (Ativo/Expirado)
   - Opções de gerenciar
3. Mostra botões para alterar ou cancelar

**Pós-condições:**
- Informações de assinatura exibidas

---

### UC-025: Alterar Plano
**Descrição:** Usuário muda de plano de assinatura.

**Atores:** Usuário autenticado

**Pré-condições:**
- Usuário está logado
- Tem uma assinatura ativa

**Fluxo Principal:**
1. Usuário acessa "Minha Assinatura"
2. Clica em "Alterar Plano"
3. Sistema exibe planos disponíveis
4. Usuário seleciona novo plano
5. Sistema calcula:
   - Valor da diferença (upgrade/downgrade)
   - Data de aplicação
6. Usuário confirma
7. Mudança é aplicada (imediata ou próxima renovação)
8. Mensagem de sucesso

**Fluxo Alternativo:**
- Downgrade → Pode haver crédito para próxima fatura

**Regra de Negócio:**
- Upgrade é imediato
- Downgrade aplica na próxima renovação
- Se upgrade, cobra diferença imediatamente

**Pós-condições:**
- Plano alterado
- Pagamento processado se necessário

---

### UC-026: Cancelar Assinatura
**Descrição:** Usuário cancela sua assinatura paga.

**Atores:** Usuário autenticado

**Pré-condições:**
- Usuário está logado
- Tem uma assinatura paga ativa

**Fluxo Principal:**
1. Usuário acessa "Minha Assinatura"
2. Clica em "Cancelar Assinatura"
3. Sistema exibe motivos (opcional)
4. Pede confirmação com aviso:
   - Perderá benefícios premium
   - Mudar para plano gratuito
5. Usuário confirma
6. Assinatura é cancelada
7. Acesso downgrade para Plano Gratuito
8. Email de confirmação enviado

**Fluxo Alternativo:**
- Cancelar operação → Volta para assinatura

**Pós-condições:**
- Assinatura cancelada
- Usuário passa para Plano Gratuito

---

## 6. Gerenciamento de Pagamentos

### UC-027: Autorizar Pagamento
**Descrição:** Usuário autoriza um pagamento para sua assinatura usando um cartão cadastrado.

**Atores:** Usuário autenticado

**Pré-condições:**
- Usuário está logado
- Escolheu um plano pago
- Tem pelo menos um cartão cadastrado

**Fluxo Principal:**
1. Sistema exibe tela de confirmação de pagamento:
   - Plano
   - Valor
   - Cartões disponíveis
2. Usuário seleciona um cartão
3. Clica em "Autorizar Pagamento"
4. Sistema envia requisição de autorização
5. Banco retorna resposta:
   - ✅ Autorizado: Transação criada e assinatura ativada
   - ❌ Recusado: Mostrar motivo do erro
6. Mensagem de resultado é exibida

**Validações de Transação:**
- Cartão deve estar ativo
- Limite de crédito suficiente
- Máximo 3 transações em 2 minutos
- Detecta transações duplicadas

**Fluxo Alternativo (Recusada):**
- Cartão inativo → "Ative seu cartão antes de pagar"
- Limite insuficiente → "Limite de crédito insuficiente"
- Muitas transações → "Muitas transações, tente novamente em 2 minutos"
- Transação duplicada → "Transação duplicada detectada"

**Pós-condições:**
- Transação registrada
- Se autorizada: Assinatura ativada
- Comprovante enviado por email

---

### UC-028: Visualizar Histórico de Transações
**Descrição:** Usuário vê todas as transações/pagamentos realizados.

**Atores:** Usuário autenticado

**Pré-condições:**
- Usuário está logado
- Pelo menos uma transação realizada

**Fluxo Principal:**
1. Usuário acessa "Histórico de Transações" ou "Minhas Transações"
2. Sistema exibe lista com:
   - Data da transação
   - Descrição (ex: "Premium mensal")
   - Valor
   - Status (Autorizado/Recusado/Pendente)
   - Cartão usado (últimos 4 dígitos)
   - Ação: Ver detalhes
3. Usuário pode filtrar por data, status ou valor
4. Pode clicar em uma transação para detalhes

**Pós-condições:**
- Histórico exibido

---

### UC-029: Visualizar Detalhes de Transação
**Descrição:** Usuário vê detalhes específicos de uma transação.

**Atores:** Usuário autenticado

**Pré-condições:**
- Usuário está logado
- Selecionou uma transação

**Fluxo Principal:**
1. Usuário clica em uma transação
2. Sistema exibe:
   - ID da transação
   - Data e hora
   - Plano e período
   - Valor cobrado
   - Cartão usado (últimos 4 dígitos)
   - Status
   - Data de processamento
   - Comprovante (pode baixar PDF)
3. Opção de imprimir ou baixar recibo

**Pós-condições:**
- Detalhes exibidos

---

## 7. Casos de Uso Especiais

### UC-030: Bloquear Conta por Múltiplas Tentativas de Login
**Descrição:** Sistema bloqueia automaticamente a conta após 3 tentativas falhadas de login.

**Atores:** Sistema (automático)

**Pré-condições:**
- 3 tentativas de login com senha incorreta

**Fluxo Principal:**
1. Usuário tenta login pela terceira vez com senha errada
2. Sistema bloqueia a conta por 15 minutos
3. Usuário vê mensagem: "Conta temporariamente bloqueada"
4. Email de segurança é enviado
5. Após 15 minutos, conta é desbloqueada automaticamente
6. Usuário pode tentar novamente

**Pós-condições:**
- Conta bloqueada por 15 minutos
- Email de alerta enviado

---

### UC-031: Detecção de Atividades Suspeitas
**Descrição:** Sistema monitora e alerta sobre atividades suspeitas.

**Atores:** Sistema (automático)

**Pré-condições:**
- Atividade suspeita detectada

**Casos de Suspeita:**
- Login de novo local/dispositivo
- Múltiplas transações em curto período
- Tentativas de pagamento com cartão expirado
- Mudanças de dados sensíveis

**Fluxo Principal:**
1. Sistema detecta atividade suspeita
2. Envia email de alerta ao usuário
3. Solicita confirmação ou validação
4. Se necessário, bloqueia a transação

**Pós-condições:**
- Usuário alertado
- Atividade pode ser validada ou bloqueada

---

### UC-032: Recuperação de Conta Bloqueada
**Descrição:** Usuário recupera acesso a uma conta bloqueada.

**Atores:** Usuário bloqueado

**Pré-condições:**
- Conta bloqueada por segurança ou múltiplas tentativas

**Fluxo Principal:**
1. Usuário clica em "Não consegui acessar"
2. Digita seu email
3. Sistema valida identidade (pode ser:)
   - Código de segurança enviado por email
   - Respostas a perguntas de segurança
   - Verificação de telefone
4. Após validação, conta é desbloqueada
5. Usuário pode fazer login ou resetar senha

**Pós-condições:**
- Conta desbloqueada
- Acesso restaurado

---

## 8. Fluxo Completo de Uso

```
┌─────────────────────┐
│ Visitante/Novo      │
└──────────┬──────────┘
           │
           ▼
    ┌─────────────────────┐
    │ Criar Conta (UC-001)│
    │ Confirmar Email     │
    │ (UC-002)            │
    └──────────┬──────────┘
               │
               ▼
    ┌──────────────────────┐
    │ Fazer Login (UC-003) │
    └──────────┬───────────┘
               │
               ▼
    ┌────────────────────────────────┐
    │ Usuário Autenticado            │
    └───┬────────────────────────┬───┘
        │                        │
        ▼                        ▼
    ┌─────────────────────┐  ┌─────────────────┐
    │ Explorar Catálogo   │  │ Meu Perfil      │
    │ (UC-008 até 011)    │  │ (UC-007, 006)   │
    └────────┬────────────┘  └─────────────────┘
             │
             ▼
    ┌─────────────────────────────────┐
    │ Adicionar aos Favoritos         │
    │ (UC-012, 013, 014, 015)         │
    └────────┬────────────────────────┘
             │
             ▼
    ┌──────────────────────────────────┐
    │ Visualizar Planos (UC-022, 023)  │
    └────────┬─────────────────────────┘
             │
             ▼
    ┌──────────────────────────────────┐
    │ Adicionar Cartão (UC-018)        │
    └────────┬─────────────────────────┘
             │
             ▼
    ┌──────────────────────────────────┐
    │ Autorizar Pagamento (UC-027)     │
    └────────┬─────────────────────────┘
             │
             ▼
    ┌──────────────────────────────────┐
    │ Usar Plataforma 🎵              │
    │ - Ouvir Música                   │
    │ - Ver Favoritos                  │
    │ - Ver Histórico (UC-028, 029)   │
    │ - Gerenciar Assinatura (UC-024) │
    └──────────────────────────────────┘
```

---

## Resumo Geral

**Total de Casos de Uso: 32**

### Por Categoria:

| Categoria | Qtd | Casos de Uso |
|-----------|-----|--------------|
| Autenticação | 7 | UC-001 até UC-007 |
| Busca e Descoberta | 4 | UC-008 até UC-011 |
| Favoritos | 6 | UC-012 até UC-017 |
| Cartões | 4 | UC-018 até UC-021 |
| Planos e Assinaturas | 5 | UC-022 até UC-026 |
| Pagamentos | 3 | UC-027 até UC-029 |
| Casos Especiais | 3 | UC-030 até UC-032 |

---

## Atores Principais

1. **Visitante**: Usuário não autenticado
2. **Usuário Autenticado**: Pessoa registrada com conta ativa
3. **Sistema**: Processamento automático (validações, alertas, bloqueios)

---

## Regras de Negócio Principais

### Segurança
- ✅ Email confirmado obrigatório para login
- ✅ Senhas mínimo 8 caracteres
- ✅ Bloqueio após 3 tentativas falhadas de login (15 minutos)
- ✅ Recuperação segura de senha via email

### Transações
- ✅ Máximo 3 transações em 2 minutos
- ✅ Cartão deve estar ativo
- ✅ Limite de crédito deve ser suficiente
- ✅ Detecta transações duplicadas

### Assinatura
- ✅ Upgrade é imediato e cobra diferença
- ✅ Downgrade aplica na próxima renovação
- ✅ Não pode remover cartão de assinatura ativa

### Favoritos
- ✅ Usuário pode ter múltiplas músicas e bandas favoritas
- ✅ Favoritos são pessoais de cada usuário

---

## Notas Adicionais

- Todos os casos de uso requerem autenticação do usuário (exceto UC-001, 002, 004, 005)
- Dados sensíveis (cartão, transações) são criptografados
- Todas as ações são registradas para auditoria
- Respostas de erro sempre mostram motivos claros
- Email é sempre confirmado para ações críticas

---

**Última atualização:** 23 de junho de 2026  
**Versão:** 1.0  
**Status:** Completo
