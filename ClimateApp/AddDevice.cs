using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using ClimateApp.Models;
using Microsoft.Azure.Devices;

namespace ClimateApp
{
    public static class AddDevice
    {
        private static readonly RegistryManager _registryManager = 
            RegistryManager.CreateFromConnectionString(Environment.GetEnvironmentVariable("IotHub"));

        [FunctionName("AddDevice")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "devices")] HttpRequest req,
            ILogger log)
        {
            try
            {
                var data = JsonConvert.DeserializeObject<AddDeviceRequest>(await new StreamReader(req.Body).ReadToEndAsync());
                if (data == null || data.DeviceId == null)
                    return new BadRequestObjectResult("DeviceId is required");

                var device = await _registryManager.GetDeviceAsync(data.DeviceId);//Försöker hämta device
                if(device != null)//Om Device finns
                {
                    return new ConflictObjectResult(new AddDeviceResponse
                    {
                        Message = "Device already exists",
                        Device = device,
                        DeviceTwin = await _registryManager.GetTwinAsync(device.Id)//device.Id är direkt från databas, 
                    });
                }

                device = await _registryManager.AddDeviceAsync(new Device(data.DeviceId));//skapar ny device med samma instans(återanvänder samma plats i minnet)
                var twin = await _registryManager.GetTwinAsync(device.Id);

                twin.Properties.Desired["sendIntervall"] = 10000;//Desired är en ny property som jag sätter här.
                await _registryManager.UpdateTwinAsync(twin.DeviceId, twin, twin.ETag);

                return new OkObjectResult(new AddDeviceResponse
                {
                    Device = device,
                    DeviceTwin = await _registryManager.GetTwinAsync(device.Id)//device.Id är direkt från databas, 
                });
            }
            catch (Exception ex)
            {
                return new BadRequestObjectResult(new
                {
                    Error = "Unable to add new device",
                    Exception = ex.Message
                });
            }
        }
    }

    
}
