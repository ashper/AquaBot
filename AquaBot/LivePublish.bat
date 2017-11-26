dotnet clean
dotnet build -c release
dotnet publish -c release -r win7-x64 -o Publish/win7-x64