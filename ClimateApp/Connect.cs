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
using ClimateApp.Models;

namespace ClimateApp
{
    public static class Connect
    {
        private static readonly string iothub = Environment.GetEnvironmentVariable("IotHub");
        private static readonly RegistryManager _registryManager =
            RegistryManager.CreateFromConnectionString(iothub);
        
        [FunctionName("Connect")]//Connect to device in Azure
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = "devices/connect")] HttpRequest req,
            ILogger log)
        {
            try
            {
                dynamic body = JsonConvert.DeserializeObject<DeviceRequest>(await new StreamReader(req.Body).ReadToEndAsync());
                var device = await _registryManager.AddDeviceAsync(new Device(body.DeviceId));

                var connectionString = $"{iothub.Split(";")[0]};DeviceId={device.Id};SharedAccessKey{device.Authentication.SymmetricKey.PrimaryKey}";

                return new OkObjectResult(connectionString);
            }
            catch
            {
                return new BadRequestResult();
            }
        }
    }
}
