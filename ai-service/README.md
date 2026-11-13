# AI service — FastAPI + LangChain

Service interne : seule l'API .NET l'appelle, avec le header `X-Internal-Api-Key`.

## Installer
```bash
uv sync
copy .env.example .env    # puis mettre une vraie clé de 32 caractères minimum
```

## Lancer
```bash
uv run uvicorn knowledge_keeper_ai.main:create_app --factory --reload --port 8001
```
Documentation interactive : http://localhost:8001/docs

## Qualité
```bash
uv run pytest
uv run ruff check .
uv run ruff format --check .
```
