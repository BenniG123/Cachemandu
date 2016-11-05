using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cachemandu.Models
{
    interface IMappingProtocol
    {
        HashSet<List<CacheEntry>> setupEntries(int wordSize, int blockSize, int numBlocks);
    }
}
