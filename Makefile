build:
	dotnet build

run: build
	dotnet run --no-build --project src/WebApi

test:
	dotnet test --logger "console;verbosity=detailed"
