#!/usr/bin/env bash
# -ET: propagate DEBUG/RETURN/ERR traps to functions and subshells
# -e: exit on unhandled error
# pipefail: any failure in a pipe causes the pipe to fail
set -eET -o pipefail
[[ -n "${DEBUG:-}" ]] && set -x


# ====================== Utilities ====================== #

LOG_SUCCESS='\033[0;32m'
LOG_WARNING='\033[1;33m'
LOG_EMPHASIS='\033[0;36m'
NO_COLOR='\033[0m'

function echo_with_color()
{
    echo -e "${2}${1}${NO_COLOR}"
}

function isLinux() {
  [[ "$(uname -s)" == "Linux" ]];
}

function isMacOS() {
  [[ "$(uname -s)" == "Darwin" ]];
}

function isWindows() {
  ! ( isLinux || isMacOS );
}

# ======================================================= #

cd "$(dirname "$0")"

if isWindows; then
  echo "Cannot run symlink_packages.sh on Windows. Invoking the powershell version..."
  powershell "dev_init.ps1"
  exit 0
fi

readonly RAW_DIR="$(dirname "${0}")/../gdk-for-unity"

# Workaround for lack of realpath by default on MacOS
#   1. Create the directory is it doesn't exist
#   2. pushd there and record the directory through pwd
#   3. This gives you a nice resolved path.
mkdir -p "${RAW_DIR}"
pushd "${RAW_DIR}" > /dev/null
    readonly TARGET_DIRECTORY=$(pwd)
popd > /dev/null

echo_with_color "This script will create the following directory: ${TARGET_DIRECTORY}" "${LOG_WARNING}"
echo_with_color "*** If such a directory already exists it will be deleted. ***" "${LOG_EMPHASIS}"
echo_with_color "Are you ready to proceed? (y/n)" "${LOG_WARNING}"

read -r -p "" response
if [[ "${response}" =~ ^([yY][eE][sS]|[yY])+$ ]]
then
    echo "" > /dev/null
else
    echo "Stopping the setup process."
    exit 0
fi

HTTPS_URI="https://github.com/spatialos/gdk-for-unity.git"
SSH_URI="git@github.com:spatialos/gdk-for-unity.git"

PS3="Please make a selection: "
echo "Select a cloning method."
echo ""
HTTPS_OPTION="HTTPS (${HTTPS_URI})"
SSH_OPTION="SSH   (${SSH_URI})"
options=("${HTTPS_OPTION}" "${SSH_OPTION}" "Quit")
select opt in "${options[@]}"
do
    case ${opt} in
        "${HTTPS_OPTION}")
            CLONE_URI=${HTTPS_URI}
            break
            ;;
        "${SSH_OPTION}")
            CLONE_URI=${SSH_URI}
            break
            ;;
        "Quit")
            echo "Stopping the setup process."
            exit 0
            ;;
        *) echo "Invalid option ${REPLY}";;
    esac
done

if [[ -d "${TARGET_DIRECTORY}" ]]; then
    echo_with_color "Deleting existing directory at ${TARGET_DIRECTORY}..." "${LOG_WARNING}"
    rm -rf "${TARGET_DIRECTORY}"
fi

echo "Cloning SpatialOS GDK for Unity"
git clone ${CLONE_URI} "${TARGET_DIRECTORY}"

PINNED_VERSION=$(cat "$(dirname "${0}")/../../gdk.pinned")

pushd "${TARGET_DIRECTORY}"
    git checkout "${PINNED_VERSION}"
popd

echo_with_color "Finished cloning the GDK for Unity." "${LOG_SUCCESS}"

echo "Downloading SDK for GDK for Unity.."

"${TARGET_DIRECTORY}/init.sh"

echo_with_color "Finished downloading SDK for the GDK for Unity." "${LOG_SUCCESS}"

PKG_PATH="workers/unity/Packages"
SRC_PATH="${TARGET_DIRECTORY}/workers-unity/Packages"

link_package() {
  local package=$1
  if [ ! -d "$package" ]; then
    ln -s "${SRC_PATH}/$package" "${PKG_PATH}/$package"
  fi
}

link_package "io.improbable.gdk.buildsystem"
link_package "io.improbable.gdk.core"
link_package "io.improbable.gdk.gameobjectcreation"
link_package "io.improbable.gdk.mobile"
link_package "io.improbable.gdk.playerlifecycle"
link_package "io.improbable.gdk.testutils"
link_package "io.improbable.gdk.tools"
link_package "io.improbable.gdk.transformsynchronization"
