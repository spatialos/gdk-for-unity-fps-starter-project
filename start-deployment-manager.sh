#!/usr/bin/env bash
set -e -u -x -o pipefail

cd "$(dirname "$0")"

cp $2 workers/deployment_manager/DeploymentManager/default_launch.json

pushd "workers/deployment_manager"
  sh publish-linux-workers.sh
popd

spatial cloud upload deployment_manager_"$1" --force
spatial cloud launch deployment_manager_"$1" deployment_manager_launch.json deployment_manager_"$1" --snapshot=snapshots/empty.snapshot