#!/usr/bin/env bash

set -e -u -x -o pipefail

cd "$(dirname "$0")/../"

source ".shared-ci/scripts/profiling.sh"
source ".shared-ci/scripts/pinned-tools.sh"

# Download the artifacts and reconstruct the build/assemblies folder.
buildkite-agent artifact download "build\assembly\**\*" .

uploadAssembly "${ASSEMBLY_PREFIX}" "${PROJECT_NAME}"

markStartOfBlock "Launching deployments"

dotnet run -p ../gdk-for-unity/workers/unity/Packages/com.improbable.gdk.deploymentlauncher/.DeploymentLauncher/DeploymentLauncher.csproj -- \
    create \
    --project_name "${PROJECT_NAME}" \
    --assembly_name "${ASSEMBLY_NAME}" \
    --deployment_name "${ASSEMBLY_NAME}" \
    --launch_json_path cloud_launch_large.json \
    --snapshot_path snapshots/cloud.snapshot \
    --region EU \
    --tags "dev_login"

CONSOLE_URL="https://console.improbable.io/projects/${PROJECT_NAME}/deployments/${ASSEMBLY_NAME}/overview"

buildkite-agent annotate --style "success" "Deployment URL: ${CONSOLE_URL}<br/>"

dotnet run -p ../gdk-for-unity/workers/unity/Packages/com.improbable.gdk.deploymentlauncher/.DeploymentLauncher/DeploymentLauncher.csproj -- \
    create-sim \
    --project_name "${PROJECT_NAME}" \
    --assembly_name "${ASSEMBLY_NAME}" \
    --deployment_name "${ASSEMBLY_NAME}_sim_players" \
    --launch_json_path cloud_launch_large_sim_players.json \
    --region EU \
    --target_deployment "${ASSEMBLY_NAME}" \
    --flag_prefix fps \
    --simulated_coordinator_worker_type SimulatedPlayerCoordinator

CONSOLE_URL_SIM_PLAYERS="https://console.improbable.io/projects/${PROJECT_NAME}/deployments/${ASSEMBLY_NAME}_sim_players/overview"

buildkite-agent annotate --style "success" "Simulated Player Deployment URL: ${CONSOLE_URL_SIM_PLAYERS}" --append

markEndOfBlock "Launching deployments"
