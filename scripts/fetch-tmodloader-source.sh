#!/usr/bin/env bash
set -euo pipefail

ROOT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
cd "$ROOT_DIR"

mkdir -p extern
if [ ! -d extern/tModLoader/.git ]; then
  git clone --depth 1 https://github.com/tModLoader/tModLoader.git extern/tModLoader
else
  git -C extern/tModLoader pull --ff-only
fi

echo "tModLoader source is in extern/tModLoader"
