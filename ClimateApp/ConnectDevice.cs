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



                //if (body == null || body.DeviceId == null)
                //    return new BadRequestObjectResult("DeviceId is required");

                //var device = await _registryManager.GetDeviceAsync(body.DeviceId);//F�rs�ker h�mta device
                //if(device != null)//Om Device finns
                //{
                //    return new ConflictObjectResult(new AddDeviceResponse
                //    {
                //        Message = "Device already exists",
                //        Device = device,
                //        DeviceTwin = await _registryManager.GetTwinAsync(device.Id)//device.Id �r direkt fr�n databas, 
                //    });
                //}

                //device = await _registryManager.AddDeviceAsync(new Device(body.DeviceId));//skapar ny device med samma instans(�teranv�nder samma plats i minnet)
                //var twin = await _registryManager.GetTwinAsync(device.Id);

                //twin.Properties.Desired["sendIntervall"] = 10000;//Desired �r en ny property som jag s�tter h�r.
                //await _registryManager.UpdateTwinAsync(twin.DeviceId, twin, twin.ETag);

                //return new OkObjectResult(new AddDeviceResponse
                //{
                //    Device = device,
                //    DeviceTwin = await _registryManager.GetTwinAsync(device.Id)//device.Id �r direkt fr�n databas, 
                //});
            }
            catch (Exception exception)
            {
                return new BadRequestObjectResult(new HttpDeviceResponse("Unable to connect the device", exception.Message));
            }
        }
    }

    
}
