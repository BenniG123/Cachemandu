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
        public byte flags;
        public long tag;
        public long data;
    }
}