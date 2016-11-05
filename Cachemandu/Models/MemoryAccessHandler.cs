using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;

namespace Cachemandu.Models
{
    public class MemoryAccessHandler
    {
        StreamReader mLogFile;

        public MemoryAccessHandler(string logFileName)
        {
            if (File.Exists(logFileName))
            {
                
            }
        }
    }
}
