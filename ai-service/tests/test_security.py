import pytest
from fastapi.testclient import TestClient
from pydantic import ValidationError

from knowledge_keeper_ai.config import Settings


def test_info_sans_cle_renvoie_401(client: TestClient) -> None:
    assert client.get("/v1/info").status_code == 401


def test_info_avec_une_mauvaise_cle_renvoie_401(client: TestClient) -> None:
    response = client.get("/v1/info", headers={"X-Internal-Api-Key": "mauvaise-cle"})

    assert response.status_code == 401


def test_info_avec_la_bonne_cle_renvoie_le_service(client: TestClient, api_key: str) -> None:
    response = client.get("/v1/info", headers={"X-Internal-Api-Key": api_key})

    assert response.status_code == 200
    assert response.json() == {"service": "knowledge-keeper-ai", "version": "0.1.0"}


def test_une_cle_trop_courte_est_refusee_au_demarrage(monkeypatch: pytest.MonkeyPatch) -> None:
    monkeypatch.setenv("KK_AI_INTERNAL_API_KEY", "trop-courte")

    with pytest.raises(ValidationError):
        Settings()
