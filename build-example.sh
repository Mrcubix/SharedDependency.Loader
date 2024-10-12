#!/usr/bin/env bash

# if any builds ail, exit

# build the embedded libraries
# dotnet publish Example/MyPlugin.Lib/ -c Debug -o SharedDependency.Loader/libs || exit 1

rm SharedDependency.Loader/libs/*.dll

# build the plugins
dotnet publish Example/MyPlugin/ -c Debug -o build/MyPlugin || exit 1
dotnet publish Example/MyPlugin.Dependent/ -c Debug -o build/MyPlugin.Dependent || exit 1

for directory in ./build/*; do
    if [ -d "$directory" ]; then
        rm $directory/*.deps.json
    fi
done