using ExecutingDevice;

namespace ExecutDeviceIoT.Services;

public class TimedHostedService : IHostedService, IDisposable
{
    private int executionCount = 0;
    private readonly ILogger<TimedHostedService> _logger;
    private Timer? _timer = null;
    private readonly IConfiguration _config;
    public TimedHostedService(ILogger<TimedHostedService> logger, IConfiguration config)
    {
        _logger = logger;
        _config = config;
    }

    public Task StartAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Timed Hosted Service running.");

        _timer = new Timer(DoWork, null, TimeSpan.Zero,TimeSpan.FromSeconds(30));

        return Task.CompletedTask;
    }

    private void DoWork(object? state)
    {
        if (_config["DeviceInfo:Status"].Equals(Status.RUN))
        {
            // POST DATA to HEAD CONTROLLER 

            var data = new DeviceData
            {
                Name = _config["DeviceInfo:Name"],
                Data = "Good Day"
            };
            JsonContent content = JsonContent.Create(data);
            HttpClient client = new HttpClient();
            var response = client.PostAsync("https://192.168.150.3:44304/Gateway/PostDeviceData", content);
            
            _logger.LogInformation($"{DateTime.UtcNow} | POST data to main device. {response.Result.StatusCode}");
        }
           
    }

    public Task StopAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Timed Hosted Service is stopping.");

        _timer?.Change(Timeout.Infinite, 0);

        return Task.CompletedTask;
    }

    public void Dispose()
    {
        _timer?.Dispose();
    }
    
    private class DeviceData
    {
        public string Name { get; set; }
        public string Data { get; set; }
    }
}