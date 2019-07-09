#!/usr/bin/env bash

set -e -u -x -o pipefail

cd "$(dirname "$0")/../../"

function isLinux() {
  [[ "$(uname -s)" == "Linux" ]];
}

function isMacOS() {
  [[ "$(uname -s)" == "Darwin" ]];
}

function isWindows() {
  ! ( isLinux || isMacOS );
}

if isWindows; then
  echo "Cannot run symlink_packages.sh on Windows. Invoking the powershell version..."
  powershell "scripts/powershell/symlink_packages.ps1"
  exit 0
fi

# TODO: Realpath is a nono
GDK_PATH="$(realpath $(pwd)/../gdk-for-unity/workers/unity/Packages)"
PKG_PATH="$(pwd)/workers/unity/Packages"

link_package() {
  local package=$1
  if [ ! -d "$package" ]; then
    ln -s "$GDK_PATH/$package" "$PKG_PATH/$package"
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