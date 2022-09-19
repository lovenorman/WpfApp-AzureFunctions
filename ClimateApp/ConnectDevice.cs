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
using Shared.Models;

namespace ClimateApp
{
    public static class ConnectDevice
    {
        private static readonly RegistryManager _registryManager = 
            RegistryManager.CreateFromConnectionString(Environment.GetEnvironmentVariable("IotHub"));

        [FunctionName("ConnectDevice")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "devices/connect")] HttpRequest req,
            ILogger log)
        {
            try
            {
                var body = JsonConvert.DeserializeObject<HttpDeviceRequest>(await new StreamReader(req.Body).ReadToEndAsync());
                if (string.IsNullOrEmpty(body.DeviceId))//Kollar att deviceId är rätt
                    return new BadRequestObjectResult(new HttpDeviceResponse("DeviceId is required"));//Om deviceId inte är med, send badRequest.

                var device = await _registryManager.GetDeviceAsync(body.DeviceId);//Om deviceId finns med, GetDevice hämtar och skickar tillbaka info
                if (device == null)//Om devicen inte finns
                    device = await _registryManager.AddDeviceAsync(new Device(body.DeviceId));//Då vill vi skapa en device med det deviceId som skickades med 

                if(device != null)//Om device skapas korrekt
                {
                    var twin = await _registryManager.GetTwinAsync(device.Id);//Viktigt vilken id-variabel som används.Nu vill jag vara säker på att det verkligen är den devicen som jag hittat/skapat
                    twin.Properties.Desired["interval"] = 10000;
                    await _registryManager.UpdateTwinAsync(twin.DeviceId, twin, twin.ETag);//ETag är som ett id för vilken spegling av objektet vi menar.Båda har dock samma id.
                }



                //if (body == null || body.DeviceId == null)
                //    return new BadRequestObjectResult("DeviceId is required");

                //var device = await _registryManager.GetDeviceAsync(body.DeviceId);//Försöker hämta device
                //if(device != null)//Om Device finns
                //{
                //    return new ConflictObjectResult(new AddDeviceResponse
                //    {
                //        Message = "Device already exists",
                //        Device = device,
                //        DeviceTwin = await _registryManager.GetTwinAsync(device.Id)//device.Id är direkt från databas, 
                //    });
                //}

                //device = await _registryManager.AddDeviceAsync(new Device(body.DeviceId));//skapar ny device med samma instans(återanvänder samma plats i minnet)
                //var twin = await _registryManager.GetTwinAsync(device.Id);

                //twin.Properties.Desired["sendIntervall"] = 10000;//Desired är en ny property som jag sätter här.
                //await _registryManager.UpdateTwinAsync(twin.DeviceId, twin, twin.ETag);

                //return new OkObjectResult(new AddDeviceResponse
                //{
                //    Device = device,
                //    DeviceTwin = await _registryManager.GetTwinAsync(device.Id)//device.Id är direkt från databas, 
                //});
            }
            catch (Exception exception)
            {
                return new BadRequestObjectResult(new HttpDeviceResponse("Unable to connect the device", exception.Message));
            }
        }
    }

    
}
