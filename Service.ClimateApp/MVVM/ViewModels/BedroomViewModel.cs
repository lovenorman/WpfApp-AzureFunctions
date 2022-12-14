using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.ClimateApp.MVVM.ViewModels
{
    internal class BedroomViewModel
    {
        public string Title { get; set; } = "Bedroom";
        public string Temperature { get; set; } = "21";
        public string TemperatureScale { get; set; } = "°C";
        public string Humidity { get; set; } = "31";
        public string HumidityScale { get; set; } = "%";
    }
}
