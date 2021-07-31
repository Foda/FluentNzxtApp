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
    public class MultiColorEffectModeViewModel : ObservableObject, IEffectModeViewModel
    {
        private EffectMode _model;
        public EffectMode Model => _model;

        private ObservableCollection<Color> _colorSequence = new();
        public ObservableCollection<Color> ColorSequence
        {
            get => _colorSequence;
            set => SetProperty(ref _colorSequence, value);
        }

        public bool CanAddNewColor
        {
            get => ColorSequence.Count < _model.MaxColors;
        }

        public string Name => _model.Name;

        public int Speed
        {
            get => _model.Speed;
            set
            {
                _model.Speed = value;
                OnPropertyChanged(nameof(Speed));
            }
        }

        public int MinSpeed => _model.MinSpeed;
        public int MaxSpeed => _model.MaxSpeed;

        public IRelayCommand AddColorCommand { get; }
        public IRelayCommand<int> RemoveColorCommand { get; }

        public MultiColorEffectModeViewModel(EffectMode effectModeModel)
        {
            _model = effectModeModel;
            ColorSequence.Add(Color.FromArgb(255, 255, 255, 0));

            AddColorCommand = new RelayCommand(() =>
            {
                Random r = new();
                byte[] buffer = new byte[3];
                r.NextBytes(buffer);
                ColorSequence.Add(Color.FromArgb(255, buffer[0], buffer[1], buffer[2]));

                OnPropertyChanged(nameof(CanAddNewColor));
                AddColorCommand.NotifyCanExecuteChanged();
            }, () => CanAddNewColor);
        }

        public List<Color> GetColors()
        {
            return ColorSequence.ToList();
        }
    }
}
