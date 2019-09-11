using System;
using System.Collections.Generic;
using YandexTank.PhantomAmmo;

namespace YandexTank.PhantomAmmo
{
    public class PhantomAmmoGenerator
    {
        private readonly List<double> weights = new List<double>();
        
        private readonly Dictionary<int, Func<PhantomAmmoInfo>> sourcesByIndex = new Dictionary<int, Func<PhantomAmmoInfo>>();
        
        private Random r = new Random();
        
        internal PhantomAmmoGenerator(List<WeightedAmmoSource> sources)
        {
            foreach (var src in sources)
            {
                weights.Add(src.Weight);
                sourcesByIndex.Add(weights.Count - 1, src.Source);
            }
        }

        public string GetNext()
        {
            var k = r.NextDouble();

            var index = weights.BinarySearch(k);
            if (index < 0)
            {
                index = ~index;
            }

            var src = sourcesByIndex[index];

            var ammoInfo = src();
            return ammoInfo.ToString();
        }

    }
}