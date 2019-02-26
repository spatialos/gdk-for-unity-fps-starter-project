#!/usr/bin/env bash
set -e -u -x -o pipefail

cd "$(dirname "$0")/../"

SHARED_CI_DIR="$(pwd)/.shared-ci"
CLONE_URL="git@github.com:spatialos/gdk-for-unity-shared-ci.git"

# Clone the HEAD of the shared CI repo into ".shared-ci"

if [ -d "${SHARED_CI_DIR}" ]; then
    rm -rf "${SHARED_CI_DIR}"
fi

git clone "${CLONE_URL}" "${SHARED_CI_DIR}"


# Clone the GDK for Unity repository

CLONE_URI="git@github.com:spatialos/gdk-for-unity.git"
TARGET_DIRECTORY="$(realpath $(pwd)/../gdk-for-unity)"
PINNED_VERSION=$(cat ./gdk.pinned)

rm -rf "${TARGET_DIRECTORY}"

git clone ${CLONE_URI} "${TARGET_DIRECTORY}"
pushd "${TARGET_DIRECTORY}"
    git checkout "${PINNED_VERSION}"
popd
