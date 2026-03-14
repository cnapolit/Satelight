#!/bin/bash
folders=("Functions" "Classes" "Models" "Extensions")
dir="$(pwd)"
base_path="$dir/Common/Utility"

create_link() {
    local folder_name="$1"
    local path="${base_path}.Core/$folder_name"
    local target="$base_path/$folder_name"
    if [ -e "$path" ]; then
        return
    fi
    ln -s "$target" "$path"
}

for folder in "${folders[@]}"; do
    create_link "$folder"
done
