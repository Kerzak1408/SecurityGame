using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Scripts.Algorithms.FloodFill
{
    public interface ICluster<T>
    {
        List<T> Members { get; set; }
    }
}
