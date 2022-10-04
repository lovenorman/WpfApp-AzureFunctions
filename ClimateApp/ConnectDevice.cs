using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Microsoft.Azure.Devices;

namespace ClimateApp
{
    public static class ConnectDevice
    {
        [FunctionName("ConnectDevice")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "devices/connect")] HttpRequest req,
            ILogger log)
        {
            try
            {
                using var registryManager = RegistryManager.CreateFromConnectionString(Environment.GetEnvironmentVariable("IotHub"));
                var device = await registryManager.AddDeviceAsync(new Device(req.Query["deviceId"]));
            }
            catch (Exception ex)
            {

                return new BadRequestObjectResult(ex.Message);
            }
          
        }
    }
}
