from fastapi import APIRouter

router = APIRouter(prefix="/health", tags=["health"])


@router.get("/live")
def live() -> dict[str, str]:
    """Le processus répond. Aucune dépendance vérifiée."""
    return {"status": "Healthy"}


@router.get("/ready")
def ready() -> dict[str, str]:
    """Le service peut traiter des requêtes. Vérifiera le fournisseur LLM à l'étape 7."""
    return {"status": "Healthy"}
