from fastapi import FastAPI

from knowledge_keeper_ai.config import get_settings
from knowledge_keeper_ai.routers import health, info


def create_app() -> FastAPI:
    """Fabrique de l'application. Une configuration invalide fait échouer le démarrage."""
    settings = get_settings()

    app = FastAPI(title="Knowledge Keeper — AI service", version=settings.version)
    app.include_router(health.router)
    app.include_router(info.router)
    return app
