using System.ComponentModel;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace OakleanWindowsSensorInterface;

internal class Program
{
    /// <summary>
    ///     The main console entry point.
    /// </summary>
    /// <param name="args">The commandline arguments.</param>
    private static async Task Main(string[] args)
    {
        var sampleRate = GetSampleRate(args);
        var fileName = GetFileName(args);

        using var host = new HostBuilder()
            .ConfigureServices((hostContext, services) => services.AddHostedService(x => ActivatorUtilities.CreateInstance<Worker>(x, sampleRate, fileName)))
            .UseConsoleLifetime()
            .Build();

        await host.RunAsync();
    }


    #region Private Methods

    private static string GetFileName(string[] args)
    {
        var fallback = "c:/tmp/measurement.txt";
        var result = GetValue(args, fallback, "fileName");
        return result;
    }

    private static int GetSampleRate(string[] args)
    {
        var fallback = 150;
        var result = GetValue(args, fallback, "sampleRate");
        return result;
    }

    private static T GetValue<T>(string[] args, T fallback, string value)
    {
        foreach (var arg in args)
        {
            if (arg.ToLower().Contains(value.ToLower()))
            {
                var array = arg.Split('=');
                if (array.Length == 2)
                {
                    var converter = TypeDescriptor.GetConverter(typeof(T));
                    return (T)converter.ConvertFromString(array[1])!;
                }
            }
        }

        return fallback;
    }

    #endregion
}