using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NzxtLib
{
    public interface INzxtDevice
    {
        Task<bool> FindDevice();
        Task<bool> ApplyFixedColor(Color color);
        List<INzxtAccessory> LedAccessories { get; }
        List<INzxtEffectMode> EffectModes { get; }
    }
}
