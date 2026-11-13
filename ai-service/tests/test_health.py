from fastapi.testclient import TestClient


def test_live_repond_healthy(client: TestClient) -> None:
    response = client.get("/health/live")

    assert response.status_code == 200
    assert response.json() == {"status": "Healthy"}


def test_ready_repond_healthy(client: TestClient) -> None:
    response = client.get("/health/ready")

    assert response.status_code == 200
    assert response.json() == {"status": "Healthy"}


def test_les_health_checks_ne_demandent_pas_de_cle(client: TestClient) -> None:
    # Docker et l'orchestrateur doivent pouvoir les appeler sans secret.
    assert client.get("/health/live").status_code == 200
