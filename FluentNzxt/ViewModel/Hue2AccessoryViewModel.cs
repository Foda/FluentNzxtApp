using Microsoft.Toolkit.Mvvm.ComponentModel;
using NzxtLib;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI;

namespace FluentNzxt.ViewModel
{
    public class Hue2AccessoryViewModel : ObservableObject
    {
        private Hue2Accessory _model;

        public string Name => _model.Name;

        private ObservableCollection<Color> _leds;
        public ObservableCollection<Color> LEDS
        {
            get => _leds;
            private set => SetProperty(ref _leds, value);
        }

        public Hue2AccessoryViewModel(Hue2Accessory model)
        {
            _model = model;

            LEDS = new ObservableCollection<Color>();

            SetColor(Color.FromArgb(255, 0, 71, 178));
        }

        public void SetColor(Color color)
        {
            LEDS.Clear();

            for (int i = 0; i < _model.LEDCount; i++)
            {
                LEDS.Add(color);
            }
        }
    }
}
