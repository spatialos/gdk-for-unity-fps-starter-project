#!/usr/bin/env bash
set -e -u -x -o pipefail

cd "$(dirname "$0")/../"

ci/bootstrap.sh
.shared-ci/scripts/prepare-unity.sh

source .shared-ci/scripts/pinned-tools.sh
source .shared-ci/scripts/profiling.sh

UNITY_PROJECT_DIR="$(pwd)/workers/unity"

# The asset cache ip cannot be hardcoded and so is stored in an environment variable on the build agent.
# This is bash shorthand syntax for if-else predicated on the existance of the environment variable 
# where the else branch assigns an empty string.
#   i.e. - 
#   if [ -z ${UNITY_ASSET_CACHE_IP} ]; then
#       ASSET_CACHE_ARG="-CacheServerIPAddress ${UNITY_ASSET_CACHE_IP}"
#   else
#       ASSET_CACHE_ARG=""
#   fi

ASSET_CACHE_ARG=${UNITY_ASSET_CACHE_IP:+-CacheServerIPAddress "${UNITY_ASSET_CACHE_IP}"}

markStartOfBlock "$0"

markStartOfBlock "Building UnityClient"

pushd "${UNITY_PROJECT_DIR}"
    dotnet run -p ../../.shared-ci/tools/RunUnity/RunUnity.csproj -- \
        -projectPath "${UNITY_PROJECT_DIR}" \
        -batchmode \
        -quit \
        -logfile "$(pwd)/../../logs/UnityClientBuild.log" \
        -executeMethod "Improbable.Gdk.BuildSystem.WorkerBuilder.Build" \
        ${ASSET_CACHE_ARG} \
        +buildWorkerTypes "UnityClient" \
        +buildTarget "cloud"
popd

markEndOfBlock "Building UnityClient"

markStartOfBlock "Building UnityGameLogic"

pushd "${UNITY_PROJECT_DIR}"
    dotnet run -p ../../.shared-ci/tools/RunUnity/RunUnity.csproj -- \
        -projectPath "${UNITY_PROJECT_DIR}" \
        -batchmode \
        -quit \
        -logfile "$(pwd)/../../logs/UnityGameLogicBuild.log" \
        -executeMethod "Improbable.Gdk.BuildSystem.WorkerBuilder.Build" \
        ${ASSET_CACHE_ARG} \
        +buildWorkerTypes "UnityGameLogic" \
        +buildTarget "cloud"
popd

markEndOfBlock "Building UnityGameLogic"

markStartOfBlock "Building SimulatedPlayerCoordinator"

pushd "${UNITY_PROJECT_DIR}"
    dotnet run -p ../../.shared-ci/tools/RunUnity/RunUnity.csproj -- \
        -projectPath "${UNITY_PROJECT_DIR}" \
        -batchmode \
        -quit \
        -logfile "$(pwd)/../../logs/SimulatedPlayerCoordinatorBuild.log" \
        -executeMethod "Improbable.Gdk.BuildSystem.WorkerBuilder.Build" \
        ${ASSET_CACHE_ARG} \
        +buildWorkerTypes "SimulatedPlayerCoordinator" \
        +buildTarget "cloud"
popd

markEndOfBlock "Building SimulatedPlayerCoordinator"

markEndOfBlock "$0"