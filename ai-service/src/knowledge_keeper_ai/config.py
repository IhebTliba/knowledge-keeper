from functools import lru_cache

from pydantic import SecretStr, field_validator
from pydantic_settings import BaseSettings, SettingsConfigDict


class Settings(BaseSettings):
    """Configuration lue depuis l'environnement (préfixe KK_AI_) ou le fichier .env."""

    model_config = SettingsConfigDict(env_file=".env", env_prefix="KK_AI_", extra="ignore")

    service_name: str = "knowledge-keeper-ai"
    version: str = "0.1.0"
    internal_api_key: SecretStr

    @field_validator("internal_api_key")
    @classmethod
    def _api_key_must_be_long(cls, value: SecretStr) -> SecretStr:
        if len(value.get_secret_value()) < 32:
            raise ValueError("doit contenir au moins 32 caractères")
        return value


@lru_cache
def get_settings() -> Settings:
    return Settings()
