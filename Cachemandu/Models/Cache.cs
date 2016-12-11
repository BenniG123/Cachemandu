using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cachemandu.Models
{
    class Cache
    {
        private int wordSize;
        private int blockSize;
        private int numBlocks;
        private int numSets;
        private IReplacementPolicy replacementPolicy;
        HashSet<List<CacheEntry>> entries;
        private byte offsetSize;
        private byte indexSize;
        private byte tagSize;
        private int count;
        private Cache nextLayer;

        public Cache(int wordSize, int blockSize, int numBlocks, int numSets, IReplacementPolicy replacementPolicy, bool bit64, Cache nextLayer)
        {
            this.wordSize = wordSize;
            this.blockSize = blockSize;
            this.numBlocks = numBlocks;
            this.numSets = numSets;
            this.replacementPolicy = replacementPolicy;
            this.nextLayer = nextLayer;

            entries = mapEntries(wordSize, blockSize, numBlocks, numSets);
            offsetSize = (byte)Math.Round(Math.Log(wordSize, 2));
            indexSize = (byte)Math.Round(Math.Log(numBlocks, 2));
            tagSize = (byte)((bit64 ? 64 : 32) - (offsetSize + indexSize));
            count = 0;
        }

        public bool check(long addr)
        {
            bool found = false;
            int index = (int)((addr >> offsetSize) & ((0x01 << indexSize) - 1));
            long tag = (addr >> (offsetSize + indexSize)) & ((0x01 << tagSize) - 1);

            // Search cache
            foreach (List<CacheEntry> set in entries)
            {
                if (set[index].tag == tag && set[index].valid)
                {
                    set[index].mostRecentUse = count;
                    set[index].frequency++;
                    found = true;
                    break;
                }
            }

            // If not in cache....
            if (!found)
            {
                // Check lower levels
                if (nextLayer != null)
                {
                    found = nextLayer.check(addr);
                }

                // Replace
                replace(entries, index, tag, count);
            }

            // Increment program counter
            count++;

            return found;
        }

        public void replace(HashSet<List<CacheEntry>> entries, int index, long tag, int count)
        {
            // Replace in this cache
            replacementPolicy.replace(entries, index, tag, count);

            // Replace in lower cache (this technically should only be done when something is evicted, but its a simulator so this is easier)
            if (nextLayer != null)
            {
                nextLayer.replace(entries, index, tag, count);
            }
        }

        private HashSet<List<CacheEntry>> mapEntries(int wordSize, int blockSize, int numBlocks, int numSets)
        {
            HashSet<List<CacheEntry>> ret;

            ret = new HashSet<List<CacheEntry>>();
            for (int i = 0; i < numSets; i++)
            {
                List<CacheEntry> list = new List<CacheEntry>();
                for (int j = 0; j < numBlocks; j++)
                {
                    list.Add(new CacheEntry());
                }
                ret.Add(list);
            }

            return ret;
        }
    }
}
