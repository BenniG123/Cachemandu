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
    }

    public class CacheEntry
    {
        public byte valid;
        public int counter;
        public long tag;

        public CacheEntry()
        {
            valid = 0;
            counter = 0;
            tag = 0;
        }
    }
}