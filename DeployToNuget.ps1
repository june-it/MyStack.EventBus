# Release
dotnet restore
dotnet build -c Release

# Push to NuGet 
cd src/MyStack.EventBus/bin/Release
dotnet nuget push MyStack.EventBus.*.nupkg  --api-key $env:NUGET_API_KEY --source https://api.nuget.org/v3/index.json --skip-duplicate
cd ../../../../

cd src/MyStack.EventBus.Abstractions/bin/Release
dotnet nuget push MyStack.EventBus.Abstractions.*.nupkg  --api-key $env:NUGET_API_KEY --source https://api.nuget.org/v3/index.json --skip-duplicate
cd ../../../../

cd src/MyStack.EventBus.RabbitMQ/bin/Release
dotnet nuget push MyStack.EventBus.RabbitMQ.*.nupkg  --api-key $env:NUGET_API_KEY --source https://api.nuget.org/v3/index.json --skip-duplicate
cd ../../../../

