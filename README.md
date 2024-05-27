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
