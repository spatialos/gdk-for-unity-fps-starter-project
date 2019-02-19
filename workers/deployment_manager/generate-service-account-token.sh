#!/usr/bin/env bash
set -e -u -x -o pipefail

cd "$(dirname "$0")" 

if [ "$#" -ne 1 ]; then
    echo "Wrong number of arguments. Usage: <SpatialOS project name>"
fi

PROJECT_NAME="$1"

dotnet build -c Release -p:Platform=x64 ServiceAccountGenerator/ServiceAccountGenerator.csproj
dotnet run --project ServiceAccountGenerator/ $(pwd)/DeploymentManager "${PROJECT_NAME}"
