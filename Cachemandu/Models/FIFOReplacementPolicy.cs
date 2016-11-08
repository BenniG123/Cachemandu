using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cachemandu.Models
{
    class FIFOReplacementPolicy : IReplacementPolicy
    {
        public void replace(HashSet<List<CacheEntry>> entries, int index, long tag, int count)
        {
            // Replace max FIFO order (or any non-valid entry), increment FIFO order of ever other set, set replacement to 0
            int max = 0;
            CacheEntry maxEntry = null;
            foreach (List<CacheEntry> list in entries)
            {
                if (!list[index].valid)
                {
                    max = Int32.MaxValue;
                    maxEntry = list[index];
                }
                else if (list[index].fifoOrder > max)
                {
                    max = list[index].fifoOrder;
                    maxEntry = list[index];
                }
                list[index].fifoOrder++;
            }
            maxEntry.tag = tag;
            maxEntry.fifoOrder = 0;
            maxEntry.valid = true;
        }
    }
}
