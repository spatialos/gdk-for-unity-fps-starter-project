#!/usr/bin/env bash
set -e -u -x -o pipefail

cd "$(dirname "$0")/../"

ci/bootstrap.sh
.shared-ci/scripts/prepare-unity.sh

source .shared-ci/scripts/pinned-tools.sh
source .shared-ci/scripts/profiling.sh

UNITY_PROJECT_DIR="$(pwd)/workers/unity"

markStartOfBlock "$0"

markStartOfBlock "Building UnityClient"

# TODO - run unity stuff
pushd "${UNITY_PROJECT_DIR}"
    dotnet run -p ../../.shared-ci/tools/RunUnity/RunUnity.csproj -- \
        -projectPath "${UNITY_PROJECT_DIR}" \
        -batchmode \
        -quit \
        -logfile "$(pwd)/../../logs/UnityClientBuild.log" \
        -executeMethod "Improbable.Gdk.BuildSystem.WorkerBuilder.Build" \
        +buildWorkerTypes "UnityClient" \
        +buildTarget "cloud"
popd

markEndOfBlock "Building UnityClient"

markStartOfBlock "Building UnityGameLogic"

# TODO - run unity stuff
pushd "${UNITY_PROJECT_DIR}"
    dotnet run -p ../../.shared-ci/tools/RunUnity/RunUnity.csproj -- \
        -projectPath "${UNITY_PROJECT_DIR}" \
        -batchmode \
        -quit \
        -logfile "$(pwd)/../../logs/UnityGameLogicBuild.log" \
        -executeMethod "Improbable.Gdk.BuildSystem.WorkerBuilder.Build" \
        +buildWorkerTypes "UnityGameLogic" \
        +buildTarget "cloud"
popd

markEndOfBlock "Building UnityGameLogic"

markStartOfBlock "Building SimulatedPlayerCoordinator"

# TODO - run unity stuff
pushd "${UNITY_PROJECT_DIR}"
    dotnet run -p ../../.shared-ci/tools/RunUnity/RunUnity.csproj -- \
        -projectPath "${UNITY_PROJECT_DIR}" \
        -batchmode \
        -quit \
        -logfile "$(pwd)/../../logs/UnityGameLogicBuild.log" \
        -executeMethod "Improbable.Gdk.BuildSystem.WorkerBuilder.Build" \
        +buildWorkerTypes "SimulatedPlayerCoordinator" \
        +buildTarget "cloud"
popd

markEndOfBlock "Building SimulatedPlayerCoordinator"

markEndOfBlock "$0"