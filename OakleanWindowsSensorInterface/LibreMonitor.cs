using System.Diagnostics;
using LibreHardwareMonitor.Hardware;

namespace OakleanWindowsSensorInterface;

public class LibreMonitor : IDisposable
{
    private readonly Computer _computer;
    private readonly Stopwatch _stopWatch;
    private readonly List<string> _valueList = new();
    private readonly StreamWriter _writer;
    private bool _disposed;
    private bool _stopped;

    public LibreMonitor(string filePath)
    {
        _computer = new Computer
        {
            IsCpuEnabled = true,
            IsGpuEnabled = true,
            IsMemoryEnabled = true,
            IsMotherboardEnabled = false,
            IsControllerEnabled = false,
            IsNetworkEnabled = false,
            IsStorageEnabled = false
        };
        _computer.Open();

        var fileDirectory = Path.GetDirectoryName(filePath);

        if (!Directory.Exists(fileDirectory) && !string.IsNullOrWhiteSpace(fileDirectory))
        {
            Directory.CreateDirectory(fileDirectory);
        }

        _writer = new StreamWriter(new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.ReadWrite));
        _writer.AutoFlush = true;

        _stopWatch = new Stopwatch();
    }

    public void Stop()
    {
        _stopped = true;
        _stopWatch.Start();
        _computer.Close();
        _writer.Close();
    }

    public void DoMeasurement()
    {
        if (_stopped)
        {
            return;
        }

        if (!_stopWatch.IsRunning)
        {
            _stopWatch.Start();
        }

        _valueList.Clear();
        _computer.Accept(new UpdateVisitor());

        foreach (var hardware in _computer.Hardware)
        {
            var powerSensors = hardware.Sensors.Where(x => x.SensorType == SensorType.Power).ToArray();

            foreach (var sensor in powerSensors)
            {
                _valueList.Add($"{sensor.Name}|{sensor.Value}");
            }
        }

#if DEBUG
        Console.WriteLine("{0}|{1}", _stopWatch.ElapsedMilliseconds, string.Join("|", _valueList));
#endif
        _writer.WriteLine("{0}|{1}", _stopWatch.ElapsedMilliseconds, string.Join("|", _valueList));
    }

    #region IDisposable

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected void Dispose(bool disposing)
    {
        if (_disposed)
        {
            return;
        }

        if (disposing)
        {
            if (_stopped == false)
            {
                Stop();
                _writer.Dispose();
            }
        }

        _disposed = true;
    }

    ~LibreMonitor()
    {
        Dispose(false);
    }

    #endregion
}