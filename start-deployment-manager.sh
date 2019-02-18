#!/usr/bin/env bash
set -e -u -x -o pipefail

cd "$(dirname "$0")"

if [ "$#" -ne 1 ]; then
    echo "Wrong number of arguments. Usage: <Assembly postfix> <deployment manager project directory>"
fi

ASSEMBLY_POSTFIX=$1
LAUNCH_CONFIGURATION=$2

cp ${LAUNCH_CONFIGURATION} workers/deployment_manager/DeploymentManager/default_launch.json

pushd "workers/deployment_manager"
  sh publish-linux-workers.sh
popd

spatial build build-config
spatial cloud upload "deployment_manager_${ASSEMBLY_POSTFIX}" --force
spatial cloud launch "deployment_manager_${ASSEMBLY_POSTFIX}" deployment_manager_launch.json "deployment_manager_${ASSEMBLY_POSTFIX}" --snapshot=snapshots/empty.snapshot --log_level=debug
