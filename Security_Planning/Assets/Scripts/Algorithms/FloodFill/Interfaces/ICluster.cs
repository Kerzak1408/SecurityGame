using System.Collections.Generic;

namespace Assets.Scripts.Algorithms.FloodFill.Interfaces
{
    public interface ICluster<T>
    {
        List<T> Members { get; set; }
    }
}
