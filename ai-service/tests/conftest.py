import pytest
from fastapi.testclient import TestClient

from knowledge_keeper_ai.config import get_settings
from knowledge_keeper_ai.main import create_app

TEST_API_KEY = "test-internal-api-key-with-at-least-32-chars"


@pytest.fixture
def api_key() -> str:
    return TEST_API_KEY


@pytest.fixture
def client(monkeypatch: pytest.MonkeyPatch, api_key: str) -> TestClient:
    monkeypatch.setenv("KK_AI_INTERNAL_API_KEY", api_key)
    get_settings.cache_clear()  # repart d'une configuration propre pour chaque test
    return TestClient(create_app())
