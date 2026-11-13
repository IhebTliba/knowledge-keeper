import secrets
from typing import Annotated

from fastapi import Depends, Header, HTTPException, status

from knowledge_keeper_ai.config import Settings, get_settings


def require_internal_api_key(
    settings: Annotated[Settings, Depends(get_settings)],
    x_internal_api_key: Annotated[str | None, Header()] = None,
) -> None:
    """Refuse toute requête qui ne vient pas de l'API .NET (header X-Internal-Api-Key)."""
    expected = settings.internal_api_key.get_secret_value()

    # compare_digest compare en temps constant : protège contre les attaques
    # qui mesurent le temps de réponse pour deviner la clé.
    if x_internal_api_key is None or not secrets.compare_digest(x_internal_api_key, expected):
        raise HTTPException(
            status_code=status.HTTP_401_UNAUTHORIZED,
            detail="Clé d'API interne absente ou invalide.",
        )
