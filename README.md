# NETConf2020
Project from session "Building Cross-Platform Desktop Apps with Electron.NET"

## Instructions
Before trying to build the project, please install the following:
* Node.js
* .NET 5.0

Then install the CLI for Electron.NET:
```
dotnet tool install ElectronNET.CLI -g
```

To run the application, cd to the project root folder and type:
```
electronize start
```

To build a setup for a specific platform, type the following:
```
electronize build /target xxx /PublishReadyToRun false
```

Where `xxx` is one of:
* `win` (generates setup .exe file)
* `osx` (generates .dmg file)
* `linux` (generates .AppImage file)

Build output is written to the `bin/Desktop` folder.
