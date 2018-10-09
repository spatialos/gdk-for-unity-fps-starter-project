#!/usr/bin/env bash
# -ET: propagate DEBUG/RETURN/ERR traps to functions and subshells
# -e: exit on unhandled error
# pipefail: any failure in a pipe causes the pipe to fail
set -eET -o pipefail
[[ -n "${DEBUG:-}" ]] && set -x

LOG_SUCCESS='\033[0;32m'
LOG_WARNING='\033[1;33m'
LOG_EMPHASIS='\033[0;36m'
NO_COLOR='\033[0m'

echo_with_color()
{
    echo -e "${2}${1}${NO_COLOR}"
}

echo ""
echo "Welcome to the SpatialOS GDK for Unity FPS Starter Project setup script." 
echo ""

PS3="Please make a selection: "
echo "Select a cloning method."
echo ""
options=("HTTPS" "SSH" "Quit")
select opt in "${options[@]}"
do
    case $opt in
        "HTTPS")
            CLONE_URI="https://github.com/spatialos/gdk-for-unity.git"
            break
            ;;
        "SSH")
            CLONE_URI="git@github.com:spatialos/gdk-for-unity.git"
            break
            ;;
        "Quit")
            echo "Stopping the setup process."
            exit 0
            ;;
        *) echo "Invalid option $REPLY";;
    esac
done

readonly RAW_DIR="$(dirname "$0")/../../../gdk-for-unity"
readonly TARGET_DIRECTORY="$(realpath "${RAW_DIR}")"

echo_with_color "This script will create the following directory: ${TARGET_DIRECTORY}" $LOG_WARNING
echo_with_color "If such a directory already exists it will be deleted." $LOG_EMPHASIS
echo_with_color "Are you ready to proceed? (y/n)" $LOG_WARNING

read -r -p "" response
if [[ "$response" =~ ^([yY][eE][sS]|[yY])+$ ]]
then
    echo "" > /dev/null
else
    echo "Stopping the setup process."
    exit 0
fi

if [[ -d "${TARGET_DIRECTORY}" ]]; then
    echo_with_color "Deleting existing directory at ${TARGET_DIRECTORY}..." $LOG_WARNING
    rm -rf "${TARGET_DIRECTORY}"
fi

echo "Cloning SpatialOS GDK for Unity"
git clone "https://github.com/spatialos/gdk-for-unity.git" "${TARGET_DIRECTORY}"

PINNED_VERSION=$(cat "$(dirname "$0")/../../gdk.pinned")

pushd "${TARGET_DIRECTORY}"
    git checkout "${PINNED_VERSION}"
popd

echo_with_color "Finished!" $LOG_SUCCESS
