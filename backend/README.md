# Backend — ASP.NET Core 8, Clean Architecture

## Couches
| Projet | Rôle | Dépend de |
| --- | --- | --- |
| `Domain` | Entités et règles métier | rien |
| `Application` | Cas d'usage | Domain |
| `Infrastructure` | SQL Server, service IA, health checks | Application |
| `Api` | Endpoints HTTP, composition | Application, Infrastructure |

Ces règles sont vérifiées par `tests/KnowledgeKeeper.ArchitectureTests`.

## Configuration locale (une fois)
```bash
dotnet user-secrets set "ConnectionStrings:KnowledgeKeeper" "Server=localhost,14330;User Id=sa;Password=<MOT_DE_PASSE_DE_infra/.env>;TrustServerCertificate=True" --project src/KnowledgeKeeper.Api
```

## Lancer
```bash
dotnet run --project src/KnowledgeKeeper.Api
```
- `GET /health/live` : le processus répond
- `GET /health/ready` : SQL Server est joignable
- `/swagger` : documentation de l'API

## Tester (Docker doit tourner)
```bash
dotnet test
```
