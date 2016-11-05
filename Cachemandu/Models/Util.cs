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
}