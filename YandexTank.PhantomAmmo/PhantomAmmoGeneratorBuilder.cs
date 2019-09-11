using System;
using System.Collections.Generic;
using System.Linq;

namespace YandexTank.PhantomAmmo
{
    public class PhantomAmmoGeneratorBuilder
    {
        private List<WeightedAmmoSource> sources = new List<WeightedAmmoSource>();

        public PhantomAmmoGeneratorBuilder AddSource(Func<PhantomAmmoInfo> ammoSource, double weight = 1)
        {
            sources.Add(new WeightedAmmoSource(ammoSource, weight));
            return this;
        }

        public PhantomAmmoGeneratorBuilder AddSources(ICollection<Func<PhantomAmmoInfo>> ammoSources, double weight = 1)
        {
            var oneWeight = weight / ammoSources.Count;
            foreach (var src in ammoSources)
            {
                AddSource(src, oneWeight);
            }

            return this;
        }

        public PhantomAmmoGenerator Build()
        {
            // normalize
            var oneNormalizedPointWeight = 1 / sources.Sum(t => t.Weight);

            var normalizedSources = sources
                .Select(x => new WeightedAmmoSource(x.Source, x.Weight * oneNormalizedPointWeight));
            
            var cumulativeWeightSources = new List<WeightedAmmoSource>();
            double sumWeight = 0;
            foreach (var src in normalizedSources)
            {
                sumWeight += src.Weight;
                cumulativeWeightSources.Add(new WeightedAmmoSource(src.Source, sumWeight));
            }
            
            return new PhantomAmmoGenerator(cumulativeWeightSources);
        }

    }
}