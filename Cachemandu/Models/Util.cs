namespace Cachemandu.Models
{
    public enum InstType
    {
        LOAD,
        STORE,
        NONE
    }

    public class MemInst
    {
        public InstType type;
        public long addr;
        public int lineSize;
    }

    public class CacheEntry
    {
        public bool valid;
        public int mostRecentUse;
        public int frequency;
        public int fifoOrder;
        public long tag;

        public CacheEntry()
        {
            valid = false;
            mostRecentUse = 0;
            frequency = 0;
            fifoOrder = 0;
            tag = 0;
        }
    }
}