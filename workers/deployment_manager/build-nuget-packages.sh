#!/usr/bin/env bash
set -e -u -x -o pipefail

cd "$(dirname "$0")"

rm -rf ~/.nuget/packages/improbable.coresdk ~/.nuget/packages/improbable.coresdk.tools

mkdir -p nugetpackages
dotnet pack Improbable.CoreSdk.Tools/Improbable.CoreSdk.Tools.csproj -o "$(pwd)/nugetpackages"
dotnet pack Improbable.CoreSdk/Improbable.CoreSdk.csproj -o "$(pwd)/nugetpackages"

dotnet restore
