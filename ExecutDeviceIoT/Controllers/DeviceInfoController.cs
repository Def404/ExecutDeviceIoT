using System.Net;
using Microsoft.AspNetCore.Mvc;

namespace ExecutingDevice.Controllers;

[ApiController]
[Route("[controller]/[action]")]
public class DeviceInfoController : ControllerBase
{
    private readonly ILogger<DeviceInfoController> _logger;
    private readonly IConfiguration _config;

    public DeviceInfoController(ILogger<DeviceInfoController> logger, IConfiguration config)
    {
        _logger = logger;
        _config = config;
    }
    
    [HttpGet(Name = "GetStatus")]
    public async Task<IActionResult> GetStatus()
    {
        var deviceName = _config["DeviceInfo:Name"];
        var status = _config["DeviceInfo:Status"];
        
        _logger.LogInformation($"{DateTime.UtcNow} | Getting status information about the device");
        return new JsonResult(new {deviceName, status});
    }

    [HttpPost(Name = "PostChangeStatus")]
    public async Task<IActionResult> PostChangeStatus(int? value)
    {
        var oldStatus = _config["DeviceInfo:Status"];
        string newStatus; 
        switch (value)
        {
            case 0:
                newStatus = Status.STOP;
                break;
            case 1:
                newStatus = Status.RUN;
                break;
            default:
                return NotFound();
                break;
        }
        
        if(newStatus.Equals(oldStatus))
            return NotFound();
        
        _config["DeviceInfo:Status"] = newStatus;
        _logger.LogInformation($"{DateTime.UtcNow} | Change status: {oldStatus} --> {newStatus}");
        
        return Ok();
    }
    
}