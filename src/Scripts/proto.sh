#!/bin/bash
set -euo pipefail

GRPC_TOOLS_VERSION="2.76.0"
PROTO_DIR="Protos"
OUT_DIR="server/Build-out"

# Detect platform
case "$(uname -s)-$(uname -m)" in
    Darwin-arm64) PLATFORM="macosx_x64" ;;
    Darwin-x86_64) PLATFORM="macosx_x64" ;;
    Linux-x86_64) PLATFORM="linux_x64" ;;
    Linux-aarch64) PLATFORM="linux_arm64" ;;
    *) echo "Unsupported platform"; exit 1 ;;
esac

TOOLS="$HOME/.nuget/packages/grpc.tools/$GRPC_TOOLS_VERSION/tools/$PLATFORM"
PROTOC="$TOOLS/protoc"
PLUGIN="$TOOLS/grpc_csharp_plugin"

if [ ! -f "$PROTOC" ]; then
    echo "protoc not found at $PROTOC — run 'dotnet restore' first"
    exit 1
fi

mkdir -p "$OUT_DIR"

for proto in server.proto Events.proto host.proto Core.proto; do
    "$PROTOC" \
        --proto_path="$PROTO_DIR" \
        --csharp_out="$OUT_DIR" \
        --grpc_out="$OUT_DIR" \
        --plugin=protoc-gen-grpc="$PLUGIN" \
        "$PROTO_DIR/$proto"
done

echo "Generated protobuf files in $OUT_DIR"
