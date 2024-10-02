# if any builds ail, exit

# build the embedded libraries
#dotnet publish SharedDependency.Lib/ -c Debug -o SharedDependency.Loaders/libs || exit 1

# build the plugins
dotnet publish SharedDependency -c Debug -o build/MainPlugin || exit 1
dotnet publish SharedDependency.DependentPlugin -c Debug -o build/DependentPlugin || exit 1

for directory in ./build/*; do
    if [ -d "$directory" ]; then
        rm $directory/*.deps.json
    fi
done