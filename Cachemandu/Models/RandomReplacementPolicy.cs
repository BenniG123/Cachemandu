using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cachemandu.Models
{
    class RandomReplacementPolicy : IReplacementPolicy
    {
        public void replace(HashSet<List<CacheEntry>> entries, int index, long tag, int count)
        {
            CacheEntry entry = entries.ElementAt((new Random()).Next(entries.Count))[index];
            entry.tag = tag;
            entry.valid = 1;
        }
    }
}
