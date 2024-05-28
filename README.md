# Batch Hollower

Hollow multiple .NET assemblies at once.

## Requirements

- .NET 8.0.X

## Build

- cli: `dotnet publish`
- vscode: Terminal > Run Build Task... `dotnet: publish`

## Usage

1. Drop your assemblies into `Managed/`
2. Run the application
  - From source: `dotnet run`
  - From exe: `Seion.BatchHollower.exe`
3. Result is located in `Hollowed/`

## Notes

It only hollows relevant assemblies for Escape From Tarkov.
In case you want to use it for a different project, modify the blacklist.
