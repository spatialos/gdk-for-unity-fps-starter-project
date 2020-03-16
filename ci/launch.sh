#!/usr/bin/env bash

set -e -u -o pipefail

if [[ -n "${DEBUG-}" ]]; then
  set -x
fi

cd "$(dirname "$0")/../"

source ".shared-ci/scripts/pinned-tools.sh"

# Download the artifacts and reconstruct the build/assemblies folder.
echo "--- Downloading assembly :inbox_tray:"
buildkite-agent artifact download "build\assembly\**\*" .

uploadAssembly "${ASSEMBLY_PREFIX}" "${PROJECT_NAME}"

# If the RUNTIME_VERSION env variable is already set, i.e. - through the Buildkite menu
# then skip reading the file.
if [[ -n $"{RUNTIME_VERSION:-}" ]]; then
  export RUNTIME_VERSION="$(cat ../gdk-for-unity/workers/unity/Packages/io.improbable.gdk.tools/runtime.pinned)"
fi

echo "--- Launching main deployment :airplane_departure:"

dotnet run -p workers/unity/Packages/io.improbable.gdk.deploymentlauncher/.DeploymentLauncher/DeploymentLauncher.csproj -- \
    create \
    --project_name "${PROJECT_NAME}" \
    --assembly_name "${ASSEMBLY_NAME}" \
    --deployment_name "${ASSEMBLY_NAME}" \
    --launch_json_path cloud_launch_large.json \
    --snapshot_path snapshots/cloud.snapshot \
    --region EU \
    --tags "dev_login" \
    --runtime_version="${RUNTIME_VERSION}"

CONSOLE_URL="https://console.improbable.io/projects/${PROJECT_NAME}/deployments/${ASSEMBLY_NAME}/overview"

buildkite-agent annotate --style "success" "Deployment URL: ${CONSOLE_URL}<br/>"

echo "--- Launching sim player deployment :robot_face:"

dotnet run -p workers/unity/Packages/io.improbable.gdk.deploymentlauncher/.DeploymentLauncher/DeploymentLauncher.csproj -- \
    create-sim \
    --project_name "${PROJECT_NAME}" \
    --assembly_name "${ASSEMBLY_NAME}" \
    --deployment_name "${ASSEMBLY_NAME}_sim_players" \
    --launch_json_path cloud_launch_large_sim_players.json \
    --region EU \
    --runtime_version="${RUNTIME_VERSION}" \
    --target_deployment "${ASSEMBLY_NAME}" \
    --flag_prefix fps \
    --simulated_coordinator_worker_type SimulatedPlayerCoordinator

CONSOLE_URL_SIM_PLAYERS="https://console.improbable.io/projects/${PROJECT_NAME}/deployments/${ASSEMBLY_NAME}_sim_players/overview"

buildkite-agent annotate --style "success" "Simulated Player Deployment URL: ${CONSOLE_URL_SIM_PLAYERS}" --append
