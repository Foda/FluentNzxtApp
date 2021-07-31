using RBGLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI;

namespace FluentNzxt.ViewModel
{
    public interface IEffectModeViewModel
    {
        List<Color> GetColors();
        string Name { get; }
        EffectMode Model { get; }
    }
}
