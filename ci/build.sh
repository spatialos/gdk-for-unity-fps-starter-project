#!/usr/bin/env bash
set -e -u -x -o pipefail

cd "$(dirname "$0")/../"

# Get shared CI and prepare Unity
ci/bootstrap.sh
.shared-ci/scripts/prepare-unity.sh
.shared-ci/scripts/prepare-unity-mobile.sh "$(pwd)/logs/PrepareUnityMobile.log"

source ".shared-ci/scripts/pinned-tools.sh"

.shared-ci/scripts/build.sh "workers/unity" UnityClient local mono "$(pwd)/logs/UnityClientBuild-mono.log"
.shared-ci/scripts/build.sh "workers/unity" UnityGameLogic cloud mono "$(pwd)/logs/UnityGameLogicBuild-mono.log"
.shared-ci/scripts/build.sh "workers/unity" SimulatedPlayerCoordinator cloud mono "$(pwd)/logs/SimulatedPlayerCoordinatorSimulatedPlayerCoordinatorBuild-mono.log"
.shared-ci/scripts/build.sh "workers/unity" AndroidClient local mono "$(pwd)/logs/AndroidClientBuild-mono.log"

if isMacOS; then
  .shared-ci/scripts/build.sh "workers/unity" iOSClient local il2cpp "$(pwd)/logs/iOSClientBuild.log"
fi
