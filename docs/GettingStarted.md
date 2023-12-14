# Getting Started

## Local Set up

Azure Storage and Azure Cosmos must be available to run the application. Locally, the Azurite storage emulator and the Azure Cosmos emulator can be used. A docker compose file is available at the root of the project to start up the emulators needed.

## Visual Studio

In Visual Studio, you can set the `Microsoft.Developer.Api` project as the start up to run just the API. However, most likely you want to run it configured for a provider. By default, in development mode, the provider knows uses the test provider that doesn't do much, but allows for testing communication from the orchestrator to the provider. To run that, select "Configure Startup Projects" in the Solution Explorer, and select the "Test Provider" profile to run.
