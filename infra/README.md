# Infra — environnement local

## Démarrer
```bash
cd infra
copy .env.example .env      # Windows (cp sous Linux/macOS), puis changer le mot de passe
docker compose up -d
docker compose ps           # attendre "healthy"
```

## Vérifier
```bash
docker compose exec sqlserver bash /scripts/smoke-test.sh
```

## Se connecter (SSMS, Azure Data Studio, Rider)
- Serveur : `localhost,14330`
- Login : `sa` / mot de passe de `infra/.env`
- Cocher **Trust server certificate**

## Arrêter
```bash
docker compose down        # garde les données
docker compose down -v     # supprime aussi les données
```
