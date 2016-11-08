using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cachemandu.Models
{
    class LFUReplacementPolicy : IReplacementPolicy
    {
        public void replace(HashSet<List<CacheEntry>> entries, int index, long tag, int count)
        {
            // Replace min frequency (or any non-valid entry); we are assuming that frequency doesn't overflow for simplicity
            int min = Int32.MaxValue;
            CacheEntry minEntry = null;
            foreach (List<CacheEntry> list in entries)
            {
                if (!list[index].valid)
                {
                    minEntry = list[index];
                    break;
                }
                else if (list[index].frequency <= min)
                {
                    min = list[index].frequency;
                    minEntry = list[index];
                }
            }
            minEntry.tag = tag;
            minEntry.frequency = 0;
            minEntry.valid = true;
        }
    }
}
