﻿using Service.ClimateApp.MVVM.ViewModels;
using Service.ClimateApp.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Service.ClimateApp.Components
{
    /// <summary>
    /// Interaction logic for WeatherComponent.xaml
    /// </summary>
    public partial class WeatherComponent : UserControl
    {
        private readonly IWeatherService _weatherService;
        
        public WeatherComponent()
        {
            InitializeComponent();
            _weatherService = new WeatherService();
            this.DataContext = new WeatherComponentViewModel(_weatherService);
        }
    }
}
