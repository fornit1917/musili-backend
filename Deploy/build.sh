#!/usr/bin/bash

DIR="$( cd "$(dirname "$0")" ; pwd -P )"
OUT_DIR="${DIR}/dist"
OUT_DIR_FILES="${DIR}/dist/files"
UI_SRC_DIR="${DIR}/../WebUi"
API_SRC_DIR="${DIR}/../Musili.WebApi"

rm -rf "${OUT_DIR}"
mkdir "${OUT_DIR}"
mkdir "${OUT_DIR_FILES}"
mkdir "${OUT_DIR_FILES}/public"

# Check secrets.json
if [ ! -f "${DIR}/secrets.json" ]; then
    echo "File secrets.json is not found. Please, create this file in Deploy directory."
    exit 1
fi

echo "Build front-end..."
cd "${UI_SRC_DIR}"
rm -rf ./dist
npm i
npm run build
mv ./dist/* "${OUT_DIR_FILES}/public"

echo "Build back-end"
cd "${API_SRC_DIR}"
API_DIST="${API_SRC_DIR}/bin/Release/netcoreapp2.1/publish";
rm -rf "${API_DIST}";
dotnet publish -c Release
mv "${API_DIST}"/* "${OUT_DIR_FILES}"
cp "${DIR}/secrets.json" "${OUT_DIR_FILES}/appsettings.Production.json"
rm "${OUT_DIR_FILES}/appsettings.Development.json"

echo "Create tarball"
cd "${OUT_DIR_FILES}"
tar -zcvf "${OUT_DIR}/dist.tar.gz" *