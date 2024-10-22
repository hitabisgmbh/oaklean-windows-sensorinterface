using Microsoft.Extensions.Hosting;

namespace OakleanWindowsSensorInterface;

public class Worker : IHostedService, IDisposable
{
    private readonly int _sampleRate;
    private Task _backgroundTask = null!;
    private bool _disposed;
    private readonly LibreMonitor _libreMonitor;
    private bool _stopMeasurement;

    public Worker(int sampleRate, string fileName)
    {
        _sampleRate = sampleRate;
        _libreMonitor = new LibreMonitor(fileName);
    }

    public Task StartAsync(CancellationToken _)
    {
        _backgroundTask = Task.Run(async () =>
        {
            while (!_stopMeasurement)
            {
                await Task.Delay(_sampleRate);
                _libreMonitor.DoMeasurement();
            }

            _libreMonitor.Stop();
        });
        
        return Task.CompletedTask;
    }

    public async Task StopAsync(CancellationToken _)
    {
        _stopMeasurement = true;
        await _backgroundTask;
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
            _libreMonitor.Dispose();
            _backgroundTask.Dispose();
        }

        _disposed = true;
    }

    ~Worker()
    {
        Dispose(false);
    }

    #endregion
}