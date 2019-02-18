#!/usr/bin/env bash
set -e -u -x -o pipefail

cd "$(dirname "$0")" 

dotnet build -c Release -p:Platform=x64 ServiceAccountGenerator/ServiceAccountGenerator.csproj
dotnet run --project ServiceAccountGenerator/ $(pwd)/DeploymentManager "unity_gdk"
