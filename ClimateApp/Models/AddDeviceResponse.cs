using Microsoft.Azure.Devices;
using Microsoft.Azure.Devices.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClimateApp.Models
{
    internal class AddDeviceResponse
    {
        public string Message { get; set; } = "Device was succesfully added";
        public string DeviceConnectionString => $"HostName={IotHubName}DeviceId={Device.Id};SharedAccessKey={Device.Authentication.SymmetricKey.PrimaryKey}";
        public string IotHubName { get; set; } = Environment.GetEnvironmentVariable("IotHubName");
        public Device Device { get; set; } = new();
        public Twin DeviceTwin { get; set; } = new();

    }
}
