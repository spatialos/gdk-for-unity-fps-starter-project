#!/usr/bin/env bash
set -e -u -x -o pipefail

cd "$(dirname "$0")/../"

# Get shared CI and prepare Unity
ci/bootstrap.sh
.shared-ci/scripts/prepare-unity.sh
.shared-ci/scripts/prepare-unity-mobile.sh "$(pwd)/logs/PrepareUnityMobile.log"

source ".shared-ci/scripts/pinned-tools.sh"

.shared-ci/scripts/build.sh "workers/unity" UnityClient cloud mono "$(pwd)/logs/UnityClientBuild-mono.log"
.shared-ci/scripts/build.sh "workers/unity" UnityGameLogic cloud mono "$(pwd)/logs/UnityGameLogicBuild-mono.log"
.shared-ci/scripts/build.sh "workers/unity" SimulatedPlayerCoordinator cloud mono "$(pwd)/logs/SimulatedPlayerCoordinatorSimulatedPlayerCoordinatorBuild-mono.log"
.shared-ci/scripts/build.sh "workers/unity" MobileClient local mono "$(pwd)/logs/MobileClientBuild-mono.log"

