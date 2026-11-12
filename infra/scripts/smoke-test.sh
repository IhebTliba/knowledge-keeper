#!/usr/bin/env bash
# Vérifie que SQL Server répond et qu'il supporte le type VECTOR.
set -euo pipefail

/opt/mssql-tools18/bin/sqlcmd \
  -S localhost -U sa -P "$MSSQL_SA_PASSWORD" \
  -C -b -i /scripts/smoke-test.sql

echo "Smoke test OK"
