#!/usr/bin/env bash
# Regenerates docs/wiki/API-Reference.md from the compiled library.
#
# Publishing first is necessary rather than incidental: a plain library build leaves its
# dependencies in the NuGet cache, and the generator has to resolve Microsoft.Extensions.*
# to decode signatures that mention IServiceCollection or IHttpClientBuilder.
#
# Run from the repository root:
#   ./tools/gen-api-reference.sh
set -euo pipefail

ROOT="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
STAGE="$(mktemp -d)"
trap 'rm -rf "$STAGE"' EXIT

cd "$ROOT"

echo "Publishing (net10.0) to $STAGE ..."
dotnet publish src/EmmaSharper/EmmaSharper.csproj -f net10.0 -c Release -o "$STAGE" --nologo

echo "Generating docs/wiki/API-Reference.md ..."
dotnet tools/gen-api-reference.cs -- \
    "$STAGE/EmmaSharper.dll" \
    "$STAGE/EmmaSharper.xml" \
    docs/wiki/API-Reference.md

echo
echo "Done. Review the diff, then mirror the wiki (see docs/wiki/README.md)."
