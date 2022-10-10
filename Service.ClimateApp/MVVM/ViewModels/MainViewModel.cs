using Service.ClimateApp.MVVM.Cores;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.ClimateApp.MVVM.ViewModels
{
    internal class MainViewModel : ObservableObject
    {
        public MainViewModel()
        {
            KitchenViewModelProp = new();
            KitchenViewCommand = new RelayCommand(x => { CurrentView = KitchenViewModelProp; });//När vi använder KitchenViewCommand, visa en kitchenvy som currentvy.

            BedroomViewModel = new();
            BedroomViewCommand = new RelayCommand(x => { CurrentView = BedroomViewModel; });

            LivingroomViewModel = new();
            LivingroomViewCommand = new RelayCommand(x => { CurrentView = LivingroomViewModel; });

            CurrentView = KitchenViewModelProp;
            
            SetClock();

        }

        private object _currentView;

        public RelayCommand KitchenViewCommand { get; set; }
        public KitchenViewModel KitchenViewModelProp { get; set; }
        public RelayCommand BedroomViewCommand { get; set; }
        public BedroomViewModel BedroomViewModel { get; set; }
        public RelayCommand LivingroomViewCommand { get; set; }
        public LivingroomViewModel LivingroomViewModel { get; set; }

        public object CurrentView
        {
            get { return _currentView; }
            set
            {
                _currentView = value;
                OnPropertyChanged();
            }
        }

        private string? _currentTime;
        public string CurrentTime
        {
            get => _currentTime!;
            set
            {
                _currentTime = value;
                OnPropertyChanged();
            }
        }

        protected override async void second_timer_tick(object? sender, EventArgs e)
        {
            SetClock();
            await PopulateDeviceItemsAsync();
            base.second_timer_tick(sender, e);
        }

        private void SetClock()
        {
            CurrentTime = DateTime.Now.ToString("HH:mm");
            CurrentDate = DateTime.Now.ToString("dd MMMM yyyy");
        }
    }
    
}
