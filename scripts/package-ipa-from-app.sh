#!/usr/bin/env bash
set -euo pipefail

if [ $# -lt 2 ]; then
  echo "Usage: $0 /path/to/App.app output.ipa"
  exit 1
fi

APP_PATH="$1"
OUT_IPA="$2"
TMP_DIR="$(mktemp -d)"
trap 'rm -rf "$TMP_DIR"' EXIT

mkdir -p "$TMP_DIR/Payload"
cp -R "$APP_PATH" "$TMP_DIR/Payload/"
(cd "$TMP_DIR" && zip -qry "$OUT_IPA" Payload)

echo "Created $OUT_IPA"
