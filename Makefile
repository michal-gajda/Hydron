build:
	dotnet build

run: build
	dotnet run --no-build --project src/WebApi
