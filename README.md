# Korp Teste — Icaro Matheus

Sistema de emissão de Notas Fiscais desenvolvido com arquitetura de microsserviços.

## Tecnologias utilizadas

**Frontend**
- Angular 21 com componentes standalone e lazy loading
- Reactive Forms com validação
- HttpClient para consumo das APIs
- Signals para gerenciamento de estado

**Backend**
- ASP.NET Core 10 (C#)
- Entity Framework Core com PostgreSQL
- Arquitetura de microsserviços (EstoqueService + FaturamentoService)
- Comunicação HTTP entre serviços via HttpClient

**Infraestrutura**
- Docker e Docker Compose
- PostgreSQL (banco separado por serviço)

## Como executar

```bash
docker compose up --build
```

Frontend: `cd frontend/frontend && ng serve`

Acesse: http://localhost:4200

## Detalhamento técnico

**Ciclos de vida do Angular utilizados**
- `ngOnInit` — carregamento inicial dos dados via serviço

**RxJS**
- Utilizado via `HttpClient` com o operador `subscribe` para consumo das APIs REST

**Bibliotecas Angular**
- `ReactiveFormsModule` — formulários reativos com validação
- `RouterLink`, `RouterLinkActive`, `RouterOutlet` — navegação entre rotas

**Componentes visuais**
- CSS puro com variáveis, sem biblioteca de UI externa

**Frameworks no C#**
- ASP.NET Core Web API com controllers, injeção de dependência e middleware

**Tratamento de erros no backend**
- Retorno de `404 NotFound` para recursos inexistentes
- Retorno de `400 BadRequest` para saldo insuficiente ou nota já fechada
- Retorno de `503 Service Unavailable` quando o EstoqueService falha

**Uso de IA**
- Claude Code (Anthropic) utilizado como assistente durante o desenvolvimento,
gerando código comentado, estruturas de projeto e configurações Docker,
sempre com revisão humana antes de cada commit.