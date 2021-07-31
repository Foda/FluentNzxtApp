using Microsoft.UI;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using NzxtLib;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Windows.UI;
using RBGLib;

namespace FluentNzxt.ViewModel
{
    public class FixedColorEffectModeViewModel : ObservableObject, IEffectModeViewModel
    {
        private EffectMode _model;
        public EffectMode Model => _model;

        private Color _color;
        public Color Color
        {
            get => _color;
            set => SetProperty(ref _color, value);
        }

        public string Name => _model.Name;

        public FixedColorEffectModeViewModel(EffectMode effectModeModel)
        {
            _model = effectModeModel;
            _color = Colors.Red;
        }

        public List<Color> GetColors()
        {
            return new List<Color> { Color };
        }
    }
}
