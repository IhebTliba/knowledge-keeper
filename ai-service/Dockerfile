FROM python:3.12-slim

# uv, copié depuis son image officielle (version fixée)
COPY --from=ghcr.io/astral-sh/uv:0.5.11 /uv /uvx /bin/

WORKDIR /app
ENV UV_COMPILE_BYTECODE=1 \
    UV_LINK_MODE=copy

# 1. Les dépendances d'abord : cette couche reste en cache tant que pyproject.toml et uv.lock ne changent pas.
COPY pyproject.toml uv.lock ./
RUN uv sync --frozen --no-dev --no-install-project

# 2. Puis le code.
COPY src ./src
RUN uv sync --frozen --no-dev

# Ne jamais faire tourner un service en root.
RUN useradd --create-home appuser
USER appuser

ENV PATH="/app/.venv/bin:$PATH"
EXPOSE 8000
CMD ["uvicorn", "knowledge_keeper_ai.main:create_app", "--factory", "--host", "0.0.0.0", "--port", "8000"]
