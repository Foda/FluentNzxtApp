using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using RBGLib;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Windows.UI;

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

        public bool CanRemoveNewColor
        {
            get => ColorSequence.Count > _model.MinColors;
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
                OnPropertyChanged(nameof(CanRemoveNewColor));

                AddColorCommand.NotifyCanExecuteChanged();
                RemoveColorCommand.NotifyCanExecuteChanged();
            }, () => CanAddNewColor);

            RemoveColorCommand = new RelayCommand<int>((lightIdx) =>
            {
                ColorSequence.RemoveAt(lightIdx);

                OnPropertyChanged(nameof(CanAddNewColor));
                OnPropertyChanged(nameof(CanRemoveNewColor));

                AddColorCommand.NotifyCanExecuteChanged();
                RemoveColorCommand.NotifyCanExecuteChanged();
            }, (lightIdx) => CanRemoveNewColor);
        }

        public List<Color> GetColors()
        {
            return ColorSequence.ToList();
        }
    }
}
