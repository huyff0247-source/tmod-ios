#!/usr/bin/env bash
set -euo pipefail

ROOT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
cd "$ROOT_DIR"

if ! command -v dotnet >/dev/null 2>&1; then
  echo "dotnet SDK not found"
  exit 1
fi

dotnet workload install ios

dotnet restore src/TMLiOS.App/TMLiOS.App.csproj

dotnet build src/TMLiOS.App/TMLiOS.App.csproj \
  -c Release \
  -f net8.0-ios \
  -r ios-arm64 \
  /p:BuildIpa=false \
  /p:PublishTrimmed=true \
  /p:MtouchLink=None \
  /p:UseInterpreter=true \
  /p:MtouchInterpreter=-all
