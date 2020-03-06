#!/usr/bin/env bash

set -e -u -o pipefail

if [[ -n "${DEBUG-}" ]]; then
  set -x
fi

cd "$(dirname "$0")/../../"

SHA=$(git rev-parse HEAD)
ASSEMBLY_NAME="$(buildkite-agent meta-data get "assembly_name")"
RUNTIME_VERSION="$(buildkite-agent meta-data get "runtime_version")"

spatial project history upload "${ASSEMBLY_NAME}" "snapshots/cloud.snapshot" --project_name unity_gdk --tags regisseur

function addTriggerStep() {
    NFR_FILE="${1}"
    NFR_NAME=$(basename --suffix=".yaml" "${NFR_FILE}")

    echo "steps:"
    echo "  - trigger: product-research-benchmarks-run"
    echo "    label: \"nfr-${NFR_NAME}\""
    echo "    build:"
    echo "      branch: feature/nfr-framework"
    echo "      env:"
    echo "        DOCKER_IMAGE_REF: ${DOCKER_IMAGE_REF:-f9a16b1df52b66fccedabaa57ebc2e52e1679861}"
    echo "        SPATIAL_SECRET: unity-gdk-toolbelt"
    echo "      meta_data:"
    echo "        publish-environment: NONE"
    echo "        assembly: ${ASSEMBLY_NAME}"
    echo "        runtime-version: ${RUNTIME_VERSION}"
    echo "        metadata: \"fps-commit=${SHA}\""
    echo "        source-snapshot-history: ${SNAPSHOT_HISTORY}"
    cat "${NFR_FILE}" | sed 's/^/        /'
    echo ""
}

for file in ./ci/nfrs/*.yaml; do
    addTriggerStep "${file}" | buildkite-agent pipeline upload --no-interpolation
done
