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
        static int ProductId { get; }
        Task<bool> FindDevice();
        List<INzxtAccessory> LedAccessories { get; }
        List<Hue2EffectMode> EffectModes { get; }
    }
}
