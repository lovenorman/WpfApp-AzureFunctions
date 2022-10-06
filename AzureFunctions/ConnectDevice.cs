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
using AzureFunctions.Models;

namespace AzureFunctions
{
    public static class ConnectDevice
    {
        private static readonly string iotHub = Environment.GetEnvironmentVariable("IotHub");
        private static readonly RegistryManager _registryManager =
            RegistryManager.CreateFromConnectionString(iotHub);

        [FunctionName("ConnectDevice")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "devices/connect")] HttpRequest req,
            ILogger log)
        {
            try
            {
                var body = JsonConvert.DeserializeObject<DeviceRequest>(await new StreamReader(req.Body).ReadToEndAsync());
                var device = await _registryManager.AddDeviceAsync(new Device(body.DeviceId));

                var connectionstring = $"{iotHub.Split(";")[0]};DeviceId={device.Id};SharedAccessKey={device.Authentication.SymmetricKey.PrimaryKey}";

                return new OkObjectResult(connectionstring);
            }
            catch
            {

                return new BadRequestResult();
            }
            
            
        }
    }
}
