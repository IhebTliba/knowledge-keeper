from typing import Annotated

from fastapi import APIRouter, Depends
from pydantic import BaseModel

from knowledge_keeper_ai.config import Settings, get_settings
from knowledge_keeper_ai.security import require_internal_api_key

router = APIRouter(
    prefix="/v1",
    tags=["info"],
    dependencies=[Depends(require_internal_api_key)],
)


class ServiceInfo(BaseModel):
    service: str
    version: str


@router.get("/info")
def info(settings: Annotated[Settings, Depends(get_settings)]) -> ServiceInfo:
    """Endpoint protégé : l'API .NET l'utilise pour vérifier qu'elle joint le service."""
    return ServiceInfo(service=settings.service_name, version=settings.version)
