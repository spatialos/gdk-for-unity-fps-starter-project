#!/usr/bin/env bash

set -e -u -o pipefail

if [[ -n "${DEBUG-}" ]]; then
  set -x
fi

cd "$(dirname "$0")/../"

source .shared-ci/scripts/pinned-tools.sh

PROJECT_DIR="$(pwd)"
mkdir -p "${PROJECT_DIR}/logs/"

EDITMODE_TEST_RESULTS_FILE="${PROJECT_DIR}/logs/editmode-test-results.xml"

echo "--- Testing Unity: Editmode :writing_hand:"

pushd "workers/unity"
    dotnet run -p "${PROJECT_DIR}/.shared-ci/tools/RunUnity/RunUnity.csproj" -- \
        -batchmode \
        -projectPath "${PROJECT_DIR}/workers/unity" \
        -runEditorTests \
        -logfile "${PROJECT_DIR}/logs/unity-editmode-test-run.log" \
        -editorTestsResultFile "${EDITMODE_TEST_RESULTS_FILE}" \
        -editorTestsFilter Fps
popd
