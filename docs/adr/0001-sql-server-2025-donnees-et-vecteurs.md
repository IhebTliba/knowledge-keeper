# ADR 0001 — SQL Server 2025 pour les données métier et les vecteurs

- **Statut** : Accepté
- **Date** : 2026-11-12

## Contexte
Knowledge Keeper stocke des données métier (employés, postes, entretiens, fiches de savoir)
et, pour le RAG, les embeddings des extraits d'entretien. Les droits d'accès dépendent du poste
et du service : le copilote ne doit chercher que dans le savoir autorisé pour l'utilisateur.

## Décision
Utiliser **SQL Server 2025** pour les deux : tables métier et colonnes `VECTOR`, avec
`VECTOR_DISTANCE` pour la recherche par similarité.
En local, SQL Server tourne dans Docker (édition Developer), via `infra/docker-compose.yml`.

## Conséquences
- **Une seule base** : les filtres de droits d'accès (poste, service) et la recherche vectorielle
  se font dans la même requête SQL, avec des jointures classiques.
- Pas de synchronisation entre deux systèmes, une seule sauvegarde, une seule technologie à exploiter.
- Cohérent avec un environnement d'entreprise .NET qui utilise déjà SQL Server.
- Pour le MVP (quelques milliers d'extraits), une recherche exacte avec `VECTOR_DISTANCE` suffit.
  L'index approximatif (DiskANN, `VECTOR_SEARCH`) est à évaluer si le volume grossit ; vérifier
  alors son statut (preview ou GA) dans la version utilisée.
- Dépendance à SQL Server 2025 ou Azure SQL : une version plus ancienne ne supporte pas `VECTOR`.

## Alternatives envisagées
- **Chroma ou FAISS** : simples pour prototyper, mais une deuxième source de données à synchroniser,
  et des filtres de droits d'accès à dupliquer hors de SQL.
- **pgvector (PostgreSQL)** : très solide, mais ajoute une deuxième base dans une stack SQL Server.
- **Pinecone** : service managé payant, et des données RH sensibles qui sortiraient de l'entreprise.
