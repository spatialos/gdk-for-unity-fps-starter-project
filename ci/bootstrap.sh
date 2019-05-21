#!/usr/bin/env bash
set -e -u -x -o pipefail

cd "$(dirname "$0")/../"

SHARED_CI_DIR="$(pwd)/.shared-ci"
CLONE_URL="git@github.com:spatialos/gdk-for-unity-shared-ci.git"
PINNED_SHARED_CI_VERSION=$(cat ./ci/shared-ci.pinned)

# Clone the HEAD of the shared CI repo into ".shared-ci"

rm -rf "${SHARED_CI_DIR}"

mkdir "${SHARED_CI_DIR}"

# Workaround for being unable to clone a specific commit with depth of 1.
pushd "${SHARED_CI_DIR}"
    git init
    git remote add origin "${CLONE_URL}"
    git fetch --depth 20 origin master
    git checkout "${PINNED_SHARED_CI_VERSION}"
popd

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
