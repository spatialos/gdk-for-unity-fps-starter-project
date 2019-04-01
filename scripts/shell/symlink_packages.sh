#!/usr/bin/env bash

set -e -u -x -o pipefail

cd "$(dirname "$0")"

GDK_PATH="$(realpath $(pwd)/../../../gdk-for-unity/workers/unity/Packages)"
PKG_PATH="$(realpath $(pwd)/../../gdk-for-unity/workers/unity/Packages)"

link_package() {
  local package=$1
  if [ ! -d "$package" ]; then
    ln -s "$GDK_PATH/$package" "$PKG_PATH/$package"
  fi
}

link_package "com.improbable.gdk.buildsystem"
link_package "com.improbable.gdk.core"
link_package "com.improbable.gdk.gameobjectcreation"
link_package "com.improbable.gdk.mobile"
link_package "com.improbable.gdk.playerlifecycle"
link_package "com.improbable.gdk.testutils"
link_package "com.improbable.gdk.tools"
link_package "com.improbable.gdk.transformsynchronization"