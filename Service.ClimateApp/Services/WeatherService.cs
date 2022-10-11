using Service.ClimateApp.MVVM.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace Service.ClimateApp.Services
{
    internal interface IWeatherService
    {
        public Task<WeatherResponse> GetWeatherDataAsync(string uri = "https://api.openweathermap.org/data/2.5/weather?lat=59.1881139&lon=18.1140349&appid=6d13e29f2cb2164808a6ae4c8a61d5fb");

    }
    
    internal class WeatherService : IWeatherService
    {
        public async Task<WeatherResponse> GetWeatherDataAsync(string uri)
        {
            try
            {
                using var client = new HttpClient();
                var response = await client.GetFromJsonAsync<WeatherApiResponse>(uri);
                return new WeatherResponse
                {
                    Temperature = (int)response!.main.temp,
                    Humidity = response.main.humidity,
                    WeatherCondition = response.weather[0].main
                };
            }
            catch { }
            return null!;
        }
    }
}
