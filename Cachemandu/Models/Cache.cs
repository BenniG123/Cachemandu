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

        public Cache(int wordSize, int blockSize, int numBlocks, int numSets, IReplacementPolicy replacementPolicy, bool bit64)
        {
            this.wordSize = wordSize;
            this.blockSize = blockSize;
            this.numBlocks = numBlocks;
            this.numSets = numSets;
            this.replacementPolicy = replacementPolicy;

            entries = mapEntries(wordSize, blockSize, numBlocks, numSets);
            offsetSize = (byte)Math.Round(Math.Log(wordSize, 2));
            indexSize = (byte)Math.Round(Math.Log(numBlocks, 2));
            tagSize = (byte)((bit64 ? 64 : 32) - (offsetSize + indexSize));
            count = 0;
        }

        public bool check(long addr)
        {
            bool found = false;
            int index = (int)((addr >> offsetSize) & (0x01 << indexSize));
            long tag = (addr >> (offsetSize + indexSize)) & (0x01 << tagSize);

            // Search cache
            foreach (List<CacheEntry> set in entries) {
                if (set[index].tag == tag && set[index].valid == 1)
                {
                    found = true;
                    break;
                }
            }

            // If not in cache, replace
            if (!found) {
                replacementPolicy.replace(entries, index, tag, count);
                return false;
            }

            // Increment program counter
            count++;

            return true;
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
