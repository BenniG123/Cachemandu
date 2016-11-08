using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cachemandu.Models
{
    class LRUReplacementPolicy : IReplacementPolicy
    {
        public void replace(HashSet<List<CacheEntry>> entries, int index, long tag, int count)
        {
            // Replace min mostRecentUse (or any non-valid entry), increment FIFO order of ever other set, set replacement to 0
            int min = count;
            CacheEntry minEntry = null;
            foreach (List<CacheEntry> list in entries)
            {
                if (!list[index].valid)
                {
                    minEntry = list[index];
                    break;
                }
                else if (list[index].mostRecentUse <= min)
                {
                    min = list[index].mostRecentUse;
                    minEntry = list[index];
                }
            }
            minEntry.tag = tag;
            minEntry.mostRecentUse = count;
            minEntry.valid = true;
        }
    }
}
