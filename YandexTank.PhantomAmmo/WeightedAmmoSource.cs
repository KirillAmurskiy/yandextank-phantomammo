using System;

namespace YandexTank.PhantomAmmo
{
    internal class WeightedAmmoSource
    {
        public WeightedAmmoSource(Func<PhantomAmmoInfo> source, double weight)
        {
            Source = source;
            Weight = weight;
        }

        public Func<PhantomAmmoInfo> Source { get; }
            
        public double Weight { get; }
    }
}