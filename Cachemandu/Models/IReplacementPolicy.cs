using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cachemandu.Models
{
    interface IReplacementPolicy
    {
        void replace(HashSet<List<CacheEntry>> entries, int index, long tag, int count);
    }
}
