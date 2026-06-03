#!/bin/bash
set -euo pipefail

# Only run in remote (web) environments
if [ "${CLAUDE_CODE_REMOTE:-}" != "true" ]; then
  exit 0
fi

REPO_DIR="${CLAUDE_PROJECT_DIR:-$(cd "$(dirname "$0")/../.." && pwd)}"

# Install .NET 10 SDK if not present
if ! command -v dotnet &>/dev/null; then
  echo "==> Instalando .NET 10 SDK via apt..."
  apt-get update -qq 2>/dev/null || true
  DEBIAN_FRONTEND=noninteractive apt-get install -y dotnet-sdk-10.0 2>/dev/null
fi

echo "==> Restaurando pacotes .NET..."
cd "$REPO_DIR"
dotnet restore AMR.Core.slnx

echo "==> Instalando dependências do frontend (npm)..."
cd "$REPO_DIR/frontend"
npm install

echo "==> Dependências instaladas com sucesso."
