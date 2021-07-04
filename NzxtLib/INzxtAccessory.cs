using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NzxtLib
{
    public interface INzxtAccessory
    {
        string Name { get; }
        byte Id { get; }
        int LEDCount { get; }
    }
}
