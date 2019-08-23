#!/usr/bin/env bash
set -e -u -o pipefail

if [[ -n "${DEBUG-}" ]]; then
  set -x
fi

function isLinux() {
  [[ "$(uname -s)" == "Linux" ]];
}

function isMacOS() {
  [[ "$(uname -s)" == "Darwin" ]];
}

function isWindows() {
  ! ( isLinux || isMacOS );
}

cd "$(dirname "$0")/../"

echo "--- Bootstrapping :boot:"

SHARED_CI_DIR="$(pwd)/.shared-ci"
CLONE_URL="git@github.com:spatialos/gdk-for-unity-shared-ci.git"
PINNED_SHARED_CI_BRANCH=$(cat ./ci/shared-ci.pinned | cut -d' ' -f 1)
PINNED_SHARED_CI_VERSION=$(cat ./ci/shared-ci.pinned | cut -d' ' -f 2)

# Clone the HEAD of the shared CI repo into ".shared-ci"

if [[ -d "${SHARED_CI_DIR}" ]]; then
    rm -rf "${SHARED_CI_DIR}"
fi

mkdir "${SHARED_CI_DIR}"

# Workaround for being unable to clone a specific commit with depth of 1.
pushd "${SHARED_CI_DIR}"
    git init
    git remote add origin "${CLONE_URL}"
    git fetch --depth 20 origin "${PINNED_SHARED_CI_BRANCH}"
    git checkout "${PINNED_SHARED_CI_VERSION}"
popd

# Clone the GDK for Unity repository

CLONE_URI="git@github.com:spatialos/gdk-for-unity.git"
TARGET_DIRECTORY="$(realpath $(pwd)/../gdk-for-unity)"
PINNED_BRANCH=$(cat ./gdk.pinned | cut -d' ' -f 1)
PINNED_VERSION=$(cat ./gdk.pinned | cut -d' ' -f 2)
SKIP_GDK=false

if [[ -z ${BUILDKITE:-} ]]; then
    echo "Warning: About to delete ${TARGET_DIRECTORY}. Please confirm. (Default is Cancel)"
    read -p "Y/N/S > " -r
    echo    # (optional) move to a new line
    if [[ $REPLY =~ ^[Yy]$ ]]; then
        echo "Deleting..."
    elif [[ $REPLY =~ ^[Ss]$ ]]; then
        echo "Skipping..."
        SKIP_GDK=true
    else
        exit 1
    fi
fi

if [ "$SKIP_GDK" = false ] ; then
    rm -rf "${TARGET_DIRECTORY}"

    mkdir "${TARGET_DIRECTORY}"

    # Workaround for being unable to clone a specific commit with depth of 1.
    pushd "${TARGET_DIRECTORY}"
        git init
        git remote add origin "${CLONE_URI}"
        git fetch --depth 20 origin "${PINNED_BRANCH}"
        git checkout "${PINNED_VERSION}"
        echo "--- Hit init :right-facing_fist::red_button:"
        ./init.sh
    popd
fi

echo "--- Symlinking packages :link::package:"

if isWindows; then
    if [[ -z ${BUILDKITE:-} ]]; then
        powershell Start-Process -Verb RunAs -WindowStyle Hidden -Wait -FilePath dotnet.exe -ArgumentList "run", "-p", "$(pwd)/.shared-ci/tools/PackageSymLinker/PackageSymLinker.csproj", "\-\-", "--packages-source-dir", "${TARGET_DIRECTORY}/workers/unity/Packages", "--package-target-dir", "$(pwd)/workers/unity/Packages"
        exit 0
    fi
fi

EXTRA_ARGS=""

if [[ -n ${BUILDKITE:-} ]]; then
    EXTRA_ARGS="--copy"
fi

dotnet run -p ./.shared-ci/tools/PackageSymLinker/PackageSymLinker.csproj -- \
    --packages-source-dir "${TARGET_DIRECTORY}/workers/unity/Packages" \
    --package-target-dir "$(pwd)/workers/unity/Packages" "${EXTRA_ARGS}"
