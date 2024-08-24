#!/usr/bin/env bash
set -e

BASE_DIR=$(dirname "$0")

migrate() {
    echo "Migrating database"
    "${BASE_DIR}/dbmigrate" $@
}

run() {
    echo "Running application"
    dotnet "$BASE_DIR/mohaymen-codestar-Team02.dll"
}


case "$1" in
migrate)
    additional_args=${@:2}
    migrate $additional_args
    ;;
run)
    run
    ;;
*)
    migrate
    run
    ;;
esac
