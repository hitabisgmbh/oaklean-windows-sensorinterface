# Oaklean-Windows-SensorInterface

A .Net binary that provides energy measurements for the [Oaklean](https://github.com/hitabisgmbh/oaklean) library. This binary can be installed in Node.js via the [@oaklean/windows-sensorinterface](https://github.com/hitabisgmbh/oaklean/tree/main/packages/windows-sensorinterface).

## Development
### Prerequisites
- .NET Framework 8.0
- Visual Studio

### Setup
1. Install dependencies\
  Ensure you have all necessary dependencies installed before building the project. Use NuGet to restore missing packages if needed.

2. Build the project\
  Open the solution in Visual Studio and build the project using the build options (Build > Build Solution or press Ctrl+Shift+B).

## Usage
```
> .\OakleanWindowsSensorInterface.exe samplerate=100 filename=outfile.csv

Options:

samplerate: The time interval between measurements, in microseconds.\
filename: The name of the file where measurements will be saved.
```