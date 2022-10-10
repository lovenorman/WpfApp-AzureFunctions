using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.ClimateApp.MVVM.Models
{
    internal class WeatherResponse
    {
        private int _temperature;
        public int Temperature
        {
            get => _temperature;
            set
            {
                _temperature = (int)(value - 273.15);
            }
        }
    }
}
