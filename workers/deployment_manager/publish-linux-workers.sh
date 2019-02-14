#!/usr/bin/env bash
set -e -u -x -o pipefail

cd "$(dirname "$0")"

dotnet build -c Release -p:Platform=x64 CodeGen/CodeGen.csproj
dotnet build -c Release -p:Platform=x64 GeneratedCode/GeneratedCode.csproj
dotnet build -c Release -p:Platform=x64 SnapshotGenerator/SnapshotGenerator.csproj
dotnet publish DeploymentManager/DeploymentManager.csproj -r linux-x64 -c Release -p:Platform=x64 --self-contained


pushd "DeploymentManager/bin/x64/Release/netcoreapp2.1/linux-x64/publish"
  jar -cMf DeploymentManager@Linux.zip *
popd

mkdir -p ../../build/assembly/worker/
mv DeploymentManager/bin/x64/Release/netcoreapp2.1/linux-x64/publish/DeploymentManager@Linux.zip ../../build/assembly/worker/DeploymentManager@Linux.zip
