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
        public bool valid;
        public int counter;
        public long tag;

        public CacheEntry()
        {
            valid = false;
            counter = 0;
            tag = 0;
        }
    }
}