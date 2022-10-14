using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.ClimateApp.MVVM.Models
{
    internal class DirectMethodRequest
    {
        private string DeviceId { get; set; } = null;
        private string MethodName { get; set; } = null;

        private object? Payload { get; set; } = null;
    }
}
