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
using HttpDeviceResponse = ClimateApp.Models.HttpDeviceResponse;
using HttpDeviceRequest = ClimateApp.Models.HttpDeviceRequest;

namespace ClimateApp
{
    public static class ConnectDevice
    {
        private static readonly RegistryManager _registryManager = 
            RegistryManager.CreateFromConnectionString(Environment.GetEnvironmentVariable("IotHub"));

        [FunctionName("ConnectDevice")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "devices/connectdevices")] HttpRequest req,
            ILogger log)
        {
            try
            {
                var body = JsonConvert.DeserializeObject<HttpDeviceRequest>(await new StreamReader(req.Body).ReadToEndAsync());
                if (string.IsNullOrEmpty(body.DeviceId))//Kollar att deviceId �r r�tt
                    return new BadRequestObjectResult(new HttpDeviceResponse("DeviceId is required"));//Om deviceId inte �r med, send badRequest.

                var device = await _registryManager.GetDeviceAsync(body.DeviceId);//Om deviceId finns med, GetDevice h�mtar och skickar tillbaka info
                if (device == null)//Om devicen inte finns
                    device = await _registryManager.AddDeviceAsync(new Device(body.DeviceId));//D� vill vi skapa en device med det deviceId som skickades med 

                if(device != null)//Om device skapas korrekt
                {
                    var twin = await _registryManager.GetTwinAsync(device.Id);//Viktigt vilken id-variabel som anv�nds.Nu vill jag vara s�ker p� att det verkligen �r den devicen som jag hittat/skapat
                    twin.Properties.Desired["interval"] = 10000;
                    await _registryManager.UpdateTwinAsync(twin.DeviceId, twin, twin.ETag);//ETag �r som ett id f�r vilken spegling av objektet vi menar.B�da har dock samma id.
                }
                return new OkObjectResult(new HttpDeviceResponse("Device connected", device));

            }
            catch (Exception exception)
            {
                return new BadRequestObjectResult(new HttpDeviceResponse("Unable to connect the device", exception.Message));
            }
        }
    }

    
}
