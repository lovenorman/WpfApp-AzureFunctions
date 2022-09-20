using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Device.Climate.MVVM.ViewModels
{
    internal class MainViewModel : ObservableObject    
    {
        public MainViewModel()
        {
            KitchenView = new KitchenViewModel();
            CurrentView = KitchenView;

            KitchenViewCommand = new RelayCommand(x => { CurrentView = KitchenView; });
        }
        
        private object _currentView;
        
        public RelayCommand KitchenViewCommand { get; set; }

        public KitchenViewModel KitchenView { get; set; }

        public object CurrentView
        {
            get { return _currentView; }
            set
            {
                _currentView = value;
                OnPropertyChanged();
            }
        }
    }
}
