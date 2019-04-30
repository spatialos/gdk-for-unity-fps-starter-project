#!/usr/bin/env bash
set -e -u -x -o pipefail

cd "$(dirname "$0")/../"

SHARED_CI_DIR="$(pwd)/.shared-ci"
CLONE_URL="git@github.com:spatialos/gdk-for-unity-shared-ci.git"

# Clone the HEAD of the shared CI repo into ".shared-ci"

if [[ -d "${SHARED_CI_DIR}" ]]; then
    rm -rf "${SHARED_CI_DIR}"
fi

git clone --verbose --depth 1 "${CLONE_URL}" "${SHARED_CI_DIR}"

# Clone the GDK for Unity repository

CLONE_URI="git@github.com:spatialos/gdk-for-unity.git"
TARGET_DIRECTORY="$(realpath $(pwd)/../gdk-for-unity)"
PINNED_VERSION=$(cat ./gdk.pinned)

rm -rf "${TARGET_DIRECTORY}"

mkdir "${TARGET_DIRECTORY}"

# Workaround for being unable to clone a specific commit with depth of 1.
pushd "${TARGET_DIRECTORY}"
    git init
    git remote add origin "${CLONE_URI}"
    git fetch --depth 20 origin develop
    git checkout "${PINNED_VERSION}"
popd
