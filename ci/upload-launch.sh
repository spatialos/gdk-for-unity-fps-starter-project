#!/usr/bin/env bash

set -e -u -x -o pipefail

cd "$(dirname "$0")/../"

PREFIX="fps"
PROJECT_NAME="unity_gdk"

source ".shared-ci/scripts/profiling.sh"
source ".shared-ci/scripts/pinned-tools.sh"

# Download the artifacts and reconstruct the build/assemblies folder.
buildkite-agent artifact download "build\assembly\**\*" .

setAssemblyName "${PREFIX}"

spatial cloud upload "${ASSEMBLY_NAME}" --log_level=debug --force --enable_pre_upload_check=false --project_name="${PROJECT_NAME}"

markStartOfBlock "Launching deployments"

dotnet run -p workers/unity/Packages/com.improbable.gdk.deploymentlauncher/.DeploymentLauncher/DeploymentLauncher.csproj -- \
    create "${PROJECT_NAME}" "${ASSEMBLY_NAME}" \
    "${ASSEMBLY_NAME}" cloud_launch_large.json snapshots/cloud.snapshot \
    "${ASSEMBLY_NAME}_sim_players" cloud_launch_large_sim_players.json

if [[ -n "${BUILDKITE-}" ]]; then
  CONSOLE_URL="https://console.improbable.io/projects/${PROJECT_NAME}/deployments/${ASSEMBLY_NAME}"
  buildkite-agent annotate --style "success" "Deployment URL: ${CONSOLE_URL}"
fi

markEndOfBlock "Launching deployments"
