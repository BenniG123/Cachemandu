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
        private IMappingProtocol mappingProtocol;
        private IReplacementPolicy replacementPolicy;
        HashSet<List<CacheEntry>> entries;
        private byte offsetSize;
        private byte indexSize;
        private byte tagSize;

        public Cache(int wordSize, int blockSize, int numBlocks, IMappingProtocol mappingProtocol, IReplacementPolicy replacementPolicy, bool bit64)
        {
            this.wordSize = wordSize;
            this.blockSize = blockSize;
            this.numBlocks = numBlocks;
            this.mappingProtocol = mappingProtocol;
            this.replacementPolicy = replacementPolicy;

            entries = mappingProtocol.setupEntries(wordSize, blockSize, numBlocks);
            offsetSize = (byte)Math.Round(Math.Log(wordSize, 2));
            indexSize = (byte)Math.Round(Math.Log(numBlocks, 2));
            tagSize = (byte)((bit64 ? 64 : 32) - (offsetSize + indexSize));
        }

        public bool check(long addr)
        {
            bool found = false;
            long index = (addr >> offsetSize) & (0x01 << indexSize);
            long tag = (addr >> (offsetSize + indexSize)) & (0x01 << tagSize);

            // Search cache
            foreach (List<CacheEntry> set in entries) {
                foreach (CacheEntry entry in set)
                {
                    if (entry.tag == tag)
                    {
                        found = true;
                    }
                }
            }

            // If not in cache, replace
            if (!found) {
                replacementPolicy.replace(entries, index, tag);
                return false;
            }

            return true;
        }
    }
}
